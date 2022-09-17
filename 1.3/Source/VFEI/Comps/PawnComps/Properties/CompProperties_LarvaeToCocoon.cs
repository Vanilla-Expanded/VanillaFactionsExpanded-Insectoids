using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RimWorld;
using Verse;
using UnityEngine;

namespace VFEI.PawnComps
{
    class CompProperties_LarvaeToCocoon : CompProperties
    {
        public CompProperties_LarvaeToCocoon()
        {
            this.compClass = typeof(CompLarvaeToCocoon);
        }
        #pragma warning disable 0649
        public int ticksBeforeBecomingCocoon;
    }
}
