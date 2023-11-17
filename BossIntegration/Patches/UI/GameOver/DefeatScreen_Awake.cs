using BossIntegration.UI;
using HarmonyLib;
using Il2CppAssets.Scripts.Unity.UI_New.GameOver;
using System;

namespace BossIntegration.Patches.UI.GameOver;

[HarmonyPatch(typeof(DefeatScreen), nameof(DefeatScreen.Awake))]
internal class DefeatScreen_Awake
{
    [HarmonyPostfix]
    internal static void Postfix(DefeatScreen __instance)
    {
        __instance.retryLastRoundButton.onClick.AddListener(new Action(() =>
        {
            if (ModBoss.Cache.Count > 0)
            {
                ModBossUI.Init();
            }
        }));
    }
}