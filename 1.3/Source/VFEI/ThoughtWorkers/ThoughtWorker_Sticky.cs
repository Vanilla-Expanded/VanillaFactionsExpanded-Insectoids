using RimWorld;
using Verse;

namespace VFEI.ThoughtWorkers
{
    internal class ThoughtWorker_Sticky : ThoughtWorker
    {
        protected override ThoughtState CurrentStateInternal(Pawn p)
        {
            if (p.health.hediffSet.HasHediff(VFEIDefOf.VFEI_PheromoneSecretor))
            {
                return true;
            }
            return false;
        }
    }
}