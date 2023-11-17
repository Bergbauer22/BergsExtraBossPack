using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Components;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using BossIntegration.UI.Menus.Bosses;
using Il2Cpp;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Unity.Menu;
using Il2CppAssets.Scripts.Unity.UI_New.ChallengeEditor;
using Il2CppAssets.Scripts.Unity.UI_New.Popups;
using Il2CppNinjaKiwi.Common;
using Il2CppSystem.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static BossIntegration.BossIntegration;

namespace BossIntegration.UI;

internal class BossesMenu : ModGameMenu<ExtraSettingsScreen>
{
    static ModHelperText? authorsLabel;
    static ModHelperText? displayNameLabel;
    static ModHelperText? systemNameLabel;
    static ModHelperText? extraCreditLabel;

    static ModHelperText? descriptionLabel;
    static ModHelperScrollPanel? descriptionPanel;

    static ModHelperDropdown? roundDropdown;
    static ModHelperPanel? roundPanel;

    static ModHelperText? speedLabel;
    static ModHelperText? healthLabel;

    static ModHelperText? skullLabel;
    static ModHelperText? timerLabel;

    static ModHelperScrollPanel? modifPanel;

    static ModBoss? Boss;

    static bool firstBoss = true;
    const string InfoIcon = VanillaSprites.InfoBtn2;
    const int modifiersIconHeight = 150;

    /// <inheritdoc/>
    public override bool OnMenuOpened(Il2CppSystem.Object data)
    {
        CommonForegroundHeader.SetText("Bosses");

        var panelTransform = GameMenu.gameObject.GetComponentInChildrenByName<RectTransform>("Panel");
        var panel = panelTransform.gameObject;
        panel.DestroyAllChildren();

        var bossMenu = panel.AddModHelperPanel(new Info("ModsMenu", 3600, 1900), null);
        CreateLeftMenu(bossMenu);
        CreateRightMenu(bossMenu);

        return false;
    }

    /// <inheritdoc/>
    public override void OnMenuClosed()
    {
        base.OnMenuClosed();
        firstBoss = true;
    }

    static ModHelperPanel? bossPanel;

    private static void CreateLeftMenu(ModHelperPanel bossMenu)
    {
        var Padding = 50;

        var leftMenu = bossMenu.AddPanel(
            new Info("LeftMenu", (MenuWidth - 1750) / -2f, 0, 1750, MenuHeight),
            VanillaSprites.MainBGPanelBlue, RectTransform.Axis.Vertical, Padding, Padding
        );

        var bossList = leftMenu.AddScrollPanel(new Info("BossListScroll", InfoPreset.Flex), RectTransform.Axis.Vertical,
        VanillaSprites.BlueInsertPanelRound, Padding, Padding);

        foreach (var item in ModBoss.Cache.Values)
        {
            bossList.AddScrollContent(CreateItem(item));
        }

        var settingsPanel = leftMenu.AddScrollPanel(new Info("SettingsScroll", leftMenu.RectTransform.sizeDelta.x, 150), RectTransform.Axis.Horizontal, null, Padding, Padding);
        settingsPanel.AddButton(new Info("BossSetupBtn", 150), VanillaSprites.SettingsBtn, new Action(() =>
        {
            ModGameMenu.Open<BossesSettings>();
        }));

    }

