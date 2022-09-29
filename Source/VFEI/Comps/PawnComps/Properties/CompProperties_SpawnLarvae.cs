using Verse;

namespace VFEI.PawnComps
{
    internal class CompProperties_SpawnLarvae : CompProperties
    {
        public CompProperties_SpawnLarvae()
        {
            this.compClass = typeof(CompSpawnLarvae);
        }

#pragma warning disable 0649
        public int ticksBetweenSpawn;

        public int numberToSpawn;
    }
}