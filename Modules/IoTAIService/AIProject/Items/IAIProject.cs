using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace IoTAIService.AIProject.Items
{
    public interface IAIProject
    {
        Task Init(ITAServiceProvider provider);
    }
}
