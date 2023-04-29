using HarmonyLib;
using RimWorld;
using Verse;

namespace VFEI
{
    [HarmonyPatch(typeof(PawnAddictionHediffsGenerator), "ApplyAddiction")]
    static class VFEI_PawnAddictionHediffsGenerator_ApplyAddiction_Patch
    {
        public static bool Prefix(Pawn pawn, ChemicalDef chemicalDef)
        {
            return pawn.kindDef.defName != "Refugee" || chemicalDef.defName != "VFEI_RoyalJellyChemical";
        }
    }
}
