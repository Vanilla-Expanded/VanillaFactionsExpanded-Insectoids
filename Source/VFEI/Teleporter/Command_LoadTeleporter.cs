using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace VFEI
{
    internal class Command_LoadTeleporter : Command
    {
        public CompTeleporter teleporterComp;

        /** Teleporter actions */

        public void PostActionDone()
        {
            Find.WorldTargeter.StopTargeting();
            Find.Targeter.StopTargeting();
            this.teleporterComp.parent.Destroy();
        }

        public ActiveDropPod ActionMakeDropPodInfo()
        {
            ThingOwner directlyHeldThings = teleporterComp.GetDirectlyHeldThings();
            ActiveDropPod activeDropPod = (ActiveDropPod)ThingMaker.MakeThing(ThingDefOf.ActiveDropPod);
            activeDropPod.Contents = new ActiveDropPodInfo();
            activeDropPod.Contents.innerContainer.TryAddRangeOrTransfer(directlyHeldThings, destroyLeftover: true);
            activeDropPod.Contents.openDelay = 0;
            activeDropPod.Contents.leaveSlag = false;

            return activeDropPod;
        }

        public void ActionAttackSettlement(ActiveDropPodInfo info, int tile, Settlement settlement, bool edge = false)
        {
            List<ActiveDropPodInfo> infos = new List<ActiveDropPodInfo>() { info };
            Thing lookTarget = TransportPodsArrivalActionUtility.GetLookTarget(infos);
            Map map = GetOrGenerateMapUtility.GetOrGenerateMap(tile, null);

            TaggedString letterLabel = "LetterLabelCaravanEnteredEnemyBase".Translate();
            TaggedString letterText = "LetterTransportPodsLandedInEnemyBase".Translate(settlement.Label).CapitalizeFirst();
            SettlementUtility.AffectRelationsOnAttacked(settlement, ref letterText);

            if (!settlement.HasMap)
            {
                Find.TickManager.Notify_GeneratedPotentiallyHostileMap();
                PawnRelationUtility.Notify_PawnsSeenByPlayer_Letter(map.mapPawns.AllPawns, ref letterLabel, ref letterText, "LetterRelatedPawnsInMapWherePlayerLanded".Translate(Faction.OfPlayer.def.pawnsPlural), true);
            }

            Find.LetterStack.ReceiveLetter(letterLabel, letterText, LetterDefOf.NeutralEvent, lookTarget, settlement.Faction);

            IntVec3 edgeCell = CellFinder.RandomEdgeCell(settlement.Map);
            this.teleporterComp.innerContainer.TryDropAll(edgeCell, map, ThingPlaceMode.Near, nearPlaceValidator: i => i.InBounds(map) && i.Walkable(settlement.Map));
            SpawnCenterOrEdge(edge, map, infos);
            PostActionDone();
        }

        public void ActionFormCaravan(int tile)
        {
            List<Pawn> tmpPawns = new List<Pawn>();
            for (int i = teleporterComp.innerContainer.Count - 1; i >= 0; --i)
            {
                if (teleporterComp.innerContainer[i] is Pawn p)
                {
                    tmpPawns.Add(p);
                    teleporterComp.innerContainer.Remove(p);
                }
            }

            if (!GenWorldClosest.TryFindClosestPassableTile(tile, out int foundTile))
                foundTile = tile;

            Caravan caravan = CaravanMaker.MakeCaravan(tmpPawns, Faction.OfPlayer, foundTile, true);
            List<Thing> tmpContainedThings = new List<Thing>();
            tmpContainedThings.AddRange(teleporterComp.innerContainer);

            for (int i = 0; i < tmpContainedThings.Count; ++i)
            {
                teleporterComp.innerContainer.Remove(tmpContainedThings[i]);
                CaravanInventoryUtility.GiveThing(caravan, tmpContainedThings[i]);
            }

            Messages.Message("TeleportDoneCaravan".Translate(), caravan, MessageTypeDefOf.TaskCompletion);
            PostActionDone();
        }

        public void ActionGiveGift(Settlement settlement)
        {
            for (int i = 0; i < teleporterComp.innerContainer.Count; ++i)
            {
                if (teleporterComp.innerContainer[i] is Pawn prisonner)
                {
                    if (prisonner.RaceProps.Humanlike)
                    {
                        if (prisonner.HomeFaction == settlement.Faction)
                        {
                            GenGuest.AddHealthyPrisonerReleasedThoughts(prisonner);
                        }
                        else
                        {
                            if (PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive_FreeColonists.TryRandomElement(out Pawn result))
                                Find.HistoryEventsManager.RecordEvent(new HistoryEvent(HistoryEventDefOf.SoldSlave, result.Named(HistoryEventArgsNames.Doer)));
                        }
                    }
                    else if (prisonner.RaceProps.Animal && prisonner.relations != null)
                    {
                        Pawn directRelationPawn = prisonner.relations.GetFirstDirectRelationPawn(PawnRelationDefOf.Bond);
                        if (directRelationPawn != null && directRelationPawn.needs.mood != null)
                        {
                            prisonner.relations.RemoveDirectRelation(PawnRelationDefOf.Bond, directRelationPawn);
                            directRelationPawn.needs.mood.thoughts.memories.TryGainMemory(ThoughtDefOf.SoldMyBondedAnimalMood);
                        }
                    }
                }
            }
            FactionGiftUtility.GiveGift(new List<ActiveDropPodInfo>() { this.ActionMakeDropPodInfo().Contents }, settlement);
            PostActionDone();
        }

        public void ActionGiveToCaravan(Caravan caravan)
        {
            List<Thing> tmpContainedThings = new List<Thing>();
            tmpContainedThings.AddRange(teleporterComp.innerContainer);
            for (int i = 0; i < tmpContainedThings.Count; ++i)
            {
                teleporterComp.innerContainer.Remove(tmpContainedThings[i]);
                caravan.AddPawnOrItem(tmpContainedThings[i], true);
                if (tmpContainedThings[i] is Pawn pawn && pawn != null && !pawn.IsWorldPawn())
                    Find.WorldPawns.PassToWorld(pawn);
            }
            Messages.Message("TeleportDoneCAdded".Translate(caravan.Name), caravan, MessageTypeDefOf.TaskCompletion);
            PostActionDone();
        }

        public void ActionVisiteSite(Site site, bool edge = false)
        {
            List<ActiveDropPodInfo> infos = new List<ActiveDropPodInfo>() { ActionMakeDropPodInfo().Contents };
            Thing lookTarget = TransportPodsArrivalActionUtility.GetLookTarget(infos);

            Map map = GetOrGenerateMapUtility.GetOrGenerateMap(site.Tile, site.PreferredMapSize, null);
            if (!site.HasMap)
            {
                Find.TickManager.Notify_GeneratedPotentiallyHostileMap();
                PawnRelationUtility.Notify_PawnsSeenByPlayer_Letter_Send(map.mapPawns.AllPawns, "LetterRelatedPawnsInMapWherePlayerLanded".Translate(Faction.OfPlayer.def.pawnsPlural), LetterDefOf.NeutralEvent, true);
            }
            if (site.Faction != null && site.Faction != Faction.OfPlayer)
                Faction.OfPlayer.TryAffectGoodwillWith(site.Faction, Faction.OfPlayer.GoodwillToMakeHostile(site.Faction), reason: HistoryEventDefOf.AttackedSettlement);

            Messages.Message("MessageTransportPodsArrived".Translate(), lookTarget, MessageTypeDefOf.TaskCompletion);
            SpawnCenterOrEdge(edge, map, infos);
            PostActionDone();
        }

        public void ActionNone(ActiveDropPod activeDropPod, int tile)
        {
            for (int i = 0; i < activeDropPod.Contents.innerContainer.Count; i++)
            {
                if (activeDropPod.Contents.innerContainer[i] is Pawn p && (p.Faction == Faction.OfPlayer || p.HostFaction == Faction.OfPlayer))
                    PawnBanishUtility.Banish(p, tile);
            }
            activeDropPod.Contents.innerContainer.ClearAndDestroyContentsOrPassToWorld();

            Messages.Message("TeleportDestroyed".Translate(), new GlobalTargetInfo(tile), MessageTypeDefOf.NegativeEvent);
            PostActionDone();
        }

        public void SpawnCenterOrEdge(bool edge, Map map, List<ActiveDropPodInfo> infos)
        {
            if (edge)
                PawnsArrivalModeDefOf.EdgeDrop.Worker.TravelingTransportPodsArrived(infos, map);
            else
                PawnsArrivalModeDefOf.CenterDrop.Worker.TravelingTransportPodsArrived(infos, map);
        }

        /** Targeting */

        public string TargetingLabelGetter(GlobalTargetInfo target, IThingHolder teleporter)
        {
            if (!target.IsValid) return null;

            IEnumerable<FloatMenuOption> options = GetTeleporterFloatMenuOptionsAt(target.Tile);
            if (!options.Any()) return string.Empty;
            if (options.Count() == 1)
            {
                if (options.First().Disabled) GUI.color = ColorLibrary.RedReadable;

                return options.First().Label;
            }
            return target.WorldObject is MapParent worldObject ? "ClickToSeeAvailableOrders_WorldObject".Translate(worldObject.LabelCap) : "ClickToSeeAvailableOrders_Empty".Translate();
        }

        public bool ChoseWorldTarget(GlobalTargetInfo target)
        {
            if (!target.IsValid)
            {
                Messages.Message("MessageTransportPodsDestinationIsInvalid".Translate(), MessageTypeDefOf.RejectInput, false);
                return false;
            }
            IEnumerable<FloatMenuOption> options = GetTeleporterFloatMenuOptionsAt(target.Tile);
            if (!options.Any())
            {
                if (Find.World.Impassable(target.Tile))
                {
                    Messages.Message("MessageTransportPodsDestinationIsInvalid".Translate(), MessageTypeDefOf.RejectInput, false);
                    return false;
                }
                return true;
            }
            if (options.Count() == 1)
            {
                if (options.First().Disabled)
                    return false;
                options.First().action();
                return true;
            }
            Find.WindowStack.Add(new FloatMenu(options.ToList()));
            return false;
        }

        /** Main */

        public override void ProcessInput(Event ev)
        {
            base.ProcessInput(ev);
            Find.WindowStack.Add(new Dialog_LoadTeleporter(this.teleporterComp.Map, this.teleporterComp));

            CameraJumper.TryJump(CameraJumper.GetWorldTarget(this.teleporterComp.parent));
            Find.WorldSelector.ClearSelection();
            Find.WorldTargeter.BeginTargeting(ChoseWorldTarget, true, null, false, null, t => TargetingLabelGetter(t, teleporterComp));
        }

        /** Options */

        private IEnumerable<FloatMenuOption> GetTeleporterFloatMenuOptionsAt(int tile)
        {
            List<IThingHolder> teleporters = new List<IThingHolder>() { this.teleporterComp };
            bool anything = false;

            if (TransportPodsArrivalAction_FormCaravan.CanFormCaravanAt(teleporters, tile) && !Find.WorldObjects.AnySettlementBaseAt(tile) && !Find.WorldObjects.AnySiteAt(tile))
            {
                yield return new FloatMenuOption("FormCaravanHere".Translate(), () => ActionFormCaravan(tile));
                anything = true;
            }

            List<WorldObject> worldObjects = Find.WorldObjects.AllWorldObjects;
            for (int i = 0; i < worldObjects.Count; ++i)
            {
                if (worldObjects[i].Tile == tile)
                {
                    if (worldObjects[i] is Settlement settlement)
                    {
                        if (settlement != null && settlement.Spawned)
                        {
                            if (settlement.Attackable && TransportPodsArrivalActionUtility.AnyNonDownedColonist(teleporters) && !settlement.EnterCooldownBlocksEntering()) // Attack settlement
                            {
                                yield return new FloatMenuOption("TeleportAndAttackCenter".Translate(settlement.LabelShortCap), () => ActionAttackSettlement(ActionMakeDropPodInfo().Contents, tile, settlement));
                                yield return new FloatMenuOption("TeleportAndAttackEgde".Translate(settlement.LabelShortCap), () => ActionAttackSettlement(ActionMakeDropPodInfo().Contents, tile, settlement, true));
                                anything = true;
                            }
                            if (settlement.Faction != null && settlement.Faction != Faction.OfPlayer && !settlement.Faction.def.permanentEnemy && !settlement.HasMap && !teleporterComp.GetDirectlyHeldThings().ToList().FindAll(t => t is Pawn p && p.IsQuestLodger()).Any())
                            {
                                yield return new FloatMenuOption("TeleportAndGift".Translate(settlement.Faction.Name, FactionGiftUtility.GetGoodwillChange(teleporters, settlement).ToStringWithSign()), () => ActionGiveGift(settlement));
                                anything = true;
                            }
                        }
                    }
                    else if (worldObjects[i] is Caravan caravan && caravan != null && caravan.Spawned && caravan.IsPlayerControlled)
                    {
                        yield return new FloatMenuOption("TeleportAddToCaravan".Translate(caravan.LabelShortCap), () => ActionGiveToCaravan(caravan));
                        anything = true;
                    }
                    else if (worldObjects[i] is Site s && s.Tile == tile && s.Spawned && !s.EnterCooldownBlocksEntering() && TransportPodsArrivalActionUtility.AnyNonDownedColonist(new List<IThingHolder>() { teleporterComp }))
                    {
                        yield return new FloatMenuOption("TeleportToSiteCenter".Translate(s.Label), () => ActionVisiteSite(s));
                        yield return new FloatMenuOption("TeleportToSiteEdge".Translate(s.Label), () => ActionVisiteSite(s, true));
                        anything = true;
                    }

                    if (worldObjects[i] is MapParent mapParent && mapParent.Spawned && mapParent.HasMap && !mapParent.EnterCooldownBlocksEntering())
                    {
                        yield return new FloatMenuOption("TeleportAndTarget".Translate(), () =>
                        {
                            Current.Game.CurrentMap = mapParent.Map;
                            CameraJumper.TryHideWorld();

                            Find.Targeter.BeginTargeting(TargetingParameters.ForDropPodsDestination(), (targetInfo) =>
                            {
                                teleporterComp.innerContainer.TryDropAll(targetInfo.Cell, mapParent.Map, ThingPlaceMode.Near, null, p => p.Walkable(mapParent.Map));
                            }, actionWhenFinished: () =>
                            {
                                PostActionDone();
                                if (!Find.Maps.Contains(mapParent.Map)) return;
                                Current.Game.CurrentMap = mapParent.Map;
                            });
                        });
                        anything = true;
                    }
                }
            }

            if (!anything || (!anything && !Find.World.Impassable(tile)))
                yield return new FloatMenuOption("TransportPodsContentsWillBeLost".Translate(), () => ActionNone(ActionMakeDropPodInfo(), tile));
        }
    }
}