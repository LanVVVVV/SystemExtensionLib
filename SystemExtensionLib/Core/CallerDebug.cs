using System;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace SystemExtensionLib.Core;

internal static class CallerDebug
{
    private static string GenerateAbbreviation(string rawName)
    {
        if (string.IsNullOrEmpty(rawName)) return "Unknown";

        var sb = new StringBuilder();
        foreach (char c in rawName)
        {
            if (char.IsUpper(c)) sb.Append(c);
        }

        if (sb.Length > 0) return sb.ToString();

        int fallbackLength = Math.Min(rawName.Length, 10);
        return rawName.Substring(0, fallbackLength);
    }

    private static string GetModAbbreviation(string modName)
    {
        if (string.IsNullOrEmpty(modName)) return "UnknownMod";

        string cleanName = modName.Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries)[0];

        return GenerateAbbreviation(cleanName);
    }

    private static string GetAssemblyAbbreviation(Assembly? asm)
    {
        if (asm == null) return "UnknownAsm";

        string? name = asm.GetName().Name;

        return string.IsNullOrEmpty(name) ? "UnknownAsm" : GenerateAbbreviation(name);
    }

    internal static void Log(string modName, string message)
    {
        Debug.Log($"[SEL-{GetModAbbreviation(modName)}] {message}");
    }

    internal static void Log(Assembly asm, string message)
    {
        string prefix = GetAssemblyAbbreviation(asm);
        Debug.Log($"[SEL-{prefix}] {message}");
    }

    internal static void LogWarning(string modName, string message)
    {
        Debug.LogWarning($"[SEL-{GetModAbbreviation(modName)}] {message}");
    }

    internal static void LogWarning(Assembly asm, string message)
    {
        string prefix = GetAssemblyAbbreviation(asm);
        Debug.LogWarning($"[SEL-{prefix}] {message}");
    }

    internal static void LogError(string modName, string message)
    {
        Debug.LogError($"[SEL-{GetModAbbreviation(modName)}] {message}");
    }
    internal static void LogError(Assembly asm, string message)
    {
        string prefix = GetAssemblyAbbreviation(asm);
        Debug.LogError($"[SEL-{prefix}] {message}");
    }
}
