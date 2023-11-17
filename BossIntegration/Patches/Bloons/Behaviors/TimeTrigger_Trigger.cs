using BTD_Mod_Helper;
using HarmonyLib;
using Il2CppAssets.Scripts.Simulation.Bloons.Behaviors;

namespace BossIntegration;

[HarmonyPatch(typeof(TimeTrigger), nameof(TimeTrigger.Trigger))]
internal static class TimeTrigger_Trigger
{
    [HarmonyPrefix]
    private static void Prefix(TimeTrigger __instance)
    {
        if (ModBoss.BossesAlive.ContainsKey(__instance.bloon.Id))
        {
            ModBoss.Cache[__instance.bloon.bloonModel.name].TimerTick(__instance.bloon);
        }
    }
}