using System.Collections.Generic;
using System.Linq;

using RimWorld;
using Verse;

namespace VFEI
{
    class Projectile_Psychic : Projectile
    {
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref this.ticksToDetonation, "ticksToDetonation", 0, false);
        }

        public override void Tick()
        {
            base.Tick();
            if (this.ticksToDetonation > 0)
            {
                this.ticksToDetonation--;
                if (this.ticksToDetonation <= 0)
                {
                    this.Explode();
                }
            }
        }

        protected override void Impact(Thing hitThing, bool blockedByShield = false)
        {
            if (this.def.projectile.explosionDelay == 0)
            {
                this.Explode();
                return;
            }
            this.landed = true;
            this.ticksToDetonation = this.def.projectile.explosionDelay;
            GenExplosion.NotifyNearbyPawnsOfDangerousExplosive(this, this.def.projectile.damageDef, this.launcher.Faction);
        }

        protected virtual void Explode()
        {
            IEnumerable<Pawn> list = GenRadial.RadialCellsAround(base.Position, 5, true).SelectMany(c => c.GetThingList(base.Map)).Where(t => t is Pawn).Cast<Pawn>();
            if (list.Any())
            {
                foreach (Pawn t in list)
                {
                    t.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.Berserk, null, true, false, null, false);
                }
            }

            Map map = base.Map;
            this.Destroy(DestroyMode.Vanish);
            if (this.def.projectile.explosionEffect != null)
            {
                Effecter effecter = this.def.projectile.explosionEffect.Spawn();
                effecter.Trigger(new TargetInfo(base.Position, map, false), new TargetInfo(base.Position, map, false));
                effecter.Cleanup();
            }

            var proj = def.projectile;
            GenExplosion.DoExplosion(Position,
                                     map,
                                     proj.explosionRadius,
                                     proj.damageDef,
                                     launcher,
                                     DamageAmount,
                                     ArmorPenetration,
                                     proj.soundExplode,
                                     equipmentDef,
                                     def,
                                     intendedTarget.Thing,
                                     proj.postExplosionSpawnThingDef,
                                     proj.postExplosionSpawnChance,
                                     proj.postExplosionSpawnThingCount,
                                     proj.postExplosionGasType,
                                     proj.applyDamageToExplosionCellsNeighbors,
                                     proj.preExplosionSpawnThingDef,
                                     proj.preExplosionSpawnChance,
                                     proj.preExplosionSpawnThingCount,
                                     proj.explosionChanceToStartFire,
                                     proj.explosionDamageFalloff);
        }

        private int ticksToDetonation;
    }
}
