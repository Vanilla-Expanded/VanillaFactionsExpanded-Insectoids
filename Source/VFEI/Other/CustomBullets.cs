﻿using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace VFEI.Other
{
    internal class PlasmapiercerBullet : Projectile
    {
        protected override void Impact(Thing hitThing, bool blockedByShield = false)
        {
            Map map = base.Map;
            base.Impact(hitThing);
            BattleLogEntry_RangedImpact battleLogEntry_RangedImpact = new BattleLogEntry_RangedImpact(this.launcher, hitThing, this.intendedTarget.Thing, this.equipmentDef, this.def, this.targetCoverDef);
            Find.BattleLog.Add(battleLogEntry_RangedImpact);
            if (hitThing != null)
            {
                DamageInfo dinfo = new DamageInfo(this.def.projectile.damageDef, base.DamageAmount, base.ArmorPenetration, this.ExactRotation.eulerAngles.y, this.launcher, null, this.equipmentDef, DamageInfo.SourceCategory.ThingOrUnknown, this.intendedTarget.Thing);
                hitThing.TakeDamage(dinfo).AssociateWithLog(battleLogEntry_RangedImpact);
                Pawn pawn = hitThing as Pawn;
                if (pawn != null && pawn.BodySize <= 1f)
                {
                    var hediffOnPawn = pawn.health?.hediffSet?.GetFirstHediffOfDef(VFEIDefOf.VFEI_TempDown);
                    if (hediffOnPawn == null) pawn.health.AddHediff(VFEIDefOf.VFEI_TempDown);
                }
                if (pawn != null && pawn.stances != null && pawn.BodySize <= this.def.projectile.StoppingPower + 0.001f)
                {
                    pawn.stances.stagger.StaggerFor(95);
                }
                if (this.def.projectile.extraDamages == null)
                {
                    return;
                }
                using (List<ExtraDamage>.Enumerator enumerator = this.def.projectile.extraDamages.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        ExtraDamage extraDamage = enumerator.Current;
                        if (Rand.Chance(extraDamage.chance))
                        {
                            DamageInfo dinfo2 = new DamageInfo(extraDamage.def, extraDamage.amount, extraDamage.AdjustedArmorPenetration(), this.ExactRotation.eulerAngles.y, this.launcher, null, this.equipmentDef, DamageInfo.SourceCategory.ThingOrUnknown, this.intendedTarget.Thing);
                            hitThing.TakeDamage(dinfo2).AssociateWithLog(battleLogEntry_RangedImpact);
                        }
                    }
                    return;
                }
            }
            SoundDefOf.BulletImpact_Ground.PlayOneShot(new TargetInfo(base.Position, map, false));
            if (base.Position.GetTerrain(map).takeSplashes)
            {
                FleckMaker.WaterSplash(this.ExactPosition, map, Mathf.Sqrt(base.DamageAmount) * 1f, 4f);
                return;
            }
            FleckMaker.Static(this.ExactPosition, map, FleckDefOf.ShotHit_Dirt, 1f);
        }
    }

    internal class PlasmaCutterBullet : Projectile
    {
        protected override void Impact(Thing hitThing, bool blockedByShield = false)
        {
            Map map = base.Map;
            base.Impact(hitThing);
            BattleLogEntry_RangedImpact battleLogEntry_RangedImpact = new BattleLogEntry_RangedImpact(this.launcher, hitThing, this.intendedTarget.Thing, this.equipmentDef, this.def, this.targetCoverDef);
            Find.BattleLog.Add(battleLogEntry_RangedImpact);
            if (hitThing != null)
            {
                float damageA = (float)base.DamageAmount + Rand.RangeInclusive(base.DamageAmount / 8, base.DamageAmount / 2);
                DamageInfo dinfo = new DamageInfo(this.def.projectile.damageDef, damageA, base.ArmorPenetration, this.ExactRotation.eulerAngles.y, this.launcher, null, this.equipmentDef, DamageInfo.SourceCategory.ThingOrUnknown, this.intendedTarget.Thing);
                hitThing.TakeDamage(dinfo).AssociateWithLog(battleLogEntry_RangedImpact);
                Pawn pawn = hitThing as Pawn;
                if (pawn != null && pawn.stances != null && pawn.BodySize <= this.def.projectile.StoppingPower + 0.001f)
                {
                    pawn.stances.stagger.StaggerFor(95);
                }
                if (this.def.projectile.extraDamages == null)
                {
                    return;
                }
                using (List<ExtraDamage>.Enumerator enumerator = this.def.projectile.extraDamages.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        ExtraDamage extraDamage = enumerator.Current;
                        if (Rand.Chance(extraDamage.chance))
                        {
                            DamageInfo dinfo2 = new DamageInfo(extraDamage.def, extraDamage.amount, extraDamage.AdjustedArmorPenetration(), this.ExactRotation.eulerAngles.y, this.launcher, null, this.equipmentDef, DamageInfo.SourceCategory.ThingOrUnknown, this.intendedTarget.Thing);
                            hitThing.TakeDamage(dinfo2).AssociateWithLog(battleLogEntry_RangedImpact);
                        }
                    }
                    return;
                }
            }
            SoundDefOf.BulletImpact_Ground.PlayOneShot(new TargetInfo(base.Position, map, false));
            if (base.Position.GetTerrain(map).takeSplashes)
            {
                FleckMaker.WaterSplash(this.ExactPosition, map, Mathf.Sqrt(base.DamageAmount) * 1f, 4f);
                return;
            }
            FleckMaker.Static(this.ExactPosition, map, FleckDefOf.ShotHit_Dirt, 1f);
        }
    }
}