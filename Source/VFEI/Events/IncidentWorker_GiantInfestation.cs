using System;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace VFEI.Events
{
    internal class IncidentWorker_GiantInfestation : IncidentWorker
    {
        public static Thing SpawnTunnels(int hiveCount, Map map, bool spawnAnywhereIfNoGoodCell = false, bool ignoreRoofedRequirement = false, string questTag = null)
        {
            TryFindCell(out IntVec3 loc, map);
            Thing thing = GenSpawn.Spawn(ThingMaker.MakeThing(VFEIDefOf.VFEI_TunnelHiveSpawner, null), loc, map, WipeMode.FullRefund);
            QuestUtility.AddQuestTag(thing, questTag);
            for (int i = 0; i < hiveCount - 1; i++)
            {
                loc = CompSpawnerHives.FindChildHiveLocation(thing.Position, map, VFEIDefOf.VFEI_LargeHive, ThingDefOf.Hive.GetCompProperties<CompProperties_SpawnerHives>(), ignoreRoofedRequirement, true);
                if (loc.IsValid)
                {
                    thing = GenSpawn.Spawn(ThingMaker.MakeThing(VFEIDefOf.VFEI_TunnelHiveSpawner, null), loc, map, WipeMode.FullRefund);
                    QuestUtility.AddQuestTag(thing, questTag);
                }
            }
            return thing;
        }

        public static bool TryFindCell(out IntVec3 cell, Map map)
        {
            cell = CellFinderLoose.RandomCellWith(
                (i) => !i.Fogged(map) && i.DistanceToEdge(map) < 25 && i.GetRoof(map) == RoofDefOf.RoofRockThick && i.Walkable(map) && i.GetTemperature(map) > -17f,
                map);
            if (!cell.InBounds(map))
            {
                return false;
            }
            return true;
        }

        protected override bool CanFireNowSub(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            return base.CanFireNowSub(parms) && HiveUtility.TotalSpawnedHivesCount(map) < 30 && TryFindCell(out IntVec3 intVec, map) && map.listerBuildings.AllBuildingsColonistOfDef(DefDatabase<ThingDef>.GetNamed("VFEI_SonicInfestationRepeller")).ToList().Count == 0;
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            Thing t = SpawnTunnels(Mathf.Max(GenMath.RoundRandom(parms.points / 220f), 1), map, false, false, null);
            base.SendStandardLetter(parms, t, Array.Empty<NamedArgument>());
            Find.TickManager.slower.SignalForceNormalSpeedShort();
            return true;
        }
    }
}