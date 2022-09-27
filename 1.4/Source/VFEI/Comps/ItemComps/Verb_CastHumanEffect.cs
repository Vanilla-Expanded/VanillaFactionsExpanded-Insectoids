using RimWorld;
using Verse;
using Verse.AI;

namespace VFEI
{
    public class Verb_CastHumanEffect : Verb_CastBase
    {
        protected override bool TryCastShot()
        {
            Thing thing = this.currentTarget.Thing;

            if (this.CasterPawn == null || thing == null || !(thing is Pawn p && !p.AnimalOrWildMan()) || thing.Faction == Faction.OfPlayer)
                return false;

            foreach (CompTargetEffect comp in this.EquipmentSource.GetComps<CompTargetEffect>())
                comp.DoEffectOn(this.CasterPawn, thing);

            this.ReloadableCompSource?.UsedOnce();
            return true;
        }
    }
}