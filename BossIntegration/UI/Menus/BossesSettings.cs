using BTD_Mod_Helper;
using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Components;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Unity.UI_New.Settings;
using Il2CppTMPro;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace BossIntegration.UI.Menus.Bosses;

internal class BossesSettings : ModGameMenu<HotkeysScreen>
{
    static ModHelperPanel? panel;
    static Dictionary<ModHelperButton, bool>? buttons;

    const float size = 150;
    const float spacing = 25;
    const int maxBoss = 8;
    const int maxRounds = 12;

    const float tableWidth = maxRounds * (size + spacing); // (rounds.Length > maxRounds - 1 ? maxRounds : rounds.Length + 1) * (size + spacing);
    const float tableHeight = maxBoss * (size + spacing); // (ModBoss.Cache.Count > maxBoss - 1 ? maxBoss : ModBoss.Cache.Count + 1) * (size + spacing);

    public static string Path => ModHelper.ModHelperDirectory + "\\Mod Settings\\BossesSetting.json";

    static GameObject? canvas;

    public override bool OnMenuOpened(Il2CppSystem.Object data)
    {
        canvas = GameMenu.gameObject;
        canvas.DestroyAllChildren();
        GameMenu.saved = true;

        CommonForegroundHeader.SetText("SETTINGS");

        Init(canvas);

        return true;
    }

    public override void OnMenuClosed()
    {
        Dictionary<string, Dictionary<int, bool>> formattedPermissions = new();

        foreach (var permissionsRounds in ModBoss.Permissions)
        {
            foreach (var perm in permissionsRounds.Value)
            {
                if (perm.Value)
                    continue;

                if (!formattedPermissions.ContainsKey(permissionsRounds.Key))
                    formattedPermissions.Add(permissionsRounds.Key, new());
                formattedPermissions[permissionsRounds.Key].Add(perm.Key, perm.Value);
            }
        }

        File.WriteAllText(Path, JsonConvert.SerializeObject(formattedPermissions));
       
    }

    private static void Init(GameObject canvas)
    {
        int[] rounds = CompileAllRounds();

        canvas.AddModHelperPanel(new Info("TableBG", 0, (maxBoss * (size + spacing) - tableHeight) / 2, tableWidth, tableHeight), VanillaSprites.MainBGPanelBlueNotchesShadow);
        var table = canvas.AddModHelperScrollPanel(new Info("Table", 0, (maxBoss * (size + spacing) - tableHeight) / 2, tableWidth - spacing / 2, tableHeight - spacing / 2), RectTransform.Axis.Horizontal, VanillaSprites.PanelEmpty);
        table.ScrollRect.vertical = true;//ModBoss.Cache.Count > maxBoss - 1;
        table.ScrollRect.horizontal = true;//rounds.Length > maxRounds - 1;
        table.ScrollContent.gameObject.RemoveComponent<HorizontalLayoutGroup>();
        table.ScrollContent.RemoveComponent<ContentSizeFitter>();
        table.Mask.showMaskGraphic = false;

        buttons = new Dictionary<ModHelperButton, bool>();

        panel = table.AddPanel(new Info("MainPanel", rounds.Length * size + (rounds.Length - 1) * spacing, ModBoss.Cache.Count * (size + 2 * spacing)), null);
        table.AddScrollContent(panel);
        //panel.RectTransform.localPosition = new Vector2(0, -panel.RectTransform.sizeDelta.y + ModBoss.Cache.Count * 20);

        table.ScrollContent.RectTransform.sizeDelta = new Vector2(panel.RectTransform.sizeDelta.x, panel.RectTransform.sizeDelta.y / 2);

        var roundScrollPanel = RoundPanelInit(table, rounds);
        var bossScrollPanel = BossPanelInit(table, rounds);
        ExtraSettingsPanel(canvas, table);

        table.ScrollRect.onValueChanged.AddListener(new Action<Vector2>((r) =>
        {
            roundScrollPanel.ScrollRect.content.position = FollowOtherScroll(roundScrollPanel, table, true, false);
            bossScrollPanel.ScrollRect.content.position = FollowOtherScroll(bossScrollPanel, table, false, true);
        }));
    }

