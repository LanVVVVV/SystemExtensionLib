using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace SystemExtensionLib.Tools;

/// <summary>
/// 文件路径谓词构建器。<br/>
/// 提供针对文件路径的基础判断条件，并支持通过 And、Or、Not 进行链式逻辑组合。<br/>
/// 适用于资源过滤（filterPredicate）及覆盖策略（overwritePredicate）等场景。<para/>
/// File path predicate builder.<br/>
/// Provides basic path matching conditions and supports chained logical combinations via And, Or, and Not.<br/>
/// Suitable for scenarios such as resource filtering (filterPredicate) and overwrite strategies (overwritePredicate).
/// </summary>
public static class FilePredicates
{
    #region Preset Filters
    /// <summary>
    /// 预置过滤器集合。<br/>
    /// 提供开箱即用的常用判断条件。<para/>
    /// Preset filters collection.<br/>
    /// Provides ready-to-use common matching conditions.
    /// </summary>
    public static class Presets
    {
        /// <summary>
        /// 仅匹配 Properties.Configs 命名空间下的 JSON 嵌入资源文件。<br/>
        /// Matches only JSON embedded resource files under the Properties.Configs namespace.
        /// </summary>
        public static Func<string, bool> ConfigJsonFilter =>
            FilterByResourceFolder("Properties.Configs").And(FilterByExtension(".json"));
    }
    #endregion

    #region Logical Combinators
    /// <summary>
    /// 逻辑与组合器：将当前谓词与另一个谓词组合。<br/>
    /// 仅当两个谓词都返回 true 时，结果才为 true。<para/>
    /// Logical AND combinator: Combines the current predicate with another.<br/>
    /// Returns true only if both predicates return true.
    /// </summary>
    /// <param name="f1">当前谓词委托（作为扩展方法的 this 参数）。<br/>Current predicate delegate (the 'this' parameter of the extension method).</param>
    /// <param name="f2">要组合的另一个谓词委托。<br/>The other predicate delegate to combine.</param>
    /// <returns>组合后的新谓词委托。<br/>The newly combined predicate delegate.</returns>
    public static Func<string, bool> And(this Func<string, bool> f1, Func<string, bool> f2)
        => res => f1(res) && f2(res);

    /// <summary>
    /// 逻辑或组合器：将当前谓词与另一个谓词组合。<br/>
    /// 只要其中一个谓词返回 true，结果即为 true。<para/>
    /// Logical OR combinator: Combines the current predicate with another.<br/>
    /// Returns true if either predicate returns true.
    /// </summary>
    /// <param name="f1">当前谓词委托（作为扩展方法的 this 参数）。<br/>Current predicate delegate (the 'this' parameter of the extension method).</param>
    /// <param name="f2">要组合的另一个谓词委托。<br/>The other predicate delegate to combine.</param>
    /// <returns>组合后的新谓词委托。<br/>The newly combined predicate delegate.</returns>
    public static Func<string, bool> Or(this Func<string, bool> f1, Func<string, bool> f2)
        => res => f1(res) || f2(res);

    /// <summary>
    /// 逻辑非取反器：反转当前谓词的匹配结果。<br/>
    /// 将原本返回 true 的结果变为 false，反之亦然。<para/>
    /// Logical NOT inverter: Inverts the matching result of the current predicate.<br/>
    /// Changes true to false, and vice versa.
    /// </summary>
    /// <param name="f">要取反的谓词委托。<br/>The predicate delegate to invert.</param>
    /// <returns>取反后的新谓词委托。<br/>The newly inverted predicate delegate.</returns>
    public static Func<string, bool> Not(this Func<string, bool> f)
        => res => !f(res);
    #endregion

    #region Base Predicate Builders
    /// <summary>
    /// 按指定扩展名匹配文件。<br/>
    /// Matches files by the specified extension.
    /// </summary>
    /// <param name="extension">文件扩展名（带或不带 '.' 均可）。<br/>File extension (with or without '.').</param>
    /// <returns>匹配指定扩展名的谓词委托。<br/>Predicate delegate matching the specified extension.</returns>
    public static Func<string, bool> FilterByExtension(string extension)
    {
        if (!extension.StartsWith("."))
            extension = "." + extension;
        return resPath => resPath.EndsWith(extension, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// 按嵌入资源路径中的文件夹层级匹配文件。<br/>
    /// 支持匹配单级文件夹（如 "Configs"）或多级嵌套路径（如 "Properties.Configs"）。<para/>
    /// Matches files by folder level in the embedded resource path.<br/>
    /// Supports matching single-level folders (e.g., "Configs") or multi-level nested paths (e.g., "Properties.Configs").
    /// </summary>
    /// <param name="resourceFolderName">嵌入资源路径中的文件夹名称（以点号分隔的命名空间或目录名）。<br/>Folder name in the embedded resource path (dot-separated namespace or directory name).</param>
    /// <returns>匹配指定嵌入资源文件夹的谓词委托。<br/>Predicate delegate matching the specified embedded resource folder.</returns>
    public static Func<string, bool> FilterByResourceFolder(string resourceFolderName)
    {
        if (resourceFolderName.Contains('.'))
        {
            string escapedFolder = Regex.Escape(resourceFolderName);
            string pattern = $"(^|\\.){escapedFolder}(\\.|$)";

            var regex = new Regex(pattern, RegexOptions.Compiled);

            return resPath => regex.IsMatch(resPath);
        }

        return resPath => resPath.Split('.').Contains(resourceFolderName, StringComparer.Ordinal);
    }

    /// <summary>
    /// 按嵌入资源路径中的文件夹层级和指定扩展名组合匹配文件。<br/>
    /// Matches files by folder level and extension in the embedded resource path.
    /// </summary>
    /// <param name="resourceFolderName">嵌入资源路径中的文件夹名称（以点号分隔的命名空间或目录名）。<br/>Folder name in the embedded resource path (dot-separated namespace or directory name).</param>
    /// <param name="extension">文件扩展名（带或不带 '.' 均可）。<br/>File extension (with or without '.').</param>
    /// <returns>同时满足文件夹和扩展名条件的谓词委托。<br/>Predicate delegate satisfying both folder and extension conditions.</returns>
    public static Func<string, bool> FilterByFolderAndExtension(string resourceFolderName, string extension) =>
        FilterByResourceFolder(resourceFolderName).And(FilterByExtension(extension));

    /// <summary>
    /// 按通配符模式匹配文件名。<br/>
    /// 示例: "*.json" → 只匹配 JSON 文件。<para/>
    /// Matches file names by wildcard pattern.<br/>
    /// Example: "*.json" → Matches only JSON files.
    /// </summary>
    /// <param name="patternFileName">通配符模式，例如 "*.json"、"data_*.xml"。<br/>Wildcard pattern, e.g., "*.json", "data_*.xml".</param>
    /// <returns>匹配通配符模式的谓词委托。<br/>Predicate delegate matching the wildcard pattern.</returns>
    public static Func<string, bool> FilterByWildcard(string patternFileName)
    {
        string regexPattern = "^" + Regex.Escape(patternFileName)
            .Replace("\\*", ".*")
            .Replace("\\?", ".") + "$";

        var regex = new Regex(regexPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);

        return fileName => regex.IsMatch(fileName);
    }
    #endregion
}