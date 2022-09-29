

using Verse;
using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using System.Diagnostics;
using Verse.Sound;

namespace InsectoidBioengineering
{
    public class Building_BioengineeringIncubator : Building, IThingHolder
    {
        private System.Random rand = new System.Random();
        public ThingOwner innerContainerFirstGenome = null;
        public ThingOwner innerContainerSecondGenome = null;
        public ThingOwner innerContainerThirdGenome = null;

        public Map map;

        public bool ExpectingFirstGenome = false;
        public bool ExpectingSecondGenome = false;
        public bool ExpectingThirdGenome = false;

        public bool StartInsertionJobs = false;

        public string theFirstGenomeIAmGoingToInsert = "None";
        public string theSecondGenomeIAmGoingToInsert = "None";
        public string theThirdGenomeIAmGoingToInsert = "None";

        protected bool contentsKnownFirst = false;
        protected bool contentsKnownSecond = false;
        protected bool contentsKnownThird = false;

        public bool IncubationStarted = false;
        public int IncubationCounter = 0;
        public string IncubatingInsectoid = "";
        public bool IsHostile = false;

        public const int rareTicksPerDay = 240;
        public const int ticksPerDay = 60000;

        public CompPowerTrader compPowerTrader;

        public Building_BioengineeringIncubator()
        {
            this.innerContainerFirstGenome = new ThingOwner<Thing>(this, false, LookMode.Deep);
            this.innerContainerSecondGenome = new ThingOwner<Thing>(this, false, LookMode.Deep);
            this.innerContainerThirdGenome = new ThingOwner<Thing>(this, false, LookMode.Deep);

        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            this.compPowerTrader = base.GetComp<CompPowerTrader>();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Deep.Look<ThingOwner>(ref this.innerContainerFirstGenome, "innerContainerFirstGenome", new object[]
            {
                this
            });
            Scribe_Deep.Look<ThingOwner>(ref this.innerContainerSecondGenome, "innerContainerSecondGenome", new object[]
            {
                this
            });
            Scribe_Deep.Look<ThingOwner>(ref this.innerContainerThirdGenome, "innerContainerThirdGenome", new object[]
            {
                this
            });

            Scribe_Values.Look<string>(ref this.theFirstGenomeIAmGoingToInsert, "theFirstGenomeIAmGoingToInsert", "None", false);
            Scribe_Values.Look<string>(ref this.theSecondGenomeIAmGoingToInsert, "theSecondGenomeIAmGoingToInsert", "None", false);
            Scribe_Values.Look<string>(ref this.theThirdGenomeIAmGoingToInsert, "theThirdGenomeIAmGoingToInsert", "None", false);
            Scribe_Values.Look<bool>(ref this.ExpectingFirstGenome, "ExpectingFirstGenome", false, false);
            Scribe_Values.Look<bool>(ref this.ExpectingSecondGenome, "ExpectingSecondGenome", false, false);
            Scribe_Values.Look<bool>(ref this.ExpectingThirdGenome, "ExpectingThirdGenome", false, false);
            Scribe_Values.Look<bool>(ref this.StartInsertionJobs, "StartInsertionJobs", false, false);
            Scribe_Values.Look<bool>(ref this.contentsKnownFirst, "contentsKnownFirst", false, false);
            Scribe_Values.Look<bool>(ref this.contentsKnownSecond, "contentsKnownSecond", false, false);
            Scribe_Values.Look<bool>(ref this.contentsKnownThird, "contentsKnownThird", false, false);
            Scribe_Values.Look<bool>(ref this.IncubationStarted, "IncubationStarted", false, false);
            Scribe_Values.Look<int>(ref this.IncubationCounter, "IncubationCounter", 0, false);
            Scribe_Values.Look<string>(ref this.IncubatingInsectoid, "IncubatingInsectoid", "", false);
            Scribe_Values.Look<bool>(ref this.IsHostile, "IsHostile", false, false);




        }

        public ThingOwner GetDirectlyHeldThings()
        {
            return this.innerContainerFirstGenome;
        }

        public void GetChildHolders(List<IThingHolder> outChildren)
        {
            ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, this.GetDirectlyHeldThings());
        }

