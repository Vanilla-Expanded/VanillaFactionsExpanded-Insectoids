using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace VFEI
{
    public class CompTargetEffect_EraseInRadius : CompTargetEffect
    {
        public int radius = 10;

        public override void DoEffectOn(Pawn user, Thing target)
        {
            Map map = user.Map;
            IntVec3 position = user.Position;

            IEnumerable<IntVec3> list = GenRadial.RadialCellsAround(position, radius, true);
            foreach (IntVec3 t in list)
            {
                IEnumerable<Thing> things = t.GetThingList(map);
                foreach (Thing item in things.ToList())
                {
                    if (item is Pawn p) 
                    {
                        Notify_KilledPawn(p);
                        p.inventory.DestroyAll();
                        p.equipment.DestroyAllEquipment();
                        p.Kill(new DamageInfo(DamageDefOf.Crush, 99999, 99999));
                        Corpse pCorpse = p.Corpse;
                        pCorpse.Destroy();
                    }
                    else item.Destroy(DestroyMode.Vanish);
                }
                if (t.Roofed(map))
                {
                    map.roofGrid.SetRoof(t, null);
                }
            }
            FleckMaker.Static(position, map, FleckDefOf.PsycastAreaEffect, radius);
        }
    }
}