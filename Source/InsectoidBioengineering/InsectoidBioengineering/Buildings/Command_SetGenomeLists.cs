using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse.AI;
using Verse;
using System.Linq;


namespace InsectoidBioengineering
{

    public static class GenomeListClass
    {
        public static void Process(Building_BioengineeringIncubator building, int slot)
        {
            List<FloatMenuOption> list = new List<FloatMenuOption>();

            list.Add(new FloatMenuOption("VFEI_None".Translate(), delegate
            {
                switch (slot)
                {
                    case 1:
                        building.theFirstGenomeIAmGoingToInsert = "None";
                        break;
                    case 2:
                        building.theSecondGenomeIAmGoingToInsert = "None";
                        break;
                    case 3:
                        building.theThirdGenomeIAmGoingToInsert = "None";
                        break;
                }
            }, MenuOptionPriority.Default, null, null, 29f, null, null));

            foreach (AcceptedGenomesDef element in DefDatabase<AcceptedGenomesDef>.AllDefs)
            {
                foreach (string thisGenome in element.genomes.Where(thisGenome => thisGenome != "None"))
                {
                    list.Add(new FloatMenuOption(thisGenome.Translate(), delegate
                    {
                        switch (slot)
                        {
                            case 1:
                                building.ExpectingFirstGenome = true;
                                building.theFirstGenomeIAmGoingToInsert = thisGenome;
                                break;
                            case 2:
                                building.ExpectingSecondGenome = true;
                                building.theSecondGenomeIAmGoingToInsert = thisGenome;
                                break;
                            case 3:
                                building.ExpectingThirdGenome = true;
                                building.theThirdGenomeIAmGoingToInsert = thisGenome;
                                break;
                        }
                    }, MenuOptionPriority.Default, null, null, 29f, null, null));
                }
            }
            Find.WindowStack.Add(new FloatMenu(list));
        }

    }


    [StaticConstructorOnStartup]
    public class Command_SetFirstGenomeList : Command
    {

        public Map map;
        public Building_BioengineeringIncubator building;
        public List<Thing> genome;



        public Command_SetFirstGenomeList()
        {
            foreach (object obj in Find.Selector.SelectedObjects)
            {
                building = obj as Building_BioengineeringIncubator;
                if (building != null)
                {
                    foreach (AcceptedGenomesDef element in DefDatabase<AcceptedGenomesDef>.AllDefs)
                    {
                        foreach (string genome in element.genomes)
                        {
                            if (building.theFirstGenomeIAmGoingToInsert == genome)
                            {
                                icon = ContentFinder<Texture2D>.Get("Things/Item/GenomeIcons/" + genome, true);
                                defaultLabel = (genome + "_InsertFirstGenome").Translate();
                            }
                        }
                    }
                }
            }
        }

        public override void ProcessInput(Event ev)
        {
            base.ProcessInput(ev);
            GenomeListClass.Process(this.building, 1);

        }
    }


    [StaticConstructorOnStartup]
    public class Command_SetSecondGenomeList : Command
    {
        public Map map;
        public Building_BioengineeringIncubator building;
        public List<Thing> genome;

        public Command_SetSecondGenomeList()
        {
            foreach (object obj in Find.Selector.SelectedObjects)
            {
                building = obj as Building_BioengineeringIncubator;
                if (building != null)
                {
                    foreach (AcceptedGenomesDef element in DefDatabase<AcceptedGenomesDef>.AllDefs)
                    {
                        foreach (string genome in element.genomes)
                        {
                            if (building.theSecondGenomeIAmGoingToInsert == genome)
                            {
                                icon = ContentFinder<Texture2D>.Get("Things/Item/GenomeIcons/" + genome, true);
                                defaultLabel = (genome + "_InsertSecondGenome").Translate();
                            }
                        }
                    }
                }
            }
        }

        public override void ProcessInput(Event ev)
        {
            base.ProcessInput(ev);
            GenomeListClass.Process(this.building, 2);
        }

    }

    [StaticConstructorOnStartup]
    public class Command_SetThirdGenomeList : Command
    {

        public Map map;
        public Building_BioengineeringIncubator building;
        public List<Thing> genome;



        public Command_SetThirdGenomeList()
        {
            foreach (object obj in Find.Selector.SelectedObjects)
            {
                building = obj as Building_BioengineeringIncubator;
                if (building != null)
                {
                    foreach (AcceptedGenomesDef element in DefDatabase<AcceptedGenomesDef>.AllDefs)
                    {
                        foreach (string genome in element.genomes)
                        {
                            if (building.theThirdGenomeIAmGoingToInsert == genome)
                            {
                                icon = ContentFinder<Texture2D>.Get("Things/Item/GenomeIcons/" + genome, true);
                                defaultLabel = (genome + "_InsertThirdGenome").Translate();
                            }
                        }
                    }
                }
            }
        }

        public override void ProcessInput(Event ev)
        {
            base.ProcessInput(ev);
            GenomeListClass.Process(this.building, 3);
        }





    }


}

