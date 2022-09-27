using RimWorld;
using Verse;

namespace VFEI.Events
{
    internal class IncidentWorker_FirstRaid : IncidentWorker
    {
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            return false;
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            PlayerKnowledgeDatabase.SetKnowledge(ConceptDefOf.ShieldBelts, 1f);
            IncidentParms incidentParms = StorytellerUtility.DefaultParmsNow(IncidentCategoryDefOf.ThreatBig, (Map)parms.target);
            incidentParms.faction = Find.FactionManager.FirstFactionOfDef(VFEIDefOf.VFEI_Insect);
            incidentParms.raidStrategy = RaidStrategyDefOf.ImmediateAttack;
            incidentParms.raidArrivalMode = PawnsArrivalModeDefOf.EdgeWalkIn;
            incidentParms.points *= 10;
            Find.Storyteller.incidentQueue.Add(IncidentDefOf.RaidEnemy, Find.TickManager.TicksGame, incidentParms);
            return true;
        }
    }
}