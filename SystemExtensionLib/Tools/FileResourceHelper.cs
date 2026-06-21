using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using SystemExtensionLib.Core;
using UnityEngine;

namespace SystemExtensionLib.Tools;

/// <summary>
/// 文件与嵌入资源操作工具集。<br/>
/// 提供嵌入资源的加载与导出、外部文件的读取与批量遍历等底层 IO 辅助功能。<para/>
/// File and embedded resource operation utilities.<br/>
/// Provides underlying IO helper functions such as loading and exporting embedded resources, and reading and batch traversing external files.
/// </summary>
public static class FileResourceHelper
{
    #region Load

    /// <summary>
    /// 从程序集中加载指定的嵌入资源文件。<br/>
    /// Loads the specified embedded resource file from an assembly.
    /// </summary>
    /// <param name="fileName">嵌入资源文件名（不含命名空间前缀）。<br/>Embedded resource file name (without namespace prefix).</param>
    /// <param name="asm">可选程序集，默认为调用方程序集。<br/>Optional assembly, defaults to the calling assembly.</param>
    /// <returns>
    /// 包含文件内容的 TextAsset；未找到时返回 null。<br/>
    /// TextAsset containing file content; returns null if not found.
    /// </returns>
    public static TextAsset? LoadEmbeddedFile(string fileName, Assembly? asm = null)
    {
        asm ??= Assembly.GetCallingAssembly();
        var resourceName = asm.GetManifestResourceNames()
            .FirstOrDefault(n => n.EndsWith("." + fileName, StringComparison.Ordinal));

        if (resourceName == null)
        {
            CallerDebug.LogWarning(asm, $"Embedded resource not found: {fileName}");
            return null;
        }

        using var stream = asm.GetManifestResourceStream(resourceName);
        using var reader = new StreamReader(stream);
        return new TextAsset(reader.ReadToEnd());
    }

    /// <summary>
    /// 尝试将外部文件加载为 TextAsset。<br/>
    /// Attempts to load an external file as a TextAsset.
    /// </summary>
    /// <param name="filePath">外部文件路径。<br/>External file path.</param>
    /// <param name="asset">输出参数，成功加载时包含文件内容。<br/>Output parameter containing file content on successful load.</param>
    /// <param name="asm">可选程序集，默认为调用方程序集。<br/>Optional assembly, defaults to the calling assembly.</param>
    /// <returns>
    /// true = 成功加载；false = 文件不存在或读取失败。<br/>
    /// true = Successfully loaded; false = File does not exist or read failed.
    /// </returns>
    public static bool TryLoadExternalFile(
        string filePath,
        out TextAsset? asset,
        Assembly? asm = null)
    {
        asm ??= Assembly.GetCallingAssembly();
        try
        {
            if (File.Exists(filePath))
            {
                string content = File.ReadAllText(filePath);
                asset = new TextAsset(content);
                return true;
            }
        }
        catch (Exception ex)
        {
            CallerDebug.LogError(asm, $"Failed to load external file {FileIOHelper.GetRelativePath(filePath)}: {ex.Message}");
        }

        asset = null;
        return false;
    }

    /// <summary>
    /// 遍历指定目录，使用委托筛选并批量加载外部文件为 TextAsset。<br/>
    /// Traverses the specified directory, filters using a delegate, and batch loads external files as TextAssets.
    /// </summary>
    /// <param name="dirPath">目标目录路径。<br/>Target directory path.</param>
    /// <param name="filterPredicate">可选的文件过滤条件委托。为 null 时加载所有文件。<br/>Optional file filter delegate. Loads all files if null.</param>
    /// <param name="includeSubDirs">是否递归包含子文件夹。true = 遍历所有子目录；false = 仅当前目录。<br/>Whether to recursively include subdirectories. true = Traverse all subdirectories; false = Current directory only.</param>
    /// <param name="asm">可选程序集，默认为调用方程序集。<br/>Optional assembly, defaults to the calling assembly.</param>
    /// <returns>成功加载的 TextAsset 列表。<br/>List of successfully loaded TextAssets.</returns>
    public static List<TextAsset> LoadAllExternalFilesFromDir(
        string dirPath,
        Func<string, bool>? filterPredicate = null,
        bool includeSubDirs = false,
        Assembly? asm = null)
    {
        var result = new List<TextAsset>();
        asm ??= Assembly.GetCallingAssembly();

        try
        {
            if (!Directory.Exists(dirPath))
            {
                CallerDebug.LogWarning(asm, $"Directory not found: {FileIOHelper.GetRelativePath(dirPath)}");
                return result;
            }

            var searchOption = includeSubDirs ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            var files = Directory.GetFiles(dirPath, "*.*", searchOption);

            foreach (var file in files)
            {
                if (filterPredicate != null && !filterPredicate(file))
                    continue;

                if (TryLoadExternalFile(file, out var asset) && asset != null)
                {
                    result.Add(asset);
                    CallerDebug.Log(asm, $"Loaded external file: {FileIOHelper.GetRelativePath(file)}");
                }
                else
                {
                    CallerDebug.LogWarning(asm, $"Failed to load file: {FileIOHelper.GetRelativePath(file)}");
                }
            }
            CallerDebug.Log(asm, $"Loaded {result.Count} external files");
        }
        catch (Exception ex)
        {
            CallerDebug.LogError(asm, $"Error while loading files from {FileIOHelper.GetRelativePath(dirPath)}: {ex.Message}");
        }

        return result;
    }
    #endregion

