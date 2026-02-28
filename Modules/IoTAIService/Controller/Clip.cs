using AuthService.Controller;
using Common.Share;
using IoTAIService.Business;
using IoTAIService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Route;
using TemplateAction.Core;
using Common;
namespace IoTAIService.Controller
{
    /// <summary>
    /// 多模态特征提取
    /// </summary>
    public class Clip : AbstractLoginedController
    {
        /// <summary>
        /// 生成多模态特征
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<string>> GenerateFeature(In_ClipFeature data)
        {
            PythonExe py = this.ServiceProvider.GetService<PythonExe>();
            var rs = await py.GenerateCNClipFeature(data.StrArr, data.ImgArr);
            return this.Success<string>();
        }
    }
}
