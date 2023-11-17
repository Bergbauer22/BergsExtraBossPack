using HarmonyLib;
namespace BossIntegration.Patches.InGame;


[HarmonyPatch(typeof(Il2CppAssets.Scripts.Unity.UI_New.InGame.InGame), nameof(Il2CppAssets.Scripts.Unity.UI_New.InGame.InGame.Quit))]
internal class InGame_Quit
{
    [HarmonyPostfix]
    internal static void Postfix()
    {
        if (ModBoss.Cache.Count > 0)
            ModBoss.ResetUIs();
    }
}