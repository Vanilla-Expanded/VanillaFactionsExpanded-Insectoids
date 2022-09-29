using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.BaseGen;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace VFEI.Other
{
    internal class ScenPart_Settlement : ScenPart
    {
        public bool canSpawnBatteries = true;

        public bool canSpawnPowerGenerators = true;

        public FloatRange newBatteriesInitialStoredEnergyPctRange = new FloatRange(0.2f, 0.5f);

        public bool spawnRoofOverNewBatteries = true;

        private static readonly IntRange MaxDistanceBetweenBatteryAndTransmitter = new IntRange(20, 50);

        private static readonly List<IntVec3> tmpTransmitterCells = new List<IntVec3>();

        private bool hasAtleast1TurretInt;

        private readonly List<IntVec3> tmpCells = new List<IntVec3>();

        private readonly Dictionary<PowerNet, bool> tmpPowerNetPredicateResults = new Dictionary<PowerNet, bool>();

        private readonly List<Thing> tmpThings = new List<Thing>();

        public override void PostMapGenerate(Map map)
        {
            if (Find.TickManager.TicksGame < 1000f)
            {
                IntRange SettlementSizeRange = new IntRange(38, 16);
                int randomInRange = SettlementSizeRange.RandomInRange;
                int randomInRange2 = SettlementSizeRange.RandomInRange;
                IntVec3 c = map.Center;
                c.x -= 30;
                c.y -= 10;
                CellRect rect = new CellRect(c.x - randomInRange / 2, c.z - randomInRange2 / 2, randomInRange, randomInRange2);
                rect.ClipInsideMap(map);
                ResolveParams resolveParams = default(ResolveParams);
                resolveParams.rect = rect;
                resolveParams.faction = Faction.OfPlayer;
                BaseGen.globalSettings.map = map;
                BaseGen.globalSettings.minBuildings = 5;
                BaseGen.globalSettings.minBarracks = 1;
                BaseGen.globalSettings.basePart_powerPlantsCoverage = 2;
                BaseGen.symbolStack.Push("settlementNoPawns", resolveParams, null);
                BaseGen.Generate();

                bool stop = false;
                List<ThingDef> potentialGenome = new List<ThingDef>();
                potentialGenome.Add(VFEIDefOf.VFEI_DroneGenome);
                potentialGenome.Add(VFEIDefOf.VFEI_RoyalGenome);
                potentialGenome.Add(VFEIDefOf.VFEI_WarriorGenome);
                foreach (IntVec3 i in rect.ExpandedBy(5))
                {
                    foreach (var item in i.GetThingList(map))
                    {
                        if (item.Faction != null) item.SetFaction(Find.FactionManager.FirstFactionOfDef(VFEIDefOf.VFEI_Insect));
                    }
                    if (i.Fogged(map) && (i.GetThingList(map).Any((t) => t.Faction != null) || i.Walkable(map))) map.fogGrid.Unfog(i);
                    if (!stop && i.Roofed(map) && i.GetRoom(map) is Room room && room != null && room.CellCount > 20 && GenAdj.CellsAdjacent8Way(new TargetInfo(i, map)).ToList().FindAll(l => l.Walkable(map)).Count == 8)
                    {
                        foreach (var item in room.Cells)
                        {
                            List<Thing> items = item.GetThingList(map);
                            for (int u = 0; u < items.Count; u++)
                            {
                                if (items[u].def.defName != "PowerConduit" || items[u].def.defName != "StandingLamp") items[u].DeSpawn();
                            }
                        }
                        for (int b = 0; b <= 8; b++)
                        {
                            GenSpawn.Spawn(potentialGenome.RandomElement(), room.Cells.RandomElement(), map);
                        }
                        if (!room.ContainsThing(ThingDefOf.StandingLamp)) GenSpawn.Spawn(ThingDefOf.StandingLamp, room.Cells.RandomElement(), map);
                        GenSpawn.Spawn(VFEIDefOf.VFEI_BioengineeringIncubator, i, map);
                        stop = true;
                    }
                }

                IncidentParms incidentParms = new IncidentParms();
                incidentParms.faction = Find.FactionManager.FirstFactionOfDef(VFEIDefOf.VFEI_Insect);
                incidentParms.points = 1500;
                incidentParms.target = map;

                List<Pawn> pawns = PawnGroupMakerUtility.GeneratePawns(IncidentParmsUtility.GetDefaultPawnGroupMakerParms(PawnGroupKindDefOf.Combat, incidentParms, false), true).ToList();
                List<IntVec3> iv3 = rect.Cells.ToList().FindAll(x => x.Walkable(map) && !x.Roofed(map));
                for (int i = 0; i < pawns.Count; i++)
                {
                    GenSpawn.Spawn(pawns[i], iv3.RandomElement(), map);
                }
                LordMaker.MakeNewLord(incidentParms.faction, new LordJob_DefendBase(incidentParms.faction, rect.CenterCell), map, pawns);

                map.skyManager.ForceSetCurSkyGlow(1f);
                map.powerNetManager.UpdatePowerNetsAndConnections_First();
                this.UpdateDesiredPowerOutputForAllGenerators(map);
                this.EnsureBatteriesConnectedAndMakeSense(map);
                this.EnsurePowerUsersConnected(map);
                this.EnsureGeneratorsConnectedAndMakeSense(map);
                this.tmpThings.Clear();
            }
        }

        private void EnsureBatteriesConnectedAndMakeSense(Map map)
        {
            this.tmpThings.Clear();
            this.tmpThings.AddRange(map.listerThings.ThingsInGroup(ThingRequestGroup.BuildingArtificial));
            for (int i = 0; i < this.tmpThings.Count; i++)
            {
                CompPowerBattery compPowerBattery = this.tmpThings[i].TryGetComp<CompPowerBattery>();
                if (compPowerBattery != null)
                {
                    PowerNet powerNet = compPowerBattery.PowerNet;
                    if (powerNet == null || !this.HasAnyPowerGenerator(powerNet))
                    {
                        map.powerNetManager.UpdatePowerNetsAndConnections_First();
                        if (this.TryFindClosestReachableNet(compPowerBattery.parent.Position, (PowerNet x) => this.HasAnyPowerGenerator(x), map, out PowerNet powerNet2, out IntVec3 dest))
                        {
                            map.floodFiller.ReconstructLastFloodFillPath(dest, this.tmpCells);
                            if (this.canSpawnPowerGenerators)
                            {
                                int count = this.tmpCells.Count;
                                if (Rand.Chance(Mathf.InverseLerp(MaxDistanceBetweenBatteryAndTransmitter.min, MaxDistanceBetweenBatteryAndTransmitter.max, count)) && this.TrySpawnPowerGeneratorNear(compPowerBattery.parent.Position, map, compPowerBattery.parent.Faction, out Building building))
                                {
                                    this.SpawnTransmitters(compPowerBattery.parent.Position, building.Position, map, compPowerBattery.parent.Faction);
                                    powerNet2 = null;
                                }
                            }
                            if (powerNet2 != null)
                            {
                                this.SpawnTransmitters(this.tmpCells, map, compPowerBattery.parent.Faction);
                            }
                        }
                        else if (this.canSpawnPowerGenerators && this.TrySpawnPowerGeneratorNear(compPowerBattery.parent.Position, map, compPowerBattery.parent.Faction, out Building building2))
                        {
                            this.SpawnTransmitters(compPowerBattery.parent.Position, building2.Position, map, compPowerBattery.parent.Faction);
                        }
                    }
                }
            }
        }

        private void EnsureGeneratorsConnectedAndMakeSense(Map map)
        {
            this.tmpThings.Clear();
            this.tmpThings.AddRange(map.listerThings.ThingsInGroup(ThingRequestGroup.BuildingArtificial));
            for (int i = 0; i < this.tmpThings.Count; i++)
            {
                if (this.IsPowerGenerator(this.tmpThings[i]))
                {
                    PowerNet powerNet = this.tmpThings[i].TryGetComp<CompPower>().PowerNet;
                    if (powerNet == null || !this.HasAnyPowerUser(powerNet))
                    {
                        map.powerNetManager.UpdatePowerNetsAndConnections_First();
                        if (this.TryFindClosestReachableNet(this.tmpThings[i].Position, (PowerNet x) => this.HasAnyPowerUser(x), map, out PowerNet powerNet2, out IntVec3 dest))
                        {
                            map.floodFiller.ReconstructLastFloodFillPath(dest, this.tmpCells);
                            this.SpawnTransmitters(this.tmpCells, map, this.tmpThings[i].Faction);
                        }
                    }
                }
            }
        }

        private void EnsurePowerUsersConnected(Map map)
        {
            this.tmpThings.Clear();
            this.tmpThings.AddRange(map.listerThings.ThingsInGroup(ThingRequestGroup.BuildingArtificial));
            this.hasAtleast1TurretInt = this.tmpThings.Any((Thing t) => t is Building_Turret);
            for (int i = 0; i < this.tmpThings.Count; i++)
            {
                if (this.IsPowerUser(this.tmpThings[i]))
                {
                    CompPowerTrader powerComp = this.tmpThings[i].TryGetComp<CompPowerTrader>();
                    PowerNet powerNet = powerComp.PowerNet;
                    if (powerNet != null && powerNet.hasPowerSource)
                    {
                        this.TryTurnOnImmediately(powerComp, map);
                    }
                    else
                    {
                        map.powerNetManager.UpdatePowerNetsAndConnections_First();
                        if (this.TryFindClosestReachableNet(powerComp.parent.Position, (PowerNet x) => x.CurrentEnergyGainRate() - powerComp.Props.PowerConsumption * CompPower.WattsToWattDaysPerTick > 1E-07f, map, out PowerNet powerNet2, out IntVec3 dest))
                        {
                            map.floodFiller.ReconstructLastFloodFillPath(dest, this.tmpCells);
                            bool flag = false;
                            if (this.canSpawnPowerGenerators && this.tmpThings[i] is Building_Turret && this.tmpCells.Count > 13)
                            {
                                flag = this.TrySpawnPowerGeneratorAndBatteryIfCanAndConnect(this.tmpThings[i], map);
                            }
                            if (!flag)
                            {
                                this.SpawnTransmitters(this.tmpCells, map, this.tmpThings[i].Faction);
                            }
                            this.TryTurnOnImmediately(powerComp, map);
                        }
                        else if (this.canSpawnPowerGenerators && this.TrySpawnPowerGeneratorAndBatteryIfCanAndConnect(this.tmpThings[i], map))
                        {
                            this.TryTurnOnImmediately(powerComp, map);
                        }
                        else if (this.TryFindClosestReachableNet(powerComp.parent.Position, (PowerNet x) => x.CurrentStoredEnergy() > 1E-07f, map, out powerNet2, out dest))
                        {
                            map.floodFiller.ReconstructLastFloodFillPath(dest, this.tmpCells);
                            this.SpawnTransmitters(this.tmpCells, map, this.tmpThings[i].Faction);
                        }
                        else if (this.canSpawnBatteries && this.TrySpawnBatteryNear(this.tmpThings[i].Position, map, this.tmpThings[i].Faction, out Building building))
                        {
                            this.SpawnTransmitters(this.tmpThings[i].Position, building.Position, map, this.tmpThings[i].Faction);
                            if (building.GetComp<CompPowerBattery>().StoredEnergy > 0f)
                            {
                                this.TryTurnOnImmediately(powerComp, map);
                            }
                        }
                    }
                }
            }
        }

        private bool EverPossibleToTransmitPowerAt(IntVec3 c, Map map)
        {
            return c.GetTransmitter(map) != null || GenConstruct.CanBuildOnTerrain(ThingDefOf.PowerConduit, c, map, Rot4.North, null, null);
        }

        private bool HasAnyPowerGenerator(PowerNet net)
        {
            List<CompPowerTrader> powerComps = net.powerComps;
            for (int i = 0; i < powerComps.Count; i++)
            {
                if (this.IsPowerGenerator(powerComps[i].parent))
                {
                    return true;
                }
            }
            return false;
        }

        private bool HasAnyPowerUser(PowerNet net)
        {
            List<CompPowerTrader> powerComps = net.powerComps;
            for (int i = 0; i < powerComps.Count; i++)
            {
                if (this.IsPowerUser(powerComps[i].parent))
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsPowerGenerator(Thing thing)
        {
            if (thing.TryGetComp<CompPowerPlant>() != null)
            {
                return true;
            }
            CompPowerTrader compPowerTrader = thing.TryGetComp<CompPowerTrader>();
            return compPowerTrader != null && (compPowerTrader.PowerOutput > 0f || (!compPowerTrader.PowerOn && compPowerTrader.Props.PowerConsumption < 0f));
        }

        private bool IsPowerUser(Thing thing)
        {
            CompPowerTrader compPowerTrader = thing.TryGetComp<CompPowerTrader>();
            return compPowerTrader != null && (compPowerTrader.PowerOutput < 0f || (!compPowerTrader.PowerOn && compPowerTrader.Props.PowerConsumption > 0f));
        }

        private void SpawnRoofOver(Thing thing)
        {
            CellRect cellRect = thing.OccupiedRect();
            bool flag = true;
            using (CellRect.Enumerator enumerator = cellRect.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (!enumerator.Current.Roofed(thing.Map))
                    {
                        flag = false;
                        break;
                    }
                }
            }
            if (flag)
            {
                return;
            }
            int num = 0;
            CellRect cellRect2 = cellRect.ExpandedBy(2);
            foreach (IntVec3 c in cellRect2)
            {
                if (c.InBounds(thing.Map) && c.GetRoofHolderOrImpassable(thing.Map) != null)
                {
                    num++;
                }
            }
            if (num < 2)
            {
                ThingDef stuff = Rand.Element<ThingDef>(ThingDefOf.WoodLog, ThingDefOf.Steel);
                foreach (IntVec3 intVec in cellRect2.Corners)
                {
                    if (intVec.InBounds(thing.Map) && intVec.Standable(thing.Map) && intVec.GetFirstItem(thing.Map) == null && intVec.GetFirstBuilding(thing.Map) == null && intVec.GetFirstPawn(thing.Map) == null)
                    {
                        IEnumerable<IntVec3> source = GenAdj.CellsAdjacent8Way(new TargetInfo(intVec, thing.Map, false));
                        if (!source.Any((IntVec3 x) => !x.InBounds(thing.Map) || !x.Walkable(thing.Map)) && intVec.SupportsStructureType(thing.Map, ThingDefOf.Wall.terrainAffordanceNeeded))
                        {
                            Thing thing2 = ThingMaker.MakeThing(ThingDefOf.Wall, stuff);
                            GenSpawn.Spawn(thing2, intVec, thing.Map, WipeMode.Vanish);
                            thing2.SetFaction(thing.Faction, null);
                            num++;
                        }
                    }
                }
            }
            if (num > 0)
            {
                foreach (IntVec3 c2 in cellRect2)
                {
                    if (c2.InBounds(thing.Map) && !c2.Roofed(thing.Map))
                    {
                        thing.Map.roofGrid.SetRoof(c2, RoofDefOf.RoofConstructed);
                    }
                }
            }
        }

        private void SpawnTransmitters(List<IntVec3> cells, Map map, Faction faction)
        {
            for (int i = 0; i < cells.Count; i++)
            {
                if (cells[i].GetTransmitter(map) == null)
                {
                    GenSpawn.Spawn(ThingDefOf.PowerConduit, cells[i], map, WipeMode.Vanish).SetFaction(faction, null);
                }
            }
        }

        private void SpawnTransmitters(IntVec3 start, IntVec3 end, Map map, Faction faction)
        {
            bool foundPath = false;
            map.floodFiller.FloodFill(start, (IntVec3 x) => this.EverPossibleToTransmitPowerAt(x, map), delegate (IntVec3 x)
            {
                if (x == end)
                {
                    foundPath = true;
                    return true;
                }
                return false;
            }, int.MaxValue, true, null);
            if (foundPath)
            {
                map.floodFiller.ReconstructLastFloodFillPath(end, tmpTransmitterCells);
                this.SpawnTransmitters(tmpTransmitterCells, map, faction);
            }
        }

        private bool TryFindClosestReachableNet(IntVec3 root, Predicate<PowerNet> predicate, Map map, out PowerNet foundNet, out IntVec3 closestTransmitter)
        {
            this.tmpPowerNetPredicateResults.Clear();
            PowerNet foundNetLocal = null;
            IntVec3 closestTransmitterLocal = IntVec3.Invalid;
            map.floodFiller.FloodFill(root, (IntVec3 x) => this.EverPossibleToTransmitPowerAt(x, map), delegate (IntVec3 x)
            {
                Building transmitter = x.GetTransmitter(map);
                PowerNet powerNet = (transmitter != null) ? transmitter.GetComp<CompPower>().PowerNet : null;
                if (powerNet == null)
                {
                    return false;
                }
                if (!this.tmpPowerNetPredicateResults.TryGetValue(powerNet, out bool flag))
                {
                    flag = predicate(powerNet);
                    this.tmpPowerNetPredicateResults.Add(powerNet, flag);
                }
                if (flag)
                {
                    foundNetLocal = powerNet;
                    closestTransmitterLocal = x;
                    return true;
                }
                return false;
            }, int.MaxValue, true, null);
            this.tmpPowerNetPredicateResults.Clear();
            if (foundNetLocal != null)
            {
                foundNet = foundNetLocal;
                closestTransmitter = closestTransmitterLocal;
                return true;
            }
            foundNet = null;
            closestTransmitter = IntVec3.Invalid;
            return false;
        }

        private bool TrySpawnBatteryNear(IntVec3 position, Map map, Faction faction, out Building newBattery)
        {
            Predicate<IntVec3> extraValidator = null;
            if (this.spawnRoofOverNewBatteries)
            {
                extraValidator = delegate (IntVec3 x)
                {
                    foreach (IntVec3 c in GenAdj.OccupiedRect(x, Rot4.North, ThingDefOf.Battery.size).ExpandedBy(3))
                    {
                        if (c.InBounds(map))
                        {
                            List<Thing> thingList = c.GetThingList(map);
                            for (int i = 0; i < thingList.Count; i++)
                            {
                                if (thingList[i].def.PlaceWorkers != null)
                                {
                                    if (thingList[i].def.PlaceWorkers.Any((PlaceWorker y) => y is PlaceWorker_NotUnderRoof))
                                    {
                                        return false;
                                    }
                                }
                            }
                        }
                    }
                    return true;
                };
            }
            if (this.TrySpawnPowerTransmittingBuildingNear(position, map, faction, ThingDefOf.Battery, out newBattery, extraValidator))
            {
                float randomInRange = this.newBatteriesInitialStoredEnergyPctRange.RandomInRange;
                newBattery.GetComp<CompPowerBattery>().SetStoredEnergyPct(randomInRange);
                if (this.spawnRoofOverNewBatteries)
                {
                    this.SpawnRoofOver(newBattery);
                }
                return true;
            }
            return false;
        }

        private bool TrySpawnPowerGeneratorAndBatteryIfCanAndConnect(Thing forThing, Map map)
        {
            if (!this.canSpawnPowerGenerators)
            {
                return false;
            }
            IntVec3 position = forThing.Position;
            if (this.canSpawnBatteries && Rand.Chance(this.hasAtleast1TurretInt ? 1f : 0.1f) && this.TrySpawnBatteryNear(forThing.Position, map, forThing.Faction, out Building building))
            {
                this.SpawnTransmitters(forThing.Position, building.Position, map, forThing.Faction);
                position = building.Position;
            }
            if (this.TrySpawnPowerGeneratorNear(position, map, forThing.Faction, out Building building2))
            {
                this.SpawnTransmitters(position, building2.Position, map, forThing.Faction);
                return true;
            }
            return false;
        }

        private bool TrySpawnPowerGeneratorNear(IntVec3 position, Map map, Faction faction, out Building newPowerGenerator)
        {
            if (this.TrySpawnPowerTransmittingBuildingNear(position, map, faction, ThingDefOf.SolarGenerator, out newPowerGenerator, null))
            {
                map.powerNetManager.UpdatePowerNetsAndConnections_First();
                newPowerGenerator.GetComp<CompPowerPlant>().UpdateDesiredPowerOutput();
                return true;
            }
            return false;
        }

        private bool TrySpawnPowerTransmittingBuildingNear(IntVec3 position, Map map, Faction faction, ThingDef def, out Building newBuilding, Predicate<IntVec3> extraValidator = null)
        {
            TraverseParms traverseParams = TraverseParms.For(TraverseMode.PassAllDestroyableThings, Danger.Deadly, false);
            if (RCellFinder.TryFindRandomCellNearWith(position, delegate (IntVec3 x)
            {
                if (!x.Standable(map) || x.Roofed(map) || !this.EverPossibleToTransmitPowerAt(x, map))
                {
                    return false;
                }
                if (!map.reachability.CanReach(position, x, PathEndMode.OnCell, traverseParams))
                {
                    return false;
                }
                foreach (IntVec3 c in GenAdj.OccupiedRect(x, Rot4.North, def.size))
                {
                    if (!c.InBounds(map) || c.Roofed(map) || c.GetEdifice(map) != null || c.GetFirstItem(map) != null || c.GetTransmitter(map) != null)
                    {
                        return false;
                    }
                }
                return extraValidator == null || extraValidator(x);
            }, map, out IntVec3 loc, 8, 2147483647))
            {
                newBuilding = (Building)GenSpawn.Spawn(ThingMaker.MakeThing(def, null), loc, map, Rot4.North, WipeMode.Vanish, false);
                newBuilding.SetFaction(faction, null);
                return true;
            }
            newBuilding = null;
            return false;
        }

        private void TryTurnOnImmediately(CompPowerTrader powerComp, Map map)
        {
            if (powerComp.PowerOn)
            {
                return;
            }
            map.powerNetManager.UpdatePowerNetsAndConnections_First();
            if (powerComp.PowerNet != null && powerComp.PowerNet.CurrentEnergyGainRate() > 1E-07f)
            {
                powerComp.PowerOn = true;
            }
        }

        private void UpdateDesiredPowerOutputForAllGenerators(Map map)
        {
            this.tmpThings.Clear();
            this.tmpThings.AddRange(map.listerThings.ThingsInGroup(ThingRequestGroup.BuildingArtificial));
            for (int i = 0; i < this.tmpThings.Count; i++)
            {
                if (this.IsPowerGenerator(this.tmpThings[i]))
                {
                    CompPowerPlant compPowerPlant = this.tmpThings[i].TryGetComp<CompPowerPlant>();
                    if (compPowerPlant != null)
                    {
                        compPowerPlant.UpdateDesiredPowerOutput();
                    }
                }
            }
        }
    }
}