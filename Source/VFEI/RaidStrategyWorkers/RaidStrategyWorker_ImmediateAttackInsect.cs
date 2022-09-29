using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI.Group;

namespace VFEI
{
    public class RaidStrategyWorker_ImmediateAttackInsect : RaidStrategyWorker_ImmediateAttack
    {
        public override bool CanUseWith(IncidentParms parms, PawnGroupKindDef groupKind)
        {
            return parms?.faction?.def?.defName == "VFEI_Insect" && base.CanUseWith(parms, groupKind);
        }
    }
}