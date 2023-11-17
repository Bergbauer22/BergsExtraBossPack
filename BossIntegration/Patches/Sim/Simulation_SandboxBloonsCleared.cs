using BossIntegration.UI;
using HarmonyLib;
using Il2CppAssets.Scripts.Simulation;

namespace BossIntegration.Patches.Sim;

[HarmonyPatch(typeof(Simulation), nameof(Simulation.SandboxBloonsCleared))]
internal class Simulation_SandboxBloonsCleared
{
    [HarmonyPostfix]
    internal static void Postfix()
    {
        if (ModBoss.Cache.Count > 0)
        {
            ModBossUI.Init();
        }
    }
}