using RimWorld;
using RimWorld.BaseGen;
using UnityEngine;
using Verse;

namespace VFEI
{
    internal class SymbolResolver_EdgeDefenseNoPawn : SymbolResolver
    {
        public override void Resolve(ResolveParams rp)
        {
            Faction faction = rp.faction ?? Find.FactionManager.RandomEnemyFaction(false, false, true, TechLevel.Undefined);
            int width;
            if (rp.edgeDefenseWidth != null)
            {
                width = rp.edgeDefenseWidth.Value;
            }
            else if (rp.edgeDefenseMortarsCount != null && rp.edgeDefenseMortarsCount.Value > 0)
            {
                width = 4;
            }
            else
            {
                width = (Rand.Bool ? 2 : 4);
            }
            width = Mathf.Clamp(width, 1, Mathf.Min(rp.rect.Width, rp.rect.Height) / 2);
            int num2;
            int num3;
            bool flag;
            bool flag2;
            bool flag3;
            switch (width)
            {
                case 1:
                    num2 = (rp.edgeDefenseTurretsCount ?? 0);
                    num3 = 0;
                    flag = false;
                    flag2 = true;
                    flag3 = true;
                    break;

                case 2:
                    num2 = (rp.edgeDefenseTurretsCount ?? (rp.rect.EdgeCellsCount / 30));
                    num3 = 0;
                    flag = false;
                    flag2 = false;
                    flag3 = true;
                    break;

                case 3:
                    num2 = (rp.edgeDefenseTurretsCount ?? (rp.rect.EdgeCellsCount / 30));
                    num3 = (rp.edgeDefenseMortarsCount ?? (rp.rect.EdgeCellsCount / 75));
                    flag = (num3 == 0);
                    flag2 = false;
                    flag3 = true;
                    break;

                default:
                    num2 = (rp.edgeDefenseTurretsCount ?? (rp.rect.EdgeCellsCount / 30));
                    num3 = (rp.edgeDefenseMortarsCount ?? (rp.rect.EdgeCellsCount / 75));
                    flag = true;
                    flag2 = false;
                    flag3 = false;
                    break;
            }
            if (faction != null && faction.def.techLevel < TechLevel.Industrial)
            {
                num2 = 0;
                num3 = 0;
            }
            CellRect rect = rp.rect;
            for (int j = 0; j < width; j++)
            {
                if (j % 2 == 0)
                {
                    ResolveParams rp3 = rp;
                    rp3.faction = faction;
                    rp3.rect = rect;
                    BaseGen.symbolStack.Push("edgeSandbags", rp3, null);
                    if (!flag)
                    {
                        break;
                    }
                }
                rect = rect.ContractedBy(1);
            }
            CellRect rect2 = flag3 ? rp.rect : rp.rect.ContractedBy(1);
            for (int k = 0; k < num3; k++)
            {
                ResolveParams rp4 = rp;
                rp4.faction = faction;
                rp4.rect = rect2;
                BaseGen.symbolStack.Push("edgeMannedMortarNoPawns", rp4, null);
            }
            CellRect rect3 = flag2 ? rp.rect : rp.rect.ContractedBy(1);
            for (int l = 0; l < num2; l++)
            {
                ResolveParams rp5 = rp;
                rp5.faction = faction;
                rp5.singleThingDef = ThingDefOf.Turret_MiniTurret;
                rp5.rect = rect3;
                rp5.edgeThingAvoidOtherEdgeThings = new bool?(rp.edgeThingAvoidOtherEdgeThings ?? true);
                BaseGen.symbolStack.Push("edgeThing", rp5, null);
            }
        }
    }
}