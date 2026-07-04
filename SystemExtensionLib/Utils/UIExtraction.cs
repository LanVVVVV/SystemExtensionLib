#pragma warning disable CS1591

using MBMScripts;
using SystemExtensionLib.Tools;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SystemExtensionLib.Utils;

public static class UIExtraction
{
    private static GameObject? _renameButton;
    private static Transform? _rootpanel;

    public static GameObject ExtractionButton(
        out Image image
        )
    {
        if (_renameButton is null)
        {
            var root = GameObject.Find("Window Female Information (Window)");
            _renameButton = root.transform.Find("Canvas/LetterBox/Frame/Window (0)/Favorite/Rename").gameObject;
        }

        _renameButton.SetActive(false);
        var button = GameObject.Instantiate(_renameButton);
        _renameButton.SetActive(true);

        button.name = "Button";

        var rt = button.GetComponent<RectTransform>();
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = new Vector2(0, 0);
        rt.sizeDelta = new Vector2(28, 28);

        ComponentTools.RemoveClickEvent(button);

        image = button.GetComponent<Image>();

        return button;
    }

    public static GameObject ExtractionPanelWindow(
        out GameObject body,
        out TextMeshProUGUI tmp,
        out ReferenceFormattingText labelRft,
        out GameObject exitIcon
        )
    {
        if (_rootpanel is null)
        {
            var rootUnit = GameObject.Find("Unit List (Window)");
            _rootpanel = rootUnit.transform.Find("Canvas/LetterBox/Frame/Window/Background");
        }

        // === Panel ===
        GameObject panel = new("Panel", typeof(RectTransform));

        var panelRT = panel.GetComponent<RectTransform>();

        panelRT.sizeDelta = new Vector2(400,300);
        panel.SetActive(false);

        // === Body ===
        var rootbody = _rootpanel.Find("Background (Translucent)").gameObject;
        body = GameObject.Instantiate(rootbody, panelRT);
        body.name = "Body";
        var bodyRT = body.GetComponent<RectTransform>();
        bodyRT.offsetMax = new Vector2(-6, -16);

        // === Border ===
        var rootBorder = _rootpanel.Find("Border").gameObject;
        var border = GameObject.Instantiate(rootBorder, panelRT);
        border.name = "Border";

        // === Label ===
        var rootLabel = _rootpanel.Find("Label").gameObject;
        var label = GameObject.Instantiate(rootLabel, panelRT);
        label.name = "Label";
        tmp = label.GetComponent<TextMeshProUGUI>();
        tmp.fontSizeMax = 24f;
        tmp.enableAutoSizing = true;
        labelRft = label.GetComponent<ReferenceFormattingText>();
        labelRft.Value = "CustomLabel";

        // === Exit Button ===
        var rootExitIcon = _rootpanel.Find("Exit Button").gameObject;
        exitIcon = GameObject.Instantiate(rootExitIcon, panelRT);
        exitIcon.name = "Exit Button";
        ComponentTools.RemoveClickEvent(exitIcon);

        return panel;
    }
}