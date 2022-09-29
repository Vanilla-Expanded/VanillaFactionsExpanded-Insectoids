using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;

namespace VFEI
{
    [HarmonyPatch(typeof(IncidentWorker_Infestation), "TryExecuteWorker", MethodType.Normal)]
    internal class IncidentWorker_Infestation_TryExecuteWorker_Prefix
    {
        [HarmonyPrefix]
        private static bool Prefix(IncidentParms parms, ref bool __result)
        {
            Map map = (Map)parms.target;
            if (map.listerBuildings.AllBuildingsColonistOfDef(DefDatabase<ThingDef>.GetNamed("VFEI_SonicInfestationRepeller")).Count() > 0)
            {
                if (Find.FactionManager.AllFactionsVisible.Where((f) => f.def.defName == "VFEI_Insect").Count() > 0)
                {
                    IncidentParms incidentParms = StorytellerUtility.DefaultParmsNow(IncidentCategoryDefOf.ThreatBig, map);
                    incidentParms.faction = Find.FactionManager.AllFactionsVisible.Where((f) => f.def.defName == "VFEI_Insect").First();
                    incidentParms.raidStrategy = VFEIDefOf.VFEI_ImmediateAttackInsect;
                    incidentParms.points = (float)1.5 * incidentParms.points;
                    Find.Storyteller.incidentQueue.Add(IncidentDefOf.RaidEnemy, Find.TickManager.TicksGame, incidentParms);
                }
                Messages.Message("InfestationNegated".Translate(), MessageTypeDefOf.NeutralEvent);
                __result = true;
                return false;
            }
            else
            {
                __result = true;
                return true;
            }
        }
    }
}