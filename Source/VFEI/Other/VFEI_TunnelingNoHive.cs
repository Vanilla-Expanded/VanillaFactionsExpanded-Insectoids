using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace VFEI
{
    [StaticConstructorOnStartup]
    internal class VFEI_TunnelingNoHive : ThingWithComps
    {
        public List<ThingDef> filthTypes = new List<ThingDef>();

        private static readonly float FilthSpawnRadius = 3f;

        private readonly FloatRange ResultSpawnDelay = new FloatRange(26f, 30f);

        private int secondarySpawnTick;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref this.secondarySpawnTick, "secondarySpawnTick");
            Scribe_Collections.Look<ThingDef>(ref this.filthTypes, "filthTypes");
        }

        public void ResetStaticData()
        {
            filthTypes.Clear();
            filthTypes.Add(ThingDefOf.Filth_Dirt);
            filthTypes.Add(ThingDefOf.Filth_Dirt);
            filthTypes.Add(ThingDefOf.Filth_Dirt);
            filthTypes.Add(ThingDefOf.Filth_RubbleRock);
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            if (!respawningAfterLoad)
            {
                this.secondarySpawnTick = Find.TickManager.TicksGame + 200;
            }
            this.ResetStaticData();
        }

        public override void Tick()
        {
            if (base.Spawned)
            {
                this.ResetStaticData();
                Vector3 vector = base.Position.ToVector3Shifted();
                if (CellFinder.TryFindRandomReachableCellNear(base.Position, base.Map, FilthSpawnRadius, TraverseParms.For(TraverseMode.NoPassClosedDoors, Danger.Deadly, false), null, null, out IntVec3 c, 999999))
                {
                    FilthMaker.TryMakeFilth(c, base.Map, filthTypes.RandomElement<ThingDef>(), 1, FilthSourceFlags.None);
                }
                FleckMaker.ThrowDustPuffThick(new Vector3(vector.x, 0f, vector.z)
                {
                    y = AltitudeLayer.MoteOverhead.AltitudeFor()
                }, base.Map, Rand.Range(1.5f, 3f), new Color(1f, 1f, 1f, 2.5f));

                if (this.secondarySpawnTick <= Find.TickManager.TicksGame)
                {
                    this.Destroy(DestroyMode.Vanish);
                }
            }
        }
    }
}