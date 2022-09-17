using RimWorld;
using RimWorld.BaseGen;
using System.Collections.Generic;
using Verse;

namespace VFEI
{
    internal class SymbolResolver_OutdoorLightingIbase : SymbolResolver
    {
        private const float Margin = 2f;

        private static List<CompGlower> nearbyGlowers = new List<CompGlower>();

        public override void Resolve(ResolveParams rp)
        {
            Map map = BaseGen.globalSettings.map;
            ThingDef thingDef;
            thingDef = ThingDefOf.GlowPod;
            this.FindNearbyGlowers(rp.rect);
            for (int i = 0; i < rp.rect.Area / 4; i++)
            {
                IntVec3 randomCell = rp.rect.RandomCell;
                if (randomCell.Standable(map) && randomCell.GetFirstItem(map) == null && randomCell.GetFirstPawn(map) == null && randomCell.GetFirstBuilding(map) == null)
                {
                    Region region = randomCell.GetRegion(map, RegionType.Set_Passable);
                    if (region != null && region.Room.PsychologicallyOutdoors && region.Room.UsesOutdoorTemperature && !this.AnyGlowerNearby(randomCell) && !BaseGenUtility.AnyDoorAdjacentCardinalTo(randomCell, map))
                    {
                        if (rp.spawnBridgeIfTerrainCantSupportThing == null || rp.spawnBridgeIfTerrainCantSupportThing.Value)
                        {
                            BaseGenUtility.CheckSpawnBridgeUnder(thingDef, randomCell, Rot4.North);
                        }
                        Thing thing = GenSpawn.Spawn(thingDef, randomCell, map, WipeMode.Vanish);
                        if (thing.def.CanHaveFaction && thing.Faction != rp.faction)
                        {
                            thing.SetFaction(rp.faction, null);
                        }
                        SymbolResolver_OutdoorLightingIbase.nearbyGlowers.Add(thing.TryGetComp<CompGlower>());
                    }
                }
            }
            SymbolResolver_OutdoorLightingIbase.nearbyGlowers.Clear();
        }

        private bool AnyGlowerNearby(IntVec3 c)
        {
            for (int i = 0; i < SymbolResolver_OutdoorLightingIbase.nearbyGlowers.Count; i++)
            {
                if (c.InHorDistOf(SymbolResolver_OutdoorLightingIbase.nearbyGlowers[i].parent.Position, SymbolResolver_OutdoorLightingIbase.nearbyGlowers[i].Props.glowRadius + 2f))
                {
                    return true;
                }
            }
            return false;
        }

        private void FindNearbyGlowers(CellRect rect)
        {
            Map map = BaseGen.globalSettings.map;
            SymbolResolver_OutdoorLightingIbase.nearbyGlowers.Clear();
            rect = rect.ExpandedBy(4);
            rect = rect.ClipInsideMap(map);
            foreach (IntVec3 intVec in rect)
            {
                Region region = intVec.GetRegion(map, RegionType.Set_Passable);
                if (region != null && region.Room.PsychologicallyOutdoors)
                {
                    List<Thing> thingList = intVec.GetThingList(map);
                    for (int i = 0; i < thingList.Count; i++)
                    {
                        CompGlower compGlower = thingList[i].TryGetComp<CompGlower>();
                        if (compGlower != null)
                        {
                            SymbolResolver_OutdoorLightingIbase.nearbyGlowers.Add(compGlower);
                        }
                    }
                }
            }
        }
    }
}