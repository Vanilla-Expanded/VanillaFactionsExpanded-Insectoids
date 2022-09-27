using RimWorld;
using System.Collections.Generic;
using Verse;

namespace VFEI.Comps.ItemComps
{
    internal class CompRandomHediffGiver : HediffComp
    {
        public Properties.CompProperties_RandomHediffGiver Props
        {
            get
            {
                return (Properties.CompProperties_RandomHediffGiver)this.props;
            }
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            int MaxTryNumb = 20;
            bool mutate = false;
            for (int i = 0; i < MaxTryNumb; i++)
            {
                RecipeDef randRecipe = this.Props.allowedRecipeDefs.RandomElement();
                BodyPartRecord bodyPartRecord;
                bool isImplant;
                mutate = ApplyMutation(randRecipe, out bodyPartRecord, out isImplant);
                if (mutate)
                {
                    if (isImplant || randRecipe.defName == "VFEI_InstallSynapticCerebellum")
                    {
                        MaxTryNumb = i;
                        this.Pawn.health.AddHediff(randRecipe.addsHediff, bodyPartRecord);

                        string label1 = "MutationOutcome".Translate();
                        string text1 = "MutationOutcomeLetter".Translate(randRecipe.label.Substring(8));
                        Find.LetterStack.ReceiveLetter(label1, text1, LetterDefOf.NeutralEvent, new TargetInfo(this.Pawn.Position, this.Pawn.Map, false), null, null);

                        this.Pawn.health.RemoveHediff(this.parent);
                    }
                    else if (!isImplant)
                    {
                        MaxTryNumb = i;
                        this.Pawn.health.RestorePart(bodyPartRecord);
                        this.Pawn.health.AddHediff(randRecipe.addsHediff, bodyPartRecord);

                        string label1 = "MutationOutcome".Translate();
                        string text1 = "MutationOutcomeLetter".Translate(randRecipe.label.Substring(8));
                        Find.LetterStack.ReceiveLetter(label1, text1, LetterDefOf.NeutralEvent, new TargetInfo(this.Pawn.Position, this.Pawn.Map, false), null, null);

                        this.Pawn.health.RemoveHediff(this.parent);
                    }
                }
            }
            if (!mutate)
            {
                string label = "FailedMutation".Translate();
                string text = "FailedMutationLetter".Translate();
                Find.LetterStack.ReceiveLetter(label, text, LetterDefOf.NeutralEvent, new TargetInfo(this.Pawn.Position, this.Pawn.Map, false), null, null);

                this.Pawn.health.RemoveHediff(this.parent);
            }
        }

        private bool ApplyMutation(RecipeDef recipeDef, out BodyPartRecord bodyPartRecord, out bool isImplant)
        {
            isImplant = false;
            bodyPartRecord = MedicalRecipesUtility.GetFixedPartsToApplyOn(recipeDef, this.Pawn).RandomElement();
            if (recipeDef.addsHediff.addedPartProps == null || recipeDef.addsHediff == VFEIDefOf.VFEI_SynapticCerebellum) // IsImplant?
            {
                if (!this.Pawn.health.hediffSet.HasHediff(recipeDef.addsHediff)) // If don't already have it
                {
                    isImplant = true;
                    return true; // Mutate
                }
                return false; // Don't mutate
            }
            else // Not an implant
            {
                List<Hediff> hediffs = this.Pawn.health.hediffSet.hediffs;
                for (int i = hediffs.Count - 1; i >= 0; i--) // Check each part for already existing part
                {
                    if (hediffs[i].Part == bodyPartRecord && !hediffs[i].def.tendable) // If part have heddiff, and it's not tentable
                    {
                        return false; // Don't mutate
                    }
                }
                return true; // Mutate
            }
        }
    }
}