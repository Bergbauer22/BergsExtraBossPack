// Adapted from WarperSan's BossPack 

using BergsExtraBossPack;
using BossIntegration.UI;
using BossIntegration.UI.Menus.Bosses;
using BTD_Mod_Helper;
using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Bloons;
using BTD_Mod_Helper.Api.Components;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using Il2Cpp;
using Il2CppAssets.Scripts;
using Il2CppAssets.Scripts.Models.Bloons;
using Il2CppAssets.Scripts.Models.Bloons.Behaviors;
using Il2CppAssets.Scripts.Simulation.Bloons;
using Il2CppAssets.Scripts.Unity.UI_New;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace BossIntegration;

/// <summary>
/// Class for adding a new boss to the game
/// </summary>
public abstract class ModBoss : ModBloon
{
    public static readonly Dictionary<string, ModBoss> Cache = new();

    public static Dictionary<ObjectId, BossUI> BossesAlive = new();

    #region Permissions
    public static Dictionary<string, Dictionary<int, bool>> Permissions = InitPermissions();
    private static Dictionary<string, Dictionary<int, bool>> InitPermissions()
    {
        var permissions = new Dictionary<string, Dictionary<int, bool>>();

        if (File.Exists(BossesSettings.Path))
        {
            var json = JObject.Parse(File.ReadAllText(BossesSettings.Path));
            foreach (var (name, token) in json)
            {
                if (token == null)
                    continue;

                var p = token.ToObject<Dictionary<int, bool>>();

                if (p != null)
                    permissions.Add(name, p);
            }
        }

        return permissions;
    }

    public static bool GetPermission(ModBoss boss, int round)
        => ModBoss.Permissions.TryGetValue(boss.ToString() ?? "", out var permissions) && permissions.TryGetValue(round, out bool allowed) ? allowed : true;
    #endregion

    #region Mandatories
    internal void OnSpawnMandatory(Bloon bloon, ModBoss boss)
    {
        int round = InGame.Bridge.GetCurrentRound() + 1;
        BossRoundInfo info = RoundsInfo[round];

        // No duplicates
        if (info.defeatIfPreviousNotDefeated != null && (bool)info.defeatIfPreviousNotDefeated)
        {
            if (ModBossUI.MainPanel != null)
            {
                for (int i = 0; i < ModBossUI.MainPanel.transform.childCount; i++)
                {
                    if (ModBossUI.MainPanel.transform.GetChild(i).gameObject.name == $"{Name}-Panel")
                    {
                        InGame.instance.Lose();
                        InGame.instance.SetHealth(0);
                        break;
                    }
                }
            }
        }

        // Skulls
        if (bloon.bloonModel.HasBehavior<HealthPercentTriggerModel>() && info.skullCount != null)
        {
            // Puts default skulls placement
            if (info.percentageValues == null)
            {
                uint skullsCount = (uint)info.skullCount;

                List<float> pV = new List<float>();

                if (skullsCount > 0)
                {
                    for (int i = 1; i <= skullsCount; i++)
                    {
                        pV.Add(1f - 1f / (skullsCount + 1) * i);
                    }
                }

                info.percentageValues = pV.ToArray();
            }

            HealthPercentTriggerModel bossSkulls = bloon.bloonModel.GetBehavior<HealthPercentTriggerModel>();
            bossSkulls.percentageValues = info.percentageValues;
            bossSkulls.preventFallthrough = info.preventFallThrough != null ? (bool)info.preventFallThrough : false;
        }

        // Timer
        if (bloon.bloonModel.HasBehavior<TimeTriggerModel>())
        {
            bloon.bloonModel.RemoveBehaviors<TimeTriggerModel>();
        }

        if (info.interval != null || Interval != null)
        {
            TimeTriggerModel timer = new TimeTriggerModel(Name + "-TimerTick"); //bloon.bloonModel.GetBehavior<TimeTriggerModel>();
            timer.actionIds = new string[] { Name + "TimerTick" };

            if (info.interval != null)
            {
                timer.interval = (float)info.interval;
            }
            else if (Interval != null)
            {
                timer.interval = (float)Interval;
            }
            timer.triggerImmediately = info.triggerImmediately != null ? (bool)info.triggerImmediately : false;

            bloon.bloonModel.AddBehavior(timer);
        }

        bloon.UpdatedModel(bloon.bloonModel);

        if (InGameButtonsHolder.subPanel == null)
            return;

        // Boss Icon
        ModHelperButton icon = InGameButtonsHolder.subPanel.AddButton(new Info(Name + "-Icon", 170), ModContent.GetTextureGUID(mod, Icon), new System.Action(() =>
        {
            foreach (var item in BossesAlive)
            {
                if (item.Value.Panel == null)
                    continue;

                item.Value.Panel.SetActive(item.Key == bloon.Id);
            }
        }));

        InGameButtonsHolder.AddButton(icon);
        BossUI ui = new BossUI()
        {
            Boss = boss,
            RoundInfo = info,
            Icon = icon,
        };

        if (ModBossUI.MainPanel != null)
        {
            AddBossPanel(ModBossUI.MainPanel, bloon, ref ui);
        }

        BossesAlive.Add(bloon.Id, ui);

        if (BossesAlive.Count + ModBossUI.WaitingPanels.Count > maxLines)
        {
            var bosses = BossesAlive.Values.ToArray();
            for (int i = 0; i < bosses.Length - 1; i++)
            {
                bosses[i].Panel.SetActive(false);
            }
        }

        OnSpawn(bloon);
    }
    internal void OnLeakMandatory(Bloon bloon)
    {
        RemoveUI(bloon);
        OnLeak(bloon);
    }
    internal void OnPopMandatory(Bloon bloon)
    {
        RemoveUI(bloon);
        OnPop(bloon);
    }
    internal void OnDamageMandatory(Bloon bloon, float totalAmount)
    {
        if (BossesAlive.TryGetValue(bloon.Id, out BossUI ui))
        {
            if (ui.HpBar != default(Image))
                ui.HpBar.fillAmount = (float)bloon.health / (float)bloon.bloonModel.maxHealth;
                ModHelper.Log<BergsExtraBossPackMOD>(ui.HpBar.fillAmount);

            if (ui.HpText != default(ModHelperText))
            {
                SetHP(Mathf.FloorToInt(bloon.health), bloon.bloonModel.maxHealth, ui.HpText);
                //ModHelper.Msg<BergsExtraBossPackMOD>(bloon.health);
            }
        }

        OnDamage(bloon, totalAmount);
    }
    #endregion

