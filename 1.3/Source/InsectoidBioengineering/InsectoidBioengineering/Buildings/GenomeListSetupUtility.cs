using RimWorld;
using Verse;

using UnityEngine;


namespace InsectoidBioengineering
{
    public static class GenomeListSetupUtility
    {
        public static Command_SetFirstGenomeList SetFirstGenomeListCommand(Building_BioengineeringIncubator passingbuilding, Map passingMap)
        {
            return new Command_SetFirstGenomeList()
            {
                defaultDesc = "VFEI_InsertFirstGenomeDesc".Translate(),
               
               // icon = ContentFinder<Texture2D>.Get("Things/Item/VFEI_NoGenome", true),
                hotKey = KeyBindingDefOf.Misc1,
                map = passingMap,
                building = passingbuilding

            };
        }

        public static Command_SetSecondGenomeList SetSecondGenomeListCommand(Building_BioengineeringIncubator passingbuilding, Map passingMap)
        {
            return new Command_SetSecondGenomeList()
            {
                defaultDesc = "VFEI_InsertSecondGenomeDesc".Translate(),                             
                hotKey = KeyBindingDefOf.Misc1,
                map = passingMap,
                building = passingbuilding

            };
        }

        public static Command_SetThirdGenomeList SetThirdGenomeListCommand(Building_BioengineeringIncubator passingbuilding, Map passingMap)
        {
            return new Command_SetThirdGenomeList()
            {
                defaultDesc = "VFEI_InsertThirdGenomeDesc".Translate(),              
                hotKey = KeyBindingDefOf.Misc1,
                map = passingMap,
                building = passingbuilding

            };
        }
    }
}
