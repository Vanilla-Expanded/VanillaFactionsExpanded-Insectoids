using Verse;

namespace VFEI.PawnComps
{
    internal class CompProperties_LarvaeToCocoon : CompProperties
    {
        public CompProperties_LarvaeToCocoon()
        {
            this.compClass = typeof(CompLarvaeToCocoon);
        }

#pragma warning disable 0649
        public int ticksBeforeBecomingCocoon;
    }
}