        public virtual void EjectContentsFirst()
        {
            this.innerContainerFirstGenome.TryDropAll(this.InteractionCell, base.Map, ThingPlaceMode.Near, null, null);
            this.contentsKnownFirst = true;
            this.TickRare();
        }

        public virtual void EjectContentsSecond()
        {
            this.innerContainerSecondGenome.TryDropAll(this.InteractionCell, base.Map, ThingPlaceMode.Near, null, null);
            this.contentsKnownSecond = true;
            this.TickRare();
        }

        public virtual void EjectContentsThird()
        {
            this.innerContainerThirdGenome.TryDropAll(this.InteractionCell, base.Map, ThingPlaceMode.Near, null, null);
            this.contentsKnownThird = true;
            this.TickRare();
        }

        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {

            EjectContentsFirst();
            EjectContentsSecond();
            EjectContentsThird();
            base.Destroy(mode);
        }

        public bool TryAcceptFirstGenome(Thing thing, bool allowSpecialEffects = true)
        {
            bool result;

            bool flag;
            if (thing.holdingOwner != null)
            {
                thing.holdingOwner.TryTransferToContainer(thing, this.innerContainerFirstGenome, thing.stackCount, true);
                flag = true;
            }
            else
            {
                flag = this.innerContainerFirstGenome.TryAdd(thing, true);
            }
            if (flag)
            {
                if (thing.Faction != null && thing.Faction.IsPlayer)
                {
                    this.contentsKnownFirst = true;
                }
                result = true;
            }
            else
            {
                result = false;
            }
            this.TickRare();
            return result;
        }

        public bool TryAcceptSecondGenome(Thing thing, bool allowSpecialEffects = true)
        {
            bool result;

            bool flag;
            if (thing.holdingOwner != null)
            {
                thing.holdingOwner.TryTransferToContainer(thing, this.innerContainerSecondGenome, thing.stackCount, true);
                flag = true;
            }
            else
            {
                flag = this.innerContainerSecondGenome.TryAdd(thing, true);
            }
            if (flag)
            {
                if (thing.Faction != null && thing.Faction.IsPlayer)
                {
                    this.contentsKnownSecond = true;
                }
                result = true;
            }
            else
            {
                result = false;
            }
            this.TickRare();
            return result;
        }

        public bool TryAcceptThirdGenome(Thing thing, bool allowSpecialEffects = true)
        {
            bool result;

            bool flag;
            if (thing.holdingOwner != null)
            {
                thing.holdingOwner.TryTransferToContainer(thing, this.innerContainerThirdGenome, thing.stackCount, true);
                flag = true;
            }
            else
            {
                flag = this.innerContainerThirdGenome.TryAdd(thing, true);
            }
            if (flag)
            {
                if (thing.Faction != null && thing.Faction.IsPlayer)
                {
                    this.contentsKnownThird = true;
                }
                result = true;
            }
            else
            {
                result = false;
            }
            this.TickRare();
            return result;
        }




