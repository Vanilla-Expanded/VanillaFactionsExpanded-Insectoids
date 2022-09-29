using RimWorld;
using Verse;
using Verse.AI.Group;
using Verse.Sound;

namespace VFEI.PawnComps
{
    internal class CompSpawnLarvae : ThingComp
    {
        private int nextSpawn;
        private int tickToSpawn;

        private CompProperties_SpawnLarvae Props
        {
            get
            {
                return (CompProperties_SpawnLarvae)this.props;
            }
        }

        public override void Initialize(CompProperties props)
        {
            base.Initialize(props);
            this.nextSpawn = Find.TickManager.TicksGame + Props.ticksBetweenSpawn;
            this.tickToSpawn = Props.ticksBetweenSpawn;
        }

        public override string CompInspectStringExtra()
        {
            if (tickToSpawn > 0 && this.parent.Map.mapPawns.AllPawnsSpawnedCount < 100)
            {
                return "LarvaeTimeSpawn".Translate(tickToSpawn.ToStringTicksToPeriod());
            }
            return "NoMoreSpawningAllowed".Translate();
        }

        public override void CompTick()
        {
            base.CompTick();
            if (this.parent is Pawn pa && pa.Map != null && pa.Spawned && !pa.health.Downed && pa.Map.mapPawns.AllPawnsSpawned.Count < 100)
            {
                if (Find.TickManager.TicksGame == this.nextSpawn)
                {
                    for (int i = 0; i < Props.numberToSpawn; i++)
                    {
                        IntVec3 vec3 = this.parent.Position.RandomAdjacentCell8Way();
                        if (!vec3.InBounds(this.parent.Map) || !vec3.Walkable(this.parent.Map)) break;
                        Pawn p = PawnGenerator.GeneratePawn(VFEIDefOf.VFEI_Insectoid_Larvae, this.parent.Faction);
                        GenSpawn.Spawn(p, vec3, this.parent.Map);
                        if (pa.mindState.spawnedByInfestationThingComp == true)
                        {
                            p.mindState.spawnedByInfestationThingComp = true;
                            SpawnedPawnParams spp = new SpawnedPawnParams();
                            spp.aggressive = false;
                            spp.defSpot = vec3;
                            spp.defendRadius = 5;
                            Lord lord = LordMaker.MakeNewLord(this.parent.Faction, new LordJob_DefendAndExpandHive(spp), pa.Map, null);
                            lord.AddPawn(p);
                        }
                    }
                    FilthMaker.TryMakeFilth(this.parent.Position, this.parent.Map, ThingDefOf.Filth_Slime, 1);
                    SoundDefOf.Hive_Spawn.PlayOneShot(new TargetInfo(this.parent));
                    this.nextSpawn = Find.TickManager.TicksGame + this.Props.ticksBetweenSpawn;
                    this.tickToSpawn = Props.ticksBetweenSpawn;
                }
                if (this.tickToSpawn > 0)
                {
                    this.tickToSpawn--;
                }
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<int>(ref this.nextSpawn, "QueenNextSpawn");
            Scribe_Values.Look<int>(ref this.tickToSpawn, "QueenNextSpawnUtilsString");
        }
    }
}