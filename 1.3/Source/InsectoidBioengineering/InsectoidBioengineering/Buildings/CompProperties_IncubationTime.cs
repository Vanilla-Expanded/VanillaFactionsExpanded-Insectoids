using Verse;

namespace InsectoidBioengineering
{
    public class CompProperties_IncubationTime : CompProperties
    {

        public float timeInDays = 1f;

        public CompProperties_IncubationTime()
        {
            this.compClass = typeof(CompIncubationTime);
        }


    }
}