using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI.Group;
using Verse.Sound;

namespace VFEI.BuildingClass
{
    [StaticConstructorOnStartup]
    internal class VFEI_TunnelHiveSpawner : TunnelHiveSpawner
    {
        private Faction faction;
        private List<ThingDef> filthTypes = new List<ThingDef>();
        private int secondarySpawnTickVFE;
        private List<PawnKindDef> spawnablePawnKinds = new List<PawnKindDef>();
        private Sustainer sustainer;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref this.secondarySpawnTickVFE, "secondarySpawnTickVFE", 0);
            Scribe_References.Look(ref this.faction, "faction");
            Scribe_Collections.Look(ref spawnablePawnKinds, "spawnablePawnKinds", LookMode.Def);
            Scribe_Collections.Look(ref filthTypes, "filthTypes", LookMode.Def);
        }

        public override void PostMake()
        {
            filthTypes.Clear();
            filthTypes.Add(ThingDefOf.Filth_Dirt);
            filthTypes.Add(ThingDefOf.Filth_Dirt);
            filthTypes.Add(ThingDefOf.Filth_Dirt);
            filthTypes.Add(ThingDefOf.Filth_RubbleRock);

            spawnablePawnKinds.Clear();
            spawnablePawnKinds.Add(PawnKindDefOf.Megascarab);
            spawnablePawnKinds.Add(PawnKindDefOf.Spelopede);
            spawnablePawnKinds.Add(PawnKindDefOf.Megaspider);
            spawnablePawnKinds.Add(VFEIDefOf.VFEI_Insectoid_RoyalMegaspider);

            faction = Find.FactionManager.FirstFactionOfDef(VFEIDefOf.VFEI_Insect);
            base.PostMake();
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            if (!respawningAfterLoad)
            {
                this.secondarySpawnTickVFE = Find.TickManager.TicksGame + new FloatRange(26f, 30f).RandomInRange.SecondsToTicks();
            }
            // Create sustainer
            LongEventHandler.ExecuteWhenFinished(delegate
            {
                this.sustainer = SoundDefOf.Tunnel.TrySpawnSustainer(SoundInfo.InMap(this, MaintenanceType.PerTick));
            });
        }

        public override void Tick()
        {
            if (base.Spawned)
            {
                this.sustainer.Maintain();
                Vector3 vector = base.Position.ToVector3Shifted();

                if (Rand.MTBEventOccurs(0.3f, 1f, 1.TicksToSeconds()) && CellFinder.TryFindRandomReachableCellNear(base.Position, base.Map, 3f, TraverseParms.For(TraverseMode.NoPassClosedDoors), null, null, out IntVec3 c))
                {
                    FilthMaker.TryMakeFilth(c, base.Map, filthTypes.RandomElement());
                }

                if (Rand.MTBEventOccurs(0.2f, 1f, 1.TicksToSeconds()))
                {
                    FleckMaker.ThrowDustPuffThick(new Vector3(vector.x, 0f, vector.z)
                    {
                        y = AltitudeLayer.MoteOverhead.AltitudeFor()
                    }, base.Map, Rand.Range(1.5f, 3f), new Color(1f, 1f, 1f, 2.5f));
                }

                if (this.secondarySpawnTickVFE <= Find.TickManager.TicksGame)
                {
                    Hive hive;
                    this.sustainer.End();

                    List<Pawn> toSpawn = new List<Pawn>();
                    List<Pawn> queens = new List<Pawn>();

                    if (Rand.Bool)
                    {
                        hive = (Hive)GenSpawn.Spawn(ThingMaker.MakeThing(ThingDefOf.Hive, null), base.Position, base.Map);
                        hive.SetFaction(faction);
                        hive.questTags = this.questTags;
                        hive.GetComps<CompSpawner>().ToList().Find(s => s.PropsSpawner.thingToSpawn == ThingDefOf.InsectJelly).TryDoSpawn();
                    }
                    else
                    {
                        hive = (Hive)GenSpawn.Spawn(ThingMaker.MakeThing(VFEIDefOf.VFEI_LargeHive, null), base.Position, base.Map);
                        hive.SetFaction(faction, null);
                        hive.questTags = this.questTags;
                        hive.GetComps<CompSpawner>().ToList().Find(s => s.PropsSpawner.thingToSpawn == VFEIDefOf.VFEI_RoyalInsectJelly).TryDoSpawn();

                        // Spawn queen
                        if (base.Map.mapPawns.AllPawnsSpawned.Where((p) => p.kindDef == VFEIDefOf.VFEI_Insectoid_Queen).Count() == 0)
                        {
                            Pawn queen = PawnGenerator.GeneratePawn(VFEIDefOf.VFEI_Insectoid_Queen, faction);
                            GenSpawn.Spawn(queen, CellFinder.RandomClosewalkCellNear(base.Position, base.Map, 4), base.Map);
                            queen.mindState.spawnedByInfestationThingComp = this.spawnedByInfestationThingComp;
                            this.insectsPoints -= queen.kindDef.combatPower;

                            queens.Add(queen);
                            SpawnedPawnParams spawnedPawnParams = new SpawnedPawnParams
                            {
                                aggressive = false,
                                defendRadius = 2,
                                defSpot = base.Position,
                                spawnerThing = hive
                            };

                            LordMaker.MakeNewLord(faction, new LordJob_DefendAndExpandHive(spawnedPawnParams), base.Map, queens);
                        }
                    }

                    if (this.insectsPoints > 0f)
                    {
                        this.insectsPoints = Mathf.Max(this.insectsPoints, spawnablePawnKinds.Min((PawnKindDef x) => x.combatPower));

                        while (this.insectsPoints > 0)
                        {
                            if (!spawnablePawnKinds.Where((PawnKindDef x) => x.combatPower <= this.insectsPoints).TryRandomElement(out PawnKindDef pawnKindDef)) break;
                            this.insectsPoints -= pawnKindDef.combatPower;

                            Pawn pawn = PawnGenerator.GeneratePawn(pawnKindDef, Faction.OfInsects);
                            GenSpawn.Spawn(pawn, CellFinder.RandomClosewalkCellNear(base.Position, base.Map, 2), base.Map);
                            pawn.mindState.spawnedByInfestationThingComp = this.spawnedByInfestationThingComp;

                            toSpawn.Add(pawn);
                        }
                    }

                    if (toSpawn.Any())
                    {
                        LordMaker.MakeNewLord(faction, new LordJob_AssaultColony(faction), base.Map, toSpawn);
                    }

                    this.Destroy();
                }
            }
        }
    }
}