using CSnakes.Runtime;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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


        public async Task<List<List<float>>> GenerateCNClipFeature(List<string> strArr, string imgStr, string projCode)
        {
            return await Task.Run(() =>
            {

                try
                {
                    var py = _provider.GetService<IPythonEnvironment>();
                    var outfgclip = py.Outfgclip();
                    string base64Str = null;
                    if (!string.IsNullOrEmpty(imgStr))
                    {
                        base64Str = imgStr.Replace("data:image/png;base64,", "").Replace("data:image/jpg;base64,", "").Replace("data:image/jpeg;base64,", "");
                    }

                    var input = outfgclip.Execall(strArr, base64Str, projCode);
                    List<List<float>> rs = input.Select(row => row.Select(x => (float)x).ToList()).ToList();
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
            });
        }


    }
}
