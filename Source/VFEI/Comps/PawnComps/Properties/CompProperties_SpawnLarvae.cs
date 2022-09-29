using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RimWorld;
using Verse;
using UnityEngine;

namespace VFEI.PawnComps
{
    class CompProperties_SpawnLarvae : CompProperties
    {
        public CompProperties_SpawnLarvae()
        {
            this.compClass = typeof(CompSpawnLarvae);
        }
        #pragma warning disable 0649
        public int ticksBetweenSpawn;

        public int numberToSpawn;
    }
}
