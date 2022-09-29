using UnityEngine;
using Verse;

namespace VFEI
{
    internal class VFEI_ModSettings : ModSettings
    {
        internal static bool pulseFromRepeller = true;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<bool>(ref pulseFromRepeller, "pulseFromRepeller", true);
        }
    }

    internal class VFEI_Mod : Mod
    {
        public static VFEI_ModSettings settings;
        private Vector2 scrollPosition = Vector2.zero;

        public VFEI_Mod(ModContentPack content) : base(content)
        {
            settings = GetSettings<VFEI_ModSettings>();
        }

        public override string SettingsCategory() => "SName".Translate();

        public override void DoSettingsWindowContents(Rect rect)
        {
            Listing_Standard list = new Listing_Standard();
            Widgets.BeginScrollView(rect, ref scrollPosition, rect, true);
            list.Begin(rect);
            list.CheckboxLabeled("PusleLabel".Translate(), ref VFEI_ModSettings.pulseFromRepeller, "PusleTooltip".Translate());
            list.End();
            Widgets.EndScrollView();
            settings.Write();
        }
    }
}