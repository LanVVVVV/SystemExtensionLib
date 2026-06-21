using System;
using System.IO;
using System.Linq;

namespace SystemExtensionLib.Tools;

/// <summary>
/// 文件与路径操作工具集。<br/>
/// 提供文件创建、目录初始化、路径转换及嵌入资源名称解析等基础 IO 辅助功能。<para/>
/// File and path operation utilities.<br/>
/// Provides basic IO helper functions such as file creation, directory initialization, path conversion, and embedded resource name parsing.
/// </summary>
public static class FileIOHelper
{
    /// <summary>
    /// 创建或清空一个文件。<br/>
    /// 如果文件已存在，将被覆盖为空内容。<para/>
    /// Creates or clears a file.<br/>
    /// If the file already exists, it will be overwritten with empty content.
    /// </summary>
    /// <param name="filePath">目标文件的绝对或相对路径。<br/>Absolute or relative path of the target file.</param>
    /// <returns>
    /// true = 覆盖了已存在的文件；<br/>
    /// false = 新建了文件。<para/>
    /// true = Overwrote an existing file;<br/>
    /// false = Created a new file.
    /// </returns>
    public static bool CreateEmptyFile(string filePath)
    {
        bool existed = File.Exists(filePath);
        using var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
        return existed;
    }

    /// <summary>
    /// 确保指定的目录存在。如果目录不存在则自动创建。<br/>
    /// Ensures the specified directory exists. Creates it automatically if it does not exist.
    /// </summary>
    /// <param name="dirPath">目标目录的绝对或相对路径。<br/>Absolute or relative path of the target directory.</param>
    /// <returns>
    /// true = 目录不存在，已成功新建；<br/>
    /// false = 目录已存在，无需操作。<para/>
    /// true = Directory did not exist and was successfully created;<br/>
    /// false = Directory already exists, no action needed.
    /// </returns>
    public static bool EnsureDir(string dirPath)
    {
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
            return true;
        }
        return false;
    }

    /// <summary>
    /// 将绝对路径转换为相对于当前应用程序根目录的相对路径。<br/>
    /// Converts an absolute path to a relative path based on the current application root directory.
    /// </summary>
    /// <param name="fullPath">需要转换的绝对路径。<br/>Absolute path to be converted.</param>
    /// <returns>
    /// 转换后的相对路径；如果该路径不属于当前应用程序根目录，则返回原始路径。<br/>
    /// The converted relative path; returns the original path if it does not belong to the current application root directory.
    /// </returns>
    public static string GetRelativePath(string fullPath)
    {
        var rootDir = AppDomain.CurrentDomain.BaseDirectory;

        if (fullPath.StartsWith(rootDir, StringComparison.OrdinalIgnoreCase))
        {
            string relative = fullPath.Substring(rootDir.Length)
                .TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

            relative = relative.Replace('/', Path.DirectorySeparatorChar)
                .Replace('\\', Path.DirectorySeparatorChar);

            return relative;
        }

        return fullPath;
    }

    /// <summary>
    /// 从嵌入资源完整名称中解析出文件名。<br/>
    /// 默认截取资源名称的最后两个部分（文件夹名+文件名）作为结果。<para/>
    /// Parses the file name from the full embedded resource name.<br/>
    /// By default, extracts the last two parts (folder name + file name) of the resource name as the result.
    /// </summary>
    /// <param name="resourceName">嵌入资源的完整名称（以点号分隔）。<br/>Full name of the embedded resource (separated by dots).</param>
    /// <param name="takeLastParts">指定要截取的末尾段数，默认值为 2。<br/>Number of trailing parts to extract, defaults to 2.</param>
    /// <returns>解析出的文件名。<br/>The parsed file name.</returns>
    public static string GetResourceFileName(string resourceName, int takeLastParts = 2)
    {
        string[] parts = resourceName.Split('.');
        return string.Join(".", parts.Skip(parts.Length - takeLastParts));
    }
}