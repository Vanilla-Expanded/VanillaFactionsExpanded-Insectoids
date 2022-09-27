using System;
using Verse;
using Verse.AI;
using RimWorld;

namespace InsectoidBioengineering
{
    public class WorkGiver_InsertThirdGenome : WorkGiver_Scanner
    {
        private static string NoGenesFound;

        public override ThingRequest PotentialWorkThingRequest
        {
            get
            {
                return ThingRequest.ForDef(ThingDef.Named("VFEI_BioengineeringIncubator"));
            }
        }

        public override PathEndMode PathEndMode
        {
            get
            {
                return PathEndMode.Touch;
            }
        }

        public static void ResetStaticData()
        {

            WorkGiver_InsertThirdGenome.NoGenesFound = "VFEI_NoGenesFound".Translate();
        }


        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            Building_BioengineeringIncubator building_incubator = t as Building_BioengineeringIncubator;
            if (building_incubator == null || !building_incubator.ExpectingThirdGenome || !building_incubator.StartInsertionJobs)
            {
                return false;
            }

            if (!t.IsForbidden(pawn))
            {
                LocalTargetInfo target = t;
                if (pawn.CanReserve(target, 1, 1, null, forced))
                {
                    if (pawn.Map.designationManager.DesignationOn(t, DesignationDefOf.Deconstruct) != null)
                    {
                        return false;
                    }
                    if (this.FindGene(pawn, building_incubator.theThirdGenomeIAmGoingToInsert, building_incubator) == null)
                    {
                        JobFailReason.Is(WorkGiver_InsertThirdGenome.NoGenesFound, null);
                        return false;
                    }
                    return !t.IsBurning();
                }
            }
            return false;
        }

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            Building_BioengineeringIncubator building_incubator = (Building_BioengineeringIncubator)t;
            Thing t2 = this.FindGene(pawn, building_incubator.theThirdGenomeIAmGoingToInsert, building_incubator);
            return new Job(DefDatabase<JobDef>.GetNamed("VFEI_InsertThirdGenomeJob", true), t, t2);
        }

        private Thing FindGene(Pawn pawn, string theThirdGenomeIAmGoingToInsert, Building_BioengineeringIncubator building_incubator)
        {
            Predicate<Thing> predicate = (Thing x) => !x.IsForbidden(pawn) && pawn.CanReserve(x, 1, 1, null, false);
            IntVec3 position = pawn.Position;
            Map map = pawn.Map;
            ThingRequest thingReq = ThingRequest.ForDef(ThingDef.Named(theThirdGenomeIAmGoingToInsert));
            PathEndMode peMode = PathEndMode.ClosestTouch;
            TraverseParms traverseParams = TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false);
            Predicate<Thing> validator = predicate;
            return GenClosest.ClosestThingReachable(position, map, thingReq, peMode, traverseParams, 9999f, validator, null, 0, -1, false, RegionType.Set_Passable, false);
        }
    }
}

