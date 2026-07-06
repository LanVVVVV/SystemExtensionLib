#pragma warning disable CS1591

using MBM.ModLoader.Core;
using SystemExtensionLib.Systems;
using SystemExtensionLib.Tools;
using UnityEngine;

namespace SystemExtensionLib.Core;

public static class ModEntry
{
    internal const string ModName = "SystemExtensionLib";

    public static void Load()
    {
        ConfigSystem.Initialize();

        Loader.OnAllModsLoaded += () => Localization.OnLanguageChanged += OnLanguageChanged;

        Log($"{ModName} Mod loaded!");
    }
    internal static void Log(string msg) => Debug.Log($"[SEL] {msg}");

    internal static void LogWarning(string msg) => Debug.LogWarning($"[SEL] {msg}");

    internal static void LogError(string msg) => Debug.LogError($"[SEL] {msg}");

    private static void OnLanguageChanged(string langCode)
    {
        LabelLocHelpers.OnLanguageChanged();
        Log($"language changed: {langCode}");
    }
}