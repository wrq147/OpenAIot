using Python.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace IoTAIService
{
    public static class PythonRuntimeHelper
    {
        /// <summary>
        /// 从指定Python目录自动搜索并设置Python.Runtime的PythonDLL路径（跨平台）
        /// </summary>
        /// <param name="pythonRootDir">Python安装根目录（如 /usr/bin 或 C:\Python313）</param>
        /// <returns>找到的Python库文件路径</returns>
        /// <exception cref="FileNotFoundException">未找到有效Python库文件时抛出</exception>
        public static string AutoSetPythonDllPath(string pythonRootDir)
        {
            if (!Directory.Exists(pythonRootDir))
            {
                throw new DirectoryNotFoundException($"Python目录不存在: {pythonRootDir}");
            }

            // 1. 根据系统获取Python库文件的搜索规则
            var (searchPatterns, fallbackName) = GetPythonLibrarySearchRules();

            // 2. 搜索符合规则的文件
            string pythonLibPath = null;
            foreach (var pattern in searchPatterns)
            {
                var files = Directory.GetFiles(pythonRootDir, pattern, SearchOption.AllDirectories)
                                     .Where(f => !IsInvalidSymbolicLink(f)) // 排除无效链接
                                     .OrderByDescending(f => GetPythonVersionFromFileName(f)) // 按版本降序
                                     .ToList();

                if (files.Any())
                {
                    pythonLibPath = files.First();
                    break;
                }
            }

            // 3. 尝试备用名称（如python3.dll/libpython3.so）
            if (string.IsNullOrEmpty(pythonLibPath))
            {
                var fallbackPath = Path.Combine(pythonRootDir, fallbackName);
                if (File.Exists(fallbackPath) && !IsInvalidSymbolicLink(fallbackPath))
                {
                    pythonLibPath = fallbackPath;
                }
            }

            // 4. 验证并设置路径
            if (string.IsNullOrEmpty(pythonLibPath))
            {
                throw new FileNotFoundException(
                    $"在目录 {pythonRootDir} 中未找到有效的Python库文件\n" +
                    $"搜索规则: {string.Join(", ", searchPatterns)} | 备用名称: {fallbackName}");
            }

            // 设置PythonDLL路径
            Runtime.PythonDLL = pythonLibPath;
            Console.WriteLine($"✅ 自动找到Python库文件: {pythonLibPath}");

            return pythonLibPath;
        }

        /// <summary>
        /// 根据操作系统获取Python库文件的搜索规则
        /// </summary>
        private static (string[] searchPatterns, string fallbackName) GetPythonLibrarySearchRules()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // Windows: 优先找python3xx.dll（如python313.dll），备用python3.dll
                return (new[] { "python3[0-9][0-9].dll", "python3[0-9].dll" }, "python3.dll");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                // Linux: 优先找libpython3.x.so（如libpython3.13.so），备用libpython3.so
                return (new[] { "libpython3.[0-9][0-9]?.so", "libpython3.so.*" }, "libpython3.so");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                // Mac: 优先找libpython3.x.dylib，备用libpython3.dylib
                return (new[] { "libpython3.[0-9][0-9]?.dylib" }, "libpython3.dylib");
            }
            else
            {
                throw new PlatformNotSupportedException($"不支持的操作系统: {RuntimeInformation.OSDescription}");
            }
        }

        /// <summary>
        /// 从文件名提取Python版本号（用于排序）
        /// </summary>
        private static Version GetPythonVersionFromFileName(string filePath)
        {
            var fileName = Path.GetFileName(filePath);
            // 匹配 python313.dll -> 3.13, libpython3.13.so -> 3.13
            var match = System.Text.RegularExpressions.Regex.Match(fileName, @"python3(\d+)(\d+)?" +
                                                                     @"|python3\.(\d+)\.(\d+)?");
            if (match.Success)
            {
                try
                {
                    int major = 3;
                    int minor = int.TryParse(match.Groups[1].Value, out var m1) ? m1 :
                               int.TryParse(match.Groups[3].Value, out var m3) ? m3 : 0;
                    int build = int.TryParse(match.Groups[2].Value, out var b2) ? b2 :
                               int.TryParse(match.Groups[4].Value, out var b4) ? b4 : 0;
                    return new Version(major, minor, build);
                }
                catch
                {
                    // 解析失败返回最低版本
                    return new Version(3, 0);
                }
            }
            return new Version(3, 0);
        }

        /// <summary>
        /// 判断是否为无效的符号链接（空链接/指向不存在的文件）
        /// </summary>
        private static bool IsInvalidSymbolicLink(string filePath)
        {
            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    // Windows: 检查符号链接目标是否存在
                    var fileInfo = new FileInfo(filePath);
                    if (fileInfo.Attributes.HasFlag(FileAttributes.ReparsePoint))
                    {
                        // 尝试获取真实路径，失败则为无效链接
                        return !File.Exists(Path.GetFullPath(filePath));
                    }
                }
                else
                {
                    // Linux/Mac: 检查软链接目标
                    var realPath = Path.GetFullPath(filePath);
                    return !File.Exists(realPath);
                }
            }
            catch
            {
                // 异常视为无效链接
                return true;
            }
            return false;
        }
    }
}
