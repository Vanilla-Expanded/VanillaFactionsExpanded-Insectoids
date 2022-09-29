using RimWorld;
using Verse;

namespace VFEI
{
    internal class IngestionOutcomeDoer_Immunity : IngestionOutcomeDoer
    {
        public float percent = 0.05f;

        protected override void DoIngestionOutcomeSpecial(Pawn pawn, Thing ingested)
        {
            if (pawn.health != null && pawn.health.hediffSet.HasImmunizableNotImmuneHediff() && pawn.health.hediffSet.HasHediff(HediffDefOf.WoundInfection) && pawn.health.immunity != null && pawn.health.immunity.GetImmunity(HediffDefOf.WoundInfection) != 1)
            {
                ImmunityRecord cPawn = pawn.health.immunity.GetImmunityRecord(HediffDefOf.WoundInfection);
                cPawn.immunity += (percent / 100) * ingested.stackCount;
            }
        }
    }
}