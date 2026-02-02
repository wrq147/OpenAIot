using IoTAIService.AIProject.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace IoTAIService.AIProject
{
    public class AIProjectManager
    {
        private ITAServiceProvider _provider;
        public AIProjectManager(ITAServiceProvider provider)
        {
            _provider = provider;
        }
        public async Task Init()
        {
            await FaceProject.Init(_provider);
            await DetectionProject.Init(_provider);
        }
    }
}
