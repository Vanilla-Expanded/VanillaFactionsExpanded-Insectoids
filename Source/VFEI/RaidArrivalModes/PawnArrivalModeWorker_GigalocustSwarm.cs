using System.Collections.Generic;
using RimWorld;
using Verse;

namespace VFEI.RaidArrivalModes
{
    internal class PawnArrivalModeWorker_GigalocustSwarm : PawnsArrivalModeWorker
    {
        public override void Arrive(List<Pawn> pawns, IncidentParms parms)
        {
            pawns.Clear();
            Map map = (Map)parms.target;
            int n = (int)parms.points / (int)VFEIDefOf.VFEI_Insectoid_Gigalocust.combatPower;
            for (int i = 0; i < n; i++)
            {
                RCellFinder.TryFindRandomCellNearWith(parms.spawnCenter, (c) => c.Walkable(map) && (c.GetRoof(map) == RoofDefOf.RoofConstructed || !c.Roofed(map)), map, out IntVec3 intVec3, 1, 20);
                Pawn pawn = PawnGenerator.GeneratePawn(VFEIDefOf.VFEI_Insectoid_Gigalocust, parms.faction);
                SkyfallerMaker.SpawnSkyfaller(VFEIDefOf.VFEI_GigalocustIncoming, pawn, intVec3, map);
                pawns.Add(pawn);
            }
        }

        public override bool TryResolveRaidSpawnCenter(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            if (!parms.spawnCenter.IsValid || !parms.spawnCenter.Walkable(map) || (parms.spawnCenter.Roofed(map) && parms.spawnCenter.GetRoof(map) != RoofDefOf.RoofConstructed))
                parms.spawnCenter = CellFinderLoose.RandomCellWith((i) => i.Walkable(map) && (i.GetRoof(map) == RoofDefOf.RoofConstructed || !i.Roofed(map)), map);
            parms.spawnRotation = Rot4.Random;
            return true;
        }
    }
}