using HarmonyLib;
using RimWorld;

namespace VFEI
{
    [HarmonyPatch(typeof(Faction), "HasGoodwill", MethodType.Getter)]
    internal class Faction_HasGoodwill_Postfix
    {
        [HarmonyPostfix]
        private static void PostFix(ref Faction __instance, ref bool __result)
        {
            if (__instance.def.defName == "VFEI_Insect") __result = false;
        }
    }
}