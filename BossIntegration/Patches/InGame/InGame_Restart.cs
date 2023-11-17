using BossIntegration.UI;
using HarmonyLib;
namespace BossIntegration.Patches.InGame;

[HarmonyPatch(typeof(Il2CppAssets.Scripts.Unity.UI_New.InGame.InGame), nameof(Il2CppAssets.Scripts.Unity.UI_New.InGame.InGame.Restart))]
internal class InGame_Restart
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