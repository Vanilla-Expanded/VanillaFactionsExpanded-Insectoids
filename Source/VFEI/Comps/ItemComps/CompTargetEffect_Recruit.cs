using RimWorld;
using RimWorld.Planet;
using Verse;

namespace VFEI
{
    public class CompTargetEffect_Recruit : CompTargetEffect
    {
        public override void DoEffectOn(Pawn user, Thing target)
        {
            Pawn pawn = (Pawn)target;
            if (!pawn.AnimalOrWildMan() && (pawn.Faction != null || pawn.IsPrisoner))
            {
                if (pawn.Dead)
                {
                    return;
                }
                else
                {
                    pawn.Faction.TryAffectGoodwillWith(Faction.OfPlayer, -50, lookTarget: new GlobalTargetInfo?(pawn));

                    pawn.SetFaction(Faction.OfPlayer, user);
                    Find.LetterStack.ReceiveLetter(TranslatorFormattedStringExtensions.Translate("LetterLabelForcedRecruit", pawn), TranslatorFormattedStringExtensions.Translate("LetterArtifactRecruit", pawn), LetterDefOf.PositiveEvent, pawn);
                }
            }
            else return;
        }
    }
}