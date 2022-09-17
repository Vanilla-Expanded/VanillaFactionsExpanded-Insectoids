using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace VFEI
{
    internal class DeathActionWorker_InsectsFlee : DeathActionWorker
    {
        public override RulePackDef DeathRules
        {
            get
            {
                return RulePackDefOf.Transition_Died;
            }
        }

        public override void PawnDied(Corpse corpse)
        {
            IEnumerable<Thing> pawns = corpse.Map.listerThings.AllThings.Where((t) => t is Pawn && t.def.race.FleshType == FleshTypeDefOf.Insectoid && t.Faction == corpse.Faction);
            foreach (Pawn pawn in pawns)
            {
                IntVec3 intVec3 = new IntVec3();
                CellFinder.TryFindRandomPawnExitCell(pawn, out intVec3);
                pawn.jobs.StartJob(new Verse.AI.Job(VFEIDefOf.InsectFlee, new LocalTargetInfo(intVec3)), Verse.AI.JobCondition.Succeeded);
            }
        }
    }
}