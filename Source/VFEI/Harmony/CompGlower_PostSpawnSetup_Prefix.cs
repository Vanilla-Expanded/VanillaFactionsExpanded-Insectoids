using HarmonyLib;
using Verse;

namespace VFEI
{
    [HarmonyPatch(typeof(CompGlower), "PostSpawnSetup", MethodType.Normal)]
    internal class CompGlower_PostSpawnSetup_Prefix
    {
        [HarmonyPrefix]
        private static bool Prefix(ref CompGlower __instance)
        {
            if (__instance.parent.def.defName == "VFEI_Plant_Armillarix")
            {
                return false;
            }
            return true;
        }
    }
}