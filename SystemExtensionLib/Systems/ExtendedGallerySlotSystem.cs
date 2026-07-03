#pragma warning disable CS1591

using HarmonyLib;
using MBMScripts;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SystemExtensionLib.Systems;

public static partial class ExtendedGallerySlotSystem
{
    private static bool _initialized = false;

    private static Dictionary<AppLayout, (Transform parent, Transform parent2)> LayoutMap { get; set; } = null!;

    private static void Init()
    {
        if(_initialized)
            return;

        var root = GameObject.Find("Galley");
        var canvas = root?.transform.Find("Canvas")?.gameObject;
        var slaveLayout = canvas?.transform.Find("LetterBox/Frame/Slave Customize/Layout");
        var slaveAppearanceLeft = slaveLayout?.transform.Find("Appearance2/Slave");
        var slaveAppearanceRight = slaveLayout?.transform.Find("Appearance1/Slave");
        var slaveColor = slaveLayout?.transform.Find("Color/Content");
        var slave2AppearanceLeft = slaveLayout?.transform.Find("Slave2/Appearance2 (1)/Slave");
        var slave2AppearanceRight = slaveLayout?.transform.Find("Slave2/Appearance1 (1)/Slave");
        var slave2Color = slaveLayout?.transform.Find("Slave2/Color (1)/Content");

        LayoutMap =
                new()
        {
            { AppLayout.Left, (slaveAppearanceLeft!, slave2AppearanceLeft!) },
            { AppLayout.Right, (slaveAppearanceRight!, slave2AppearanceRight!) },
            { AppLayout.Color, ( slaveColor!, slave2Color!) }
        };

        foreach (var content in new[] { slaveAppearanceRight, slave2AppearanceRight, slaveColor, slave2Color })
        {
            var contentSizeFitter = content!.gameObject.AddComponent<ContentSizeFitter>();
            contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            var customFitter = content!.gameObject.AddComponent<CustomFitter>();
            rectformArrayRef(customFitter) = [content.parent.GetComponent<RectTransform>()];
        }

        _initialized = true;
    }

    private static readonly AccessTools.FieldRef<CustomFitter, RectTransform[]> rectformArrayRef =
            AccessTools.FieldRefAccess<CustomFitter, RectTransform[]>("m_RectTransformArray");
}
