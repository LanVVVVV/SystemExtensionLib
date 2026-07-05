using MBMScripts;
using System;
using System.Collections.Generic;
using System.Reflection;
using SystemExtensionLib.Core;
using SystemExtensionLib.Tools;
using SystemExtensionLib.Utils;
using UnityEngine;

namespace SystemExtensionLib.Systems;

/// <summary>
/// 扩展画廊插槽系统。<br/>
/// 用于在画廊窗口中动态注入扩展区域，并注册不同类型的插槽（颜色、切换、通用）。<para/>
/// Extended Gallery Slot System.<br/>
/// Provides dynamic injection of extended UI areas in the Gallery window,
/// and supports registration of multiple slot types (color, change, general).
/// </summary>
/// <remarks>
/// 本系统通过调用 <c>GalleryExtraction</c> 提供的插槽模板，结合 Mod 名称与插槽名称，
/// 自动生成 UI 节点并挂载到画廊扩展区域。支持语言刷新机制，确保插槽标签在语言切换时自动更新。<para/>
/// This system uses slot templates from <c>GalleryExtraction</c>, combines Mod name and slot name
/// to generate UI nodes, and attaches them to the extended gallery area. It supports language refresh
/// to ensure slot labels update automatically when the language changes.
/// </remarks>
public static partial class ExtendedGallerySlotSystem
{
    /// <summary>
    /// 注册一个带左右箭头的画廊切换插槽。<br/>
    /// Registers a gallery change slot with left/right arrow buttons.<para/>
    /// 创建两个切换插槽，设置标签本地化，并为左右箭头绑定点击事件。<br/>
    /// Creates two change slots, assigns localized labels, and binds click events to left/right arrows.<para/>
    /// 信息文本的 Reference Component 需手动挂载。<br/>
    /// Informational text Reference Component that requires manual attachment.<para/>
    /// 可使用 <see cref="ComponentTools.SetReferenceArray{TUpdater}(TUpdater,List{Reference},string)"/> 
    /// 辅助绑定 Reference 与 Updater 组件。<br/>
    /// Use <see cref="ComponentTools.SetReferenceArray{TUpdater}(TUpdater,List{Reference},string)"/> 
    /// to assist in binding.
    /// </summary>
    /// <param name="modName">Mod 名称。<br/>Name of the Mod.</param>
    /// <param name="slotName">插槽名称。<br/>Name of the slot.</param>
    /// <param name="appLayout">应用布局枚举，用于确定插槽挂载位置。<br/>App layout enum specifying slot parent locations.</param>
    /// <param name="GetLabelLocalization">返回本地化标签的委托。<br/>Delegate returning localized label text.</param>
    /// <param name="OnLeftArrowClick">左箭头点击事件，输入参数为左箭头对象自身。<br/>Callback invoked when the left arrow is clicked, with the arrow GameObject itself passed as the input parameter.</param>
    /// <param name="OnRightArrowClick">右箭头点击事件，输入参数为右箭头对象自身。<br/>Callback invoked when the right arrow is clicked, with the arrow GameObject itself passed as the input parameter.</param>
    /// <param name="typeValues">输出参数，包含两个信息文本挂载点。<br/>Output parameter containing two informational text GameObjects.</param>
    /// <returns>新建的两个插槽对象。<br/>Tuple of the newly created slot GameObjects.</returns>
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

    /// <summary>
    /// 注册一个颜色显示插槽。<br/>
    /// Registers a gallery color slot.<para/>
    /// 创建两个颜色插槽，设置标签本地化，并返回颜色选择器。<br/>
    /// Creates two color slots, assigns localized labels, and returns flexible color pickers.<para/>
    /// 信息文本的 Reference Component 需手动挂载。<br/>
    /// Informational text Reference Component that requires manual attachment.<para/>
    /// 可使用 <see cref="ComponentTools.SetReferenceArray{TUpdater}(TUpdater,List{Reference},string)"/> 
    /// 辅助绑定 Reference 与 Updater 组件。<br/>
    /// Use <see cref="ComponentTools.SetReferenceArray{TUpdater}(TUpdater,List{Reference},string)"/> 
    /// to assist in binding.
    /// </summary>
    /// <param name="modName">Mod 名称。<br/>Name of the Mod.</param>
    /// <param name="slotName">插槽名称。<br/>Name of the slot.</param>
    /// <param name="appLayout">应用布局枚举，用于确定插槽挂载位置。<br/>App layout enum specifying slot parent locations.</param>
    /// <param name="GetLabelLocalization">返回本地化标签的委托。<br/>Delegate returning localized label text.</param>
    /// <param name="flexibleColorPickers">输出参数，包含两个颜色选择器。<br/>Output parameter containing two flexible color pickers.</param>
    /// <returns>新建的两个插槽对象。<br/>Tuple of the newly created slot GameObjects.</returns>
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

    /// <summary>
    /// 将画廊插槽挂载到指定布局。<br/>
    /// Attaches gallery slots to the specified layout.<para/>
    /// 如果传入为空则记录错误，否则初始化并挂载到布局父对象。<br/>
    /// Logs error if slots are null; otherwise initializes and attaches them to layout parents.
    /// </summary>
    /// <param name="modName">Mod 名称。<br/>Name of the Mod.</param>
    /// <param name="slotName">插槽名称。<br/>Name of the slot.</param>
    /// <param name="gallerySlots">要挂载的插槽对象。<br/>Slots to be attached.</param>
    /// <param name="appLayout">应用布局枚举。<br/>App layout enum.</param>
    /// <returns>挂载后的插槽对象。<br/>Tuple of attached slot GameObjects.</returns>
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

    /// <summary>
    /// 插入插槽到指定位置。<br/>
    /// Inserts gallery slots at the specified position.<para/>
    /// 支持 First、Last、Above、Below 四种插入点。<br/>
    /// Supports First, Last, Above, and Below insertion points.
    /// </summary>
    /// <param name="gallerySlots">要插入的插槽对象。<br/>Slots to be inserted.</param>
    /// <param name="insertPoint">插入点枚举。<br/>Insertion point enum.</param>
    /// <param name="targetSlotName">目标插槽名称（Above/Below 时必需）。<br/>Target slot name (required for Above/Below).</param>
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

    /// <summary>
    /// 将二元组转换为数组。<br/>
    /// Converts a tuple of GameObjects into an array.<para/>
    /// 这样可以直接使用数组 API（如 foreach），方便遍历。<br/>
    /// This allows direct use of array APIs (e.g., foreach), making iteration easier.
    /// </summary>
    /// <param name="GameObjects">二元组对象。<br/>Tuple of GameObjects.</param>
    /// <returns>
    /// GameObject 数组，便于在循环中遍历。<br/>
    /// Array of GameObjects, convenient for iteration in loops.
    /// </returns>
    public static GameObject[] ToArray(this (GameObject, GameObject)? GameObjects)
    {
        if (GameObjects is null) 
            return [];

        return [GameObjects.Value.Item1, GameObjects.Value.Item2];
    }

    /// <summary>
    /// 将颜色插槽与颜色选择器配对。<br/>
    /// Pairs color slots with flexible color pickers.<para/>
    /// 返回一个数组，每个元素包含插槽与对应的颜色选择器。<br/>
    /// Returns an array of tuples pairing slots with their color pickers.
    /// </summary>
    /// <param name="clothesColors">颜色插槽二元组。<br/>Tuple of color slots.</param>
    /// <param name="flexibleColorPickers">颜色选择器二元组。<br/>Tuple of color pickers.</param>
    /// <returns>插槽与颜色选择器的配对数组。<br/>Array of slot-picker pairs.</returns>
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