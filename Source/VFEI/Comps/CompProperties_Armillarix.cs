using RimWorld;
using Verse;

namespace VFEI.Comps
{
    internal class CompArmillarix : ThingComp
    {
        public CompProperties_Armillarix Props
        {
            get
            {
                return (CompProperties_Armillarix)this.props;
            }
        }

        public override void CompTickRare()
        {
            Plant p = this.parent as Plant;
            if (p.LifeStage == PlantLifeStage.Mature)
            {
                this.parent.BroadcastCompSignal("armillarixOn");
            }
            else
            {
                this.parent.BroadcastCompSignal("armillarixOff");
            }
        }
    }

    internal class CompProperties_Armillarix : CompProperties
    {
        public CompProperties_Armillarix()
        {
            this.compClass = typeof(CompArmillarix);
        }
    }
}