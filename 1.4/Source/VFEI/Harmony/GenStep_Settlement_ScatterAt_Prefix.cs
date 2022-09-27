using HarmonyLib;
using RimWorld;
using RimWorld.BaseGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace VFEI
{
    [HarmonyPatch(typeof(GenStep_Settlement), "ScatterAt", MethodType.Normal)]
    internal class GenStep_Settlement_ScatterAt_Prefix
    {
        [HarmonyPrefix]
        private static bool Prefix(IntVec3 c, Map map, GenStepParams parms)
        {
            try
            {
                if (map.ParentFaction?.def.defName == "VFEI_Insect")
                {
                    IntRange SettlementSizeRange = new IntRange(44, 60);
                    CellRect rect = new CellRect(c.x - SettlementSizeRange.RandomInRange / 2, c.z - SettlementSizeRange.RandomInRange / 2, SettlementSizeRange.RandomInRange, SettlementSizeRange.RandomInRange);
                    rect.ClipInsideMap(map);

                    ResolveParams resolveParams = default;
                    resolveParams.rect = rect;
                    resolveParams.faction = map.ParentFaction;
                    resolveParams.cultivatedPlantDef = ThingDefOf.Plant_Grass;
                    resolveParams.pathwayFloorDef = DefDatabase<TerrainDef>.AllDefsListForReading.FindAll(t => t.terrainAffordanceNeeded == TerrainAffordanceDefOf.Medium && t.costStuffCount < 6).RandomElement();
                    resolveParams.wallStuff = DefDatabase<ThingDef>.AllDefsListForReading.FindAll(t => t.stuffProps != null && (t.terrainAffordanceNeeded == TerrainAffordanceDefOf.Light || t.terrainAffordanceNeeded == TerrainAffordanceDefOf.Medium || t.terrainAffordanceNeeded == TerrainAffordanceDefOf.Heavy) && t.BaseMarketValue < 6).RandomElement();

                    BaseGen.globalSettings.map = map;
                    BaseGen.globalSettings.minBuildings = 8;
                    BaseGen.globalSettings.minBarracks = 2;
                    BaseGen.symbolStack.Push("insectoidBase", resolveParams, null);
                    BaseGen.Generate();
                    BaseGen.globalSettings.map = map;
                    BaseGen.symbolStack.Push("insectoidBaseRDamage", resolveParams, null);
                    BaseGen.Generate();
                    BaseGen.globalSettings.map = map;
                    BaseGen.symbolStack.Push("insectoidRandHives", resolveParams, null);
                    BaseGen.Generate();
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                Log.Message(ex.Message);
                return true;
            }
        }
    }
}
