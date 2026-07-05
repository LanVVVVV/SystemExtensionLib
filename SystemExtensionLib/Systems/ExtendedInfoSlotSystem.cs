#pragma warning disable CS1591

using SystemExtensionLib.Components;
using SystemExtensionLib.Tools;
using SystemExtensionLib.Utils;
using UnityEngine;

namespace SystemExtensionLib.Systems;

public static partial class ExtendedInfoSlotSystem
{
    private static UpdaterExtendedAreaManager? femaleExtendedAreaManager;

    private static void InitFemaleExtendedArea()
    {
        if (femaleExtendedAreaManager == null)
        {
            InjectExtendedArea(
                out GameObject content,
                out femaleExtendedAreaManager);
            femaleExtendedAreaManager.Initialize(content.transform);
        }
    }

    private static GameObject InjectExtendedArea(
        out GameObject content,
        out UpdaterExtendedAreaManager updaterExtendedArea)
    {
        var root = GameObject.Find("Window Female Information (Window)");
        var informationLook = root.transform.Find("Canvas/LetterBox/Frame/Window (1)").transform;

        var extendedArea = InformationLookExtraction.ExtendedArea(out content);
        extendedArea.transform.SetParent(informationLook,false);
        var border = informationLook.Find("Border");
        extendedArea.transform.SetSiblingIndex(border.GetSiblingIndex());

        updaterExtendedArea = extendedArea.AddComponent<UpdaterExtendedAreaManager>();
        var referenceExtendedArea = extendedArea.AddComponent<ReferenceExtendedAreaManager>();
        updaterExtendedArea.SetReferenceArray([referenceExtendedArea]);

        extendedArea!.SetActive(true);
        return extendedArea;
    }}