    #region Virtuals
    /// <summary>
    /// Called when the boss is spawned
    /// </summary>
    /// <param name="bloon"></param>
    public virtual void OnSpawn(Bloon bloon) { }

    /// <summary>
    /// Called when the boss is leaked
    /// </summary>
    /// <param name="bloon"></param>
    public virtual void OnLeak(Bloon bloon) { }

    /// <summary>
    /// Called when the boss is popped
    /// </summary>
    /// <param name="bloon"></param>
    public virtual void OnPop(Bloon bloon) { }

    /// <summary>
    /// Called when the boss takes a damage
    /// </summary>
    /// <param name="bloon"></param>
    /// <param name="totalAmount"></param>
    public virtual void OnDamage(Bloon bloon, float totalAmount) { }

    public virtual void OnBossesRemoved() { }
    #endregion

    #region Stats
    /// <summary>
    /// The base speed of the boss, 4.5 is the default for a BAD and 25 is the default for a red bloon
    /// </summary>
    public virtual float Speed => 4.5f;

    /// <summary>
    /// The base health of the boss
    /// </summary>
    public virtual int Health => 20_000;

    /// <summary>
    /// Whether the boss should always cause defeat on leak
    /// </summary>
    public virtual bool AlwaysDefeatOnLeak => true;

    /// <summary>
    /// Whether the boss should block rounds from spawning, behaving like a normal bloon
    /// </summary>
    public virtual bool BlockRounds => false;

    /// <summary>
    /// All the informations a boss holds for a specific round
    /// </summary>
    public struct BossRoundInfo
    {
        /// <summary>
        /// Tier of the boss on this round
        /// </summary>
        public uint? tier = null;

        /// <summary>
        /// Amount of skulls the boss has
        /// </summary>
        public uint? skullCount = null;

        /// <summary>
        /// Positions of the skulls
        /// </summary>
        /// <remarks>
        /// If not specified, the skulls' position will be placed evenly (3 skulls => 0.75, 0.5, 0.25)
        /// </remarks>
        public float[]? percentageValues = null;

