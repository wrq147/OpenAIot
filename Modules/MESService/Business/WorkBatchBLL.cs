using AuthService.Fields;
using Common.Share;
using MESService.DAL;
using MESService.Model;
using ProducerService.DAL;
using ProducerService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace MESService.Business
{
    public class WorkBatchBLL
    {
        private ITAServiceProvider _provider;
        private WorkBatchDAL _workBatchDAL;
        public WorkBatchBLL(ITAServiceProvider provider, WorkBatchDAL workBatchDAL)
        {
            _provider = provider;
            _workBatchDAL = workBatchDAL;
        }
        public virtual async Task<PageObject<MZ_WorkBatch>> SelectByPage(In_WorkBatchList query, IUserInfo user)
        {
            var listpage = await _workBatchDAL.SelectByPage(query, user.OrgId);
            await FieldUtility.GenerateExtValList(_provider, listpage.List);
            return listpage;
        }

        public virtual async Task<BusResponse<MZ_WorkBatch>> Info(string id, IUserInfo user)
        {
            var info = (await _workBatchDAL.SelectList(x => x.Id == id && x.OrgId == user.OrgId)).FirstOrDefault();
            if (info == null)
            {
                return BusResponse<MZ_WorkBatch>.Error(3, "生产批次不存在");
            }
            await FieldUtility.GenerateExtObject(_provider, info, info.OrgId.Value);
            await FieldUtility.GenerateExtVals(_provider, info);
            return BusResponse<MZ_WorkBatch>.Success(info);
        }
    }
}
