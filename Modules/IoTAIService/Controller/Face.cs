using AuthService.Controller;
using Common;
using Common.Share;
using IoTAIService.Business;
using IoTAIService.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using TemplateAction.Route;

namespace IoTAIService.Controller
{
    /// <summary>
    /// 人脸接口
    /// </summary>
    public class Face : AbstractLoginedController
    {
        private AiMemBLL _aiMemBLL;
        public Face(AiMemBLL aiMemBLL)
        {
            _aiMemBLL = aiMemBLL;
        }

        /// <summary>
        /// 人脸库列表
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<List<MZ_AIHouse>>> HouseList(In_FaceHouseList data)
        {
            return (await _aiMemBLL.HouseList(data, GetUser())).ToAjaxResult();
        }

        /// <summary>
        /// 添加人脸库
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<int>> AddHouse(MZ_AIHouse data)
        {
            return (await _aiMemBLL.InsertHouse(data, GetUser())).ToAjaxResult();
        }

        /// <summary>
        /// 编辑人脸库
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<int>> EditHouse(MZ_AIHouse data)
        {
            return (await _aiMemBLL.UpdateHouse(data, GetUser())).ToAjaxResult();
        }

        /// <summary>
        /// 删除人脸库
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<int>> RemoveHouse(string[] ids)
        {
            return (await _aiMemBLL.DeleteHouse(ids, GetUser())).ToAjaxResult();
        }
    }
}
