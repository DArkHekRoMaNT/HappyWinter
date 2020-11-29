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
            if (__instance.WatchedAttributes.GetBool("frosted"))
            {
                float frostms = __instance.WatchedAttributes.GetFloat("frostms") - dt;
                if (frostms < 0) __instance.WatchedAttributes.SetBool("frosted", false);
                else __instance.WatchedAttributes.SetFloat("frostms", frostms);

                Console.WriteLine("[DD] " + frostms);

                // 75% chance to skip a tick. Lazy way to slow down :D
                if (new Random().Next(4) != 0) return false;
            }
            return true;
        }
    }
}