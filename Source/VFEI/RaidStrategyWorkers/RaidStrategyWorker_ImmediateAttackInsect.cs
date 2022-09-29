using RimWorld;

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