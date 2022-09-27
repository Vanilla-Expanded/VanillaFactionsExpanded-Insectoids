using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace VFEI
{
    [HarmonyPatch(typeof(BodyPartDef), "GetMaxHealth", MethodType.Normal)]
    internal class BodyPartDef_GetMaxHealth_Postfix
    {
        [HarmonyPostfix]
        private static void PostFix(BodyPartDef __instance, ref float __result, Pawn pawn)
        {
            foreach (Hediff hediff in pawn.health.hediffSet.hediffs)
            {
                if (hediff?.Part?.def == __instance && HediffUtility.TryGetComp<Comps.VariableHealthComp.VFEI_HediffComp_HealthModifier>(hediff) != null)
                {
                    __result += HediffUtility.TryGetComp<Comps.VariableHealthComp.VFEI_HediffComp_HealthModifier>(hediff).Props.healthPointToAdd;
                }
            }
        }
    }
}
