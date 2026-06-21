using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using SystemExtensionLib.Core;
using SystemExtensionLib.Tools;
using UnityEngine;

namespace SystemExtensionLib.Systems;

/// <summary>
/// Mod 配置管理系统。<br/>
/// 负责统一管理所有 Mod 的外部配置文件，提供目录初始化、资源导出、文件加载等一站式功能。<para/>
/// Mod Configuration Management System.<br/>
/// Manages external configuration files for all Mods, providing directory initialization, 
/// resource exporting, and file loading functionalities in one place.
/// </summary>
/// <remarks>
/// 本系统以 "根目录/Config/" 为基准，自动为每个 Mod 创建独立的配置子目录，
/// 并封装了从嵌入资源导出到外部文件加载的完整流程。<para/>
/// Based on "RootDir/Config/", this system automatically creates an independent config subdirectory 
/// for each Mod and encapsulates the complete workflow from exporting embedded resources to loading external files.
/// </remarks>
public static class ConfigSystem
{
    #region Directory and File
    private static readonly string ConfigDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config");

    /// <summary>
    /// 初始化配置系统，确保配置目录存在。<br/>
    /// Initializes the configuration system and ensures the config directory exists.
    /// </summary>
    internal static void Initialize()
    {
        try
        {
            if (FileIOHelper.EnsureDir(ConfigDir))
                ModEntry.Log($"Create a Config folder in the root directory.");
        }
        catch (Exception ex)
        {
            ModEntry.LogError($"Failed to initialize config root dir: {ex.Message}");
        }
    }

