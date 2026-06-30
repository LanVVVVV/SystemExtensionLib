using System;
using SystemExtensionLib.Core;
using SystemExtensionLib.Tools;
using SystemExtensionLib.Utils;
using UnityEngine;

namespace SystemExtensionLib.Systems;

/// <summary>
/// 扩展信息插槽系统。<br/>
/// 用于在女性信息窗口中动态注入扩展区域，并注册不同类型的插槽（文本、颜色、切换）。<para/>
/// Extended Info Slot System.<br/>
/// Provides dynamic injection of extended UI areas in the Female Information window, 
/// and supports registration of multiple slot types (text, color, change).
/// </summary>
/// <remarks>
/// 本系统通过调用 <c>InformationLookExtraction</c> 提供的插槽模板，结合 Mod 名称与插槽名称，
/// 自动生成 UI 节点并挂载到扩展区域。支持语言刷新机制，确保插槽标签在语言切换时自动更新。<para/>
/// This system uses slot templates from <c>InformationLookExtraction</c>, combines Mod name and slot name 
/// to generate UI nodes, and attaches them to the extended area. It supports language refresh to ensure 
/// slot labels update automatically when the language changes.
/// </remarks>
public partial class ExtendedInfoSlotSystem
{
    /// <summary>
    /// 注册一个文本显示插槽。<br/>
    /// Registers a text display slot.<para/>
    /// 将插槽挂载到扩展区域，并加入有序字典管理。<br/>
    /// Attaches the slot to the extended area and registers it in the ordered dictionary.<para/>
    /// 信息文本的 Reference Component 需手动挂载。<br/>
    /// Informational text Reference Component that requires manual attachment.
    /// </summary>
    /// <param name="modName">Mod 名称。<br/>Name of the Mod.</param>
    /// <param name="slotName">插槽名称。<br/>Name of the slot.</param>
    /// <param name="GetLabelLocalization">返回本地化标签的委托。<br/>Delegate returning localized label text.</param>
    /// <param name="typeValue">输出参数，插槽中用于显示信息文本的游戏对象。<br/>Output parameter, the GameObject within the slot that is responsible for displaying informational text.</param>
    /// <returns>新建的插槽对象。<br/>The newly created slot GameObject.</returns>
    public static GameObject RegisterFemaleExtendedDisplaySlot(
        string modName, string slotName,
        Func<string> GetLabelLocalization,
        out GameObject? typeValue)
    {
        var newSlot = InformationLookExtraction.EmptyDisplaySlot(out var display);
        newSlot.name = $"{CallerDebug.GenerateAbbreviation(modName)}_{slotName}";

        display.labelRs.Value = GetLabelLocalization.Invoke();
        LabelRsList.Add((display.labelRs, GetLabelLocalization));

        typeValue = display.typeValue;

        RegisterFemaleExtendedSlot(modName, slotName, newSlot);
        return newSlot;
    }

    /// <summary>
    /// 注册一个颜色显示插槽。<br/>
    /// Registers a color display slot.<para/>
    /// 将插槽挂载到扩展区域，并加入有序字典管理。<br/>
    /// Attaches the slot to the extended area and registers it in the ordered dictionary.<para/>
    /// 信息文本的 Reference Component 需手动挂载。<br/>
    /// Informational text Reference Component that requires manual attachment.
    /// </summary>
    /// <param name="modName">Mod 名称。<br/>Name of the Mod.</param>
    /// <param name="slotName">插槽名称。<br/>Name of the slot.</param>
    /// <param name="GetLabelLocalization">返回本地化标签的委托。<br/>Delegate returning localized label text.</param>
    /// <param name="typeValue">输出参数，插槽中用于显示信息文本的游戏对象。<br/>Output parameter, the GameObject within the slot that is responsible for displaying informational text.</param>
    /// <returns>新建的插槽对象。<br/>The newly created slot GameObject.</returns>
    public static GameObject RegisterFemaleExtendedColorSlot(
        string modName, string slotName,
        Func<string> GetLabelLocalization,
        out GameObject? typeValue)
    {
        var newSlot = InformationLookExtraction.EmptyDisplayColorSlot(out var display);
        newSlot.name = $"{CallerDebug.GenerateAbbreviation(modName)}_{slotName}";

        display.labelRs.Value = GetLabelLocalization.Invoke();
        LabelRsList.Add((display.labelRs, GetLabelLocalization));

        typeValue = display.typeValue;

        RegisterFemaleExtendedSlot(modName, slotName, newSlot);
        return newSlot;
    }

