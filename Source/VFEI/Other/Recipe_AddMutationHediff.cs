using RimWorld;
using System.Collections.Generic;
using Verse;

namespace VFEI.Other
{
    internal class Recipe_AddMutationHediff : RecipeWorker
    {
        public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
        {
            pawn.health.AddHediff(this.recipe.addsHediff);
        }
    }
}