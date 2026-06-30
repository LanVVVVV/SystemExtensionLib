#pragma warning disable CS1591

using MBMScripts;
using System;
using System.Collections.Generic;
using SystemExtensionLib.Utils;
using UnityEngine;

namespace SystemExtensionLib.Systems;

public partial class ExtendedInfoSlotSystem
{
    private static GameObject? _femaleExtendedArea;

    private static Transform? _femaleExtendedAreaContent;

    private static List<(ReferenceString, Func<string>)> LabelRsList { get; set; } = [];

    private static List<(ReferenceFormattingText, Func<string>)> LabelRftList { get; set; } = [];

    public static OrderedDoubleStringKeyDictionary<GameObject> FemaleExtendedAreaOrderedDic { get; private set; } = new();

    internal static void OnLanguageChanged()
    {
        foreach (var (label, GetLocalization) in LabelRsList)
        {
            label.Value = GetLocalization.Invoke();
        }
        foreach (var (label, GetLocalization) in LabelRftList)
        {
            label.Value = GetLocalization.Invoke();
        }
    }

    private static void Init()
    {
        if (_femaleExtendedArea == null)
        {
            _femaleExtendedArea = InjectExtendedArea();
            _femaleExtendedAreaContent = _femaleExtendedArea.transform.Find("Content");
        }
    }

    private static GameObject InjectExtendedArea()
    {
        var root = GameObject.Find("Window Female Information (Window)");
        var informationLook = root.transform.Find("Canvas/LetterBox/Frame/Window (1)").transform;

        var ExtendedArea = InformationLookExtraction.ExtendedArea();

        GameObject extendedArea = GameObject.Instantiate(ExtendedArea!, informationLook);
        extendedArea.name = "ExtendedArea";
        extendedArea!.SetActive(true);

        var border = informationLook.Find("Border");
        extendedArea.transform.SetSiblingIndex(border.GetSiblingIndex());

        return extendedArea;
    }
}
