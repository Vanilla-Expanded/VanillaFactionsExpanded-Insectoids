using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse.AI;
using Verse;
using System.Linq;


namespace InsectoidBioengineering
{
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
            List<FloatMenuOption> list = new List<FloatMenuOption>();

            list.Add(new FloatMenuOption("VFEI_None".Translate(), delegate
            {
                Building_BioengineeringIncubator building = (Building_BioengineeringIncubator)this.building;
                building.theFirstGenomeIAmGoingToInsert = "None";

            }, MenuOptionPriority.Default, null, null, 29f, null, null));

            foreach (AcceptedGenomesDef element in DefDatabase<AcceptedGenomesDef>.AllDefs)
            {
                foreach (string thisGenome in element.genomes.Where(thisGenome => thisGenome != "None"))
                {

                    list.Add(new FloatMenuOption(thisGenome.Translate(), delegate
                    {
                        genome = map.listerThings.ThingsOfDef(DefDatabase<ThingDef>.GetNamed(thisGenome, true));
                        if (genome.Count > 0)
                        {

                            this.TryInsertFirstGenome();
                        }
                        else
                        {
                            Messages.Message("VFEI_NoGeneticMaterial".Translate(), null, MessageTypeDefOf.NegativeEvent, true);
                        }

                    }, MenuOptionPriority.Default, null, null, 29f, null, null));

                }
            }         
            Find.WindowStack.Add(new FloatMenu(list));
        }

        private void TryInsertFirstGenome()
        {
            Building_BioengineeringIncubator building = (Building_BioengineeringIncubator)this.building;
            //Log.Message("Inserting "+ genome.RandomElement().def.defName +" on "+building.ToString());
            building.ExpectingFirstGenome = true;
            building.theFirstGenomeIAmGoingToInsert = genome.RandomElement().def.defName;
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
            List<FloatMenuOption> list = new List<FloatMenuOption>();

            list.Add(new FloatMenuOption("VFEI_None".Translate(), delegate
            {
                Building_BioengineeringIncubator building = (Building_BioengineeringIncubator)this.building;
                building.theSecondGenomeIAmGoingToInsert = "None";

            }, MenuOptionPriority.Default, null, null, 29f, null, null));
            foreach (AcceptedGenomesDef element in DefDatabase<AcceptedGenomesDef>.AllDefs)
            {
                foreach (string thisGenome in element.genomes.Where(thisGenome => thisGenome != "None"))
                {

                    list.Add(new FloatMenuOption(thisGenome.Translate(), delegate
                    {
                        genome = map.listerThings.ThingsOfDef(DefDatabase<ThingDef>.GetNamed(thisGenome, true));
                        if (genome.Count > 0)
                        {

                            this.TryInsertSecondGenome();
                        }
                        else
                        {
                            Messages.Message("VFEI_NoGeneticMaterial".Translate(), null, MessageTypeDefOf.NegativeEvent, true);
                        }

                    }, MenuOptionPriority.Default, null, null, 29f, null, null));

                }
            }
            Find.WindowStack.Add(new FloatMenu(list));
        }

        private void TryInsertSecondGenome()
        {
            Building_BioengineeringIncubator building = (Building_BioengineeringIncubator)this.building;
            //Log.Message("Inserting " + genome.RandomElement().def.defName + " on " + building.ToString());
            building.ExpectingSecondGenome = true;
            building.theSecondGenomeIAmGoingToInsert = genome.RandomElement().def.defName;
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
            List<FloatMenuOption> list = new List<FloatMenuOption>();

            list.Add(new FloatMenuOption("VFEI_None".Translate(), delegate
            {
                Building_BioengineeringIncubator building = (Building_BioengineeringIncubator)this.building;
                building.theThirdGenomeIAmGoingToInsert = "None";

            }, MenuOptionPriority.Default, null, null, 29f, null, null));
            foreach (AcceptedGenomesDef element in DefDatabase<AcceptedGenomesDef>.AllDefs)
            {
                foreach (string thisGenome in element.genomes.Where(thisGenome => thisGenome != "None"))
                {

                    list.Add(new FloatMenuOption(thisGenome.Translate(), delegate
                    {
                        genome = map.listerThings.ThingsOfDef(DefDatabase<ThingDef>.GetNamed(thisGenome, true));
                        if (genome.Count > 0)
                        {

                            this.TryInsertThirdGenome();
                        }
                        else
                        {
                            Messages.Message("VFEI_NoGeneticMaterial".Translate(), null, MessageTypeDefOf.NegativeEvent, true);
                        }

                    }, MenuOptionPriority.Default, null, null, 29f, null, null));

                }
            }
            Find.WindowStack.Add(new FloatMenu(list));
        }

        private void TryInsertThirdGenome()
        {
            Building_BioengineeringIncubator building = (Building_BioengineeringIncubator)this.building;
            //Log.Message("Inserting " + genome.RandomElement().def.defName + " on " + building.ToString());
            building.ExpectingThirdGenome = true;
            building.theThirdGenomeIAmGoingToInsert = genome.RandomElement().def.defName;
        }




    }


}

