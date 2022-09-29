using HarmonyLib;
using RimWorld;
using Verse;

namespace VFEI
{
    public class CompShieldNoRecharge : CompShield
    {
        public bool canRecharge = true;

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref canRecharge, "canRecharge");
        }

        public override void CompTick()
        {
            if (canRecharge && parent is Apparel apparel && apparel.Wearer != null)
            {
                AccessTools.Field(typeof(CompShield), "energy").SetValue(this, parent.GetStatValue(StatDefOf.EnergyShieldEnergyMax));
                canRecharge = false;
            }
        }
    }
}
