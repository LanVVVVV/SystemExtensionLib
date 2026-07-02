#pragma warning disable CS1591

using SystemExtensionLib.Utils;
using UnityEngine;

namespace SystemExtensionLib.Systems;

public partial class ExtendedInfoSlotSystem
{
    private static GameObject? _femaleExtendedArea;

    private static Transform? _femaleExtendedAreaContent;

    public static OrderedDoubleStringKeyDictionary<GameObject> FemaleExtendedAreaOrderedDic { get; private set; } = new();

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
