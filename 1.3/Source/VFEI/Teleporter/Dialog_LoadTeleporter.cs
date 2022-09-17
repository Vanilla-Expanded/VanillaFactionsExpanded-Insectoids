using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace VFEI
{
    internal class Dialog_LoadTeleporter : Window
    {
        private readonly List<TabRecord> tabsList = new List<TabRecord>();
        private readonly Vector2 BottomButtonSize = new Vector2(160f, 40f);
        private readonly CompTeleporter teleporter;
        private readonly Map map;

        private bool daysWorthOfFoodDirty = true;
        private bool foragedFoodPerDayDirty = true;
        private Pair<float, float> cachedDaysWorthOfFood;
        private Pair<ThingDef, float> cachedForagedFoodPerDay;
        private string cachedForagedFoodPerDayExplanation;

        private float lastMassFlashTime = -9999f;
        private bool massUsageDirty = true;
        private float cachedMassUsage;
        private bool caravanMassCapacityDirty = true;
        private bool caravanMassUsageDirty = true;
        private float cachedCaravanMassUsage;
        private float cachedCaravanMassCapacity;
        private string cachedCaravanMassCapacityExplanation;

        private bool tilesPerDayDirty = true;
        private float cachedTilesPerDay;
        private string cachedTilesPerDayExplanation;

        private bool visibilityDirty = true;
        private float cachedVisibility;
        private string cachedVisibilityExplanation;

        private TransferableOneWayWidget itemsTransfer;
        private TransferableOneWayWidget pawnsTransfer;
        private List<TransferableOneWay> transferables;
        private Tab tab = Tab.Pawns;

        public Dialog_LoadTeleporter(Map map, CompTeleporter teleporter)
        {
            this.map = map;
            this.teleporter = teleporter;
            this.forcePause = true;
            this.absorbInputAroundWindow = true;
        }

        private enum Tab
        {
            Pawns,
            Items,
        }

        private BiomeDef Biome => this.map.Biome;

        public float CaravanMassUsage
        {
            get
            {
                if (this.caravanMassUsageDirty)
                {
                    this.caravanMassUsageDirty = false;
                    this.cachedCaravanMassUsage = CollectionsMassCalculator.MassUsageTransferables(this.transferables, IgnorePawnsInventoryMode.IgnoreIfAssignedToUnload);
                }
                return this.cachedCaravanMassUsage;
            }
        }

        public override Vector2 InitialSize => new Vector2(1024f, UI.screenHeight);

        private float CaravanMassCapacity
        {
            get
            {
                if (this.caravanMassCapacityDirty)
                {
                    this.caravanMassCapacityDirty = false;
                    StringBuilder explanation = new StringBuilder();
                    this.cachedCaravanMassCapacity = CollectionsMassCalculator.CapacityTransferables(this.transferables, explanation);
                    this.cachedCaravanMassCapacityExplanation = explanation.ToString();
                }
                return this.cachedCaravanMassCapacity;
            }
        }

        private Pair<float, float> DaysWorthOfFood
        {
            get
            {
                if (this.daysWorthOfFoodDirty)
                {
                    this.daysWorthOfFoodDirty = false;
                    this.cachedDaysWorthOfFood = new Pair<float, float>(DaysWorthOfFoodCalculator.ApproxDaysWorthOfFood(this.transferables, this.map.Tile, IgnorePawnsInventoryMode.IgnoreIfAssignedToUnload, Faction.OfPlayer), DaysUntilRotCalculator.ApproxDaysUntilRot(this.transferables, this.map.Tile, IgnorePawnsInventoryMode.IgnoreIfAssignedToUnload));
                }
                return this.cachedDaysWorthOfFood;
            }
        }

        private Pair<ThingDef, float> ForagedFoodPerDay
        {
            get
            {
                if (this.foragedFoodPerDayDirty)
                {
                    this.foragedFoodPerDayDirty = false;
                    StringBuilder explanation = new StringBuilder();
                    this.cachedForagedFoodPerDay = ForagedFoodPerDayCalculator.ForagedFoodPerDay(this.transferables, this.Biome, Faction.OfPlayer, explanation);
                    this.cachedForagedFoodPerDayExplanation = explanation.ToString();
                }
                return this.cachedForagedFoodPerDay;
            }
        }

        private float MassCapacity => teleporter.Props.massCapacity;

        private float MassUsage
        {
            get
            {
                if (this.massUsageDirty)
                {
                    this.massUsageDirty = false;
                    this.cachedMassUsage = CollectionsMassCalculator.MassUsageTransferables(this.transferables, IgnorePawnsInventoryMode.IgnoreIfAssignedToUnload, true);
                }
                return this.cachedMassUsage;
            }
        }

        private float TilesPerDay
        {
            get
            {
                if (this.tilesPerDayDirty)
                {
                    this.tilesPerDayDirty = false;
                    StringBuilder explanation = new StringBuilder();
                    this.cachedTilesPerDay = TilesPerDayCalculator.ApproxTilesPerDay(this.transferables, this.MassUsage, this.MassCapacity, this.map.Tile, -1, explanation);
                    this.cachedTilesPerDayExplanation = explanation.ToString();
                }
                return this.cachedTilesPerDay;
            }
        }

        private string TransportersLabel => this.teleporter.parent.Label;

        private float Visibility
        {
            get
            {
                if (this.visibilityDirty)
                {
                    this.visibilityDirty = false;
                    StringBuilder explanation = new StringBuilder();
                    this.cachedVisibility = CaravanVisibilityCalculator.Visibility(this.transferables, explanation);
                    this.cachedVisibilityExplanation = explanation.ToString();
                }
                return this.cachedVisibility;
            }
        }

        public override void DoWindowContents(Rect inRect)
        {
            Rect rect1 = new Rect(0.0f, 0.0f, inRect.width, 35f);
            Text.Font = GameFont.Medium;
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(rect1, "LoadTransporters".Translate(TransportersLabel));
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;
            CaravanUIUtility.DrawCaravanInfo(new CaravanUIUtility.CaravanInfo(this.MassUsage, this.MassCapacity, "", this.TilesPerDay, this.cachedTilesPerDayExplanation, this.DaysWorthOfFood, this.ForagedFoodPerDay, this.cachedForagedFoodPerDayExplanation, this.Visibility, this.cachedVisibilityExplanation, this.CaravanMassUsage, this.CaravanMassCapacity, this.cachedCaravanMassCapacityExplanation), new CaravanUIUtility.CaravanInfo?(), this.map.Tile, new int?(), this.lastMassFlashTime, new Rect(12f, 35f, inRect.width - 24f, 40f), false);
            inRect.yMin += 52f;
            tabsList.Clear();
            tabsList.Add(new TabRecord("PawnsTab".Translate(), () => this.tab = Tab.Pawns, this.tab == Tab.Pawns));
            tabsList.Add(new TabRecord("ItemsTab".Translate(), () => this.tab = Tab.Items, this.tab == Tab.Items));
            inRect.yMin += 67f;
            Widgets.DrawMenuSection(inRect);
            TabDrawer.DrawTabs(inRect, tabsList);
            inRect = inRect.ContractedBy(17f);
            GUI.BeginGroup(inRect);

            Rect rect2 = inRect.AtZero();
            this.DoBottomButtons(rect2);
            Rect inRect1 = rect2;
            inRect1.yMax -= 59f;
            bool anythingChanged = false;
            switch (this.tab)
            {
                case Tab.Pawns:
                    this.pawnsTransfer.OnGUI(inRect1, out anythingChanged);
                    break;

                case Tab.Items:
                    this.itemsTransfer.OnGUI(inRect1, out anythingChanged);
                    break;
            }
            if (anythingChanged)
                this.CountToTransferChanged();
            GUI.EndGroup();
        }

        public override void PostOpen()
        {
            base.PostOpen();
            this.CalculateAndRecacheTransferables();
        }

        private void AddToTransferables(Thing t)
        {
            if (teleporter.parent != t) // Dont teleport the teleporter
            {
                TransferableOneWay transferableOneWay = TransferableUtility.TransferableMatching(t, this.transferables, TransferAsOneMode.PodsOrCaravanPacking);
                if (transferableOneWay == null)
                {
                    transferableOneWay = new TransferableOneWay();
                    this.transferables.Add(transferableOneWay);
                }

                if (transferableOneWay.things.Contains(t))
                    Log.Error($"Tried to add the same thing twice to TransferableOneWay: {t}");
                else
                    transferableOneWay.things.Add(t);
            }
        }

        private void CalculateAndRecacheTransferables()
        {
            this.transferables = new List<TransferableOneWay>();
            // Addd pawns
            foreach (Thing allSendablePawn in CaravanFormingUtility.AllSendablePawns(map, true))
                this.AddToTransferables(allSendablePawn);
            // Add items
            foreach (Thing allSendableItem in CaravanFormingUtility.AllReachableColonyItems(map, true, true, true))
                this.AddToTransferables(allSendableItem);

            this.pawnsTransfer = new TransferableOneWayWidget(null, null, null, "FormCaravanColonyThingCountTip".Translate(), true, IgnorePawnsInventoryMode.IgnoreIfAssignedToUnload, true, () => this.MassCapacity - this.MassUsage, tile: this.map.Tile, drawMarketValue: true, drawEquippedWeapon: true, drawNutritionEatenPerDay: true, drawForagedFoodPerDay: true);
            CaravanUIUtility.AddPawnsSections(this.pawnsTransfer, this.transferables);
            this.itemsTransfer = new TransferableOneWayWidget(this.transferables.Where(x => x.ThingDef.category != ThingCategory.Pawn), null, null, "FormCaravanColonyThingCountTip".Translate(), true, IgnorePawnsInventoryMode.IgnoreIfAssignedToUnload, true, () => this.MassCapacity - this.MassUsage, tile: this.map.Tile, drawMarketValue: true, drawItemNutrition: true, drawDaysUntilRot: true);
            this.CountToTransferChanged();
        }

        private bool CheckForErrors()
        {
            if (MassUsage > MassCapacity)
            {
                this.FlashMass();
                Messages.Message("TooBigTransportersMassUsage".Translate(), MessageTypeDefOf.RejectInput, false);
                return false;
            }
            return true;
        }

        private void CountToTransferChanged()
        {
            this.massUsageDirty = true;
            this.caravanMassUsageDirty = true;
            this.caravanMassCapacityDirty = true;
            this.tilesPerDayDirty = true;
            this.daysWorthOfFoodDirty = true;
            this.foragedFoodPerDayDirty = true;
            this.visibilityDirty = true;
        }

        private void DoBottomButtons(Rect rect)
        {
            Rect rect1 = new Rect((float)(rect.width / 2.0 - BottomButtonSize.x / 2.0), rect.height - 55f, this.BottomButtonSize.x, this.BottomButtonSize.y);
            if (Widgets.ButtonText(rect1, "AcceptButton".Translate()))
            {
                if (CaravanMassUsage > CaravanMassCapacity && CaravanMassCapacity != 0f)
                {
                    if (this.CheckForErrors())
                        Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("TransportersCaravanWillBeImmobile".Translate(), () =>
                        {
                            this.LoadInstantly();
                            this.Close(false);
                        }));
                }
                else
                {
                    this.LoadInstantly();
                    this.Close(false);
                }
            }
            if (Widgets.ButtonText(new Rect(rect1.x - 10f - this.BottomButtonSize.x, rect1.y, this.BottomButtonSize.x, this.BottomButtonSize.y), "ResetButton".Translate()))
            {
                this.CalculateAndRecacheTransferables();
            }
            if (Widgets.ButtonText(new Rect(rect1.xMax + 10f, rect1.y, this.BottomButtonSize.x, this.BottomButtonSize.y), "CancelButton".Translate()))
                this.Close();
        }

        private void FlashMass() => this.lastMassFlashTime = Time.time;

        private void LoadInstantly()
        {
            for (int i = 0; i < this.transferables.Count; i++)
                TransferableUtility.Transfer(this.transferables[i].things, this.transferables[i].CountToTransfer, (splitPiece, originalThing) => this.teleporter.GetDirectlyHeldThings().TryAdd(splitPiece.TryMakeMinified()));
        }
    }
}