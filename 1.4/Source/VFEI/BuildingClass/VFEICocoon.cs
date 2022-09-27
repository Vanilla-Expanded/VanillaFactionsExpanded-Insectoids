using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI.Group;
using Verse.Sound;

namespace VFEI
{
    internal class VFEICocoon : ThingWithComps
    {
        private bool once = true;
        private int timeBeforeInsect;
        private int timeBeforeInsectString;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref this.timeBeforeInsect, "timeBeforeInsect");
            Scribe_Values.Look<int>(ref this.timeBeforeInsectString, "timeBeforeInsectString");
            Scribe_Values.Look<bool>(ref this.once, "onceCocoonDev");
        }

        public override string GetInspectString()
        {
            return "CocoonInsectSpawnIn".Translate(timeBeforeInsectString.ToStringTicksToPeriod());
        }

        public override void Tick()
        {
            base.Tick();
            if (once) { this.timeBeforeInsect = Find.TickManager.TicksGame + 15000; timeBeforeInsectString = 15000; once = false; }
            if (Find.TickManager.TicksGame == this.timeBeforeInsect)
            {
                CellFinder.TryFindRandomReachableCellNear(this.Position, this.Map, 4, TraverseParms.For(TraverseMode.NoPassClosedDoors, Danger.Deadly, false), null, null, out IntVec3 c);
                FilthMaker.TryMakeFilth(c, this.Map, ThingDefOf.Filth_Slime);
                SoundDefOf.Hive_Spawn.PlayOneShot(new TargetInfo(this.Position, this.Map));

                List<PawnKindDef> pawnKindDefs = new List<PawnKindDef>
                {
                    PawnKindDefOf.Megascarab,
                    PawnKindDefOf.Spelopede,
                    PawnKindDefOf.Megaspider,
                    VFEIDefOf.VFEI_Insectoid_Megapede,
                    VFEIDefOf.VFEI_Insectoid_Gigalocust,
                    VFEIDefOf.VFEI_Insectoid_RoyalMegaspider
                };

                if (pawnKindDefs.Count > 0)
                {
                    Pawn p = PawnGenerator.GeneratePawn(pawnKindDefs.RandomElementByWeight(x => x.combatPower / x.race.BaseMarketValue), this.Faction);
                    p.ageTracker.AgeBiologicalTicks = 30000;
                    GenSpawn.Spawn(p, this.Position, this.Map);
                    List<Pawn> pawns = new List<Pawn> { p };
                    if (this.Map.ParentFaction == this.Faction)
                    {
                        LordMaker.MakeNewLord(this.Faction, new LordJob_DefendBase(this.Faction, this.Map.Center), this.Map, pawns);
                    }
                    else
                    {
                        pawns.ForEach(p1 => p1.mindState.spawnedByInfestationThingComp = true);
                        SpawnedPawnParams spp = new SpawnedPawnParams
                        {
                            aggressive = false,
                            defSpot = base.Position,
                            defendRadius = 5
                        };
                        LordMaker.MakeNewLord(this.Faction, new LordJob_DefendAndExpandHive(spp), this.Map, pawns);
                    }
                }
                this.Destroy();
            }
            timeBeforeInsectString--;
        }
    }
}