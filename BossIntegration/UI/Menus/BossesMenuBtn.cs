using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Components;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Unity.UI_New;
using Il2CppAssets.Scripts.Utils;
using System;
using System.Linq;
using UnityEngine;

namespace BossIntegration.UI;

internal class BossesMenuBtn
{
    private static SpriteReference Sprite => ModContent.GetSpriteReference<BossIntegration>("Icon");

    private static readonly string[] ShowOnMenus =
    {
        "MapSelectUI"
    };

    private const float AnimatorSpeed = .75f;
    private const int AnimationTicks = (int)(10 / AnimatorSpeed);

    private static ModHelperPanel? buttonPanel;
    private static ModHelperButton? bossesBtn;

    internal static void OnMenuChanged(string currentMenu, string newMenu)
    {
        if (ModBoss.Cache.Count == 0) return;

        if (ShowOnMenus.Contains(newMenu))
        {
            if (!ShowOnMenus.Contains(currentMenu))
            {
                Show();
            }
        }

        if (!ShowOnMenus.Contains(newMenu))
        {
            Hide();
        }
    }

    private static void Init()
    {
        var foregroundScreen = CommonForegroundScreen.instance.transform;
        //var backgroundScreen = CommonBackgroundScreen.instance.transform;
        var roundSetChanger = foregroundScreen.FindChild("BottomButtonPanel");
        if (roundSetChanger == null)
        {
            CreatePanel(foregroundScreen.gameObject);
        }
        //button.SetActive(true/*ModBoss.Cache.Count > 0*/);
    }

    private static void CreatePanel(GameObject screen)
    {
        buttonPanel = screen.AddModHelperPanel(new Info("BottomButtonPanel")
        {
            Anchor = new Vector2(1, 0),
            Pivot = new Vector2(1, 0)
        });

        var animator = buttonPanel.AddComponent<Animator>();
        animator.runtimeAnimatorController = Animations.PopupAnim;
        animator.speed = AnimatorSpeed;
        buttonPanel.GetComponent<Animator>().Play("PopupSlideIn");

        Create(buttonPanel);
    }

    private static void Show()
    {
        Init();
        RevealButton();
    }
    private static void Hide()
    {
        HideButton();
    }

    private static void RevealButton()
    {
        if (buttonPanel != null)
        {
            buttonPanel.SetActive(true);
            buttonPanel.GetComponent<Animator>().Play("PopupSlideIn");
        }
    }

    private static void HideButton()
    {
        if (bossesBtn is null)
            return;

        if (buttonPanel != null)
        {
            buttonPanel.GetComponent<Animator>().Play("PopupSlideOut");
            TaskScheduler.ScheduleTask(() => buttonPanel.SetActive(false), ScheduleType.WaitForFrames, AnimationTicks);
        }
    }

    public static void Create(ModHelperPanel panel)
    {
        bossesBtn = panel.AddButton(new Info("BossMenuBtn", -750, 50, 350, 350, new Vector2(1, 0), new Vector2(0.5f, 0)), Sprite.GUID,
            new Action(() => ModGameMenu.Open<BossesMenu>()));

        bossesBtn.AddText(new Info("Text", 0, -175, 500, 100), $"   Boss{(ModBoss.Cache.Count > 1 ? "es" : "")} ({ModBoss.Cache.Count})", 60f);
    }
}