        /// <summary>
        /// The description of this particular skull effect
        /// </summary>
        /// <remarks>
        /// If not specified, the API will use <see cref="SkullDescription"/>
        /// </remarks>
        public string? skullDescription = null;

        /// <summary>
        /// Determines if the boss's health should go down while it's skull effect is on
        /// </summary>
        public bool? preventFallThrough = null;

        /// <summary>
        /// Determines if the timer starts immediately
        /// </summary>
        public bool? triggerImmediately = null;

        /// <summary>
        /// Interval between ticks
        /// </summary>
        public float? interval = null;

        /// <summary>
        /// The description of this particualr timer effect
        /// </summary>
        /// <remarks>
        /// If not specified, the API will use <see cref="TimerDescription"/>
        /// </remarks>
        public string? timerDescription = null;

        /// <summary>
        /// Determines if the previous boss must be killed before this one
        /// </summary>
        public bool? defeatIfPreviousNotDefeated = null;

        public BossRoundInfo() { }
    }

    /// <summary>
    /// Informations about the boss on the round
    /// </summary>
    public abstract Dictionary<int, BossRoundInfo> RoundsInfo { get; }

    /// <summary>
    /// Modifies the boss before it is spawned, based on the round
    /// </summary>
    /// <param name="bloon"></param>
    /// <param name="round"></param>
    /// <returns></returns>
    public virtual BloonModel ModifyForRound(BloonModel bloon, int round) => bloon;

    public static float SpeedToSpeedFrames(float speed) => speed * 0.416667f / 25;

    /// <summary>
    /// The rounds the boss should spawn on
    /// </summary>
    public IEnumerable<int> SpawnRounds => RoundsInfo.Keys;

    /// <summary>
    /// Gets the tier of the given boss
    /// </summary>
    /// <param name="boss"></param>
    /// <returns>Tier of the boss or null if the boss's tier wasn't specified or the boss isn't registered</returns>
    public static uint? GetTier(Bloon boss) => BossesAlive.ContainsKey(boss.Id) ? BossesAlive[boss.Id].RoundInfo.tier : null;
    #endregion

    #region Skulls
    /// <summary>
    /// The description of the skull effect (only used by the Boss API)
    /// </summary>
    public virtual string SkullDescription => "???";

    /// <summary>
    /// Checks if the boss has any skull on any round
    /// </summary>
    public bool UsesSkulls => RoundsInfo.Any(info => info.Value.skullCount > 0);

    /// <summary>
    /// Called when the boss hits a skull
    /// </summary>
    /// <param name="boss"></param>
    public virtual void SkullEffect(Bloon boss) { }

    /// <summary>
    /// Called when the boss should get a skull remove
    /// </summary>
    /// <param name="boss"></param>
    public virtual void SkullEffectUI(Bloon boss)
    {
        if (BossesAlive[boss.Id].Skulls.Count != 0)
            BossesAlive[boss.Id].Skulls.First(img => img != null).DeleteObject();
    }
    #endregion

    #region Timer
    /// <summary>
    /// The description of the timer effect (only used by the Boss API)
    /// </summary>
    public virtual string TimerDescription => "???";

    public virtual uint? Interval => null;

    /// <summary>
    /// Checks if the boss uses a timer on any round
    /// </summary>
    public bool UsesTimer => RoundsInfo.Any(info => info.Value.interval != null) || Interval != null;

    /// <summary>
    /// Called when the boss timer ticks
    /// </summary>
    /// <param name="boss"></param>
    public virtual void TimerTick(Bloon boss) { }
    #endregion

    #region UIs
    const uint maxLines = 3;

    public struct BossUI
    {
        public ModBoss Boss;
        public BossRoundInfo RoundInfo;
        public ModHelperPanel Panel;
        public Image HpBar;
        public ModHelperText HpText;
        public ModHelperButton Icon;
        public List<ModHelperImage> Skulls;
    }

    /// <summary>
    /// The people who worked/helped to create the mod, but aren't the authors
    /// </summary>
    public virtual string? ExtraCredits { get; }

    /// <summary>
    /// Defines if the boss is using the default waiting UI
    /// </summary>
    public virtual bool UsingDefaultWaitingUi => true;

