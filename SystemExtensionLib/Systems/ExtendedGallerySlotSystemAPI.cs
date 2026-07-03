#pragma warning disable CS1591

using System;
using System.Reflection;
using SystemExtensionLib.Core;
using SystemExtensionLib.Tools;
using SystemExtensionLib.Utils;
using UnityEngine;

namespace SystemExtensionLib.Systems;

public static partial class ExtendedGallerySlotSystem
{
    public static (GameObject gallerySlot, GameObject gallerySlot2)? RegisterFemaleGalleryChangeSlot(
        string modName, string slotName,
        AppLayout appLayout,
        Func<string> GetLabelLocalization,
        Action<GameObject> OnLeftArrowClick,
        Action<GameObject> OnRightArrowClick,
        out (GameObject typeValue, GameObject typeValue2)? typeValues)
    {
        var newSlot = GalleryExtraction.EmptyGalleryChangeSlot(out var display);
        var newSlot2 = GalleryExtraction.EmptyGalleryChangeSlot(out var display2);

        foreach (var (slotObj, displayObj) in new[] { (newSlot, display), (newSlot2, display2) })
        {
            slotObj.name = $"{CallerDebug.GenerateAbbreviation(modName)}_Gallery_{slotName}";

            displayObj.labelRfy.SetLabel(GetLabelLocalization);

            ComponentTools.AddClickEvent(displayObj.arrowLeft, () => OnLeftArrowClick(displayObj.arrowLeft));
            ComponentTools.AddClickEvent(displayObj.arrowRight, () => OnRightArrowClick(displayObj.arrowRight));
        }

        var newSlots = (newSlot, newSlot2);

        var gallerySlots = RegisterFemaleGallerySlot(modName, slotName, newSlots, appLayout);

        typeValues = (display.typeValue, display2.typeValue);

        return gallerySlots;
    }

    public static (GameObject gallerySlot, GameObject gallerySlot2)? RegisterFemaleGalleryColorSlot(
        string modName, string slotName,
        AppLayout appLayout,
        Func<string> GetLabelLocalization,
        out (FlexibleColorPicker flexibleColorPicker, FlexibleColorPicker flexibleColorPicker2)? flexibleColorPickers)
    {
        var newSlot = GalleryExtraction.EmptyGalleryColorSlot(out var labelRfy, out var flexibleColorPicker);
        var newSlot2 = GalleryExtraction.EmptyGalleryColorSlot(out var labelRfy2, out var flexibleColorPicker2);

        foreach (var (slotObj, labelRfyObj) in new[] { (newSlot, labelRfy), (newSlot2, labelRfy2) })
        {
            slotObj.name = $"{CallerDebug.GenerateAbbreviation(modName)}_Gallery_{slotName}";

            labelRfyObj.SetLabel(GetLabelLocalization);
        }

        var newSlots = (newSlot, newSlot2);

        var gallerySlots = RegisterFemaleGallerySlot(modName, slotName, newSlots, appLayout);

        flexibleColorPickers = (flexibleColorPicker, flexibleColorPicker2);

        return gallerySlots;
    }

    public static (GameObject gallerySlot, GameObject gallerySlot2)? RegisterFemaleGallerySlot(
        string modName, string slotName,
        (GameObject gallerySlot, GameObject gallerySlot2)? gallerySlots, 
        AppLayout appLayout)
    {
        if (gallerySlots is null)
        {
            CallerDebug.LogError(modName, $"RegisterFemaleGallerySlot called with null '{slotName}' GallerySlot.");
            return null;
        }

        Init();

        (Transform parent, Transform parent2) = LayoutMap[appLayout];

        gallerySlots.Value.gallerySlot.transform.SetParent(parent, false);
        gallerySlots.Value.gallerySlot2.transform.SetParent(parent2, false);

        gallerySlots.Value.gallerySlot.SetActive(true);
        gallerySlots.Value.gallerySlot2.SetActive(true);

        return gallerySlots;
    }

    public static void Insert(
        (GameObject gallerySlot, GameObject gallerySlot2)? gallerySlots,
        InsertPoint insertPoint,
        string? targetSlotName = null
        )
    {
        if (gallerySlots is null) 
            return;

        foreach (var slot in gallerySlots.ToArray())
        {
            var parent = slot.transform.parent;
            if (targetSlotName == null)
            {
                switch (insertPoint)
                {
                    case InsertPoint.First:
                        slot.transform.SetAsFirstSibling();
                        break;

                    case InsertPoint.Last:
                        slot.transform.SetAsLastSibling();
                        break;

                    default:
                        var asm = Assembly.GetCallingAssembly();
                        CallerDebug.LogWarning(asm, $"targetSlotName is necessary of Above and Below");
                        break;
                }
            }
            else
            {
                var target = parent.Find(targetSlotName);
                if (target == null)
                {
                    var asm = Assembly.GetCallingAssembly();
                    CallerDebug.LogWarning(asm, $"Target GallerySlot '{targetSlotName}' not found under parent {parent.name}");
                    continue;
                }

                switch (insertPoint)
                {
                    case InsertPoint.Above:
                        slot.transform.SetSiblingIndex(target.GetSiblingIndex());
                        break;

                    case InsertPoint.Below:
                        slot.transform.SetSiblingIndex(target.GetSiblingIndex() + 1);
                        break;

                    default:
                        var asm = Assembly.GetCallingAssembly();
                        CallerDebug.LogWarning(asm, $"targetSlotName is unnecessary of First and Last");
                        break;
                }
            }
        }
    }

    public static GameObject[] ToArray(this (GameObject, GameObject)? GameObjects)
    {
        if (GameObjects is null) 
            return [];

        return [GameObjects.Value.Item1, GameObjects.Value.Item2];
    }

    public static (GameObject, FlexibleColorPicker)[] PairWithPickers(
        this (GameObject ColorSlot, GameObject ColorSlot2)? clothesColors,
        (FlexibleColorPicker ColorPicker, FlexibleColorPicker ColorPicker2)? flexibleColorPickers)
    {
        return
        [
            (clothesColors!.Value.ColorSlot, flexibleColorPickers!.Value.ColorPicker),
            (clothesColors.Value.ColorSlot2, flexibleColorPickers.Value.ColorPicker2)
        ];
    }
}

public enum AppLayout
{
    Left,
    Right,
    Color
}

public enum InsertPoint
{
    First,
    Last,
    Above,
    Below
}