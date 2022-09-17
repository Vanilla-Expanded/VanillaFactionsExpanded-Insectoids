using Verse;

namespace InsectoidBioengineering
{
    public class CompProperties_FailureRate : CompProperties
    {

        public int failureRatePercent = 5;

        public CompProperties_FailureRate()
        {
            this.compClass = typeof(CompFailureRate);
        }


    }
}