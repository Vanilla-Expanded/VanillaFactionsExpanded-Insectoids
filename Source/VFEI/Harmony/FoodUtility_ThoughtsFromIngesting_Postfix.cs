using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace VFEI
{
    [HarmonyPatch(typeof(FoodUtility), "ThoughtsFromIngesting", MethodType.Normal)]
    internal class FoodUtility_ThoughtsFromIngesting_Postfix
    {
        [HarmonyPostfix]
        private static void PostFix(Pawn ingester, ref List<FoodUtility.ThoughtFromIngesting> __result)
        {
            if (ingester.health.hediffSet.HasHediff(VFEIDefOf.VFEI_VenomGland))
            {
                __result.Clear();
            }
        }
    }
}