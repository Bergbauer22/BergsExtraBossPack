using HarmonyLib;

namespace BossIntegration.Patches.InGame;


[HarmonyPatch(typeof(Il2CppAssets.Scripts.Unity.UI_New.InGame.InGame), nameof(Il2CppAssets.Scripts.Unity.UI_New.InGame.InGame.CreateCurrentMapSave))]
internal class InGame_CreateCurrentMapSave
{
    [HarmonyPrefix]
    internal static bool Prefix()
    {
        return ModBoss.BossesAlive.Count == 0;
    }
}