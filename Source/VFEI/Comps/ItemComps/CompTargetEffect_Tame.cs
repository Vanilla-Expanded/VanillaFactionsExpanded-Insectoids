using System;
using RimWorld;
using Verse;

namespace VFEI
{
    public class CompTargetEffect_Tame : CompTargetEffect
    {
        public override void DoEffectOn(Pawn user, Thing target)
        {
            Pawn pawn = (Pawn)target;
            if (pawn.AnimalOrWildMan() && pawn.Faction == null && pawn.RaceProps.wildness < 1f && !pawn.IsPrisonerInPrisonCell())
            {
                if (pawn.Dead)
                {
                    return;
                }
                if (this.RandomNumber(1, 10) == 1)
                {
                    pawn.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.Berserk);

                    string text;
                    text = "LetterArtifactTameFail".Translate(pawn).CapitalizeFirst();
                    Find.LetterStack.ReceiveLetter("LetterLabelForcedTameFail".Translate(pawn.KindLabel, pawn).CapitalizeFirst(), text, LetterDefOf.NegativeEvent, pawn, null, null);
                }
                else
                {
                    pawn.SetFaction(Faction.OfPlayer, null);

                    TaggedString text;
                    text = "LetterArtifactTame".Translate(pawn).CapitalizeFirst();
                    Find.LetterStack.ReceiveLetter("LetterLabelForcedTame".Translate(pawn.KindLabel, pawn).CapitalizeFirst(), text, LetterDefOf.PositiveEvent, pawn, null, null);
                }
            }
            else
            {
                return;
            }
        }

        public int RandomNumber(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max);
        }
    }
}