    private static int[] CompileAllRounds()
    {
        List<int> rounds = new List<int>();

        foreach (var b in ModBoss.Cache)
        {
            foreach (var r in b.Value.RoundsInfo)
            {
                if (!rounds.Contains(r.Key))
                    rounds.Add(r.Key);
            }
        }
        rounds.Sort();
        return rounds.ToArray();
    }

    private static string GetSprite(bool state) => state ? VanillaSprites.AddMoreBtn : VanillaSprites.AddRemoveBtn;

    private static bool UpdateBossRound(ModBoss boss, int round, bool state)
    {
        if (boss == null)
            return false;

        bool result = state;
        string? bossName = boss.ToString();

        if (string.IsNullOrEmpty(bossName))
            return false;

        if (!ModBoss.Permissions.ContainsKey(bossName))
        {
            ModBoss.Permissions.Add(bossName, new());
        }

        if (ModBoss.Permissions.TryGetValue(bossName, out var value))
        {
            if (!value.ContainsKey(round))
                value.Add(round, false);
            value[round] = state;
        }

        return result;
    }

    private static ModHelperScrollPanel RoundPanelInit(ModHelperScrollPanel table, int[] rounds)
    {
        // Rounds
        var roundScrollPanel = canvas.AddModHelperScrollPanel(new Info("RoundsScrollPanel", table.RectTransform.localPosition.x, maxBoss * (size + spacing) / 2 + size, table.RectTransform.sizeDelta.x, 250), RectTransform.Axis.Horizontal, VanillaSprites.MainBGPanelGrey);
        var roundPanel = roundScrollPanel.AddPanel(new Info("RoundPanel", rounds.Length * size + (rounds.Length - 1) * spacing, size), null);
        roundScrollPanel.AddScrollContent(roundPanel);
        roundScrollPanel.ScrollRect.enabled = false;
        //roundScrollPanel.ScrollContent.RemoveComponent<HorizontalLayoutGroup>();

        if (rounds.Length < maxBoss)
        {
            roundPanel.RectTransform.localPosition = new Vector3(1100 - rounds.Length * 100, -115);
        }

        for (int i = 0; i < rounds.Length; i++)
        {
            ModHelperText t = roundPanel.AddText(new Info("RoundLabel" + rounds[i], GetXPosition(i, rounds.Length), 10, 150), rounds[i].ToString(), 69, TextAlignmentOptions.Left);
            t.Text.m_maxFontSize = 69;
            t.Text.enableAutoSizing = true;
            t.RectTransform.rotation = Quaternion.Euler(0, 0, 60);
        }
        return roundScrollPanel;
    }
    private static ModHelperScrollPanel BossPanelInit(ModHelperScrollPanel table, int[] rounds)
    {
        var bossScrollPanel = canvas.AddModHelperScrollPanel(new Info("BossesScrollPanel",
            table.RectTransform.localPosition.x - (maxBoss - 1) * (size + spacing) - spacing,
            table.RectTransform.localPosition.y,
            350, tableHeight - spacing / 2), RectTransform.Axis.Horizontal, VanillaSprites.MainBGPanelGrey);

        bossScrollPanel.ScrollContent.gameObject.RemoveComponent<VerticalLayoutGroup>();
        bossScrollPanel.ScrollRect.enabled = false;
        var bossPanel = bossScrollPanel.AddPanel(new Info("BossPanel", 150, ModBoss.Cache.Count * (size + 2 * spacing)), null);
        bossScrollPanel.ScrollContent.RemoveComponent<ContentSizeFitter>();

        bossScrollPanel.AddScrollContent(bossPanel);
        bossScrollPanel.ScrollContent.RectTransform.sizeDelta = new Vector2(bossPanel.RectTransform.sizeDelta.x, bossPanel.RectTransform.sizeDelta.y / 2);

        List<ModBoss> bosses = ModBoss.Cache.Values.ToList();

        for (int i = 0; i < bosses.Count; i++)
        {
            ModBoss boss = bosses[i];
            float yPosition = (bosses.Count / 2 - i) * (size + spacing * 2) - size / 2;

            ModHelperButton b = bossPanel.AddButton(new Info("BossLabel" + boss.Name, 0, yPosition, 150), GetTextureGUID(boss.mod, boss.Icon), new Action(() =>
            {
                bool? result = null;

                if (buttons != null)
                {
                    foreach (var item in buttons)
                    {
                        if (!item.Key.name.Contains($"{boss.Name}-Btn"))
                            continue;

                        if (result == null)
                            result = !item.Value;

                        item.Key.Button.image.SetSprite(GetSprite((bool)result));
                        UpdateBossRound(boss, int.Parse(item.Key.name.Replace($"{boss.Name}-Btn", "")), (bool)result);
                        buttons[item.Key] = (bool)result;
                    }
                }
            }));

            for (int x = 0; x < rounds.Length; x++)
            {
                var round = rounds[x];

                if (boss.RoundsInfo.Any(c => c.Key == round))
                {
                    bool isAllowed = ModBoss.GetPermission(boss, round);

                    if (panel != null && buttons != null)
                    {
                        var d = panel.AddButton(new Info($"{boss.Name}-Btn{round}", GetXPosition(x, rounds.Length), yPosition, size),
                        GetSprite(isAllowed), null);

                        buttons.Add(d, isAllowed);

                        d.Button.AddOnClick(new Function(() =>
                        {
                            bool result = UpdateBossRound(boss, round, !buttons[d]);
                            buttons[d] = result;

                            d.Image.SetSprite(GetSprite(result));
                        }));
                    }
                }
            }

            /*ModHelperText t = bossPanel.AddText(new Info("BossLabel" + bosses[i].Name,
                -spacing,
                (bosses.Count / 2 - i) * (size + spacing) + 25,
                bossScrollPanel.RectTransform.sizeDelta.x, 69),
                bosses[i].DisplayName,
                69,
                TextAlignmentOptions.Right);
            t.Text.overflowMode = TextOverflowModes.Overflow;
            t.transform.SetAsFirstSibling();
            t.Text.m_maxFontSize = 69;
            t.Text.enableAutoSizing = true;*/
        }
        return bossScrollPanel;
    }