    private static void CreateRightMenu(ModHelperPanel bossMenu)
    {
        var rightPanel = bossMenu.AddPanel(
            new Info("RightMenu", (MenuWidth - 1750) / 2f, 0, 1750, MenuHeight),
            VanillaSprites.MainBGPanelBlue, RectTransform.Axis.Vertical, Padding, Padding
        );

        rightPanel.SetActive(false);

        var topPanel = rightPanel.AddPanel(new Info("TopPanel")
        {
            Height = ModNameHeight * 2,
            FlexWidth = 1
        }, null, RectTransform.Axis.Horizontal);

        topPanel.AddImage(new Info("Icon")
        {
            Size = ModNameHeight * 2
        }, new Sprite());

        var namesPanel = topPanel.AddPanel(new Info("NamesPanel", InfoPreset.Flex)
        {
            Y = -ModNameHeight * 2,
        });

        displayNameLabel = namesPanel.AddText(new Info("DisplayName", ModNameWidth, ModNameHeight), "DisplayName", FontLarge * 1.5f);
        displayNameLabel.Text.fontSizeMax = FontLarge * 1.5f;
        displayNameLabel.Text.enableAutoSizing = true;

        systemNameLabel = namesPanel.AddText(new Info("SystemName", 0, -FontLarge * 1.5f, ModNameWidth, ModNameHeight), "SystemName", FontSmall);
        systemNameLabel.Text.fontSizeMax = FontSmall;
        systemNameLabel.Text.enableAutoSizing = true;

        //isCamoIcon = iconsPanel.AddImage(new Info("CamoIcon", ModNameHeight) { X = -Padding * 1.5f }, VanillaSprites.CamoBloonsIcon);
        //isFortifiedIcon = iconsPanel.AddImage(new Info("FortifiedIcon", ModNameHeight) { X = ModNameHeight - Padding * 1.5f }, VanillaSprites.FortifiedBloonsIcon);

        // Authors
        authorsLabel = rightPanel.AddText(new Info("Authors", ModNameWidth, ModNameHeight / 3), "Authors: ", FontSmall, Il2CppTMPro.TextAlignmentOptions.Left);
        authorsLabel.Text.enableAutoSizing = true;

        extraCreditLabel = rightPanel.AddText(new Info("ExtraCredits", ModNameWidth, ModNameHeight / 3), "Extra Credits: ", FontSmall, Il2CppTMPro.TextAlignmentOptions.Left);
        extraCreditLabel.Text.enableAutoSizing = true;

        descriptionPanel = rightPanel.AddScrollPanel(new Info("DescriptionPanel", InfoPreset.FillParent)
        {
            FlexWidth = 1,
            Height = FontSmall * 4 + Padding * 2,
        }, RectTransform.Axis.Vertical, VanillaSprites.BlueInsertPanelRound, Padding, Padding);

        descriptionLabel = descriptionPanel.AddText(new Info("Description", ModNameWidth, ModNameHeight)
        {
            Flex = 1
        }, "", FontSmall, Il2CppTMPro.TextAlignmentOptions.MidlineLeft);
        descriptionLabel.Text.enableAutoSizing = true;

        descriptionPanel.AddScrollContent(descriptionLabel);

        //roundPanel = rightPanel.AddPanel(new Info("RoundsPanel", rightPanel.RectTransform.sizeDelta.x - Padding * 2, 150));
        //roundPanel.AddText(new Info("RoundsLabel", 0, 0, 500, roundPanel.RectTransform.sizeDelta.y), "Rounds:", FontMedium, Il2CppTMPro.TextAlignmentOptions.Right);

        var mainPanel = rightPanel.AddScrollPanel(new Info("MainPanel", InfoPreset.Flex), RectTransform.Axis.Vertical, VanillaSprites.BlueInsertPanelRound, Padding, Padding);
        var mainPanelWidth = rightPanel.RectTransform.sizeDelta.x - 4 * Padding;
        var mainPanelVLG = mainPanel.ScrollContent.GetComponent<VerticalLayoutGroup>();
        mainPanelVLG.childAlignment = TextAnchor.MiddleLeft;
        mainPanelVLG.childControlHeight = false;

        var roundModifPanel = rightPanel.AddPanel(new Info("RoundModifPanel", rightPanel.RectTransform.sizeDelta.x - Padding * 2, modifiersIconHeight));

        roundPanel = roundModifPanel.AddPanel(new Info("RoundsPanel", 0, 150)
        {
            FlexWidth = 1,
        });
        roundPanel.AddText(new Info("RoundsLabel", 0, 0, 500, roundPanel.RectTransform.sizeDelta.y), "Rounds:", FontMedium, Il2CppTMPro.TextAlignmentOptions.Right);

        //modifPanel = rightPanel.AddScrollPanel(new Info("ModifPanel", rightPanel.RectTransform.sizeDelta.x - Padding * 2, 150), RectTransform.Axis.Horizontal, VanillaSprites.BlueInsertPanelRound, Padding, Padding);
        modifPanel = roundModifPanel.AddScrollPanel(new Info("ModifPanel", 0, 0, 750, modifiersIconHeight), RectTransform.Axis.Horizontal, VanillaSprites.BlueInsertPanelRound, Padding, Padding);
        modifPanel.RectTransform.localPosition = new Vector3(-roundModifPanel.RectTransform.sizeDelta.x / 2 + modifPanel.RectTransform.sizeDelta.x / 2, 0);

        // Stats
        // Speed
        var speedPanel = mainPanel.AddPanel(new Info("SpeedPanel", mainPanelWidth, ModNameHeight));
        mainPanel.AddScrollContent(speedPanel);
        speedPanel.AddComponent<HorizontalLayoutGroup>();

        speedLabel = speedPanel.AddText(new Info("SpeedLabel", speedPanel.RectTransform.sizeDelta.x - ModNameHeight, ModNameHeight), "Speed:", FontMedium, Il2CppTMPro.TextAlignmentOptions.Left);

        if (!(bool)BossIntegration.ShowBossSpeedAsPercentage.GetValue())
        {
            speedPanel.AddButton(new Info("SpeedInfo", ModNameHeight, ModNameHeight), InfoIcon,
            new Action(() =>
            {
                PopupScreen.instance.SafelyQueue(screen => screen.ShowOkPopup("For reference, a BAD has a speed of 4.5 while a red bloon has a speed of 25."));
            }));
        }

        // Health
        var healthPanel = mainPanel.AddPanel(new Info("HealthPanel", mainPanelWidth, ModNameHeight));
        mainPanel.AddScrollContent(healthPanel);
        healthPanel.AddComponent<HorizontalLayoutGroup>();

        healthLabel = healthPanel.AddText(new Info("HealthLabel", healthPanel.RectTransform.sizeDelta.x - ModNameHeight, ModNameHeight)
        {
            Flex = 1
        }, "Health:", FontMedium, Il2CppTMPro.TextAlignmentOptions.Left);
        healthPanel.AddButton(new Info("HealthInfo", ModNameHeight, ModNameHeight), InfoIcon,
            new Action(() =>
            {
                PopupScreen.instance.SafelyQueue(screen => screen.ShowOkPopup("For reference, a BAD has 20k health while a DDT has 400 health."));
            }));

        // Skull
        skullLabel = mainPanel.AddText(new Info("SkullLabel", ModNameWidth, ModNameHeight)
        {
            Flex = 1
        }, "Skull (#):", FontMedium, Il2CppTMPro.TextAlignmentOptions.Left);
        mainPanel.AddScrollContent(skullLabel);
        skullLabel.Text.overflowMode = Il2CppTMPro.TextOverflowModes.Overflow;

        // Timer
        timerLabel = mainPanel.AddText(new Info("TimerLabel", ModNameWidth, ModNameHeight)
        {
            Flex = 1
        }, "Timer (Xs):", FontMedium, Il2CppTMPro.TextAlignmentOptions.Left);
        timerLabel.Text.overflowMode = Il2CppTMPro.TextOverflowModes.Overflow;
        mainPanel.AddScrollContent(timerLabel);

        bossPanel = rightPanel;
    }

