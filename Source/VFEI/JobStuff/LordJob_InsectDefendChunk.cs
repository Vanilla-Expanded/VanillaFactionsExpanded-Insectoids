﻿using RimWorld;
using Verse;
using Verse.AI.Group;

namespace VFEI
{
    internal class LordJob_InsectDefendChunk : LordJob
    {
        private Thing chunk;
        private float defendRadius;
        private IntVec3 defSpot;
        private Faction faction;

        public LordJob_InsectDefendChunk()
        {
        }

        public LordJob_InsectDefendChunk(Thing chunk, Faction faction, float defendRadius, IntVec3 defSpot)
        {
            this.chunk = chunk;
            this.faction = faction;
            this.defendRadius = defendRadius;
            this.defSpot = defSpot;
        }

        public override bool AddFleeToil
        {
            get
            {
                return false;
            }
        }

        public override bool CanBlockHostileVisitors
        {
            get
            {
                return false;
            }
        }
        public override StateGraph CreateGraph()
        {
            StateGraph stateGraph = new StateGraph();
            if (!this.defSpot.IsValid)
            {
                Log.Warning("LordJob_InsectDefendChunk defSpot is invalid. Returning graph for LordJob_AssaultColony.");
                stateGraph.AttachSubgraph(new LordJob_AssaultColony(this.faction, true, true, false, false, true).CreateGraph());
                return stateGraph;
            }

            LordToil_DefendPoint lordToil_DefendPoint = new LordToil_DefendPoint(this.defSpot, this.defendRadius, null);
            stateGraph.StartingToil = lordToil_DefendPoint;

            LordToil_AssaultColony lordToil_AssaultColony = new LordToil_AssaultColony(false);
            stateGraph.AddToil(lordToil_AssaultColony);

            LordToil_AssaultColony lordToil_AssaultColony2 = new LordToil_AssaultColony(false);
            stateGraph.AddToil(lordToil_AssaultColony2);

            Transition transition = new Transition(lordToil_DefendPoint, lordToil_AssaultColony, false, true);
            transition.AddSource(lordToil_AssaultColony2);
            transition.AddTrigger(new Trigger_PawnCannotReachMapEdge());
            stateGraph.AddTransition(transition, false);

            Transition transition2 = new Transition(lordToil_DefendPoint, lordToil_AssaultColony2, false, true);
            transition2.AddTrigger(new Trigger_PawnHarmed(0.5f, true, null));
            transition2.AddTrigger(new Trigger_PawnLostViolently(true));
            transition2.AddTrigger(new Trigger_Memo(CompSpawnerInsectOnDamaged.MemoDamaged));
            transition2.AddPostAction(new TransitionAction_EndAllJobs());
            stateGraph.AddTransition(transition2, false);

            Transition transition3 = new Transition(lordToil_AssaultColony2, lordToil_DefendPoint, false, true);
            transition3.AddTrigger(new Trigger_TicksPassedWithoutHarmOrMemos(1300, new string[]
            {
                    CompSpawnerInsectOnDamaged.MemoDamaged
            }));
            transition3.AddPostAction(new TransitionAction_EndAttackBuildingJobs());
            stateGraph.AddTransition(transition3, false);

            Transition transition4 = new Transition(lordToil_DefendPoint, lordToil_AssaultColony, false, true);
            transition4.AddSource(lordToil_AssaultColony2);
            transition4.AddTrigger(new Trigger_ThingDamageTaken(this.chunk, 0.5f));
            transition4.AddTrigger(new Trigger_Memo(HediffGiver_Heat.MemoPawnBurnedByAir));
            stateGraph.AddTransition(transition4, false);

            return stateGraph;
        }

        public override void ExposeData()
        {
            Scribe_References.Look<Thing>(ref this.chunk, "chunk", false);
            Scribe_References.Look<Faction>(ref this.faction, "faction", false);
            Scribe_Values.Look<float>(ref this.defendRadius, "defendRadius", 0f, false);
            Scribe_Values.Look<IntVec3>(ref this.defSpot, "defSpot", default(IntVec3), false);
        }
    }
}