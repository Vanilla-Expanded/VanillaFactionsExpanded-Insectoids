using RimWorld;
using Verse;

namespace VFEI.Comps.BuildingComps
{
    internal class CompRepeller : ThingComp
    {
        private int nextMote = 0;

        public CompProperties_Repeller Props
        {
            get
            {
                return (CompProperties_Repeller)this.props;
            }
        }

        public override void CompTickRare()
        {
            if (Find.TickManager.TicksGame >= this.nextMote && this.parent.TryGetComp<CompPowerTrader>().PowerOn && VFEI_ModSettings.pulseFromRepeller)
            {
                FleckMaker.Static(this.parent.TrueCenter(), this.parent.Map, FleckDefOf.PsycastAreaEffect, 20);
                this.nextMote += 2500;
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<int>(ref this.nextMote, "nextMote");
        }

        public override void PostPostMake()
        {
            this.nextMote = Find.TickManager.TicksGame + 2500;
        }
    }
}