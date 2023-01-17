using RimWorld;
using Verse;
using Verse.Sound;

namespace VFEI
{
    public class HediffCompProperties_TurnToInsect : HediffCompProperties
    {
        public HediffCompProperties_TurnToInsect() => compClass = typeof(HediffCompTurnToInsect);
    }

    public class HediffCompTurnToInsect : HediffComp
    {
        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            var pawn = Pawn;

            if (!pawn.IsHashIntervalTick(200))
                return;

            var faction = pawn.Faction;
            if (pawn.RaceProps != null && pawn.RaceProps.Humanlike && faction == Faction.OfPlayer)
            {
                var need = (Need_Chemical)pawn.needs.TryGetNeed(VFEIDefOf.VFEI_Chemical_RoyalJelly);
                if (need != null && need.CurCategory == DrugDesireCategory.Withdrawal)
                {
                    var name = pawn.Name;
                    var loc = pawn.Position;
                    var map = pawn.Map;

                    pawn.Destroy(DestroyMode.Vanish);
                    var megaspider = PawnGenerator.GeneratePawn(PawnKindDefOf.Megaspider, faction);
                    megaspider.Name = name;

                    GenSpawn.Spawn(megaspider, loc, map, WipeMode.Vanish);
                    SoundDefOf.Hive_Spawn.PlayOneShot(new TargetInfo(pawn));

                    for (int i = 0; i < 20; i++)
                    {
                        CellFinder.TryFindRandomReachableCellNear(loc, map, 4, TraverseParms.For(TraverseMode.NoPassClosedDoors, Danger.Deadly, false), null, null, out IntVec3 c);
                        if (Rand.RangeInclusive(0, 2) == 1)
                        {
                            FilthMaker.TryMakeFilth(c, map, VFEIDefOf.Filth_BloodInsect);
                        }
                        else
                        {
                            FilthMaker.TryMakeFilth(c, map, ThingDefOf.Filth_Blood);
                        }
                    }

                    var nameStr = name.ToStringFull;
                    Find.LetterStack.ReceiveLetter("TransformationLLabel".Translate(nameStr), "TransformationLetter".Translate(nameStr), LetterDefOf.NegativeEvent, megaspider);
                }
            }
        }
    }
}