    /// <summary>
    /// 确保指定的 Mod 配置目录存在。<br/>
    /// Ensures the specified Mod configuration directory exists.
    /// </summary>
    /// <param name="modName">Mod 名称或子目录路径（支持多级路径）。<br/>Mod name or subdirectory path (supports multi-level paths).</param>
    /// <param name="modDir">输出参数，返回该 Mod 配置目录的绝对路径。<br/>Output parameter returning the absolute path of the Mod config directory.</param>
    /// <returns>
    /// true = 目录存在或已成功创建；false = 创建失败。<br/>
    /// true = Directory exists or was successfully created; false = Creation failed.
    /// </returns>
    public static bool EnsureModDir(string modName, out string modDir)
    {
        modDir = Path.Combine(ConfigDir, modName);

        try
        {
            Initialize();
            if(FileIOHelper.EnsureDir(modDir))
                ModEntry.Log($"Create a {modName} folder under the Config directory.");
            return true;
        }
        catch (Exception ex)
        {
            ModEntry.LogError($"Failed to ensure mod directory for {modName}: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 获取指定 Mod 配置文件的完整路径。<br/>
    /// Gets the full path of the specified Mod configuration file.
    /// </summary>
    /// <param name="modName">Mod 名称或子目录路径。<br/>Mod name or subdirectory path.</param>
    /// <param name="fileName">配置文件名。<br/>Configuration file name.</param>
    /// <returns>
    /// 成功时返回完整路径；失败时返回 null。<br/>
    /// Returns full path on success; null on failure.
    /// </returns>
    public static string? GetConfigPath(string modName, string fileName)
    {
        return EnsureModDir(modName, out string modDir) ? Path.Combine(modDir, fileName) : null;
    }

    /// <summary>
    /// 在指定 Mod 配置目录下创建一个空文件。<br/>
    /// Creates an empty file in the specified Mod configuration directory.
    /// </summary>
    /// <param name="modName">Mod 名称或子目录路径。<br/>Mod name or subdirectory path.</param>
    /// <param name="fileName">文件名。<br/>File name.</param>
    /// <param name="filePath">输出参数，返回文件的完整路径。<br/>Output parameter returning the full file path.</param>
    /// <returns>
    /// true = 成功创建或覆盖；false = 失败。<br/>
    /// true = Successfully created or overwritten; false = Failed.
    /// </returns>
    public static bool CreateEmptyFile(string modName, string fileName, out string? filePath)
    {
        filePath = GetConfigPath(modName, fileName);
        if (filePath == null)
            return false;
        try
        {
            if (FileIOHelper.CreateEmptyFile(filePath))
            {
                CallerDebug.Log(modName, $"Cleared existing file: {FileIOHelper.GetRelativePath(filePath)}");
            }
            else
            {
                CallerDebug.Log(modName, $"Create Empty File: {FileIOHelper.GetRelativePath(filePath)}");
            }
            return true;
        }
        catch (Exception ex)
        {
            CallerDebug.LogError(modName, $"Can't create Empty File: {FileIOHelper.GetRelativePath(filePath)}: {ex}");
            return false;
        }
    }
    #endregion

    #region Load
    /// <summary>
    /// 从配置目录加载单个外部配置文件。<br/>
    /// Loads a single external configuration file from the config directory.
    /// </summary>
    /// <param name="modName">Mod 名称或子目录路径。<br/>Mod name or subdirectory path.</param>
    /// <param name="fileName">要加载的文件名。<br/>File name to load.</param>
    /// <param name="asm">可选程序集，默认为调用方程序集。<br/>Optional assembly, defaults to the calling assembly.</param>
    /// <returns>
    /// 包含文件内容的 TextAsset，未找到则返回 null。<br/>
    /// TextAsset containing file content, or null if not found.
    /// </returns>
    public static TextAsset? LoadExternalConfig(
        string modName,
        string fileName,
        Assembly? asm = null)
    {
        asm ??= Assembly.GetCallingAssembly();

        string? filePath = GetConfigPath(modName, fileName);
        if (filePath == null) return null;

        if (FileResourceHelper.TryLoadExternalFile(filePath, out var asset, asm))
        {
            CallerDebug.Log(modName, $"Loaded external config: {FileIOHelper.GetRelativePath(filePath)}");
            return asset;
        }

        CallerDebug.LogWarning(modName, $"External config not found: {fileName}");
        return null;
    }

    /// <summary>
    /// 静默加载外部配置文件，不输出调试日志。<br/>
    /// Silently loads an external configuration file without outputting debug logs.
    /// </summary>
    /// <param name="modName">Mod 名称或子目录路径。<br/>Mod name or subdirectory path.</param>
    /// <param name="fileName">要加载的文件名。<br/>File name to load.</param>
    /// <param name="asset">输出参数，成功加载时包含 TextAsset。<br/>Output parameter containing the TextAsset on success.</param>
    /// <param name="asm">可选程序集，默认为调用方程序集。<br/>Optional assembly, defaults to the calling assembly.</param>
    /// <returns>
    /// true = 成功加载；false = 加载失败。<br/>
    /// true = Successfully loaded; false = Loading failed.
    /// </returns>
    public static bool TryLoadExternalConfig(
        string modName,
        string fileName,
        out TextAsset? asset,
        Assembly? asm = null)
    {
        asm ??= Assembly.GetCallingAssembly();

        string? filePath = GetConfigPath(modName, fileName);
        if (filePath == null)
        {
            asset = null;
            return false;
        }

        return FileResourceHelper.TryLoadExternalFile(filePath, out asset, asm);
    }

    /// <summary>
    /// 批量加载指定 Mod 配置目录下的所有外部配置文件。<br/>
    /// Batch loads all external configuration files under the specified Mod config directory.
    /// </summary>
    /// <param name="modName">Mod 名称或子目录路径。<br/>Mod name or subdirectory path.</param>
    /// <param name="filterPredicate">文件过滤条件委托。为 null 时加载所有文件。<br/>File filter delegate. Loads all files if null.</param>
    /// <param name="includeSubDirs">是否包含子文件夹。<br/>Whether to include subdirectories.</param>
    /// <param name="asm">可选程序集，默认为调用方程序集。<br/>Optional assembly, defaults to the calling assembly.</param>
    /// <returns>
    /// 成功加载的 TextAsset 列表。<br/>
    /// List of successfully loaded TextAssets.
    /// </returns>
    public static List<TextAsset> LoadAllExternalConfigs(
        string modName,
        Func<string, bool>? filterPredicate = null,
        bool includeSubDirs = false,
        Assembly? asm = null)
    {
        asm ??= Assembly.GetCallingAssembly();

        if (!EnsureModDir(modName, out string modDir)) return new List<TextAsset>();
        return FileResourceHelper.LoadAllExternalFilesFromDir(modDir, filterPredicate, includeSubDirs, asm);
    }
    #endregion

    #region Export
    /// <summary>
    /// 将 Unity Resources 中的 TextAsset 导出到外部配置目录。<br/>
    /// Exports a TextAsset from Unity Resources to the external config directory.
    /// </summary>
    /// <param name="modName">Mod 名称或子目录路径。<br/>Mod name or subdirectory path.</param>
    /// <param name="fileNameWithoutExt">资源文件名（不含扩展名），系统默认使用 ".json"。<br/>Resource file name (without extension), defaults to ".json".</param>
    /// <returns>
    /// true = 成功导出；false = 资源不存在或导出失败。<br/>
    /// true = Successfully exported; false = Resource not found or export failed.
    /// </returns>
    public static bool ExportResourcesTextAssetFile(string modName, string fileNameWithoutExt)
    {
        if (!EnsureModDir(modName, out string modDir)) return false;

        TextAsset asset = Resources.Load<TextAsset>(fileNameWithoutExt);

        if (asset == null)
        {
            CallerDebug.LogWarning(modName, $"Resource not found: {fileNameWithoutExt}");
            return false;
        }

        string filePath = Path.Combine(modDir, fileNameWithoutExt + ".json");
        try
        {
            if (FileContentComparer.WriteFileIfChanged(filePath, asset.text))
            {
                CallerDebug.Log(modName, $"Exported resource file: {FileIOHelper.GetRelativePath(filePath)}");
            }
            else
            {
                CallerDebug.Log(modName, $"Export skipped (unchanged): {FileIOHelper.GetRelativePath(filePath)}");
            }

            return true;
        }
        catch (Exception ex)
        {
            CallerDebug.LogError(modName, $"Failed to export resource {fileNameWithoutExt}.json: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 将单个嵌入资源文件导出到外部配置目录。<br/>
    /// Exports a single embedded resource file to the external config directory.
    /// </summary>
    /// <param name="modName">Mod 名称或子目录路径。<br/>Mod name or subdirectory path.</param>
    /// <param name="resourceName">要导出的嵌入资源文件名。<br/>Name of the embedded resource to export.</param>
    /// <param name="overwritePredicate">可选的覆盖策略委托。为 null 时跳过已存在的文件。<br/>Optional overwrite strategy delegate. Skips existing files if null.</param>
    /// <param name="asm">可选程序集，默认为调用方程序集。<br/>Optional assembly, defaults to the calling assembly.</param>
    /// <returns>
    /// true = 成功导出；false = 导出失败。<br/>
    /// true = Successfully exported; false = Export failed.
    /// </returns>
    public static bool ExportEmbeddedConfig(
        string modName,
        string resourceName,
        Func<string, bool>?
        overwritePredicate = null,
        Assembly? asm = null)
    {
        asm ??= Assembly.GetCallingAssembly();

        if (!EnsureModDir(modName, out string modDir)) return false;

        string filePath = Path.Combine(modDir, resourceName);
        try
        {
            bool success = FileResourceHelper.ExportEmbeddedResource(
                resourceName, 
                filePath, 
                overwritePredicate, 
                asm);

            if (success)
            {
                CallerDebug.Log(modName, $"Exported embedded resource: {FileIOHelper.GetRelativePath(filePath)}");
            }
            else
            {
                CallerDebug.Log(modName, $"Export skipped (already exists): {FileIOHelper.GetRelativePath(filePath)}");
            }

            return success;
        }
        catch (Exception ex)
        {
            CallerDebug.LogError(modName, $"Failed to export embedded resource {resourceName}: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 将指定 Mod 的所有嵌入资源文件导出到外部配置目录。<br/>
    /// 默认导出 Properties.Configs 命名空间下的 JSON 文件。<para/>
    /// Exports all embedded resource files of the specified Mod to the external config directory. <br/>
    /// Defaults to exporting JSON files under the Properties.Configs namespace.
    /// </summary>
    /// <param name="modName">Mod 名称或子目录路径。<br/>Mod name or subdirectory path.</param>
    /// <param name="filterPredicate">可选的资源过滤器委托。默认导出 Properties.Configs 命名空间下的 JSON 文件。<br/>Optional resource filter delegate. Defaults to JSON files in Properties.Configs.</param>
    /// <param name="overwritePredicate">可选的覆盖策略委托。为 null 时跳过已存在的文件。<br/>Optional overwrite strategy delegate. Skips existing files if null.</param>
    /// <param name="asm">可选程序集，默认为调用方程序集。<br/>Optional assembly, defaults to the calling assembly.</param>
    /// <returns>
    /// 成功导出的文件数量。<br/>
    /// Number of successfully exported files.
    /// </returns>
    public static int ExportAllEmbeddedConfig(
        string modName,
        Func<string, bool>? filterPredicate = null, 
        Func<string, bool>? overwritePredicate = null, 
        Assembly? asm = null)
    {
        filterPredicate ??= FilePredicates.Presets.ConfigJsonFilter;

        asm ??= Assembly.GetCallingAssembly();

        if (!EnsureModDir(modName, out string modDir)) return 0;

        return FileResourceHelper.ExportAllEmbeddedResources(modDir, filterPredicate, overwritePredicate, asm);
    }
    #endregion
}
