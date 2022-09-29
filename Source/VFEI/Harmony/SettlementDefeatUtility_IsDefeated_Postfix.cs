using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace VFEI
{
    [HarmonyPatch(typeof(SettlementDefeatUtility), "IsDefeated", MethodType.Normal)]
    internal class SettlementDefeatUtility_IsDefeated_Postfix
    {
        [HarmonyPostfix]
        private static void PostFix(Map map, Faction faction, ref bool __result)
        {
            List<Pawn> list = map.mapPawns.SpawnedPawnsInFaction(faction);
            for (int i = 0; i < list.Count; i++)
            {
                Pawn pawn = list[i];
                if (faction.def.defName == "VFEI_Insect" && pawn.RaceProps.FleshType == FleshTypeDefOf.Insectoid && GenHostility.IsActiveThreatToPlayer(pawn))
                {
                    __result = false;
                }
            }
        }
    }
}
