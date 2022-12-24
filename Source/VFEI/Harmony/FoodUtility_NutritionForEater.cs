using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace VFEI
{
    [HarmonyPatch(typeof(FoodUtility), "NutritionForEater", MethodType.Normal)]
    internal class VFEI_FoodUtility_NutritionForEater_Postfix
    {
        [HarmonyPostfix]
        private static void PostFix(Pawn eater, Thing food, ref float __result)
        {
            if (eater?.health?.hediffSet?.HasHediff(VFEIDefOf.VFEI_PredatorStomach) == true)
            {
                if (!food.def.IsRawFood())
                {
                    __result = __result * 1.1f;
                }
            }

        }
    }
}