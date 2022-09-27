using RimWorld.Planet;
using Verse;

namespace VFEI
{
    internal class WorldGenStep_ResetHillinessForISettlement : WorldGenStep
    {
        public override int SeedPart => 507014749;

        public override void GenerateFresh(string seed)
        {
            foreach (WorldObject item in Find.World.worldObjects.AllWorldObjects)
            {
                if (item.Faction.def.defName == "VFEI_Insect" && Find.WorldGrid[item.Tile].hilliness != Hilliness.Mountainous)
                {
                    Find.WorldGrid[item.Tile].hilliness = Hilliness.Mountainous;
                }
            }
        }
    }
}