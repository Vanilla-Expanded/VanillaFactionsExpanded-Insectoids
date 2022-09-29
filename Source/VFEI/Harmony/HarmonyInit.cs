using HarmonyLib;
using RimWorld;
using RimWorld.BaseGen;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Verse;
using Verse.AI;

namespace VFEI
{
    [StaticConstructorOnStartup]
    internal static class HarmonyInit
    {
        static HarmonyInit()
        {
            new Harmony("kikohi.vfe.insectoid").PatchAll();
        }

        public static Hive TryFindLargeHive(Pawn pawn, IntVec3 intVec3)
        {
            Hive thing = (Hive)pawn.Map.thingGrid.ThingAt(intVec3, VFEIDefOf.VFEI_LargeHive);
            if (thing != null)
                return thing;
            else return (Hive)pawn.Map.thingGrid.ThingAt(intVec3, ThingDefOf.Hive);
        }
    }
}