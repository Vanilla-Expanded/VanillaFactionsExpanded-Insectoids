using Verse;

namespace VFEI.Comps.VariableHealthComp
{
    internal class VFEI_HediffComp_HealthModifier : HediffComp
    {
        public VFEI_HediffCompProperties_HealthModifier Props
        {
            get
            {
                return (VFEI_HediffCompProperties_HealthModifier)this.props;
            }
        }
    }
}