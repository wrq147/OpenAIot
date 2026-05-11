using AuthService.Controller;
using ChannelUtility.Message;
using Common;
using Common.Share;
using IoTAIService.AICode;
using IoTAIService.Models;
using NPOI.HSSF.Record.CF;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.Route;
using static CSnakes.Runtime.Python.PyObjectImporters;
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
        [HttpPost]
        public async Task<DefaultAjaxResult<List<List<float>>>> GenerateFeature(In_ClipFeature data)
        {
            PythonExe py = this.ServiceProvider.GetService<PythonExe>();
            var rs = await py.GenerateCNClipFeature(data.StrArr, data.ImgStr, data.ProjCoed);
            return this.Success(rs);
        }

        /// <summary>
        /// 生成图片特征
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<List<float>>> GenerateImageFeature(In_ImgFeature data)
        {
            string base64Str = null;
            if (!string.IsNullOrEmpty(data.ImgStr))
            {
                base64Str = data.ImgStr.Replace("data:image/png;base64,", "").Replace("data:image/jpg;base64,", "").Replace("data:image/jpeg;base64,", "");
            }
            if (string.IsNullOrEmpty(base64Str))
            {
                return this.Error<List<float>>(11, "请传入图片");
            }

            var rs = this.ServiceProvider.GetService<MobileCLIP2VisionRunner>().GetBase64Feature(base64Str);
            return this.Success(new List<float>(rs));
        }
    }
}
