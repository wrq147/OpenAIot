using AuthService.Controller;
using Common;
using Common.Share;
using LLMService.Business;
using LLMService.Model;
using Microsoft.Extensions.AI;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.Route;

namespace LLMService.Controller
{
    /// <summary>
    /// 知识库接口
    /// </summary>
    public class Knowledge : AbstractLoginedController
    {
    }
}
