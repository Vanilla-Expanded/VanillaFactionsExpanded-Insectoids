using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace VFEI
{
    [HarmonyPatch(typeof(ThoughtWorker_Dark), "CurrentStateInternal", MethodType.Normal)]
    internal class ThoughtWorker_Dark_CurrentStateInternal_Postfix
    {
        [HarmonyPostfix]
        private static void PostFix(Pawn p, ref ThoughtState __result)
        {
            if (p.Awake() && p.needs.mood.recentMemory.TicksSinceLastLight > 240 && p.health.hediffSet.HasHediff(VFEIDefOf.VFEI_Antenna))
            {
                __result = ThoughtState.Inactive;
            }
        }
    }
}
