using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using SystemExtensionLib.Core;

namespace SystemExtensionLib.Tools;

/// <summary>
/// 文件内容比对与写入工具集。<br/>
/// 提供基于 MD5 哈希的文件内容比对、防冗余写入等功能。<para/>
/// File content comparison and writing utilities.<br/>
/// Provides MD5 hash-based file content comparison and redundancy-prevention writing.
/// </summary>
public static class FileContentComparer
{
    /// <summary>
    /// 计算文本的 MD5 哈希值。<br/>
    /// Computes the MD5 hash of a text.
    /// </summary>
    /// <param name="text">输入文本。<br/>Input text.</param>
    /// <returns>
    /// 大写无连字符的 MD5 哈希字符串。<br/>
    /// Uppercase MD5 hash string without hyphens.
    /// </returns>
    public static string ComputeHash(string text)
    {
        using var md5 = MD5.Create();
        byte[] bytes = Encoding.UTF8.GetBytes(text);
        byte[] hash = md5.ComputeHash(bytes);
        return BitConverter.ToString(hash).Replace("-", "");
    }

    /// <summary>
    /// 计算文件的 MD5 哈希值。<br/>
    /// Computes the MD5 hash of a file.
    /// </summary>
    /// <param name="filePath">文件路径。<br/>File path.</param>
    /// <returns>
    /// 大写无连字符的 MD5 哈希字符串；文件不存在或读取失败时返回 null。<br/>
    /// Uppercase MD5 hash string without hyphens; returns null if file does not exist or read fails.
    /// </returns>
    public static string? ComputeFileHash(string filePath)
    {
        try
        {
            using var md5 = MD5.Create();
            using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            byte[] hash = md5.ComputeHash(fs);
            return BitConverter.ToString(hash).Replace("-", "");
        }
        catch (Exception ex)
        {
            ModEntry.LogError($"[ComputeFileHash] Failed to hash file {filePath}: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// 判断文件内容是否与新内容一致。<br/>
    /// 采用分级比对策略以优化性能：<br/>
    /// 1. 文件不存在或长度不一致：直接返回 false。<br/>
    /// 2. 小于 100KB：直接进行字符串比对（最快）。<br/>
    /// 3. 大于等于 100KB：复用 MD5 哈希比对（避免全量读取内存）。<para/>
    /// Determines if file content is identical to new content.<br/>
    /// Uses a tiered comparison strategy for performance optimization:<br/>
    /// 1. File does not exist or length mismatch: returns false directly.<br/>
    /// 2. Less than 100KB: performs direct string comparison (fastest).<br/>
    /// 3. Greater than or equal to 100KB: reuses MD5 hash comparison (avoids full memory read).
    /// </summary>
    /// <param name="filePath">目标文件路径。<br/>Target file path.</param>
    /// <param name="newContent">新内容。<br/>New content.</param>
    /// <returns>
    /// true = 内容完全一致；false = 不一致或读取失败。<br/>
    /// true = Content is exactly the same; false = Mismatch or read failure.
    /// </returns>
    public static bool IsContentEqual(string filePath, string newContent)
    {
        if (!File.Exists(filePath)) return false;

        try
        {
            var fileInfo = new FileInfo(filePath);
            long fileLength = fileInfo.Length;
            byte[] newBytes = Encoding.UTF8.GetBytes(newContent);

            if (fileLength != newBytes.Length) return false;

            if (fileLength < 100 * 1024)
            {
                string oldContent = File.ReadAllText(filePath);
                return string.Equals(oldContent, newContent, StringComparison.Ordinal);
            }

            string? oldHash = ComputeFileHash(filePath);
            if (oldHash == null) return false;

            string newHash = ComputeHash(newContent);
            return string.Equals(oldHash, newHash, StringComparison.OrdinalIgnoreCase);
        }
        catch (IOException ex)
        {
            ModEntry.LogError($"[IsContentEqual] IO error: {ex.Message}");
            return false;
        }
        catch (Exception ex)
        {
            ModEntry.LogError($"[IsContentEqual] Unexpected error: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 仅在内容发生变化时写入文件。<br/>
    /// 如果文件不存在，会自动创建并写入新内容。<para/>
    /// Writes to file only if content has changed.<br/>
    /// If file does not exist, it will be created and new content written.
    /// </summary>
    /// <param name="filePath">目标文件路径。<br/>Target file path.</param>
    /// <param name="newContent">新内容。<br/>New content.</param>
    /// <returns>
    /// true = 文件已更新；false = 文件未变化，跳过写入。<br/>
    /// true = File updated; false = File unchanged, write skipped.
    /// </returns>
    public static bool WriteFileIfChanged(string filePath, string newContent)
    {
        if (IsContentEqual(filePath, newContent))
        {
            return false;
        }
        else
        {
            File.WriteAllText(filePath, newContent);
            return true;
        }
    }
}