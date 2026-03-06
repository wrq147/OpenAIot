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
    public class RuleTrigger : IInfer
    {
        public Task Execute(string deviceId, Image<Rgb24> image, AIConfigData config, List<BoxItem> boxes)
        {
            throw new NotImplementedException();
        }

        public Task Init(ITAServiceProvider provider)
        {
            throw new NotImplementedException();
        }
    }
}