        [DebuggerHidden]
        public override IEnumerable<Gizmo> GetGizmos()
        {
            map = this.Map;
            foreach (Gizmo g in base.GetGizmos())
            {
                yield return g;
            }
            if (!IncubationStarted)
            {
                yield return GenomeListSetupUtility.SetFirstGenomeListCommand(this, map);
                yield return GenomeListSetupUtility.SetSecondGenomeListCommand(this, map);
                yield return GenomeListSetupUtility.SetThirdGenomeListCommand(this, map);
                if (!this.StartInsertionJobs)
                {
                    Command_Action RB_Gizmo_StartInsertion = new Command_Action();
                    RB_Gizmo_StartInsertion.action = delegate
                    {
                        if ((ExpectingFirstGenome || ExpectingSecondGenome || ExpectingThirdGenome) || (innerContainerFirstGenome.Count > 0
                        || innerContainerSecondGenome.Count > 0 || innerContainerThirdGenome.Count > 0))
                        {
                            StartInsertionJobs = true;
                        }
                        else
                        {
                            Messages.Message("VFEI_SelectAtLeastOne".Translate(), null, MessageTypeDefOf.NegativeEvent, true);
                        }
                    };
                    RB_Gizmo_StartInsertion.defaultLabel = "VFEI_StartInsertion".Translate();
                    RB_Gizmo_StartInsertion.defaultDesc = "VFEI_StartInsertionDesc".Translate();
                    RB_Gizmo_StartInsertion.icon = ContentFinder<Texture2D>.Get("UI/VFEI_InsertGenomes", true);
                    yield return RB_Gizmo_StartInsertion;



                    Command_Action RB_Gizmo_RemoveAllGenes = new Command_Action();
                    RB_Gizmo_RemoveAllGenes.action = delegate
                    {
                        EjectContentsFirst();
                        EjectContentsSecond();
                        EjectContentsThird();
                        ExpectingFirstGenome = false;
                        ExpectingSecondGenome = false;
                        ExpectingThirdGenome = false;
                        StartInsertionJobs = false;
                        theFirstGenomeIAmGoingToInsert = "None";
                        theSecondGenomeIAmGoingToInsert = "None";
                        theThirdGenomeIAmGoingToInsert = "None";

                    };
                    RB_Gizmo_RemoveAllGenes.defaultLabel = "VFEI_RemoveAllGenes".Translate();
                    RB_Gizmo_RemoveAllGenes.defaultDesc = "VFEI_RemoveAllGenesDesc".Translate();
                    RB_Gizmo_RemoveAllGenes.icon = ContentFinder<Texture2D>.Get("UI/VFEI_RemoveGenomes", true);
                    yield return RB_Gizmo_RemoveAllGenes;

                }
                else
                {
                    Command_Action RB_Gizmo_CancelJobs = new Command_Action();
                    RB_Gizmo_CancelJobs.action = delegate
                    {
                        StartInsertionJobs = false;

                    };
                    RB_Gizmo_CancelJobs.defaultLabel = "VFEI_CancelJobs".Translate();
                    RB_Gizmo_CancelJobs.defaultDesc = "VFEI_CancelJobsDesc".Translate();
                    RB_Gizmo_CancelJobs.icon = ContentFinder<Texture2D>.Get("UI/VFEI_CancelJobs", true);
                    yield return RB_Gizmo_CancelJobs;

                    Command_Action RB_Gizmo_Engage = new Command_Action();
                    RB_Gizmo_Engage.action = delegate
                    {
                        if (ExpectingFirstGenome || ExpectingSecondGenome || ExpectingThirdGenome)
                        {
                            Messages.Message("VFEI_WaitTillJobsEnd".Translate(), null, MessageTypeDefOf.NegativeEvent, true);
                        }
                        else if (!compPowerTrader.PowerOn) {
                            Messages.Message("VFEI_NotPowered".Translate(), null, MessageTypeDefOf.NegativeEvent, true);

                        }
                        else 
                        {
                            string IncubationResult = this.BeginIncubation(theFirstGenomeIAmGoingToInsert, theSecondGenomeIAmGoingToInsert, theThirdGenomeIAmGoingToInsert);
                            if (IncubationResult != "") {
                                this.innerContainerFirstGenome.ClearAndDestroyContents();
                                this.innerContainerSecondGenome.ClearAndDestroyContents();
                                this.innerContainerThirdGenome.ClearAndDestroyContents();
                                theFirstGenomeIAmGoingToInsert = "None";
                                theSecondGenomeIAmGoingToInsert = "None";
                                theThirdGenomeIAmGoingToInsert = "None";
                                StartInsertionJobs = false;
                            }
                            else {
                                Messages.Message("VFEI_CombinationNoResults".Translate(), null, MessageTypeDefOf.NegativeEvent, true);

                            }

                            
                        }

                    };
                    RB_Gizmo_Engage.defaultLabel = "VFEI_Engage".Translate();
                    RB_Gizmo_Engage.defaultDesc = "VFEI_EngageDesc".Translate();
                    RB_Gizmo_Engage.icon = ContentFinder<Texture2D>.Get("UI/VFEI_Engage", true);
                    yield return RB_Gizmo_Engage;
                }

            }
           

        }

