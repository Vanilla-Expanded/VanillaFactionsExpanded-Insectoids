using System.Collections.Generic;
using RimWorld;
using Verse;

namespace VFEI.RaidArrivalModes
{
    internal class PawnsArrivalModeWorker_Burrow : PawnsArrivalModeWorker
    {
        public static void InsectTunnel(IncidentParms parms, List<Pawn> pawns)
        {
            Map map = (Map)parms.target;
            Faction faction = parms.faction;
            pawns.Clear();
            int point = (int)parms.points;

            for (int i = 0; i < 15; i++)
            {
                IntVec3 intvr = new IntVec3();
                RCellFinder.TryFindRandomCellNearWith(parms.spawnCenter, (intv) => intv.Walkable(map), map, out intvr, 1, 20);
                IntVec3 intvr2 = new IntVec3();
                RCellFinder.TryFindRandomCellNearWith(parms.spawnCenter, (intv) => intv.Walkable(map), map, out intvr2, 1, 10);
                FilthMaker.TryMakeFilth(intvr2, map, ThingDefOf.Filth_RubbleRock, 1, FilthSourceFlags.None);
                FilthMaker.TryMakeFilth(intvr2, map, ThingDefOf.Filth_Dirt, 1, FilthSourceFlags.None);
                FleckMaker.Static(intvr2, map, FleckDefOf.DustPuffThick, Rand.Range(1.5f, 3f));

                if (point - 100 > 0 && Rand.Bool)
                {
                    Thing thingR = ThingMaker.MakeThing(VFEIDefOf.VFEI_BurrowRoyal);
                    thingR.SetFaction(faction);
                    GenSpawn.Spawn(thingR, intvr, map);
                    pawns.Add(PawnGenerator.GeneratePawn(VFEIDefOf.VFEI_Insectoid_Gigalocust, faction));
                    point -= 100;
                }
                else if (point - 75 > 0 && Rand.Bool)
                {
                    Thing thingM = ThingMaker.MakeThing(VFEIDefOf.VFEI_BurrowMedium);
                    thingM.SetFaction(faction);
                    GenSpawn.Spawn(thingM, intvr, map);
                    pawns.Add(PawnGenerator.GeneratePawn(PawnKindDefOf.Megaspider, faction));
                    point -= 75;
                }
                else if (point - 25 > 0)
                {
                    Thing thingS = ThingMaker.MakeThing(VFEIDefOf.VFEI_BurrowSmall);
                    thingS.SetFaction(faction);
                    GenSpawn.Spawn(thingS, intvr, map);
                    pawns.Add(PawnGenerator.GeneratePawn(PawnKindDefOf.Megascarab, faction));
                    point -= 25;
                }
                else
                {
                    break;
                }
            }

            foreach (Pawn pawn in pawns)
            {
                GenSpawn.Spawn(pawn, parms.spawnCenter, map);
            }
        }

        public override void Arrive(List<Pawn> pawns, IncidentParms parms)
        {
            InsectTunnel(parms, pawns);
        }

        public override bool TryResolveRaidSpawnCenter(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            if (!parms.spawnCenter.IsValid)
            {
                parms.spawnCenter = CellFinderLoose.RandomCellWith((i) => i.Walkable(map) == true, map);
            }
            parms.spawnRotation = Rot4.Random;
            return true;
        }
    }
}