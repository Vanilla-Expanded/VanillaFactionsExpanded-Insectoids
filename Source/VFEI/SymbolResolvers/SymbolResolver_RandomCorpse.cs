using RimWorld;
using RimWorld.BaseGen;
using Verse;

namespace VFEI
{
    internal class SymbolResolver_RandomCorpse : SymbolResolver
    {
        public override void Resolve(ResolveParams rp)
        {
            Map map = BaseGen.globalSettings.map;
            for (int i = 0; i < Rand.RangeInclusive(10, 25); i++)
            {
                IntVec3 randomCell = rp.rect.RandomCell;
                if (randomCell.Standable(map) && randomCell.GetFirstItem(map) == null && randomCell.GetFirstPawn(map) == null && randomCell.GetFirstBuilding(map) == null)
                {
                    Pawn pawn = PawnGenerator.GeneratePawn(PawnKindDefOf.Villager, Find.FactionManager.RandomEnemyFaction(false, false, false));
                    pawn.Kill(new DamageInfo(DamageDefOf.Cut, 9999));
                    Corpse corpse = pawn.Corpse;
                    corpse.timeOfDeath = 10000;
                    corpse.TryGetComp<CompRottable>().RotImmediately();
                    GenSpawn.Spawn(corpse, randomCell, map);
                    for (int x = 0; x < 5; x++)
                    {
                        IntVec3 rNext = new IntVec3();
                        RCellFinder.TryFindRandomCellNearWith(randomCell, ni => ni.Walkable(map), map, out rNext, 1, 3);
                        GenSpawn.Spawn(ThingDefOf.Filth_CorpseBile, rNext, map);
                    }
                }
            }
        }
    }
}