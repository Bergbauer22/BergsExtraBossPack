using System.Linq;
using Il2CppAssets.Scripts.Unity.Menu;
using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Components;
using BTD_Mod_Helper.UI.Modded;
using HarmonyLib;
using BossIntegration.UI;
using BTD_Mod_Helper.Extensions;

namespace BossIntegration.Patches.UI;

[HarmonyPatch(typeof(MenuManager), nameof(MenuManager.CloseCurrentMenuIfPossible))]
internal static class MenuManager_CloseCurrentMenu
{
    [HarmonyPostfix]
    private static void Postfix(MenuManager __instance, ref GameMenu __state)
    {
        BossesMenuBtn.OnMenuChanged(__state.Exists()?.name ?? "",
            __instance.menuStack.ToList().SkipLast(1).LastOrDefault()?.Item1 ?? "");
    }
}