    private static ModHelperComponent CreateItem(ModBoss boss)
    {
        var mod = ModHelperPanel.Create(new Info(boss.Name)
        {
            Height = 200,
            FlexWidth = 1
        });

        var panel = mod.AddButton(new Info("MainBtn", InfoPreset.FillParent), VanillaSprites.MainBGPanelBlueNotches,
            new Action(() =>
            {
                Boss = boss;
                LoadBossInfo(boss);
                MenuManager.instance.buttonClick2Sound.Play("ClickSounds");
            }));

        panel.AddImage(new Info("Icon", Padding * 2, 0, ModIconSize, new Vector2(0, 0.5f)), GetTextureGUID(boss.mod, boss.Icon));

        panel.AddText(new Info("Name", ModNameWidth, ModNameHeight), boss.DisplayName,
            FontMedium);

        return mod;
    }

    private static void LoadBossInfo(ModBoss boss)
    {
        if (bossPanel == null)
            return;

        var sprite = GetSprite(boss.mod, boss.Icon);
        var icon = bossPanel.transform.FindChildWithName("Icon");

        if (sprite != null)
        {
            icon.GetComponent<Image>().sprite = sprite;
        }

        if (displayNameLabel != null)
            displayNameLabel.SetText(boss.DisplayName);

        if (systemNameLabel != null)
        {
            systemNameLabel.SetText(boss.ToString());
            systemNameLabel.SetActive(false);
        }

        // Icons
        //iconsPanel.Background.enabled = boss.bloonModel.isCamo || boss.bloonModel.isFortified;
        //isCamoIcon.SetActive(boss.bloonModel.isCamo);
        //isFortifiedIcon.SetActive(boss.bloonModel.isFortified);

        // Authors
        if (authorsLabel != null)
        {
            authorsLabel.SetText($"Author(s): {boss.mod.Info.Author}");
            //authorsLabel.Text.SetFaceColor(BlatantFavoritism.GetColor(boss.mod.GetModHelperData().RepoOwner));
        }

        if (extraCreditLabel != null)
        {
            extraCreditLabel.SetActive(!string.IsNullOrEmpty(boss.ExtraCredits));
            extraCreditLabel.SetText("Extra credits: " + boss.ExtraCredits);
        }

        if (descriptionPanel != null)
            descriptionPanel.SetActive($"Default description for {boss.Name}" != boss.Description);


        if (descriptionLabel != null)
            descriptionLabel.SetText(boss.Description);


        var rounds = new List<string>();
        foreach (var item in boss.SpawnRounds)
        {
            rounds.Add(item.ToString());
        }

        if (roundDropdown != null)
            roundDropdown.DeleteObject();


        if (roundPanel != null)
        {
            roundDropdown = roundPanel.AddDropdown(new Info("RoundDropdown", 575, 0, 500f, ModNameHeight), rounds,
                ModNameHeight * 4,
                new Action<int>((r) =>
                {
                    if (roundDropdown != null)
                        UpdateRoundInfos(int.Parse(roundDropdown.Dropdown.options[r].text.ToString()));
                }), VanillaSprites.BlueInsertPanelRound, FontSmall
            );

            if (rounds.Count > 0)
                UpdateRoundInfos(int.Parse(rounds[0]));
        }

        bossPanel.SetActive(true);
    }