    /// <summary>
    /// Creates the panel that shows "Boss appears in X rounds".
    /// </summary>
    /// <remarks>
    /// This must be overriden if UsingDefaultWaitingUi is set to false.
    /// </remarks>
    /// <param name="waitingHolderPanel"></param>
    /// <returns></returns>
    public virtual ModHelperPanel AddWaitPanel(ModHelperPanel waitingHolderPanel) => throw new System.NotImplementedException();

    /// <summary>
    /// Creates the panel for the boss health UI and registers the BossUI components
    /// </summary>
    /// <remarks>
    /// Here are the BossUI components you need to register:
    /// <list type="bullet">
    ///     <item>HP Text</item>
    ///     <item>HP Slider Bar</item>
    ///     <item>List of Skulls</item>
    ///     <item>The panel itself</item>
    /// </list>
    /// </remarks>
    /// <param name="holderPanel"></param>
    /// <param name="boss"></param>
    /// <param name="ui"></param>
    public virtual ModHelperPanel? AddBossPanel(ModHelperPanel holderPanel, Bloon boss, ref BossUI ui)
    {
        if (!RoundsInfo.TryGetValue(InGame.Bridge.GetCurrentRound() + 1, out BossRoundInfo infos))
        {
            return null;
        }
        var panel = holderPanel.AddPanel(new Info(Name + "-Panel"));
        panel.transform.SetAsFirstSibling();

        // HP Text
        var hpText = panel.AddText(new Info("HealthText", 0, 120, 2000, BossIntegration.FontMedium), "", BossIntegration.FontMedium, Il2CppTMPro.TextAlignmentOptions.MidlineRight);
        hpText.Text.enableAutoSizing = true;
        SetHP(boss.bloonModel.maxHealth, boss.bloonModel.maxHealth, hpText);

        ui.HpText = hpText;

        // Icon
        panel.AddImage(new Info(Name + "-Icon", -600, 0, 250), ModContent.GetSprite(mod, Icon));

        // Stars
        if (infos.tier != null)
        {
            var starsPanel = panel.AddPanel(new Info(Name + "-Stars"), VanillaSprites.BrownInsertPanel, RectTransform.Axis.Horizontal);
            starsPanel.transform.localPosition = new Vector3(-450, 140, 0);

            for (int i = 0; i < infos.tier; i++)
            {
                starsPanel.AddImage(new Info("Star" + i, 100), ModContent.GetTextureGUID<BossIntegration>("BossStar"));
            }
        }

        GameObject healthBarContainer = new GameObject("HealthBarContainer");
        healthBarContainer.transform.parent = panel.transform;
        healthBarContainer.transform.localScale = Vector3.one;
        healthBarContainer.transform.localPosition = Vector3.zero;

        // HP Bar
        GameObject fillArea = new GameObject("FillArea");
        fillArea.transform.parent = healthBarContainer.transform;
        fillArea.transform.localScale = Vector3.one;
        fillArea.transform.localPosition = new Vector3(300, 0);
        RectTransform rtFillArea = fillArea.AddComponent<RectTransform>();
        rtFillArea.anchorMax = Vector2.one;
        rtFillArea.anchorMin = Vector2.zero;

        var bossBarSlider = fillArea.AddComponent<Image>();
        bossBarSlider.type = Image.Type.Filled;
        bossBarSlider.fillMethod = Image.FillMethod.Horizontal;
        bossBarSlider.SetSprite(ModContent.GetTextureGUID<BossIntegration>("BossBarGradient"));
        bossBarSlider.rectTransform.sizeDelta = new Vector2(1500, 120);

        ui.HpBar = bossBarSlider;

        // Frame
        GameObject frame = new GameObject("Frame");
        frame.transform.parent = healthBarContainer.transform;
        frame.transform.localScale = Vector3.one;
        frame.transform.localPosition = fillArea.transform.localPosition;
        RectTransform rtFrame = frame.AddComponent<RectTransform>();
        rtFrame.anchorMax = Vector2.one;
        rtFrame.anchorMin = Vector2.zero;

        Image frameImg = frame.AddComponent<Image>();
        frameImg.type = Image.Type.Sliced;
        frameImg.SetSprite(ModContent.GetTextureGUID<BossIntegration>("BossFrame"));
        Rect rect = new Rect(0, 0, frameImg.sprite.texture.width, frameImg.sprite.texture.height);
        frameImg.sprite = Sprite.Create(frameImg.sprite.texture, rect, new Vector2(0.5f, 0.5f), 100, 1, SpriteMeshType.FullRect, new Vector4(30, 30, 30, 30));
        frameImg.pixelsPerUnitMultiplier = 0.1f;
        frameImg.rectTransform.sizeDelta = new Vector2(bossBarSlider.rectTransform.sizeDelta.x, bossBarSlider.rectTransform.sizeDelta.y + 40);

        // Skulls
        if (UsesSkulls)
        {
            float width = rtFrame.sizeDelta.x;
            ModHelperPanel skullsHolder = panel.AddPanel(new Info("SkullsHolder", rtFrame.localPosition.x, -50, width, 150));

            List<ModHelperImage> skulls = new List<ModHelperImage>();
            Il2CppStructArray<float> percentageValues = boss.bloonModel.GetBehavior<HealthPercentTriggerModel>().percentageValues;

            foreach (var item in percentageValues)
            {
                if (item > 1 || item < 0)
                {
                    ModHelper.Error<BossIntegration>($"A skull from {mod} is out of bounds. Ask {mod.Info.Author} to fix it.");
                    continue;
                }

                skulls.Add(skullsHolder.AddImage(new Info("Skull", width * item - width / 2, 0, 150), ModContent.GetTextureGUID<BossIntegration>("BossSkullPipOff")));
            }

            ui.Skulls = skulls;
        }

        ui.Panel = panel;
        return panel;
    }

