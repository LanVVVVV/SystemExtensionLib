using MBMScripts;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SystemExtensionLib.Tools;

/// <summary>
/// 标签本地化辅助工具。<br/>
/// 提供统一的接口来为 UI 标签设置本地化文本，并在语言切换时自动刷新。<para/>
/// Label Localization Helpers.<br/>
/// Provides unified methods to assign localized text to UI labels and refresh them when the language changes.
/// </summary>
/// <remarks>
/// 本工具类维护两个内部列表：<br/>
/// - <c>ReferenceString</c> 组件标签集合<br/>
/// - <c>ReferenceFormattingText</c> 组件标签集合<br/>
/// 
/// 在调用 <see cref="OnLanguageChanged"/> 时，会遍历所有已注册标签并重新应用本地化函数。<para/>
/// This helper maintains two internal lists:<br/>
/// - <c>ReferenceString</c> Component labels<br/>
/// - <c>ReferenceFormattingText</c> Component labels<br/>
/// 
/// When <see cref="OnLanguageChanged"/> is invoked, all registered labels are refreshed with their localization delegates.
/// </remarks>
public static class LabelLocHelpers
{
    private static List<(ReferenceString, Func<string>)> LabelRsList { get; set; } = [];

    private static List<(ReferenceFormattingText, Func<string>)> LabelRftList { get; set; } = [];

    /// <summary>
    /// 为指定的 <see cref="GameObject"/> 设置标签本地化文本。<br/>
    /// Sets localized text for the given <see cref="GameObject"/> label.<para/>
    /// 如果对象包含 <c>ReferenceString</c> 或 <c>ReferenceFormattingText</c> 组件，
    /// 则会调用委托获取本地化字符串并赋值，同时注册到内部列表以支持语言切换刷新。<br/>
    /// If the object contains <c>ReferenceString</c> or <c>ReferenceFormattingText</c> Component, 
    /// the delegate is invoked to assign localized text and the label is registered for language change refresh.
    /// </summary>
    /// <param name="label">目标 UI 标签对象。<br/>Target UI label GameObject.</param>
    /// <param name="GetLabelLocalization">返回本地化字符串的委托。<br/>Delegate returning localized string.</param>
    public static void SetLabel(GameObject label, Func<string> GetLabelLocalization)
    {
        var labelRs = label.GetComponent<ReferenceString>();
        if (labelRs != null)
        {
            labelRs.Value = GetLabelLocalization.Invoke();
            LabelRsList.Add((labelRs, GetLabelLocalization));
        }

        var labelRfy = label.GetComponent<ReferenceFormattingText>();
        if (labelRfy != null)
        {
            labelRfy.Value = GetLabelLocalization.Invoke();
            LabelRftList.Add((labelRfy, GetLabelLocalization));
        }
    }

    /// <summary>
    /// 为 <see cref="ReferenceString"/> 组件标签设置本地化文本并注册。<br/>
    /// Sets localized text for a <see cref="ReferenceString"/> Component label and registers it.<para/>
    /// 调用委托获取文本并赋值，同时加入内部列表以支持语言切换刷新。<br/>
    /// Invokes the delegate to assign text and registers the label for language change refresh.
    /// </summary>
    /// <param name="labelRs">目标 <see cref="ReferenceString"/> 组件标签。<br/>Target <see cref="ReferenceString"/> Component label.</param>
    /// <param name="GetLabelLocalization">返回本地化字符串的委托。<br/>Delegate returning localized string.</param>
    public static void SetLabel(this ReferenceString labelRs, Func<string> GetLabelLocalization)
    {
        labelRs.Value = GetLabelLocalization.Invoke();
        LabelRsList.Add((labelRs, GetLabelLocalization));
    }

    /// <summary>
    /// 为 <see cref="ReferenceFormattingText"/> 组件标签设置本地化文本并注册。<br/>
    /// Sets localized text for a <see cref="ReferenceFormattingText"/> Component label and registers it.<para/>
    /// 调用委托获取文本并赋值，同时加入内部列表以支持语言切换刷新。<br/>
    /// Invokes the delegate to assign text and registers the label for language change refresh.
    /// </summary>
    /// <param name="labelRfy">目标 <see cref="ReferenceFormattingText"/> 组件标签。<br/>Target <see cref="ReferenceFormattingText"/> Component label.</param>
    /// <param name="GetLabelLocalization">返回本地化字符串的委托。<br/>Delegate returning localized string.</param>
    public static void SetLabel(this ReferenceFormattingText labelRfy, Func<string> GetLabelLocalization)
    {
        labelRfy.Value = GetLabelLocalization.Invoke();
        LabelRftList.Add((labelRfy, GetLabelLocalization));
    }

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
}