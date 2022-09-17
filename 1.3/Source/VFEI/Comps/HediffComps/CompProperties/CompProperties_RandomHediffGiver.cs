using System.Collections.Generic;
using Verse;

namespace VFEI.Comps.ItemComps.Properties
{
    internal class CompProperties_RandomHediffGiver : HediffCompProperties
    {
        public CompProperties_RandomHediffGiver()
        {
            this.compClass = typeof(CompRandomHediffGiver);
        }

#pragma warning disable 0649
        public List<RecipeDef> allowedRecipeDefs;
    }
}