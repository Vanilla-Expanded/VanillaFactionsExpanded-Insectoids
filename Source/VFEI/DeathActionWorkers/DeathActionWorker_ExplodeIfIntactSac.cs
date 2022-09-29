using RimWorld;
using System.Linq;
using Verse;

namespace VFEI
{
    internal class DeathActionWorker_ExplodeIfIntactSac : DeathActionWorker
    {
        public override bool DangerousInMelee
        {
            get
            {
                return true;
            }
        }

        public override RulePackDef DeathRules
        {
            get
            {
                return RulePackDefOf.Transition_DiedExplosive;
            }
        }

        public override void PawnDied(Corpse corpse)
        {
            BodyPartRecord bodyPartRecord = (from x in corpse.InnerPawn.health.hediffSet.GetNotMissingParts()
                                             where x.def == VFEIDefOf.VFEI_ExplosiveSac
                                             select x).FirstOrDefault();
            if (bodyPartRecord != null)
            {
                GenExplosion.DoExplosion(corpse.Position, corpse.Map, 1.9f, DamageDefOf.Flame, corpse.InnerPawn, -1, -1f, null, null, null, null, null, 0f, 1, false, null, 0f, 1, 0f, false, null, null);
            }
        }
    }
}