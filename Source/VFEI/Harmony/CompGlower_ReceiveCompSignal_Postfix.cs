using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace VFEI
{
    [HarmonyPatch(typeof(CompGlower), "ReceiveCompSignal", MethodType.Normal)]
    internal class CompGlower_ReceiveCompSignal_Postfix
    {
        [HarmonyPostfix]
        private static void PostFix(string signal, ref CompGlower __instance)
        {
            if (__instance?.parent != null)
            {
                Map map = __instance.parent.Map;
                if (signal == "armillarixOn")
                {
                    map.mapDrawer.MapMeshDirty(__instance.parent.Position, MapMeshFlag.Things);
                    map.glowGrid.RegisterGlower(__instance);
                }
                else if (signal == "armillarixOff")
                {
                    map.mapDrawer.MapMeshDirty(__instance.parent.Position, MapMeshFlag.Things);
                    map.glowGrid.DeRegisterGlower(__instance);
                }
            }
        }
    }
}
