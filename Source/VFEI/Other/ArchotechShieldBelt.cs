using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;
using RimWorld;

namespace VFEI
{
	[StaticConstructorOnStartup]
    public class ArchotechShieldBelt : ShieldBelt
	{
        private float energy;

        private bool shouldRecharge = true;

        private float EnergyMax
        {
			get
			{
				return this.GetStatValue(StatDefOf.EnergyShieldEnergyMax, true);
			}
		}

        public override void ExposeData()
        {
			base.ExposeData();
			Scribe_Values.Look<float>(ref this.energy, "energy");
			Scribe_Values.Look<bool>(ref this.shouldRecharge, "shouldRecharge");
		}

        public override void Tick()
        {
			if (shouldRecharge)
			{
				this.energy = this.EnergyMax;
				this.shouldRecharge = false;
			}
		}
	}
}
