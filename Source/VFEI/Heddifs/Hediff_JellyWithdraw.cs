using RimWorld;
using Verse;
using Verse.Sound;

namespace VFEI
{
    internal class Hediff_JellyWithdraw : Hediff_Addiction
    {
        public override void ExposeData()
        {
            base.ExposeData();
        }

        public override void Tick()
        {
            base.Tick();
            if ((pawn.RaceProps != null && pawn.RaceProps.Humanlike) || pawn.Faction != Faction.OfPlayer)
            {
                Need.CurLevel = 1;
            }
            else if (Need?.CurCategory != null && Need.CurCategory == DrugDesireCategory.Withdrawal)
            {
                IntVec3 colonistLoc = this.pawn.Position;
                Name colonistName = this.pawn.Name;
                Map map = this.pawn.Map;
                this.pawn.Destroy(DestroyMode.Vanish);
                PawnKindDef pawnKindDef = PawnKindDefOf.Megaspider;
                Pawn megaspider = PawnGenerator.GeneratePawn(pawnKindDef, this.pawn.Faction);
                megaspider.Name = colonistName;
                GenSpawn.Spawn(megaspider, colonistLoc, map, WipeMode.Vanish);
                SoundDefOf.Hive_Spawn.PlayOneShot(new TargetInfo(this.pawn));
                for (int i = 0; i < 20; i++)
                {
                    CellFinder.TryFindRandomReachableCellNear(colonistLoc, map, 4, TraverseParms.For(TraverseMode.NoPassClosedDoors, Danger.Deadly, false), null, null, out IntVec3 c);
                    int flag2 = Rand.RangeInclusive(0, 2);
                    if (flag2 == 1)
                    {
                        FilthMaker.TryMakeFilth(c, map, VFEIDefOf.Filth_BloodInsect);
                    }
                    else
                    {
                        FilthMaker.TryMakeFilth(c, map, ThingDefOf.Filth_Blood);
                    }
                }
                TaggedString text = "TransformationLetter".Translate(colonistName.ToStringFull);
                Find.LetterStack.ReceiveLetter("TransformationLLabel".Translate(colonistName.ToStringFull), text, LetterDefOf.NegativeEvent, megaspider);
            }
        }
    }
}