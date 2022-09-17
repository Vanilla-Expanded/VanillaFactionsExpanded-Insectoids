using RimWorld;
using RimWorld.BaseGen;
using Verse;

namespace VFEI
{
    internal class SymbolResolver_SettlementNoPawns : SymbolResolver
    {
        public override void Resolve(ResolveParams rp)
        {
            Faction faction = Faction.OfPlayer;
            int num = 0;
            if (rp.edgeDefenseWidth != null)
            {
                num = rp.edgeDefenseWidth.Value;
            }
            else if (rp.rect.Width >= 20 && rp.rect.Height >= 20)
            {
                num = (Rand.Bool ? 2 : 4);
            }
            float num2 = (float)rp.rect.Area / 144f * 0.17f;
            BaseGen.globalSettings.minEmptyNodes = ((num2 < 1f) ? 0 : GenMath.RoundRandom(num2));
            BaseGen.symbolStack.Push("outdoorLighting", rp, null);
            int num3 = Rand.Chance(0.75f) ? GenMath.RoundRandom((float)rp.rect.Area / 400f) : 0;
            for (int i = 0; i < num3; i++)
            {
                ResolveParams resolveParams2 = rp;
                resolveParams2.faction = faction;
                BaseGen.symbolStack.Push("firefoamPopper", resolveParams2, null);
            }

            ResolveParams resolveParams4 = rp;
            resolveParams4.rect = rp.rect.ContractedBy(num);
            resolveParams4.faction = faction;
            BaseGen.symbolStack.Push("ensureCanReachMapEdge", resolveParams4, null);
            ResolveParams resolveParams5 = rp;
            resolveParams5.rect = rp.rect.ContractedBy(num);
            resolveParams5.faction = faction;
            resolveParams5.floorOnlyIfTerrainSupports = new bool?(rp.floorOnlyIfTerrainSupports ?? true);
            BaseGen.symbolStack.Push("basePart_outdoors", resolveParams5, null);
            ResolveParams resolveParams6 = rp;
            resolveParams6.floorDef = TerrainDefOf.Bridge;
            resolveParams6.floorOnlyIfTerrainSupports = new bool?(rp.floorOnlyIfTerrainSupports ?? true);
            resolveParams6.allowBridgeOnAnyImpassableTerrain = new bool?(rp.allowBridgeOnAnyImpassableTerrain ?? true);
            BaseGen.symbolStack.Push("floor", resolveParams6, null);
        }
    }
}