        public string BeginIncubation(string genome1, string genome2, string genome3) {

            IncubatingInsectoid = "";
            foreach (InsectoidCombinationDef element in DefDatabase<InsectoidCombinationDef>.AllDefs)
            {
                if ((genome1 == element.genomes[0] && genome2 == element.genomes[1] && genome3 == element.genomes[2]) ||
                    (genome1 == element.genomes[0] && genome2 == element.genomes[2] && genome3 == element.genomes[1]) ||
                    (genome1 == element.genomes[1] && genome2 == element.genomes[0] && genome3 == element.genomes[2]) ||
                    (genome1 == element.genomes[1] && genome2 == element.genomes[2] && genome3 == element.genomes[0]) ||
                    (genome1 == element.genomes[2] && genome2 == element.genomes[0] && genome3 == element.genomes[1]) ||
                    (genome1 == element.genomes[2] && genome2 == element.genomes[1] && genome3 == element.genomes[0])

                    )
                {
                    
                    IncubatingInsectoid = element.result.RandomElement();
                    IsHostile = element.isHostile;
                    if (IncubatingInsectoid != "")
                    {
                        IncubationStarted = true;
                        return IncubatingInsectoid;
                    }
                    
                    
                }
               

            }
            return "";

        }

        public override void TickRare()
        {
            base.TickRare();
            if (IncubationStarted) {
                IncubationCounter++;
                if (!compPowerTrader.PowerOn)
                {
                    Messages.Message("VFEI_IncubationFailurePower".Translate(), this, MessageTypeDefOf.NegativeEvent, true);
                    IncubationCounter = 0;
                    IncubationStarted = false;
                }

                if (IncubationCounter > rareTicksPerDay*this.TryGetComp<CompIncubationTime>().Props.timeInDays)
                {
                    if (rand.NextDouble() < (this.TryGetComp<CompFailureRate>().Props.failureRatePercent / 100)){

                        List<IncubationFailureDef> tempFailureList = DefDatabase<IncubationFailureDef>.AllDefs.ToList();
                        IncubationFailureDef incubationFailure = tempFailureList.RandomElementByWeight(((IncubationFailureDef s) => s.commonality));
                        if (incubationFailure.justKill)
                        {
                            Messages.Message("VFEI_FailureDeath".Translate(), this, MessageTypeDefOf.NegativeEvent, true);
                            for (int i = 0; i < 20; i++)
                            {
                                IntVec3 c;
                                CellFinder.TryFindRandomReachableCellNear(this.Position, this.Map, 2, TraverseParms.For(TraverseMode.NoPassClosedDoors, Danger.Deadly, false), null, null, out c);
                                FilthMaker.TryMakeFilth(c, this.Map, ThingDef.Named("Filth_BloodInsect"));
                            }
                            SoundDefOf.Hive_Spawn.PlayOneShot(new TargetInfo(this.Position, this.Map, false));
                            PawnGenerationRequest request = new PawnGenerationRequest(PawnKindDef.Named(IncubatingInsectoid), Faction.OfPlayer, PawnGenerationContext.NonPlayer, -1, true, true, false, false, false, false, 1f, false, true, true, false, false);
                            Pawn pawn = PawnGenerator.GeneratePawn(request);
                            GenSpawn.Spawn(pawn, CellFinder.RandomClosewalkCellNear(this.Position, this.Map, 3, null), this.Map, WipeMode.Vanish);
                            pawn.Kill(null);

                    }
                        else if (incubationFailure.creatureDef!=null)
                        {
                            Messages.Message("VFEI_FailureMonstrosity".Translate(), this, MessageTypeDefOf.NegativeEvent, true);
                            PawnGenerationRequest request = new PawnGenerationRequest(PawnKindDef.Named(incubationFailure.creatureDef), Faction.OfInsects, PawnGenerationContext.NonPlayer, -1, true, true, false, false, false, false, 1f, false, true, true, false, false);
                            Pawn pawn = PawnGenerator.GeneratePawn(request);
                            GenSpawn.Spawn(pawn, CellFinder.RandomClosewalkCellNear(this.Position, this.Map, 3, null), this.Map, WipeMode.Vanish);
                            SoundDefOf.Hive_Spawn.PlayOneShot(new TargetInfo(this.Position, this.Map, false));
                            for (int i = 0; i < 20; i++)
                            {
                                IntVec3 c;
                                CellFinder.TryFindRandomReachableCellNear(this.Position, this.Map, 2, TraverseParms.For(TraverseMode.NoPassClosedDoors, Danger.Deadly, false), null, null, out c);
                                FilthMaker.TryMakeFilth(c, this.Map, ThingDefOf.Filth_AmnioticFluid);

                            }
                            pawn.mindState.mentalStateHandler.TryStartMentalState(DefDatabase<MentalStateDef>.GetNamed("ManhunterPermanent", true), null, true, false, null, false);
                        }
                        else if (incubationFailure.hediffDef!= null)
                        {
                            Messages.Message("VFEI_FailureDisease".Translate(), this, MessageTypeDefOf.NegativeEvent, true);
                            PawnGenerationRequest request = new PawnGenerationRequest(PawnKindDef.Named(IncubatingInsectoid), Faction.OfPlayer, PawnGenerationContext.NonPlayer, -1, true, true, false, false, false, false, 1f, false, true, true, false, false);
                            Pawn pawn = PawnGenerator.GeneratePawn(request);
                            GenSpawn.Spawn(pawn, CellFinder.RandomClosewalkCellNear(this.Position, this.Map, 3, null), this.Map, WipeMode.Vanish);
                            SoundDefOf.Hive_Spawn.PlayOneShot(new TargetInfo(this.Position, this.Map, false));
                            for (int i = 0; i < 20; i++)
                            {
                                IntVec3 c;
                                CellFinder.TryFindRandomReachableCellNear(this.Position, this.Map, 2, TraverseParms.For(TraverseMode.NoPassClosedDoors, Danger.Deadly, false), null, null, out c);
                                FilthMaker.TryMakeFilth(c, this.Map, ThingDefOf.Filth_AmnioticFluid);

                            }
                            pawn.health.AddHediff(incubationFailure.hediffDef);
                        }



                    }
                    else {
                        Faction faction;
                        if (IsHostile)
                        {
                            faction = Faction.OfInsects;
                        }
                        else
                        {
                            faction = Faction.OfPlayer;
                        }
                        PawnGenerationRequest request = new PawnGenerationRequest(PawnKindDef.Named(IncubatingInsectoid), faction, PawnGenerationContext.NonPlayer, -1, true, true, false, false, false, false, 1f, false, true, true, false, false);
                        Pawn pawn = PawnGenerator.GeneratePawn(request);
                        GenSpawn.Spawn(pawn, CellFinder.RandomClosewalkCellNear(this.Position, this.Map, 3, null), this.Map, WipeMode.Vanish);
                        SoundDefOf.Hive_Spawn.PlayOneShot(new TargetInfo(this.Position, this.Map, false));
                        for (int i = 0; i < 20; i++)
                        {
                            IntVec3 c;
                            CellFinder.TryFindRandomReachableCellNear(this.Position, this.Map, 2, TraverseParms.For(TraverseMode.NoPassClosedDoors, Danger.Deadly, false), null, null, out c);
                            FilthMaker.TryMakeFilth(c, this.Map, ThingDefOf.Filth_AmnioticFluid);

                        }
                        if (IsHostile)
                        {
                            pawn.mindState.mentalStateHandler.TryStartMentalState(DefDatabase<MentalStateDef>.GetNamed("ManhunterPermanent", true), null, true, false, null, false);
                        }

                    }
                    
                    IncubationCounter = 0;
                    IncubationStarted = false;

                }

            }

        }

        public override string GetInspectString()
        {


            string text = base.GetInspectString();
            string incubationTxt = "";

            if (IncubationStarted)
            {
                incubationTxt = "\n"+"VFEI_IncubationInProgress".Translate(ThingDef.Named(this.IncubatingInsectoid).LabelCap) + ((int)(ticksPerDay*this.TryGetComp<CompIncubationTime>().Props.timeInDays) -(IncubationCounter*250)).ToStringTicksToPeriod(true, false, true, true);
            }


            return text + incubationTxt;
        }



    }
}
