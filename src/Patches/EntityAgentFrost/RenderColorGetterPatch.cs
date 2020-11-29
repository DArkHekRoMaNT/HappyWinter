using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;

namespace appyWinter.Patches.EntityAgentFrost
{

    [HarmonyPatch(typeof(Entity))]
    [HarmonyPatch("RenderColor", MethodType.Getter)]
    class RenderColorGetterPatch
    {
        static bool Prefix(Entity __instance, ref int __result)
        {
            if ((__instance as EntityAgent)?.WatchedAttributes.GetBool("frosted") == true)
            {
                float frostms = (__instance as EntityAgent).WatchedAttributes.GetFloat("frostms");
                __result = ColorUtil.ColorOverlay(ColorUtil.ToRgba(255, 100, 100, 255), ColorUtil.WhiteArgb, 1 / 250f);
                return false;
            }
            return true;
        }
    }
}