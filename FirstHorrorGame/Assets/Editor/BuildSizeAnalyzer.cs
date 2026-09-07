#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class BuildSizeAnalyzer
{
    [MenuItem("Tools/Build Size Analyzer/Build & Analyze")]
    public static void BuildAndAnalyze()
    {
        BuildPlayerOptions options =
            BuildPlayerWindow.DefaultBuildMethods.GetBuildPlayerOptions(
                new BuildPlayerOptions());

        options.options |= BuildOptions.DetailedBuildReport;

        Debug.Log("========================================");
        Debug.Log("        BUILD SIZE ANALYZER");
        Debug.Log("========================================");

        Debug.Log($"Build Target : {options.target}");
        Debug.Log($"Output Path  : {options.locationPathName}");

        BuildReport report = BuildPipeline.BuildPlayer(options);

        if (report == null)
        {
            Debug.LogError("BuildReport를 가져오지 못했습니다.");
            return;
        }

        if (report.summary.result != BuildResult.Succeeded)
        {
            Debug.LogError($"빌드 실패: {report.summary.result}");
            return;
        }

        Debug.Log(
            $"Build Success / Total Size : " +
            $"{FormatBytes((ulong)report.summary.totalSize)}");

        AnalyzePackedAssets(report);
    }

    private static void AnalyzePackedAssets(BuildReport report)
    {
        var allAssets = new List<PackedAssetInfo>();

        foreach (PackedAssets packedAssets in report.packedAssets)
        {
            if (packedAssets == null || packedAssets.contents == null)
                continue;

            allAssets.AddRange(packedAssets.contents);
        }

        Debug.Log("========================================");
        Debug.Log($"TOTAL PACKED ASSETS : {allAssets.Count:N0}");
        Debug.Log("========================================");

        AnalyzeByType(allAssets);
        AnalyzeByFolder(allAssets);
        AnalyzeTopAssets(allAssets);
    }

    // ============================================================
    // 1. 타입별 용량
    // ============================================================

    private static void AnalyzeByType(List<PackedAssetInfo> assets)
    {
        Debug.Log("");
        Debug.Log("========================================");
        Debug.Log("        SIZE BY ASSET TYPE");
        Debug.Log("========================================");

        var groups = assets
            .GroupBy(x => x.type)
            .Select(g => new
            {
                Type = g.Key,
                Size = g.Sum(x => (long)x.packedSize),
                Count = g.Count()
            })
            .OrderByDescending(x => x.Size);

        foreach (var group in groups)
        {
            Debug.Log(
                $"{group.Type,-25} | " +
                $"{FormatBytes((ulong)group.Size),12} | " +
                $"{group.Count,6:N0} assets");
        }
    }

    // ============================================================
    // 2. 폴더별 용량
    // ============================================================

    private static void AnalyzeByFolder(List<PackedAssetInfo> assets)
    {
        Debug.Log("");
        Debug.Log("========================================");
        Debug.Log("        SIZE BY FOLDER");
        Debug.Log("========================================");

        var folderGroups = new Dictionary<string, FolderInfo>();

        foreach (PackedAssetInfo asset in assets)
        {
            if (string.IsNullOrEmpty(asset.sourceAssetPath))
                continue;

            string path = asset.sourceAssetPath.Replace("\\", "/");

            string folder = GetTopLevelFolder(path);

            if (!folderGroups.TryGetValue(folder, out FolderInfo info))
            {
                info = new FolderInfo();
                folderGroups.Add(folder, info);
            }

            info.Size += (long)asset.packedSize;
            info.Count++;
        }

        var sortedFolders = folderGroups
            .OrderByDescending(x => x.Value.Size);

        foreach (var folder in sortedFolders)
        {
            Debug.Log(
                $"{folder.Key,-50} | " +
                $"{FormatBytes((ulong)folder.Value.Size),12} | " +
                $"{folder.Value.Count,6:N0} assets");
        }
    }

    // ============================================================
    // 3. 개별 에셋 TOP 100
    // ============================================================

    private static void AnalyzeTopAssets(List<PackedAssetInfo> assets)
    {
        Debug.Log("");
        Debug.Log("========================================");
        Debug.Log("        TOP 100 LARGEST ASSETS");
        Debug.Log("========================================");

        var sortedAssets = assets
            .Where(x => x.packedSize > 0)
            .OrderByDescending(x => x.packedSize)
            .Take(100)
            .ToArray();

        for (int i = 0; i < sortedAssets.Length; i++)
        {
            PackedAssetInfo asset = sortedAssets[i];

            Debug.Log(
                $"{i + 1,3}. " +
                $"{FormatBytes((ulong)asset.packedSize),12} | " +
                $"{asset.type,-20} | " +
                $"{asset.sourceAssetPath}");
        }
    }

    // ============================================================
    // Assets/ 이하 첫 번째 폴더 기준으로 그룹화
    // ============================================================

    private static string GetTopLevelFolder(string path)
    {
        if (string.IsNullOrEmpty(path))
            return "Unknown";

        const string assetsPrefix = "Assets/";

        if (path.StartsWith(assetsPrefix))
        {
            string relativePath = path.Substring(assetsPrefix.Length);

            int slashIndex = relativePath.IndexOf('/');

            if (slashIndex >= 0)
                return "Assets/" + relativePath.Substring(0, slashIndex);

            return "Assets";
        }

        if (path.StartsWith("Packages/"))
        {
            string relativePath = path.Substring("Packages/".Length);

            int slashIndex = relativePath.IndexOf('/');

            if (slashIndex >= 0)
                return "Packages/" + relativePath.Substring(0, slashIndex);

            return "Packages";
        }

        int firstSlash = path.IndexOf('/');

        if (firstSlash >= 0)
            return path.Substring(0, firstSlash);

        return path;
    }

    // ============================================================
    // 용량 포맷
    // ============================================================

    private static string FormatBytes(ulong bytes)
    {
        if (bytes >= 1024UL * 1024UL * 1024UL)
            return $"{bytes / (1024f * 1024f * 1024f):F2} GB";

        if (bytes >= 1024UL * 1024UL)
            return $"{bytes / (1024f * 1024f):F2} MB";

        if (bytes >= 1024UL)
            return $"{bytes / 1024f:F2} KB";

        return $"{bytes} B";
    }

    private class FolderInfo
    {
        public long Size;
        public int Count;
    }
}

#endif