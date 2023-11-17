using BTD_Mod_Helper;
using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Components;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BossIntegration.UI;

internal class InGameButtonsHolder
{
    const int padding = 20;

    private static ModHelperScrollPanel? mainPanel = default;
    internal static ModHelperPanel? subPanel = default;
    internal static Animator? animator;
    private const float AnimatorSpeed = 0.75f;
    private const int AnimationTicks = (int)(11 / AnimatorSpeed);

    internal static List<ModHelperButton>? icons;

    internal static void Init()
    {
        var rightPanel = InGame.instance.uiRect.transform.Find("RightPanel(Clone)").gameObject.AddModHelperPanel(new Info("MainPanel", 1200, -1200, 960, 350));
        rightPanel.transform.SetAsFirstSibling();

        mainPanel = rightPanel.AddScrollPanel(new Info("ButtonPanel", 960, 350), RectTransform.Axis.Horizontal, VanillaSprites.BrownInsertPanelDark);
        mainPanel.SetActive(false);

        animator = mainPanel.AddComponent<Animator>();
        animator.runtimeAnimatorController = Animations.PopupAnim;
        animator.speed = AnimatorSpeed;
        animator.Play("PopupSlideOut");

        subPanel = mainPanel.AddPanel(new Info("SubPanel", InfoPreset.Flex));

        ContentSizeFitter contentSizeFitter = subPanel.AddComponent<ContentSizeFitter>();
        contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        mainPanel.ScrollContent.transform.position += new Vector3(padding, padding / 2);

        mainPanel.AddScrollContent(subPanel);

        GridLayoutGroup gridGroup = subPanel.AddComponent<GridLayoutGroup>();
        gridGroup.SetPadding(padding);
        gridGroup.constraint = GridLayoutGroup.Constraint.FixedRowCount;
        gridGroup.constraintCount = 2;

        subPanel.transform.localPosition = new Vector3(40, -150);

        icons = new List<ModHelperButton>();
    }

    internal static void AddButton(ModHelperButton btn)
    {
        if (icons == null || animator == null || mainPanel == null)
            return;

        if (icons.Contains(btn))
            return;

        icons.Add(btn);

        if (icons.Count == 1)
        {
            mainPanel.SetActive(true);
            animator.Play("PopupSlideIn");
        }
    }

    internal static void RemoveButton(ModHelperButton btn)
    {
        if (icons == null || animator == null || mainPanel == null)
            return;

        if (!icons.Contains(btn))
            return;

        icons.Remove(btn);

        if (icons.Count == 0)
        {
            animator.Play("PopupSlideOut");
            TaskScheduler.ScheduleTask(() => mainPanel.SetActive(false), ScheduleType.WaitForFrames, AnimationTicks);
        }
    }

    internal static void ResetIcons()
    {
        if (icons == null)
            return;

        foreach (var item in icons)
        {
            if (item is null)
                continue;

            item.DeleteObject();
        }
        icons.Clear();
    }
}