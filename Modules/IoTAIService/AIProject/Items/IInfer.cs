using ChannelUtility.Message;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTAIService.AIProject.Items
{
    public interface IInfer : IAIProject
    {
        Task Execute(string deviceId, Image<Rgb24> image, AIConfigData config, List<BoxItem> boxes);
    }
}
