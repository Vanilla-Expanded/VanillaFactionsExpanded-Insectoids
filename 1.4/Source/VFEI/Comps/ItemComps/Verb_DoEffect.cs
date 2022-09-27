using RimWorld;
using System.Linq;
using Verse;
using Verse.AI;

namespace VFEI
{
    internal class Verb_DoEffect : Verb
    {
        protected override bool TryCastShot()
        {
            if (this.CasterPawn != null)
            {
                this.EquipmentSource.GetComps<CompTargetEffect>().ToList()[0].DoEffectOn(this.CasterPawn, null);
                return true;
            }
            return false;
        }
    }
}