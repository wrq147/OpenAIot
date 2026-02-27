using Microsoft.Extensions.Options;
using Python.Runtime;
using System;
using System.IO;
using TemplateAction.Core;

namespace IoTAIService
{
    public class PythonExe
    {
        private ITAServiceProvider _provider;
        public PythonExe(ITAServiceProvider provider)
        {
            _provider = provider;
        }
      

        public void Init()
        {
            var aiOption = _provider.GetService<IOptions<IoTAIOption>>().Value;
            Runtime.PythonDLL = PythonRuntimeHelper.AutoSetPythonDllPath(aiOption.PythonHome);
            PythonEngine.PythonHome = aiOption.PythonHome;
            PythonEngine.PythonPath = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + @"AIScript";
        }
        public void Invoke(string name)
        {
            using (Py.GIL())
            {
                try
                {
                    dynamic sys = Py.Import("sys");
                    string scriptDir = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + @"AIScript";
                    sys.path.append(scriptDir);

                    dynamic targetModule = Py.Import(name);
                    var result = targetModule.exexx(10, 20);
                    Console.WriteLine($"调用Python add函数结果: {result}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"调用出错: {ex.Message}");
                }
                finally
                {
                    // 关闭Python引擎
                    PythonEngine.Shutdown();
                }
            }
        }
    }
}