    private static float GetXPosition(int index, int length) => (-length / 2 + index) * (spacing + size) + (1 - length % 2) * size / 2;
    private static void ExtraSettingsPanel(GameObject canvas, ModHelperScrollPanel table)
    {
        var extraSettingsPanel = canvas.AddModHelperScrollPanel(new Info("ExtraSettingsPanel", -size - spacing, -875, table.RectTransform.sizeDelta.x + 350 + 2 * spacing, 300),
            RectTransform.Axis.Horizontal, VanillaSprites.BlueInsertPanelRound, 50);

        var btn = extraSettingsPanel.AddButton(new Info("EnableAll", 500, 250), VanillaSprites.GreenBtnLong, new Action(() => { SetAllButtons(true); }));
        var txt = btn.AddText(new Info("Text", InfoPreset.FillParent), "Enable All", 69);
        txt.Text.enableAutoSizing = true;
        txt.Text.m_maxFontSize = 69;
        extraSettingsPanel.AddScrollContent(btn);

        btn = extraSettingsPanel.AddButton(new Info("DisableAll", 500, 250), VanillaSprites.RedBtnLong, new Action(() => { SetAllButtons(false); }));
        txt = btn.AddText(new Info("Text", InfoPreset.FillParent), "Disable All", 69);
        txt.Text.enableAutoSizing = true;
        txt.Text.m_maxFontSize = 69;
        extraSettingsPanel.AddScrollContent(btn);
    }

    private static void SetAllButtons(bool state)
    {
        if (buttons != null)
        {
            foreach (var item in buttons)
            {
                if (item.Value == state)
                    continue;

                item.Key.Image.SetSprite(GetSprite(state));
                item.Key.Button.onClick.Invoke();
            }
        }
    }
    private static Vector3 FollowOtherScroll(ModHelperScrollPanel target, ModHelperScrollPanel source, bool horizontal = false, bool vertical = false)
    {
        return new Vector3(
                horizontal ? target.RectTransform.position.x + source.ScrollRect.content.position.x - source.ScrollRect.rectTransform.position.x : target.ScrollRect.content.position.x,
                vertical ? target.RectTransform.position.y + source.ScrollRect.content.position.y - source.ScrollRect.rectTransform.position.y : target.ScrollRect.content.position.y,
                0);
    }
}
