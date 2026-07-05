#pragma warning disable CS1591

using MBMScripts;
using System;
using System.Collections.Generic;
using SystemExtensionLib.Utils;
using UnityEngine;

namespace SystemExtensionLib.Components;

public class UpdaterExtendedAreaManager : Updater
{
    public OrderedDoubleStringKeyDictionary<GameObject> ExtendedAreaOrderedDic { get; private set; } = new();

    public Dictionary<(string, string), Func<Character, bool>> SlotsVisibilityConditions { get; private set; } = new();

    public Transform? Content { get; private set; }

    public void Initialize(Transform content) => Content = content;

    public void RegisterExtendedSlot(string modName, string slotName, GameObject infoSlot)
    {
        ExtendedAreaOrderedDic[modName, slotName] = infoSlot;
        infoSlot.transform.SetParent(Content, false);
        infoSlot.SetActive(true);
    }

    public void RegisterVisibilityCondition((string, string) key, Func<Character, bool> condition)
    {
        SlotsVisibilityConditions[key] = condition;
    }

    protected override void Display()
    {
        foreach (Reference reference in ReferenceArray)
        {
            switch (reference.ReferenceType)
            {
                case EReferenceType.Unit:
                    Unit unit = reference.GetUnit();
                    SetSlotsVisibility(unit);
                    break;
            }
        }
    }

    private void SetSlotsVisibility(Unit unit)
    {
        int activedNum = 0;

        if (unit is not Character character) return;

        foreach (KeyValuePair<(string, string), GameObject> slot in ExtendedAreaOrderedDic.GetOrderedItems())
        {
            bool isVisible = true;
            if (SlotsVisibilityConditions.TryGetValue(slot.Key, out var condition))
            {
                isVisible = condition(character);
            }

            if (slot.Value.activeSelf != isVisible)
            {
                slot.Value.SetActive(isVisible);
            }

            if (isVisible) activedNum++;
        }

        SetChildrenVisibility(activedNum > 0);
    }

    private void SetChildrenVisibility(bool visible)
    {
        foreach (Transform child in transform)
        {
            if (child.gameObject.activeSelf != visible)
            {
                child.gameObject.SetActive(visible);
            }
        }
    }
}