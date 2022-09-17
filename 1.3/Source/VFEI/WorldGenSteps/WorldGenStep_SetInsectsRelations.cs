using RimWorld;
using RimWorld.Planet;
using Verse;

namespace VFEI
{
    internal class WorldGenStep_SetInsectsRelations : WorldGenStep
    {
        public override int SeedPart => 507014750;

        public override void GenerateFresh(string seed)
        {
            Faction fac = Find.World.factionManager.OfInsects;
            if (fac != null && fac.def == FactionDefOf.Insect)
            {
                foreach (Faction fac2 in Find.World.factionManager.AllFactions)
                {
                    if (fac2 != null && fac2.def.defName == "VFEI_Insect")
                    {
                        FactionRelation vIRel = new FactionRelation
                        {
                            other = fac2,
                            kind = FactionRelationKind.Neutral
                        };
                        FactionRelation VFEIRel = new FactionRelation
                        {
                            other = fac,
                            kind = FactionRelationKind.Neutral
                        };

                        fac.SetRelation(vIRel);
                        fac2.SetRelation(VFEIRel);
                    }
                }
            }
        }
    }
}