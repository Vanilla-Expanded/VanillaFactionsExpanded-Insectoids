using RimWorld.BaseGen;
using Verse;

namespace VFEI
{
    internal class SymbolResolver_RandomDamage : SymbolResolver
    {
        public override void Resolve(ResolveParams rp)
        {
            Map map = BaseGen.globalSettings.map;
            map.listerThings.AllThings.FindAll(t1 => t1.Faction == Find.FactionManager.FirstFactionOfDef(VFEIDefOf.VFEI_Insect)).ForEach(t => t.HitPoints -= t.HitPoints / Rand.RangeInclusive(3, 10));
            map.listerThings.AllThings.FindAll(t2 => t2.def.IsMeat || t2.def.defName == "Pemmican").ForEach(t => t.DeSpawn());
        }
    }
}