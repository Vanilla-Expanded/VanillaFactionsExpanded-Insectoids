using Verse;

namespace VFEI.Comps.PawnComps.Properties
{
    class CompProperties_NoBleedHearthAttack : HediffCompProperties
    {
        public CompProperties_NoBleedHearthAttack()
        {
            this.compClass = typeof(CompNoBleedHearthAttack);
        }
    }
}
