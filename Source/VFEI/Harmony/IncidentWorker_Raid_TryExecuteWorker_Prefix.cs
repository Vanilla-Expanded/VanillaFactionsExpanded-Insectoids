using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;

namespace VFEI
{
    [HarmonyPatch(typeof(IncidentWorker_Raid), "TryExecuteWorker", MethodType.Normal)]
    internal class IncidentWorker_Raid_TryExecuteWorker_Prefix
    {
        [HarmonyPrefix]
        private static bool Prefix(IncidentParms parms, ref IncidentWorker_Raid __instance, ref bool __result)
        {
            if (parms.faction != null && parms.faction.def.defName == "VFEI_Insect")
            {
                Map map = (Map)parms.target;
                if (!__instance.TryGenerateRaidInfo(parms, out List<Pawn> pawns))
                {
                    __result = false;
                    return false;
                }

                List<TargetInfo> targetInfoList = new List<TargetInfo>();
                if (parms.pawnGroups != null)
                {
                    List<List<Pawn>> source = IncidentParmsUtility.SplitIntoGroups(pawns, parms.pawnGroups);
                    List<Pawn> list = source.MaxBy(x => x.Count);
                    if (list.Any())
                        targetInfoList.Add(list[0]);
                    for (int index = 0; index < source.Count; ++index)
                    {
                        if (source[index] != list && source[index].Any())
                            targetInfoList.Add(source[index][0]);
                    }
                }
                else if (pawns.Any())
                {
                    foreach (Pawn pawn in pawns)
                        targetInfoList.Add(pawn);
                }

                if (parms.raidArrivalMode.defName == "VFEI_GigalocustSwarm")
                    targetInfoList = new List<TargetInfo> { new TargetInfo(parms.spawnCenter, map) };

                string label = parms.raidStrategy.letterLabelEnemy + ": " + parms.faction.Name;
                string desc = string.Format(parms.raidArrivalMode.textEnemy, parms.faction.def.pawnsPlural, parms.faction.Name.ApplyTag(parms.faction)).CapitalizeFirst() + "\n\n" + parms.raidStrategy.arrivalTextEnemy;

                Find.LetterStack.ReceiveLetter(label, desc, LetterDefOf.ThreatBig, targetInfoList);
                parms.raidStrategy.Worker.MakeLords(parms, pawns);

                __result = true;
                return false;
            }
            return true;
        }
    }
}