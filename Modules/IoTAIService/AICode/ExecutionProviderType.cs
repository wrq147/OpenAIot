using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTAIService.AICode
{
    /// <summary>
    /// 执行提供者类型
    /// </summary>
    public enum ExecutionProviderType
    {
        CPU,            // CPU 执行
        NVIDIA_TensorRT,// NVIDIA TensorRT
        NVIDIA_CUDA,    // NVIDIA CUDA
        AMD_DirectML,   // AMD DirectML
        Ascend_CANN,    // 华为昇腾 CANN
        Hygon_DCU,      // 海光 DCU
        Biren_BRPC,     // 壁仞 BRPC
        Other_GPU,      // 其他国产/小众 GPU
        Unknown         // 未知类型
    }
}
