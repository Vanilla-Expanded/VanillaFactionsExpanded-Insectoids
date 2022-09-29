using Verse;

namespace VFEI.Other
{
    internal class RepellerPlaceWorker : PlaceWorker
    {
        public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map, Thing thingToIgnore = null, Thing thing = null)
        {
            GenDraw.DrawRadiusRing(loc, 50);
            return true;
        }
    }
}