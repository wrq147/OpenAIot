using AuthService;
using Common;
using Common.Share;
using IoTService.DAL;
using IoTService.Models;
using JiebaNet.Segmenter;
using MyAccess.DB.Builder.WhereToSql;
using Quartz.Util;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace IoTService.Business
{
    public class IotScriptBLL
    {
        private ITAServiceProvider _provider;
        private IotScriptDAL _scriptDAL;
        public IotScriptBLL(ITAServiceProvider provider, IotScriptDAL scriptDAL)
        {
            _provider = provider;
            _scriptDAL = scriptDAL;
        }

        public virtual async Task<PageObject<MZ_IotScript>> SelectPage(In_ScriptList query)
        {
            var user = _provider.GetUser();

            Expression<Func<MZ_IotScript, bool>> expression = x => (x.OrgId == user.OrgId) || (x.OrgId == 1 && x.IsSystem == "1");
            if (!string.IsNullOrEmpty(query.SearchKey))
            {
                var keys = new JiebaSegmenter().CutForSearch(query.SearchKey);
                expression = expression.And(x => SonSqlFun.FullSearch("KeyWords", keys));
            }
            var rsp = await _scriptDAL.SelectPage(expression, query, "create_time desc");
            foreach(var item in rsp.List)
            {
                item.KeyWords = null;
            }
            return rsp; 
        }

        public virtual async Task<BusResponse<MZ_IotScript>> Info(string id)
        {
            var info = await _scriptDAL.Select(id);
            if (info == null)
            {
                return BusResponse<MZ_IotScript>.Error(111, "脚本模板不存在");
            }
            return BusResponse<MZ_IotScript>.Success(info);
        }
        public virtual async Task<BusResponse<int>> Update(MZ_IotScript data)
        {
            var user = _provider.GetUser();
            if (user.OrgId <= 0)
            {
                return BusResponse<int>.Error(112, "非企业用户无法修改脚本模板");
            }
            var oldScript = await _scriptDAL.Select(data.Id);
            if (oldScript == null)
            {
                return BusResponse<int>.Error(113, "脚本模板不存在");
            }
            if (oldScript.OrgId != user.OrgId)
            {
                return BusResponse<int>.Error(114, "无权修改当前脚本模板");
            }
            string tmpname = data.Name ?? oldScript.Name;
            string tmpremark = data.Remark ?? oldScript.Remark;
            data.KeyWords = string.Join(" ", new JiebaSegmenter().CutForSearch(tmpname + " " + tmpremark ?? string.Empty));
            data.IsSystem = null;
            data.SetUpdateBy(user);
            return BusResponse<int>.Success(await _scriptDAL.Update(data));
        }
        public virtual async Task<BusResponse<int>> Insert(MZ_IotScript data)
        {
            var user = _provider.GetUser();
            if (user.OrgId <= 0)
            {
                return BusResponse<int>.Error(112, "非企业用户无法添加脚本模板");
            }
            data.Id = MyAccess.Core.StringTool.GetGUID();
            data.OrgId = user.OrgId;
            data.KeyWords = string.Join(" ", new JiebaSegmenter().CutForSearch(data.Name + " " + data.Remark ?? string.Empty));
            if (user.OrgId != 1)
            {
                data.IsSystem = "0";
            }
            data.InitModelTSL ??= string.Empty;
            data.SetCreateBy(user);

            return BusResponse<int>.Success(await _scriptDAL.Insert(data));
        }
        public virtual async Task<BusResponse<int>> Delete(string[] ids)
        {
            var user = _provider.GetUser();
            if (user.OrgId <= 0)
            {
                return BusResponse<int>.Error(112, "非企业用户无法删除脚本模板");
            }
            try
            {
                var num = await _scriptDAL.Delete(x => x.OrgId == user.OrgId && ids.Contains(x.Id));
                return BusResponse<int>.Success(num);
            }
            catch (Exception ex)
            {
                return BusResponse<int>.Error(111, ex.Message);
            }
        }
    }
}
