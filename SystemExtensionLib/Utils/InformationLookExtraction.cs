#pragma warning disable CS1591

using MBMScripts;
using SystemExtensionLib.Sprites;
using SystemExtensionLib.Tools;
using UnityEngine;
using UnityEngine.UI;

namespace SystemExtensionLib.Utils;

public static class InformationLookExtraction
{
    private const float FixedWidth = 178f;

    private static GameObject? _root;
    private static GameObject? _canvas;
    private static Transform? _contentLeft;
    private static Transform? _contentRight;
    private static Transform? _informationLook;

    public static GameObject EmptyDisplayColorSlot(
        out (ReferenceString labelRs, GameObject? typeValue) display)
    {
        Init();

        var referenceSlot = _contentLeft!.Find("Hair Color").gameObject;

        referenceSlot.SetActive(false);
        var newSlot = GameObject.Instantiate(referenceSlot.gameObject);
        referenceSlot.SetActive(true);

        newSlot.name = "newColorDisplaySlot";

        display.labelRs = newSlot.GetComponentInChildren<ReferenceString>(true);
        display.labelRs.Value = "Unset";


        display.typeValue = null;
        foreach (var mb in newSlot.GetComponentsInChildren<MonoBehaviour>(true))
        {
            if (mb is ReferenceCharacterLook)
            {
                display.typeValue = mb.gameObject;
                GameObject.DestroyImmediate(mb);
            }
        }

        return newSlot;
    }

    public static GameObject EmptyDisplaySlot(
        out (ReferenceString labelRs, GameObject? typeValue) display)
    {
        Init();

        var referenceSlot = _contentLeft!.Find("Front Hair").gameObject;

        referenceSlot.SetActive(false);
        var newSlot = GameObject.Instantiate(referenceSlot.gameObject);
        referenceSlot.SetActive(true);

        newSlot.name = "newDisplaySlot";

        display.labelRs = newSlot!.transform.Find("Text").GetComponent<ReferenceString>();
        display.labelRs.Value = "Unset";

        display.typeValue = null;
        foreach (var mb in newSlot.GetComponentsInChildren<MonoBehaviour>(true))
        {
            if (mb is ReferenceCharacterLook)
            {
                display.typeValue = mb.gameObject;
                GameObject.DestroyImmediate(mb);
            }
        }

        return newSlot;
    }

    public static GameObject EmptyChangeSlot(
        out (ReferenceFormattingText labelRfy, GameObject? typeValue, 
        GameObject arrowLeft, GameObject arrowRight) display)
    {
        Init();

        var referenceSlot = _contentRight!.Find("Voice").gameObject;

        referenceSlot.SetActive(false);
        var newSlot = GameObject.Instantiate(referenceSlot.gameObject);
        referenceSlot.SetActive(true);

        newSlot.name = "newChangeSlot";

        display.labelRfy = newSlot!.transform.Find("Text").GetComponent<ReferenceFormattingText>();
        display.labelRfy.Value = "Unset";

        var newSlotBottomText = newSlot.transform.Find("Bottom/Text");
        foreach (var mb in newSlotBottomText.GetComponents<MonoBehaviour>())
        {
            if (mb is ReferenceCharacterLook or ReferenceFormattingText)
                GameObject.DestroyImmediate(mb);
        }
        display.typeValue = newSlotBottomText.gameObject;

        var newSlotBottomArrow = newSlot.transform.Find("Bottom/Arrow");
        foreach (var mb in newSlotBottomArrow.GetComponents<MonoBehaviour>())
        {
            if (mb is ReferenceCharacterLook or UpdaterGameObject)
                GameObject.DestroyImmediate(mb);
        }

        var arrowLeft = newSlotBottomArrow.transform.Find("Left").gameObject;
        var arrowRight = newSlotBottomArrow.transform.Find("Right").gameObject;
        foreach (var arrow in new[] { arrowLeft, arrowRight })
            ComponentTools.RemoveClickEvent(arrow);

        display.arrowLeft = arrowLeft;
        display.arrowRight = arrowRight;

        return newSlot;
    }

    internal static GameObject ExtendedArea(
        out GameObject content)
    {
        if (UISpriteLoad.InterfaceExtendedArea is null)
            UISpriteLoad.LoadSprite();

        Init();

        // === Panel ===
        GameObject panel = new GameObject("ExtendedArea", typeof(RectTransform));
        var panelRT = panel.GetComponent<RectTransform>();

        panelRT.anchorMin = new Vector2(0, 0);
        panelRT.anchorMax = new Vector2(0, 1);

        panelRT.sizeDelta = new Vector2(FixedWidth, 0);

        panelRT.pivot = new Vector2(1, 0);

        panel.SetActive(false);

        // === Background ===
        var rootbackground = _informationLook!.Find("Background (Translucent)").gameObject;
        var background = GameObject.Instantiate(rootbackground, panelRT);
        background.name = "Background (Translucent)";
        var backgroundRT = background.GetComponent<RectTransform>();
        backgroundRT.anchoredPosition += new Vector2(11f, 0f);

        // === Border ===
        var rootBorder = _informationLook.Find("Border").gameObject;
        var border = GameObject.Instantiate(rootBorder, panelRT);
        border.name = "Border";
        var image = border.GetComponent<Image>();
        image.sprite = UISpriteLoad.InterfaceExtendedArea;
        var borderRT = border.GetComponent<RectTransform>();
        borderRT.anchoredPosition += new Vector2(11f, 0f);

        // === Content ===
        var rootcontent = _informationLook.Find("Content/Upper Left").gameObject;
        content = GameObject.Instantiate(rootcontent, panelRT);

        content.name = "Content";
        for (int i = content.transform.childCount - 1; i >= 0; i--)
        {
            GameObject.DestroyImmediate(content.transform.GetChild(i).gameObject);
        }

        var components = content.GetComponents<Component>();
        foreach (var comp in components)
        {
            if (comp is not RectTransform and not GridLayoutGroup)
                GameObject.DestroyImmediate(comp);
        }

        var contentRT = content.GetComponent<RectTransform>();
        contentRT.anchoredPosition = new Vector2(26f, -56f);

        return panel;
    }

    private static void Init()
    {
        if (_root == null)
        {
            _root = GameObject.Find("Window Female Information (Window)");
            _canvas = _root?.transform.Find("Canvas")?.gameObject;
            _contentLeft = _canvas?.transform.Find("LetterBox/Frame/Window (1)/Content/Upper Left");
            _contentRight = _canvas?.transform.Find("LetterBox/Frame/Window (1)/Content/Upper Right");
            _informationLook = _canvas?.transform.Find("LetterBox/Frame/Window (1)");
        }

        if (PlayData.Instance is null)
            _canvas!.SetActive(false);
    }
}