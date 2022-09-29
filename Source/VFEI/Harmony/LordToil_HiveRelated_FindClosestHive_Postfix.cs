using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace VFEI
{
    [HarmonyPatch(typeof(LordToil_HiveRelated), "FindClosestHive", MethodType.Normal)]
    internal class LordToil_HiveRelated_FindClosestHive_Postfix
    {
        [HarmonyPostfix]
        private static void PostFix(LordToil_HiveRelated __instance, ref Hive __result, Pawn pawn)
        {
            if (pawn.Faction.def == VFEIDefOf.VFEI_Insect)
            {
                Hive hive = (Hive)GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForDef(VFEIDefOf.VFEI_LargeHive), PathEndMode.Touch,
                                                                    TraverseParms.For(pawn), 10f, (Thing x) => x.Faction == pawn.Faction, null, 0, 30);
                if (hive != null)
                {
                    __result = hive;
                }
            }
        }
    }
}