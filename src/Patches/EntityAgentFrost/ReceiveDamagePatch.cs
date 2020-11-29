using System;
using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;

namespace HappyWinter.Patches.EntityAgentFrost
{

    [HarmonyPatch(typeof(EntityAgent), "ReceiveDamage")]
    class ReceiveDamagePatch
    {
        static bool Prefix(EntityAgent __instance, ref DamageSource damageSource)
        {
            if (damageSource.Type == EnumDamageType.Frost)
            {
                __instance.WatchedAttributes.SetBool("frosted", true);
                __instance.WatchedAttributes.SetFloat("frostms", new Random().Next(2000, 5000) / 1000f); // sec
                Console.WriteLine("[DD] " + __instance.WatchedAttributes.GetFloat("frostms"));
            }
            return true;
        }
    }
}