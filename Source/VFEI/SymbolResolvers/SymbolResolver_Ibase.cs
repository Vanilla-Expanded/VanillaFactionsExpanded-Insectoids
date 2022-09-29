using RimWorld;
using RimWorld.BaseGen;
using Verse;
using Verse.AI.Group;

namespace VFEI
{
    internal class SymbolResolver_Ibase : SymbolResolver
    {
        public static readonly FloatRange DefaultPawnsPoints = new FloatRange(1150f, 1600f);

        public override void Resolve(ResolveParams rp)
        {
            Map map = BaseGen.globalSettings.map;
            Faction faction = map.ParentFaction;
            int num = 0;
            if (rp.edgeDefenseWidth != null)
            {
                num = rp.edgeDefenseWidth.Value;
            }
            else if (rp.rect.Width >= 20 && rp.rect.Height >= 20 && (faction.def.techLevel >= TechLevel.Industrial || Rand.Bool))
            {
                num = (Rand.Bool ? 2 : 4);
            }
            float num2 = (float)rp.rect.Area / 144f * 0.17f;
            BaseGen.globalSettings.minEmptyNodes = ((num2 < 1f) ? 0 : GenMath.RoundRandom(num2));
            Lord singlePawnLord = rp.singlePawnLord ?? LordMaker.MakeNewLord(faction, new LordJob_DefendBase(faction, rp.rect.CenterCell), map, null);
            TraverseParms traverseParms = TraverseParms.For(TraverseMode.PassDoors, Danger.Deadly, false);
            ResolveParams resolveParams = rp;
            resolveParams.rect = rp.rect;
            resolveParams.faction = faction;
            resolveParams.singlePawnLord = singlePawnLord;
            resolveParams.pawnGroupKindDef = (rp.pawnGroupKindDef ?? PawnGroupKindDefOf.Settlement);
            resolveParams.singlePawnSpawnCellExtraPredicate = (rp.singlePawnSpawnCellExtraPredicate ?? ((IntVec3 x) => map.reachability.CanReachMapEdge(x, traverseParms)));
            if (resolveParams.pawnGroupMakerParams == null)
            {
                resolveParams.pawnGroupMakerParams = new PawnGroupMakerParms();
                resolveParams.pawnGroupMakerParams.tile = map.Tile;
                resolveParams.pawnGroupMakerParams.faction = faction;
                resolveParams.pawnGroupMakerParams.points = (rp.settlementPawnGroupPoints ?? SymbolResolver_Settlement.DefaultPawnsPoints.RandomInRange);
                resolveParams.pawnGroupMakerParams.inhabitants = true;
                resolveParams.pawnGroupMakerParams.seed = rp.settlementPawnGroupSeed;
            }
            BaseGen.symbolStack.Push("pawnGroup", resolveParams, null);
            BaseGen.symbolStack.Push("insectoidBaseLightning", rp, null);

            PawnGenerationRequest value = new PawnGenerationRequest(VFEIDefOf.VFEI_Insectoid_Queen, faction);
            ResolveParams resolveParams7 = rp;
            resolveParams7.faction = faction;
            resolveParams7.singlePawnGenerationRequest = new PawnGenerationRequest?(value);
            resolveParams7.rect = rp.rect;
            resolveParams7.singlePawnLord = singlePawnLord;
            BaseGen.symbolStack.Push("pawn", resolveParams7, null);
            ResolveParams resolveParams4 = rp;
            resolveParams4.rect = rp.rect.ContractedBy(num);
            resolveParams4.faction = faction;
            BaseGen.symbolStack.Push("ensureCanReachMapEdge", resolveParams4, null);
            ResolveParams resolveParams5 = rp;
            resolveParams5.rect = rp.rect.ContractedBy(num);
            resolveParams5.faction = faction;
            resolveParams5.floorOnlyIfTerrainSupports = new bool?(rp.floorOnlyIfTerrainSupports ?? true);
            resolveParams5.wallStuff = (rp.wallStuff ?? BaseGenUtility.RandomCheapWallStuff(rp.faction, true));
            resolveParams5.chanceToSkipWallBlock = new float?(rp.chanceToSkipWallBlock ?? 0.1f);
            resolveParams5.clearEdificeOnly = new bool?(rp.clearEdificeOnly ?? true);
            resolveParams5.noRoof = new bool?(rp.noRoof ?? true);
            resolveParams5.chanceToSkipFloor = new float?(rp.chanceToSkipFloor ?? 0.1f);
            resolveParams5.filthDef = ThingDefOf.Filth_Slime;
            resolveParams5.filthDensity = new FloatRange(0.5f, 1f);
            resolveParams5.cultivatedPlantDef = null;
            BaseGen.symbolStack.Push("basePart_outdoors", resolveParams5, null);
            ResolveParams resolveParams6 = rp;
            resolveParams6.floorDef = TerrainDefOf.Bridge;
            resolveParams6.floorOnlyIfTerrainSupports = new bool?(rp.floorOnlyIfTerrainSupports ?? true);
            resolveParams6.allowBridgeOnAnyImpassableTerrain = new bool?(rp.allowBridgeOnAnyImpassableTerrain ?? true);
            resolveParams6.chanceToSkipFloor = new float?(rp.chanceToSkipFloor ?? 0.1f);
            resolveParams6.filthDensity = new FloatRange(0.5f, 1f);
            BaseGen.symbolStack.Push("floor", resolveParams6, null);
            BaseGen.symbolStack.Push("insectoidBaseRCorpse", rp, null);
        }
    }
}