    private static void UpdateRoundInfos(int round)
    {
        if (Boss == null)
            return;

        var copy = Boss.ModifyForRound(Game.instance.model.GetBloon(Boss.Id).Duplicate(), round);

        if (speedLabel != null)
            speedLabel.SetText($"Speed: {((bool)BossIntegration.ShowBossSpeedAsPercentage.GetValue() ? (copy.speed / 4.5f * 100).ToString("F2") + "% of a BAD" : copy.speed)}");

        if (healthLabel != null)
            healthLabel.SetText("Health: " + FormatNumber(copy.maxHealth));

        if (Boss.RoundsInfo.TryGetValue(round, out var roundInfo))
        {
            if (skullLabel != null)
            {
                skullLabel.SetActive(Boss.UsesSkulls);
                if (roundInfo.skullCount != null)
                {
                    skullLabel.SetText($"Skull{(roundInfo.skullCount > 1 ? "s" : "")} ({roundInfo.skullCount}): {(roundInfo.skullDescription == null ? Boss.SkullDescription : roundInfo.skullDescription)}");
                    SizeOverflowText(skullLabel);
                }
            }

            if (timerLabel != null)
            {
                timerLabel.SetActive(Boss.UsesTimer);

                if (roundInfo.interval == null)
                    roundInfo.interval = Boss.Interval;

                if (roundInfo.interval != null)
                {
                    timerLabel.SetText($"Timer ({roundInfo.interval}s): {(roundInfo.timerDescription == null ? Boss.TimerDescription : roundInfo.timerDescription)}");
                    SizeOverflowText(timerLabel);
                }
            }
        }

        if (modifPanel != null)
        {
            modifPanel.ScrollContent.transform.DestroyAllChildren();

            if (Boss.SpawnRounds.First() != round && roundInfo.defeatIfPreviousNotDefeated == true)
            {
                modifPanel.AddScrollContent(modifPanel.AddButton(new Info("DefeatBeforeSpawnBtn", modifiersIconHeight), VanillaSprites.GoFastForwardIcon, new Action(() => { ShowPropertyPopup("Previous tier must be killed before this one appears."); })));
            }

            if (copy.isCamo)
            {
                modifPanel.AddScrollContent(modifPanel.AddButton(new Info("CamoBtn", modifiersIconHeight), VanillaSprites.CamoBloonIcon, new Action(() => { ShowPropertyPopup("This boss has Camo properties."); })));
            }

            if (copy.isFortified)
            {
                modifPanel.AddScrollContent(modifPanel.AddButton(new Info("FortifiedBtn", modifiersIconHeight), VanillaSprites.FortifiedBloonIcon, new Action(() => { ShowPropertyPopup("This boss is fortified."); })));
            }

            foreach (BloonProperties property in Enum.GetValues(typeof(BloonProperties)))
            {
                if (property == BloonProperties.None)
                    continue;

                if (copy.bloonProperties.HasFlag(property))
                {
                    switch (property)
                    {
                        case BloonProperties.Lead:
                            modifPanel.AddScrollContent(modifPanel.AddButton(new Info("CamoBtn", modifiersIconHeight), VanillaSprites.LeadBloonIcon, new Action(() => { ShowPropertyPopup("This boss has Lead properties."); })));
                            break;
                        case BloonProperties.Black:
                            modifPanel.AddScrollContent(modifPanel.AddButton(new Info("BlackBtn", modifiersIconHeight), VanillaSprites.BlackBloonIcon, new Action(() => { ShowPropertyPopup("This boss has Black properties."); })));
                            break;
                        case BloonProperties.White:
                            modifPanel.AddScrollContent(modifPanel.AddButton(new Info("WhiteBtn", modifiersIconHeight), VanillaSprites.WhiteBloonIcon, new Action(() => { ShowPropertyPopup("This boss has White properties."); })));
                            break;
                        case BloonProperties.Purple:
                            modifPanel.AddScrollContent(modifPanel.AddButton(new Info("PurpleBtn", modifiersIconHeight), VanillaSprites.PurpleBloonIcon, new Action(() => { ShowPropertyPopup("This boss has Purple properties."); })));
                            break;
                        case BloonProperties.Frozen:
                            modifPanel.AddScrollContent(modifPanel.AddButton(new Info("FrozenBtn", modifiersIconHeight), VanillaSprites.AbsoluteZeroUpgradeIcon, new Action(() => { ShowPropertyPopup("This boss has Frozen properties."); })));
                            break;
                        case BloonProperties.None:
                        default:
                            modifPanel.AddScrollContent(modifPanel.AddButton(new Info("HowBtn", modifiersIconHeight), VanillaSprites.HomeIcon, new Action(() => { ShowPropertyPopup("How ?! You're not supposed to see that !\nContact WarperSan about this."); })));
                            break;
                    }
                }
            }

            modifPanel.SetActive(modifPanel.ScrollContent.transform.childCount != 0);
        }

        firstBoss = false;
    }

