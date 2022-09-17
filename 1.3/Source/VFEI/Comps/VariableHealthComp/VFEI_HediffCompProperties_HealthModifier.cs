using Verse;

namespace VFEI.Comps.VariableHealthComp
{
    internal class VFEI_HediffCompProperties_HealthModifier : HediffCompProperties
    {
        public VFEI_HediffCompProperties_HealthModifier()
        {
            this.compClass = typeof(VFEI_HediffComp_HealthModifier);
        }

        public float healthPointToAdd = 0f;
    }
}