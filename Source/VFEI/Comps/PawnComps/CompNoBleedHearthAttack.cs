using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Verse;
using RimWorld;

namespace VFEI.Comps.PawnComps
{
    class CompNoBleedHearthAttack : HediffComp
    {
		private Properties.CompProperties_NoBleedHearthAttack Props
		{
			get
			{
				return (Properties.CompProperties_NoBleedHearthAttack)this.props;
			}
		}

		public override void CompPostTick(ref float severityAdjustment)
		{
			if (Find.TickManager.TicksGame % 500 == 0)
			{
				if (this.Pawn.health.hediffSet.BleedRateTotal > 0)
				{
					foreach (Hediff h in this.Pawn.health.hediffSet.hediffs)
					{
						if (h.Bleeding)
						{
							h.Tended(0.3f, 0.3f);
						}
					}

					if (Rand.RangeInclusive(0, 100) < 5)
					{
                        List<BodyPartDef> bodyPartDefs = new List<BodyPartDef>
                        {
                            BodyPartDefOf.Heart
                        };
                        HediffGiverUtility.TryApply(this.Pawn, VFEIDefOf.HeartAttack, bodyPartDefs, true);
					}
				}
			}
		}
	}
}
