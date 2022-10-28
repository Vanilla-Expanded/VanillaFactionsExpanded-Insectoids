
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace VFEI
{
	public class ThinkNode_TitanBeetle : ThinkNode_Conditional
	{


		protected override bool Satisfied(Pawn pawn)
		{
			if (pawn.def == VFEIDefOf.VFEI_VatGrownTitanbeetle)
			{
				return true;
			}
			return false;
		}
	}
}
