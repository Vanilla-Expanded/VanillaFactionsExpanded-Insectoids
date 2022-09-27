using RimWorld;
using Verse;

namespace VFEI.Comps.HediffComps
{
    internal class CompSpawnJelly : HediffComp
    {
        public int NextBeforeSpawn = 0;

        public CompProperties.CompProperties_SpawnJelly Props
        {
            get
            {
                return (CompProperties.CompProperties_SpawnJelly)this.props;
            }
        }

        public override void CompExposeData()
        {
            Scribe_Values.Look<int>(ref this.NextBeforeSpawn, "NextBeforeSpawn", 0, false);
        }

        public override void CompPostMake()
        {
            this.NextBeforeSpawn = Find.TickManager.TicksGame + 60000;
            base.CompPostMake();
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            if (Find.TickManager.TicksGame == this.NextBeforeSpawn)
            {
                this.NextBeforeSpawn += 60000;
                Thing thing = ThingMaker.MakeThing(ThingDefOf.InsectJelly);
                thing.stackCount = Rand.RangeInclusive(2, 5);
                GenSpawn.Spawn(thing, this.Pawn.Position, this.Pawn.Map);
            }
        }
    }
}