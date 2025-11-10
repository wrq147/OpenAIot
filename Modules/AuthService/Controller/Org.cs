using AuthService.Fields;
using AuthService.Model;
using Common;
using Common.Share;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.Route;

namespace AuthService.Controller
{
    /// <summary>
    /// 新的企业API
    /// </summary>
    public class Org : AbstractLoginedController
    {
        private OrgBLL _org;
        public Org(OrgBLL org)
        {
            _org = org;
        }

        /// <summary>
        /// 恢复指定企业
        /// </summary>
        /// <param name="org"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<AjaxResult> Recovery(In_RecoveryOrg org)
        {
            return (await _org.Recovery(org)).ToAjaxResult();
        }

        /// <summary>
        /// 查询指定组织
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> Search(string key)
        {
            return this.Success(await _org.SearchOrg(key));
        }

        /// <summary>
        /// 获取指定组织详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> Info(long id)
        {
            return this.Success(await _org.SelectById(id));
        }

        /// <summary>
        /// 通过邀请码加入企业
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<AjaxResult> Join(In_JoinAO data)
        {
            return (await _org.Join(data)).ToAjaxResult();
        }

        /// <summary>
        /// 修改当前企业
        /// </summary>
        /// <param name="dept"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<AjaxResult> Edit(MZ_Org data)
        {
            return (await _org.UpdateOrg(data)).ToAjaxResult();
        }

        /// <summary>
        /// 创建新企业
        /// </summary>
        /// <param name="org"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<AjaxResult> Create(MZ_Org org)
        {
            return (await _org.Create(org)).ToAjaxResult();
        }

        /// <summary>
        /// 解散企业
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> Remove(long id)
        {
            return (await _org.Delete(id)).ToAjaxResult();
        }

        /// <summary>
        /// 移交企业
        /// </summary>
        /// <param name="id">企业Id</param>
        /// <param name="uid">移交的目标用户Id</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> ChangeCreator(long id, long uid)
        {
            return (await _org.ChangeCreator(id, uid)).ToAjaxResult();
        }

        /// <summary>
        /// 退出企业
        /// </summary>
        /// <param name="id">企业Id</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> Exit(long id)
        {
            return (await _org.DeleteUser(id)).ToAjaxResult();
        }

        /// <summary>
        /// 企业拥有的主题列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<List<MZ_AppStyle>>> StyleList()
        {
            return (await _org.StyleList()).ToAjaxResult();
        }

        /// <summary>
        /// 更改企业的主题
        /// </summary>
        /// <param name="styleId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> ChangeStyle(string styleId)
        {
            return (await _org.ChangeStyle(styleId)).ToAjaxResult();
        }

        /// <summary>
        /// 获取指定组织的扩展字段信息
        /// </summary>
        /// <param name="orgId">组织Id</param>
        /// <param name="field">扩展字段</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<MZ_OrgExt>> Field(long orgId, string field)
        {
            return this.Success(await _org.SelectOrgExt(orgId, field));
        }

        /// <summary>
        /// 保存组织的扩展字段信息
        /// </summary>
        /// <param name="field"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<int>> SaveField(string field, string val)
        {
            var user = GetUser();
            return (await _org.SaveOrgExt(user.OrgId, field, val)).ToAjaxResult();
        }

        /// <summary>
        /// 获取表单信息列表
        /// </summary>
        /// <param name="field">产品、用户、部门、供应商、报工、工序</param>
        /// <param name="ext">是否包含扩展字段信息</param>
        /// <param name="isfixed">是否包含固定字段信息</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<List<FieldBase>>> FormFields(string field, bool ext = false, bool isfixed = true)
        {
            var user = GetUser();
            return (await _org.FormFields(user.OrgId, field, ext, isfixed)).ToAjaxResult();
        }


        /// <summary>
        /// 通过关键词搜索关联对象
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<PageObject<ObjectListItem>>> SearchObject(SearchPageParam query)
        {
            var user = GetUser();
            if (user.OrgId <= 0)
            {
                return this.Error<PageObject<ObjectListItem>>(133, "请切换到企业账号");
            }
            return (await FieldUtility.SearchObject(this.ServiceProvider, query, user)).ToAjaxResult();
        }
    }
}
