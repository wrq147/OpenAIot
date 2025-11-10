using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace Common
{
    public interface IRepository
    {
        /// <summary>
        /// 服务提供者
        /// </summary>
        ITAServiceProvider Provider { get; set; }
    }
}