    internal void SetHP(float health, float maxHealth, ModHelperText hpText)
    {
        //ModHelper.Log<BergsExtraBossPackMOD>(BossesMenu.FormatNumber((double)health) + BossesMenu.FormatNumber(maxHealth));
        hpText.SetText((bool)BossIntegration.FormatBossHP.GetValue()
            ? $"{BossesMenu.FormatNumber((double)health)} / {BossesMenu.FormatNumber(maxHealth)}"
            : $"{health} / {maxHealth}");
    }

    internal static void ResetUIs()
    {
        foreach (var item in ModBoss.BossesAlive)
        {
            if (item.Value.Boss != null)
                item.Value.Boss.OnBossesRemoved();
        }
        ModBoss.BossesAlive = new();
    }

    internal void RemoveUI(Bloon bloon)
    {
        if (!BossesAlive.ContainsKey(bloon.Id))
            return;

        BossUI ui = BossesAlive[bloon.Id];

        if (ui.Panel != null)
        {
            if (ui.Panel.enabled)
            {
                if (BossesAlive.Count - 1 > 0)
                {
                    BossesAlive.First(b => b.Key != bloon.Id).Value.Panel.SetActive(true);
                }
            }

            ui.Panel.DeleteObject();
        }

        if (ui.Icon != null)
        {
            InGameButtonsHolder.RemoveButton(ui.Icon);
            ui.Icon.DeleteObject();
        }

        BossesAlive.Remove(bloon.Id);
    }
    #endregion

    #region Base
    /// <inheritdoc />
    public sealed override string BaseBloon => BloonType.Bad;

    /// <inheritdoc/>
    public override void ModifyBaseBloonModel(BloonModel bloonModel)
    {
        bloonModel.RemoveAllChildren();
        bloonModel.danger = 16;
        bloonModel.overlayClass = BloonOverlayClass.Dreadbloon;
        bloonModel.bloonProperties = BloonProperties.None;
        bloonModel.tags = new Il2CppStringArray(new[] { "Bad", "Moabs", "Boss" });
        bloonModel.maxHealth = Health;
        bloonModel.speed = Speed;
        bloonModel.isBoss = true;

        if (!bloonModel.HasBehavior<HealthPercentTriggerModel>())
        {
            bloonModel.AddBehavior(new HealthPercentTriggerModel(Name + "-SkullEffect", false, new float[] { }, new string[] { Name + "SkullEffect" }, false));
        }

        Cache[bloonModel.name] = this;
    }


    /// <inheritdoc />
    public override void Register()
    {
        if (RoundsInfo.Count == 0)
            return;

        base.Register();
    }
    /// <inheritdoc />
    public sealed override bool KeepBaseId => false;
    /// <inheritdoc />
    public sealed override bool Regrow => false;
    /// <inheritdoc />
    public sealed override string RegrowsTo => "";
    /// <inheritdoc />
    public sealed override float RegrowRate => 3;
    #endregion
}