#pragma warning disable CS1591

using MBMScripts;
using SystemExtensionLib.Tools;
using UnityEngine;
using UnityEngine.UI;

namespace SystemExtensionLib.Utils;

public static class GalleryExtraction
{
    private static GameObject? _root;
    private static GameObject? _canvas;
    private static Transform? _slaveLayout;
    private static Transform? _slaveAppearanceLeft;
    private static Transform? _slaveColor;

    public static GameObject EmptyGalleryChangeSlot(
        out (ReferenceFormattingText labelRfy, GameObject typeValue,
        GameObject arrowLeft, GameObject arrowRight) display)
    {
        Init();

        var referenceSlot = _slaveAppearanceLeft!.Find("Voice").gameObject;

        referenceSlot.SetActive(false);
        var newSlot = GameObject.Instantiate(referenceSlot.gameObject);
        referenceSlot.SetActive(true);

        newSlot.name = "newGalleryChangeSlot";

        display.labelRfy = newSlot!.transform.Find("Text (TMP)").GetComponent<ReferenceFormattingText>();
        display.labelRfy.Value = "Unset";

        var newSlotTypeText = newSlot.transform.Find("Type/Text (TMP)");
        foreach (var mb in newSlotTypeText.GetComponents<MonoBehaviour>())
        {
            if (mb is ReferenceCharacterLook or ReferenceFormattingText)
                GameObject.DestroyImmediate(mb);
        }
        display.typeValue = newSlotTypeText.gameObject;

        var arrowLeft = newSlot.transform.Find("Left").gameObject;
        var arrowRight = newSlot.transform.Find("Right").gameObject;
        foreach (var arrow in new[] { arrowLeft, arrowRight })
            ComponentTools.RemoveClickEvent(arrow);

        display.arrowLeft = arrowLeft;
        display.arrowRight = arrowRight;

        return newSlot;
    }

    public static GameObject EmptyGalleryColorSlot(
        out ReferenceFormattingText labelRfy,
        out FlexibleColorPicker flexibleColorPicker)
    {
        Init();

        var referenceSlot = _slaveColor!.Find("EyeColor").gameObject;

        referenceSlot.SetActive(false);
        var newSlot = GameObject.Instantiate(referenceSlot.gameObject);
        referenceSlot.SetActive(true);

        newSlot.name = "newGalleryColorSlot";

        labelRfy = newSlot!.transform.Find("Text (TMP)").GetComponent<ReferenceFormattingText>();
        labelRfy.Value = "Unset";

        foreach (var mb in newSlot.GetComponentsInChildren<MonoBehaviour>(true))
        {
            if (mb is ColorPickerInitialization || mb is InteractionCustomize)
                Object.DestroyImmediate(mb);
        }

        flexibleColorPicker = newSlot.GetComponent<FlexibleColorPicker>();
        flexibleColorPicker.onColorChange.RemoveAllListeners();

        return newSlot;
    }

    public static GameObject OnlyColorPicker(
    out FlexibleColorPicker flexibleColorPicker
    )
    {
        var colorPicker = EmptyGalleryColorSlot(
            out var _,
            out flexibleColorPicker);

        colorPicker.name = "Color Picker";

        var colorPickerRT = colorPicker.GetComponent<RectTransform>();
        colorPickerRT.anchorMin = Vector2.zero;
        colorPickerRT.anchorMax = Vector2.one;
        colorPickerRT.pivot = Vector2.up;
        colorPickerRT.anchoredPosition = Vector2.zero;
        colorPickerRT.offsetMin = Vector2.zero;
        colorPickerRT.offsetMax = Vector2.zero;

        GameObject.DestroyImmediate(colorPickerRT.Find("Text (TMP)").gameObject);
        GameObject.DestroyImmediate(colorPicker.GetComponent<Image>());

        return colorPicker;
    }

    private static void Init()
    {
        if (_root == null)
        {
            _root = GameObject.Find("Galley");
            _canvas = _root?.transform.Find("Canvas")?.gameObject;
            _slaveLayout = _canvas?.transform.Find("LetterBox/Frame/Slave Customize/Layout");
            _slaveAppearanceLeft = _slaveLayout?.transform.Find("Appearance2/Slave");
            _slaveColor = _slaveLayout?.transform.Find("Color/Content");
        }
    }
}