    /// <summary>
    /// 注册一个带左右箭头的切换插槽。<br/>
    /// Registers a change slot with left/right arrow buttons.<para/>
    /// 将插槽挂载到扩展区域，并加入有序字典管理。<br/>
    /// Attaches the slot to the extended area and registers it in the ordered dictionary.<para/>
    /// 信息文本的 Reference Component 需手动挂载。<br/>
    /// Informational text Reference Component that requires manual attachment.
    /// </summary>
    /// <param name="modName">Mod 名称。<br/>Name of the Mod.</param>
    /// <param name="slotName">插槽名称。<br/>Name of the slot.</param>
    /// <param name="GetLabelLocalization">返回本地化标签的委托。<br/>Delegate returning localized label text.</param>
    /// <param name="OnLeftArrowClick">左箭头点击事件，输入参数为左箭头对象自身。<br/>Callback invoked when the left arrow is clicked, with the arrow GameObject itself passed as the input parameter.</param>
    /// <param name="OnRightArrowClick">右箭头点击事件，输入参数为右箭头对象自身。<br/>Callback invoked when the right arrow is clicked, with the arrow GameObject itself passed as the input parameter.</param>
    /// <param name="typeValue">输出参数，插槽中用于显示信息文本的游戏对象。<br/>Output parameter, the GameObject within the slot that is responsible for displaying informational text.</param>
    /// <returns>新建的插槽对象。<br/>The newly created slot GameObject.</returns>
    public static GameObject RegisterFemaleExtendedChangeSlot(
        string modName, string slotName,
        Func<string> GetLabelLocalization,
        Action<GameObject> OnLeftArrowClick,
        Action<GameObject> OnRightArrowClick,
        out GameObject? typeValue)
    {
        var newSlot = InformationLookExtraction.EmptyChangeSlot(out var display);
        newSlot.name = $"{CallerDebug.GenerateAbbreviation(modName)}_{slotName}";

        display.labelRfy.Value = GetLabelLocalization.Invoke();
        LabelRftList.Add((display.labelRfy, GetLabelLocalization));

        ComponentTools.AddClickEvent(display.arrowLeft, () => OnLeftArrowClick(display.arrowLeft));
        ComponentTools.AddClickEvent(display.arrowRight, () => OnRightArrowClick(display.arrowRight));

        typeValue = display.typeValue;

        RegisterFemaleExtendedSlot(modName, slotName, newSlot);
        return newSlot;
    }

    /// <summary>
    /// 将插槽挂载到扩展区域，并加入有序字典管理。<br/>
    /// Attaches the slot to the extended area and registers it in the ordered dictionary.
    /// </summary>
    /// <param name="modName">Mod 名称。<br/>Name of the Mod.</param>
    /// <param name="slotName">插槽名称。<br/>Name of the slot.</param>
    /// <param name="infoSlot">插槽对象。<br/>The slot GameObject.</param>
    /// <returns>成功挂载的插槽对象，或 null。<br/>The attached slot GameObject, or null if failed.</returns>
    public static GameObject? RegisterFemaleExtendedSlot(string modName, string slotName, GameObject infoSlot)
    {
        if (infoSlot is null)
        {
            CallerDebug.LogError(modName, $"RegisterFemaleExtendedSlot called with null '{slotName}' InfoSlot.");
            return null;
        }

        Init();
        FemaleExtendedAreaOrderedDic[modName, slotName] = infoSlot;
        infoSlot.transform.SetParent(_femaleExtendedAreaContent);
        infoSlot.transform.localScale = Vector3.one;
        infoSlot.SetActive(true);
        return infoSlot;
    }
}
