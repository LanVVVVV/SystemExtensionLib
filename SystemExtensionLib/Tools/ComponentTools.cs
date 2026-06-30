#pragma warning disable CS1591

using HarmonyLib;
using MBMScripts;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SystemExtensionLib.Tools;

/// <summary>
/// MBM 组件工具类。<br/>
/// 专门用于操作 MBM 框架下的交互组件，例如按钮点击事件和引用数组。<para/>
/// MBM Component Tools.<br/>
/// Provides helper methods specifically for MBM framework components, 
/// such as managing button click events and reference arrays.
/// </summary>
public static class ComponentTools
{
    /// <summary>
    /// 移除指定 MBM 按钮上的点击事件。<br/>
    /// Removes all click events attached to the specified MBM button.
    /// </summary>
    /// <param name="button">
    /// 输入参数，目标 MBM 按钮对象。<br/>
    /// Input parameter, the target MBM button GameObject.
    /// </param>
    public static void RemoveClickEvent(GameObject button)
    {
        foreach (var mb in button.GetComponentsInChildren<MonoBehaviour>())
        {
            if (mb is InteractionClickEvent or InteractionUnit)
            {
                GameObject.DestroyImmediate(mb);
            }
        }

        var interType = AccessTools.TypeByName("MBMScripts.Interaction");
        var inter = button.GetComponent(interType);
        if (inter == null)
            return;

        var prop = AccessTools.Property(interType, "OnClickCallback");

        prop.SetValue(inter, null);
    }

    /// <summary>
    /// 为指定 MBM 按钮添加点击事件。<br/>
    /// Adds a click event to the specified MBM button.
    /// </summary>
    /// <param name="button">
    /// 输入参数，目标 MBM 按钮对象。<br/>
    /// Input parameter, the target MBM button GameObject.
    /// </param>
    /// <param name="onClick">
    /// 点击事件回调函数。<br/>
    /// Callback function invoked when the MBM button is clicked.
    /// </param>
    public static void AddClickEvent(GameObject button, Action onClick)
    {
        var interType = AccessTools.TypeByName("MBMScripts.Interaction");
        var inter = button.GetComponent(interType);
        if(inter == null)
            return;

        var prop = AccessTools.Property(interType, "OnClickCallback");
        if(prop == null)
            return;

        var callback = prop.GetValue(inter) as Delegate;
        var newCallback = Delegate.Combine(callback, onClick);
        prop.SetValue(inter, newCallback);
    }

    /// <summary>
    /// 设置指定 MBM Updater 的Reference组件数组。<br/>
    /// Sets the Reference Components array of the specified MBM Updater.
    /// </summary>
    /// <typeparam name="TUpdater">
    /// MBM Updater 类型参数。<br/>
    /// Type parameter representing the MBM Updater.
    /// </typeparam>
    /// <param name="targetUpdater">
    /// 输入参数，目标 MBM Updater 实例。<br/>
    /// Input parameter, the target MBM Updater instance.
    /// </param>
    /// <param name="refs">
    /// 引用列表，用于更新 MBM Updater 的引用数组。<br/>
    /// List of references used to update the MBM Updater's reference array.
    /// </param>
    /// <param name="fieldName">
    /// 字段名称，默认为 "m_ReferenceArray"。<br/>
    /// Field name, defaults to "m_ReferenceArray".
    /// </param>
    public static void SetReferenceArray<TUpdater>(this TUpdater targetUpdater, 
        List<Reference> refs,
        string fieldName = "m_ReferenceArray") where TUpdater : Updater
    {
        var fieldRef = AccessTools.FieldRefAccess<TUpdater, List<Reference>>(fieldName);
        fieldRef(targetUpdater) = refs;
    }
}