using System;
using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace appyWinter.Patches.EntityAgentFrost
{

    [HarmonyPatch(typeof(EntityAgent), "OnGameTick")]
    class OnGameTickPatch
    {
        static bool Prefix(EntityAgent __instance, ref float dt)
        {
            __instance.Stats.Remove("walkspeed", "frostmod");

            if (__instance.WatchedAttributes.GetBool("frosted"))
            {
                float frostms = __instance.WatchedAttributes.GetFloat("frostms") - dt;
                if (frostms < 0) __instance.WatchedAttributes.SetBool("frosted", false);
                else __instance.WatchedAttributes.SetFloat("frostms", frostms);

                float walkspeed = __instance.Stats.GetBlended("walkspeed");
                float frostmod = -0.5f; // 50% slowly
                __instance.Stats.Set("walkspeed", "frostmod", walkspeed * frostmod, false);
            }
            return true;
        }
    }
}