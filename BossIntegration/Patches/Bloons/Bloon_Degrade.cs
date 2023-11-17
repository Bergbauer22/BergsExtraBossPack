using BTD_Mod_Helper;
using HarmonyLib;
using Il2CppAssets.Scripts.Simulation.Bloons;
namespace BossIntegration.Patches.Bloons;

[HarmonyPatch(typeof(Bloon), nameof(Bloon.Degrade))]
internal class Bloon_Degrade
{
    [HarmonyPostfix]
    internal static void Postfix(Bloon __instance)
    {
        try
        {
            if (ModBoss.Cache.TryGetValue(__instance.bloonModel.name, out var value))
                value.OnPopMandatory(__instance);
        }
        catch (System.Exception e)
        {
            ModHelper.Error<BossIntegration>(e);
        }
    }
}