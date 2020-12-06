using System;
using HarmonyLib;
using Vintagestory.API.Common;

namespace HappyWinter.Patches.EntityAgentFrost
{

    [HarmonyPatch(typeof(EntityAgent), "ReceiveDamage")]
    class ReceiveDamagePatch
    {
        static bool Prefix(EntityAgent __instance, ref DamageSource damageSource)
        {
            if (__instance.WatchedAttributes.GetBool("receiveFrostDamage"))
            {
                __instance.WatchedAttributes.SetBool("receiveFrostDamage", false);
                __instance.WatchedAttributes.SetBool("frosted", true);
                __instance.WatchedAttributes.SetFloat("frostms", new Random().Next(2000, 5000) / 1000f); // sec
            }
            return true;
        }
    }
}