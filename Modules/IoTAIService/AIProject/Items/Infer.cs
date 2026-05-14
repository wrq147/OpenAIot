using ChannelUtility.Message;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace IoTAIService.AIProject.Items
{
    public abstract class Infer : IAIProject
    {
        public abstract Task Init(ITAServiceProvider provider);

        public abstract Task Execute(AIDetectRequestMeesage req, Image<Rgb24> image, AIConfigData config, List<BoxItem> boxes);
    }
}
