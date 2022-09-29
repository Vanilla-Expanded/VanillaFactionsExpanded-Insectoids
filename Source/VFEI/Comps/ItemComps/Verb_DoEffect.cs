using System.Linq;
using RimWorld;
using Verse;

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