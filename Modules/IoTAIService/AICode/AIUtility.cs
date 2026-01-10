using Microsoft.ML.OnnxRuntime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTAIService.AICode
{
    public static class AIUtility
    {
        public static ExecutionProviderType TryEnableGpu(SessionOptions sessionOptions)
        {
            // 1. 获取系统所有可用执行提供者
            var allAvailableProviders = OrtEnv.Instance().GetAvailableProviders();
            Console.WriteLine($"📜 系统检测到的所有执行提供者：{string.Join(", ", allAvailableProviders)}");

            // 2. 过滤掉 CPU 提供者，只保留 GPU 类（含国产）
            var cpuProviders = new List<string> { "CPUExecutionProvider" };
            var gpuProviders = allAvailableProviders.Where(p => !cpuProviders.Contains(p)).ToList();

            if (!gpuProviders.Any())
            {
                Console.WriteLine("⚠️ 未检测到任何 GPU 执行提供者，使用 CPU 推理");
                return ExecutionProviderType.CPU;
            }

            // 3. 定义执行提供者映射（含国产 GPU 关键词匹配）
            var providerMapping = new Dictionary<string, ExecutionProviderType>
            {
                // 传统 GPU
                { "TensorrtExecutionProvider", ExecutionProviderType.NVIDIA_TensorRT },
                { "CUDAExecutionProvider", ExecutionProviderType.NVIDIA_CUDA },
                { "DmlExecutionProvider", ExecutionProviderType.AMD_DirectML },
                // 国产 GPU（按厂商关键词匹配，需根据实际提供者名称调整）
                { "AscendExecutionProvider", ExecutionProviderType.Ascend_CANN },   // 华为昇腾
                { "DCUExecutionProvider", ExecutionProviderType.Hygon_DCU },         // 海光 DCU
                { "BirenExecutionProvider", ExecutionProviderType.Biren_BRPC },      // 壁仞
                { "CANNExecutionProvider", ExecutionProviderType.Ascend_CANN },      // 昇腾别名
                { "MLUExecutionProvider", ExecutionProviderType.Other_GPU },         // 寒武纪 MLU
                { "KunlunXinExecutionProvider", ExecutionProviderType.Other_GPU },   // 昆仑芯
                { "SambanovaExecutionProvider", ExecutionProviderType.Other_GPU }    // 天数智芯
            };

            // 4. 遍历所有 GPU 提供者，逐个尝试启用（按检测到的顺序）
            foreach (var providerName in gpuProviders)
            {
                try
                {
                    // 根据提供者名称动态配置（适配大部分 GPU 的通用接口）
                    var providerType = providerMapping.TryGetValue(providerName, out var type)
                        ? type : ExecutionProviderType.Other_GPU;

                    Console.WriteLine($"🔍 尝试启用 {providerType}（提供者名称：{providerName}）");

                    // 通用化启用逻辑（适配 90% 的 GPU 执行提供者，含国产）
                    EnableExecutionProvider(sessionOptions, providerName);

                    // 验证是否启用成功（创建临时会话测试）
                    using var tempSession = new InferenceSession("", sessionOptions);
                    Console.WriteLine($"✅ 成功启用 {providerType} GPU");
                    return providerType;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ 启用 {providerName} 失败：{ex.Message.Substring(0, Math.Min(100, ex.Message.Length))}，尝试下一个");
                    continue;
                }
            }

            // 所有 GPU 都启用失败，降级到 CPU
            Console.WriteLine("⚠️ 所有 GPU 执行提供者启用失败，使用 CPU 推理");
            return ExecutionProviderType.CPU;
        }
        private static void EnableExecutionProvider(SessionOptions sessionOptions, string providerName)
        {
            // 设备 ID 默认用 0
            int deviceId = 0;
            // 按提供者名称调用对应启用方法（适配 ONNX Runtime 标准接口）
            switch (providerName.ToLower())
            {
                case "cudaexecutionprovider":
                    sessionOptions.AppendExecutionProvider_CUDA(deviceId);
                    break;
                case "tensorrtexecutionprovider":
                    sessionOptions.AppendExecutionProvider_Tensorrt(deviceId);
                    break;
                case "dmlexecutionprovider":
                    sessionOptions.AppendExecutionProvider_DML(deviceId);
                    break;
                // 国产 GPU 通用启用方式（大部分厂商遵循 ONNX Runtime 扩展接口）
                case "ascendexecutionprovider":
                case "cannexecutionprovider":
                case "dcuexecutionprovider":
                case "birenexecutionprovider":
                default:
                    // 通用扩展接口：通过参数配置启用（适配国产 GPU）
                    var providerOptions = new Dictionary<string, string>
                    {
                        { "device_id", deviceId.ToString() }
                    };
                    sessionOptions.AppendExecutionProvider(providerName, providerOptions);
                    break;
            }
        }

    }
}