    private static void ShowPropertyPopup(string message)
    {
        PopupScreen.instance.SafelyQueue(screen => screen.ShowOkPopup(message));
    }

    private static void SizeOverflowText(ModHelperText label)
    {
        int lineCount = label.Text.GetTextInfo(label.Text.text).lineCount;

        var timerLineCount = lineCount + (firstBoss ? -1 : 0);
        label.RectTransform.sizeDelta = new Vector2(label.RectTransform.sizeDelta.x,
            lineCount == 1 ? label.Text.fontSize : label.Text.fontSize * timerLineCount + label.Text.lineSpacing * (timerLineCount - 1));
    }


    private static readonly string[] NumSuffixs = new string[] { "K", "M", "B", "T", "q", "Q", "s", "S", "O", "N", "d", "U", "D", "!", "@", "#", "$", "%", "^", "&", "*", "[", "]", "{", "}", ";" };
    internal static string FormatNumber(double number)
    {
        var result = number.ToString().Split(',')[0];

        if (result.Length < 4)
            return result;

        var index = 0;
        while (result.Length - 3 * (index + 1) > 3) { index++; }

        var commaPos = result.Length - 3 * (index + 1);
        var rest = result.Substring(commaPos, 4 - commaPos);

        while (rest.Substring(rest.Length - 1, 1) == "0")
        {
            if (rest.Length == 1)
            {
                rest = "";
                break;
            }

            rest = rest.Substring(0, rest.Length - 1);
        }

        return result.Substring(0, commaPos) + (rest.Length == 0 ? "" : ",") + rest + (index >= NumSuffixs.Length ? "?" : NumSuffixs[index]);
    }
}