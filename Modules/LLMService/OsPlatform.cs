using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LLMService
{
    public static class OsPlatform
    {
        public static bool IsWindows() => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        public static bool IsLinux() => RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
        public static bool IsMacOS() => RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

        public static string GetShellName()
        {
            if (IsWindows()) return "cmd";
            if (IsLinux() || IsMacOS()) return "bash";
            return "unknown";
        }

        public static string GetScriptExtension()
        {
            if (IsWindows()) return ".bat";
            if (IsLinux() || IsMacOS()) return ".sh";
            return ".tmp";
        }

        /// 给AI的系统信息（让AI知道生成什么脚本）
        public static string GetSystemPrompt()
        {
            return $@"
当前操作系统: {(IsWindows() ? "Windows" : IsLinux() ? "Linux" : "macOS")}
默认脚本 shell: {GetShellName()}
脚本扩展名: {GetScriptExtension()}
请根据当前系统生成可直接运行的脚本代码。
";
        }
    }
}