    #region Export
    /// <summary>
    /// 将单个嵌入资源导出到外部文件。<br/>
    /// Exports a single embedded resource to an external file.
    /// </summary>
    /// <param name="resourceName">嵌入资源的完整名称。<br/>Full name of the embedded resource.</param>
    /// <param name="filePath">目标文件路径。<br/>Target file path.</param>
    /// <param name="overwritePredicate">可选的覆盖策略委托。接收文件名，返回 true 表示允许覆盖。<br/>Optional overwrite strategy delegate. Receives file name and returns true to allow overwriting.</param>
    /// <param name="asm">可选程序集，默认为调用方程序集。<br/>Optional assembly, defaults to the calling assembly.</param>
    /// <returns>
    /// true = 成功导出；false = 资源不存在或已跳过覆盖。<br/>
    /// true = Successfully exported; false = Resource not found or overwrite skipped.
    /// </returns>
    public static bool ExportEmbeddedResource(
        string resourceName,
        string filePath, Func<string, bool>?
        overwritePredicate = null,
        Assembly? asm = null)
    {
        asm ??= Assembly.GetCallingAssembly();

        try
        {
            if (!asm.GetManifestResourceNames().Contains(resourceName))
            {
                CallerDebug.LogWarning(asm, $"Embedded resource not found: {resourceName}");
                return false;
            }

            string fileName = Path.GetFileName(filePath);
            bool shouldOverwrite = overwritePredicate?.Invoke(fileName) ?? false;

            if (shouldOverwrite || !File.Exists(filePath))
            {
                using var stream = asm.GetManifestResourceStream(resourceName);
                using var reader = new StreamReader(stream);
                File.WriteAllText(filePath, reader.ReadToEnd());

                CallerDebug.Log(asm, $"Exported embedded {fileName} to {FileIOHelper.GetRelativePath(filePath)}" +
                    (shouldOverwrite ? " (overwritten)" : ""));
                return true;
            }
            else
            {
                CallerDebug.Log(asm, $"External file already exists: {FileIOHelper.GetRelativePath(filePath)}, skipped export.");
                return false;
            }
        }
        catch (Exception ex)
        {
            CallerDebug.LogError(asm, $"Failed to export embedded resource {resourceName}: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 批量导出指定程序集中的嵌入资源到目标目录。<br/>
    /// Supports custom filter conditions and overwrite strategies.<para/>
    /// Batch exports embedded resources from the specified assembly to the target directory.<br/>
    /// Supports custom filter conditions and overwrite strategies.
    /// </summary>
    /// <param name="dirPath">目标目录路径。<br/>Target directory path.</param>
    /// <param name="filterPredicate">可选的资源过滤条件委托。接收完整资源名，返回 true 表示需要导出。<br/>Optional resource filter delegate. Receives full resource name and returns true to export.</param>
    /// <param name="overwritePredicate">可选的覆盖策略委托。接收文件名，返回 true 表示允许覆盖。<br/>Optional overwrite strategy delegate. Receives file name and returns true to allow overwriting.</param>
    /// <param name="asm">可选程序集，默认为调用方程序集。<br/>Optional assembly, defaults to the calling assembly.</param>
    /// <returns>成功导出的文件数量。<br/>Number of successfully exported files.</returns>
    public static int ExportAllEmbeddedResources(
        string dirPath,
        Func<string, bool>? filterPredicate = null,
        Func<string, bool>? overwritePredicate = null,
        Assembly? asm = null)
    {
        int successCount = 0;
        asm ??= Assembly.GetCallingAssembly();

        try
        {
            var resources = asm.GetManifestResourceNames();

            if (filterPredicate != null)
                resources = resources.Where(filterPredicate).ToArray();

            foreach (var res in resources)
            {
                string fileName = FileIOHelper.GetResourceFileName(res);
                string filePath = Path.Combine(dirPath, fileName);

                if (ExportEmbeddedResource(res, filePath, overwritePredicate, asm))
                    successCount++;
            }

            CallerDebug.Log(asm, $"Exported {successCount}/{resources.Length} embedded files to {FileIOHelper.GetRelativePath(dirPath)}");
        }
        catch (Exception ex)
        {
            CallerDebug.LogError(asm, $"Failed to batch export embedded resources: {ex.Message}");
        }

        return successCount;
    }
    #endregion
}