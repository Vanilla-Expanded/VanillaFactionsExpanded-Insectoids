using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.BaseGen;
using UnityEngine;
using Verse;

namespace VFEI
{
    internal class SymbolResolver_MannedMortarNoPawns : SymbolResolver
    {
        public override bool CanResolve(ResolveParams rp)
        {
            Map map = BaseGen.globalSettings.map;
            if (!base.CanResolve(rp))
            {
                return false;
            }
            int num = 0;
            using (CellRect.Enumerator enumerator = rp.rect.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current.Standable(map))
                    {
                        num++;
                    }
                }
            }
            return num >= 2;
        }

        public override void Resolve(ResolveParams rp)
        {
            Map map = BaseGen.globalSettings.map;
            Faction faction;
            if ((faction = rp.faction) == null)
            {
                faction = (Find.FactionManager.RandomEnemyFaction(false, false, true, TechLevel.Industrial) ?? Find.FactionManager.RandomEnemyFaction(false, false, true, TechLevel.Undefined));
            }
            Faction faction2 = faction;
            Rot4 rot = rp.thingRot ?? Rot4.Random;
            ThingDef thingDef;
            if ((thingDef = rp.mortarDef) == null)
            {
                thingDef = (from x in DefDatabase<ThingDef>.AllDefsListForReading
                            where x.category == ThingCategory.Building && x.building.IsMortar && x.building.buildingTags.Contains("Artillery_MannedMortar")
                            select x).RandomElement<ThingDef>();
            }
            ThingDef thingDef2 = thingDef;
            if (!this.TryFindMortarSpawnCell(rp.rect, rot, thingDef2, out IntVec3 intVec))
            {
                return;
            }
            ThingDef thingDef3 = TurretGunUtility.TryFindRandomShellDef(thingDef2, false, true, true, faction2.def.techLevel, false, 250f);
            if (thingDef3 != null)
            {
                ResolveParams resolveParams2 = rp;
                resolveParams2.faction = faction2;
                resolveParams2.singleThingDef = thingDef3;
                resolveParams2.singleThingStackCount = new int?(Rand.RangeInclusive(5, Mathf.Min(8, thingDef3.stackLimit)));
                BaseGen.symbolStack.Push("thing", resolveParams2, null);
            }
            ResolveParams resolveParams3 = rp;
            resolveParams3.faction = faction2;
            resolveParams3.singleThingDef = thingDef2;
            resolveParams3.rect = CellRect.SingleCell(intVec);
            resolveParams3.thingRot = new Rot4?(rot);
            BaseGen.symbolStack.Push("thing", resolveParams3, null);
        }

        private bool TryFindMortarSpawnCell(CellRect rect, Rot4 rot, ThingDef mortarDef, out IntVec3 cell)
        {
            Map map = BaseGen.globalSettings.map;
            Predicate<CellRect> edgeTouchCheck;
            if (rot == Rot4.North)
            {
                edgeTouchCheck = delegate (CellRect x)
                {
                    IEnumerable<IntVec3> cells = x.Cells;
                    return cells.Any((IntVec3 y) => y.z == rect.maxZ);
                };
            }
            else if (rot == Rot4.South)
            {
                edgeTouchCheck = delegate (CellRect x)
                {
                    IEnumerable<IntVec3> cells = x.Cells;
                    return cells.Any((IntVec3 y) => y.z == rect.minZ);
                };
            }
            else if (rot == Rot4.West)
            {
                edgeTouchCheck = delegate (CellRect x)
                {
                    IEnumerable<IntVec3> cells = x.Cells;
                    return cells.Any((IntVec3 y) => y.x == rect.minX);
                };
            }
            else
            {
                edgeTouchCheck = delegate (CellRect x)
                {
                    IEnumerable<IntVec3> cells = x.Cells;
                    return cells.Any((IntVec3 y) => y.x == rect.maxX);
                };
            }
            return CellFinder.TryFindRandomCellInsideWith(rect, delegate (IntVec3 x)
            {
                CellRect obj = GenAdj.OccupiedRect(x, rot, mortarDef.size);
                return ThingUtility.InteractionCellWhenAt(mortarDef, x, rot, map).Standable(map) && obj.FullyContainedWithin(rect) && edgeTouchCheck(obj);
            }, out cell);
        }
    }
}