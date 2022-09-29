using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace VFEI
{
    internal class CompProperties_Teleporter : CompProperties
    {
        public float massCapacity = 500f;

        public CompProperties_Teleporter() => this.compClass = typeof(CompTeleporter);
    }

    [StaticConstructorOnStartup]
    internal class CompTeleporter : ThingComp, IThingHolder
    {
        public ThingOwner innerContainer;
        private static readonly Texture2D LoadCommandTex = ContentFinder<Texture2D>.Get("UI/ArchotechTeleporterGizmo");

        public CompTeleporter() => this.innerContainer = new ThingOwner<Thing>(this);

        public Map Map => this.parent.MapHeld;
        public CompProperties_Teleporter Props => (CompProperties_Teleporter)this.props;

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (Gizmo gizmo in base.CompGetGizmosExtra())
            {
                yield return gizmo;
            }
            Command_LoadTeleporter load = new Command_LoadTeleporter
            {
                defaultLabel = "TeleportButton".Translate(),
                defaultDesc = "TeleportDesc".Translate(),
                icon = LoadCommandTex,
                teleporterComp = this
            };
            yield return load;
            yield break;
        }

        public override void CompTickRare()
        {
            base.CompTickRare();
            if (!innerContainer.NullOrEmpty() && !Find.WorldTargeter.IsTargeting && !Find.Targeter.IsTargeting)
            {
                innerContainer.TryDropAll(this.parent.Position, Map, ThingPlaceMode.Near, null, p => p.Walkable(Map), false);
            }
        }

        public void GetChildHolders(List<IThingHolder> outChildren) => ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, this.GetDirectlyHeldThings());

        public ThingOwner GetDirectlyHeldThings() => this.innerContainer;

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Deep.Look(ref this.innerContainer, "innerContainer", this);
        }
    }
}