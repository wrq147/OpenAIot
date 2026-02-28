using Microsoft.Extensions.Options;
using Python.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace IoTAIService
{
    public class PythonExe : IDisposable
    {
        private ITAServiceProvider _provider;
        private bool _isInitialized = false;
        private dynamic _outclipModule;
        public PythonExe(ITAServiceProvider provider)
        {
            _provider = provider;
        }


        public void Init()
        {
            if (_isInitialized) return;
            var aiOption = _provider.GetService<IOptions<IoTAIOption>>().Value;
            Runtime.PythonDLL = PythonRuntimeHelper.AutoSetPythonDllPath(aiOption.PythonHome);
            PythonEngine.PythonHome = aiOption.PythonHome;
            PythonEngine.PythonPath = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + @"AIScript";
            PythonEngine.Initialize();

            using (Py.GIL())
            {
                dynamic sys = Py.Import("sys");
                string scriptDir = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + @"AIScript";
                sys.path.append(scriptDir);
            }


            _isInitialized = true;
        }
        public async Task<object> GenerateCNClipFeature(List<string> strArr, List<string> imgArr)
        {
            return await Task.Run(() =>
            {
                using (Py.GIL())
                {
                    try
                    {
                        if (_outclipModule == null)
                        {
                            _outclipModule = Py.Import("outclip");
                        }

                        // 执行Python函数并获取结果
                        dynamic result = _outclipModule.execall(strArr, imgArr);
                        Console.WriteLine($"调用Python execall函数结果: {result}");
                        return ConvertPythonResultToCSharp(result);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"调用出错: {ex.Message}");
                        // 可以返回null或抛出异常，根据业务需求调整
                        throw new InvalidOperationException("Python函数调用失败", ex);
                    }
                }
            });
        }
        private object ConvertPythonResultToCSharp(dynamic pythonResult)
        {
            // 情况1：如果返回的是字符串
            if (pythonResult is PyString pyStr)
            {
                return pyStr.ToString();
            }

            // 情况2：如果返回的是数字（int/float）
            if (pythonResult is PyInt pyInt)
            {
                return pyInt.ToInt32();
            }
            if (pythonResult is PyFloat pyFloat)
            {
                return pyFloat.ToDouble();
            }

            // 情况3：如果返回的是列表（最常见）
            if (pythonResult is PyList pyList)
            {
                List<object> csharpList = new List<object>();
                foreach (var item in pyList)
                {
                    // 递归转换列表中的每个元素
                    csharpList.Add(ConvertPythonResultToCSharp(item));
                }
                return csharpList;
            }

            // 情况4：如果返回的是字典
            if (pythonResult is PyDict pyDict)
            {
                Dictionary<object, object> csharpDict = new Dictionary<object, object>();
                foreach (var key in pyDict.Keys())
                {
                    var value = pyDict[key];
                    csharpDict.Add(ConvertPythonResultToCSharp(key), ConvertPythonResultToCSharp(value));
                }
                return csharpDict;
            }

            // 情况5：如果是numpy数组（AI场景常见）
            try
            {
                // 尝试调用numpy数组的tolist()方法转为普通列表
                dynamic tolistMethod = pythonResult.tolist;
                if (tolistMethod != null)
                {
                    dynamic listResult = pythonResult.tolist();
                    return ConvertPythonResultToCSharp(listResult);
                }
            }
            catch
            {
                // 不是numpy数组，继续后续判断
            }

            // 其他情况：直接返回字符串表示或原始对象
            return pythonResult?.ToString() ?? null;
        }

    
        public void Dispose()
        {
            if (_isInitialized)
            {
                PythonEngine.Shutdown();
                _isInitialized = false;
            }
        }
    }
}
