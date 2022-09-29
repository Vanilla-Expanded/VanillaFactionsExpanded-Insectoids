using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace VFEI
{
    [HarmonyPatch(typeof(JobGiver_MaintainHives), "TryGiveJob", MethodType.Normal)]
    public class JobGiver_MaintainHives_TryGiveJob_Patch
    {
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var found = false;
            var codes = new List<CodeInstruction>(instructions);

            OpCode last = codes[0].opcode;

            for (int i = 0; i < codes.Count; i++)
            {
                if (!found && last == OpCodes.Castclass && codes[i].opcode == OpCodes.Stloc_3)
                {
                    yield return codes[i];
                    yield return new CodeInstruction(OpCodes.Ldarg_1, null);
                    yield return new CodeInstruction(OpCodes.Ldloc_2, null);
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(HarmonyInit), "TryFindLargeHive", null, null));
                    yield return new CodeInstruction(OpCodes.Stloc_3, null);
                    found = true;
                }
                else
                {
                    yield return codes[i];
                }
                last = codes[i].opcode;
            }
            if (!found)
                Log.Error("Cannot find OpCodes.Castclass && OpCodes.Stloc_3 in JobGiver_MaintainHives.TryGiveJob");
            yield break;
        }
    }
}
