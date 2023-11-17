using BossIntegration;
using BossIntegration.UI;
using BTD_Mod_Helper;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Api.ModOptions;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using MelonLoader;



namespace BossIntegration;

public class BossIntegration : BloonsTD6Mod
{
    public static readonly int FontSmall = 52;
    public static readonly int FontMedium = 69;
    public static readonly int FontLarge = 80;

    public static readonly int Padding = 50;
    public static readonly int MenuWidth = 3600;
    public static readonly int MenuHeight = 1900;

    public static readonly int ModNameHeight = 150;
    public static readonly int ModNameWidth = 1000;

    public static readonly int ModIconSize = 250;


    private static readonly ModSettingCategory General = new("General")
    {
        collapsed = false,
        icon = VanillaSprites.SettingsIcon
    };

    public static readonly ModSettingBool ShowBossSpeedAsPercentage = new(true)
    {
        description = "Toggles showing the boss speed in the boss menu as a percentage or the literal value.",
        category = General,
        icon = VanillaSprites.FasterBloonsIcon,
    };

    public static readonly ModSettingBool FormatBossHP = new(false)
    {
        description = "If toggled, the bosses HP will be format to make it easier to read.\n\nWarning: this process will make the game slower because of the calculations.",
        category = General,
        icon = VanillaSprites.LivesIcon,
    };

    public override void OnInGameLoaded(InGame inGame)
    {
        if (ModBoss.Cache.Count > 0)
        {
            ModBossUI.Init();
        }
    }
}