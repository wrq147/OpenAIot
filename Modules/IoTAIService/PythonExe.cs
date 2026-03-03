using Microsoft.Extensions.Options;
using Python.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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
        /// <summary>
        /// 从指定Python目录自动搜索并设置Python.Runtime的PythonDLL路径（跨平台）
        /// </summary>
        /// <param name="pythonRootDir">Python安装根目录（如 /usr/bin 或 C:\Python313）</param>
        /// <returns>找到的Python库文件路径</returns>
        /// <exception cref="FileNotFoundException">未找到有效Python库文件时抛出</exception>
        private string AutoSetPythonDllPath(string pythonRootDir)
        {
            if (!Directory.Exists(pythonRootDir))
            {
                throw new DirectoryNotFoundException($"Python目录不存在: {pythonRootDir}");
            }

            // 根据系统获取Python库文件的搜索规则
            var searchPatterns = GetPythonLibrarySearchRules();

            // 搜索符合规则的文件
            string pythonLibPath = null;
            foreach (var pattern in searchPatterns)
            {
                var files = Directory.GetFiles(pythonRootDir, pattern, SearchOption.AllDirectories)
                    .OrderByDescending(f => f.Length)
                                     .ToList();

                if (files.Any())
                {
                    pythonLibPath = files.First();
                    break;
                }
            }

            // 验证并设置路径
            if (string.IsNullOrEmpty(pythonLibPath))
            {
                throw new FileNotFoundException($"在目录 {pythonRootDir} 中未找到有效的Python库文件");
            }

            // 设置PythonDLL路径
            Runtime.PythonDLL = pythonLibPath;
            Console.WriteLine($"自动找到Python库文件: {pythonLibPath}");

            return pythonLibPath;
        }

        /// <summary>
        /// 根据操作系统获取Python库文件的搜索规则
        /// </summary>
        private string[] GetPythonLibrarySearchRules()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // Windows: 优先找python3xx.dll（如python313.dll
                return new[] { "python3*.dll" };
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                // Linux: 优先找libpython3.x.so（如libpython3.13.so）
                return new[] { "libpython3.*.so", "libpython3.so.*" };
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                // Mac: 优先找libpython3.x.dylib
                return new[] { "libpython3.*.dylib" };
            }
            else
            {
                throw new PlatformNotSupportedException($"不支持的操作系统: {RuntimeInformation.OSDescription}");
            }
        }

        public void Init()
        {
            try
            {
                if (_isInitialized) return;
                var aiOption = _provider.GetService<IOptions<IoTAIOption>>().Value;
                if (string.IsNullOrEmpty(aiOption.PythonHome)) return;

                Runtime.PythonDLL = AutoSetPythonDllPath(aiOption.PythonHome);

                PythonEngine.Initialize();
                PythonEngine.BeginAllowThreads();
                using (Py.GIL())
                {
                    dynamic sys = Py.Import("sys");
                    string scriptDir = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + @"AIScript";
                    sys.path.append(scriptDir);
                }
                _isInitialized = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Python引擎初始化失败:" + ex.Message);
                _isInitialized = false;
            }
        }

        public async Task<List<List<float>>> GenerateCNClipFeature(List<string> strArr, List<string> imgArr)
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
                        PyList strPyList = null;
                        PyList imgPyList = null;
                        if (strArr != null && strArr.Count > 0)
                        {
                            strPyList = new PyList();
                            foreach (string str in strArr)
                            {
                                strPyList.Append(new PyString(str));
                            }
                        }
                        if (imgArr != null && imgArr.Count > 0)
                        {
                            imgPyList = new PyList();
                            foreach (string str in imgArr)
                            {
                                var base64Str = str.Replace("data:image/png;base64,", "").Replace("data:image/jpg;base64,", "").Replace("data:image/jpeg;base64,", "");
                                imgPyList.Append(new PyString(base64Str));
                            }
                        }

                        // 执行Python函数并获取结果
                        dynamic result = _outclipModule.execall(strPyList, imgPyList);
                        var pyDict = new PyDict(result);
                        List<List<float>> rs = null;
                        if (strPyList != null)
                        {
                            var txt_feat = pyDict.GetItem("text_features");
                            rs = Convert2DPyListTo2DList(txt_feat);
                        }
                        if (imgPyList != null)
                        {
                            var img_feat = pyDict.GetItem("image_features");
                            rs = Convert2DPyListTo2DList(img_feat);
                        }
                        if (rs == null)
                        {
                            return new List<List<float>>();
                        }
                        return rs;
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

        private List<List<float>> Convert2DPyListTo2DList(PyObject pyObj)
        {
            var nestedList = new List<List<float>>();
            var pyList = new PyList(pyObj);
            foreach (PyObject item in pyList)
            {
                nestedList.Add(Convert1DPyListTo2DList(item));
            }
            return nestedList;
        }
        private List<float> Convert1DPyListTo2DList(PyObject pyObj)
        {
            var nestedList = new List<float>();
            var pyList = new PyList(pyObj);
            foreach (PyObject item in pyList)
            {
                nestedList.Add(item.As<float>());
            }
            return nestedList;
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
