using RimWorld;
using Verse;

namespace VFEI.Other
{
    internal class VFEI_Plant_Armillarix : Plant
    {
        public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
        {
            this.Map.glowGrid.DeRegisterGlower(this.TryGetComp<CompGlower>());
            base.DeSpawn(mode);
        }

        public override void TickLong()
        {
            base.TickLong();
            if (this.LifeStage == PlantLifeStage.Mature)
            {
                this.BroadcastCompSignal("armillarixOn");
            }
            else
            {
                this.BroadcastCompSignal("armillarixOff");
            }
        }
    }
}