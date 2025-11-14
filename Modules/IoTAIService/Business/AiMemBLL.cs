using Common;
using Common.IdGenerator;
using Common.Share;
using IoTAIService.AICode;
using IoTAIService.DAL;
using IoTAIService.Models;
using Microsoft.ML.OnnxRuntime.Tensors;
using MyAccess.DB.Builder.WhereToSql;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace IoTAIService.Business
{
    public class AiMemBLL
    {
        private ITAServiceProvider _provider;
        private SnowflakeHelper _snowflake;
        private AiMemDAL _aimemDAL;
        private AiHouseDAL _aiHouseDAL;
        public AiMemBLL(ITAServiceProvider provider, SnowflakeHelper snowflake, AiMemDAL aiMemDAL, AiHouseDAL aiHouseDAL)
        {
            _provider = provider;
            _snowflake = snowflake;
            _aimemDAL = aiMemDAL;
            _aiHouseDAL = aiHouseDAL;
        }
        public virtual async Task<BusResponse<List<MZ_AIHouse>>> HouseList(In_FaceHouseList data, IUserInfo user)
        {
            Expression<Func<MZ_AIHouse, bool>> expression = x => x.OrgId == user.OrgId;
            if (!string.IsNullOrEmpty(data.Key))
            {
                expression = expression.And(x => x.HouseName.Contains(data.Key));
            }
            if (!string.IsNullOrEmpty(data.Status))
            {
                expression = expression.And(x => x.Status == data.Status);
            }
            var tlist = await _aiHouseDAL.SelectList(expression, "CreatedOn desc");
            var tfaceCountList = await _aimemDAL.GetFaceCount(user.OrgId);
            foreach (var tlib in tlist)
            {
                var tmpcc = tfaceCountList.Where(x => x.HouseId == tlib.Id).FirstOrDefault();
                if (tmpcc != null)
                {
                    tlib.FaceCount = tmpcc.TotalFace.Value;
                }
            }
            return BusResponse<List<MZ_AIHouse>>.Success(tlist);
        }
        public virtual async Task<BusResponse<MZ_AIHouse>> Info(string id)
        {
            var info = await _aiHouseDAL.Select(id);
            if (info == null)
            {
                return BusResponse<MZ_AIHouse>.Error(111, "数据源不存在");
            }
            return BusResponse<MZ_AIHouse>.Success(info);
        }
        public virtual async Task<BusResponse<int>> InsertHouse(MZ_AIHouse data, IUserInfo user)
        {
            if (user.OrgId <= 0)
            {
                return BusResponse<int>.Error(112, "非企业用户无法新增建模库");
            }
            var snowflake = _provider.GetService<SnowflakeHelper>();
            data.Id = snowflake.NextId().ToString();
            data.OrgId = user.OrgId;
            data.CreatedOn = DateTime.Now;
            return BusResponse<int>.Success(await _aiHouseDAL.Insert(data));
        }
        public virtual async Task<BusResponse<int>> UpdateHouse(MZ_AIHouse data, IUserInfo user)
        {
            if (user.OrgId <= 0)
            {
                return BusResponse<int>.Error(112, "非企业用户无法修改建模库");
            }
            var old = await _aiHouseDAL.Select(data.Id);
            if (old == null)
            {
                return BusResponse<int>.Error(113, "建模库不存在");
            }

            data.OrgId = null;
            return BusResponse<int>.Success(await _aiHouseDAL.Update(data));
        }

        public virtual async Task<BusResponse<int>> DeleteHouse(string[] ids, IUserInfo user)
        {
            if (user.OrgId <= 0)
            {
                return BusResponse<int>.Error(112, "非企业用户无法删除建模库");
            }
            try
            {
                await _aimemDAL.Delete(x => x.OrgId == user.OrgId && ids.Contains(x.HouseId));
                var num = await _aiHouseDAL.Delete(x => x.OrgId == user.OrgId && ids.Contains(x.Id));
                return BusResponse<int>.Success(num);
            }
            catch (Exception ex)
            {
                return BusResponse<int>.Error(111, ex.Message);
            }
        }
        public virtual async Task<BusResponse<PageObject<MZ_AIMem>>> FacePage(In_FaceList data, IUserInfo user)
        {
            var rs = await _aimemDAL.FacePage(data, user);
            return BusResponse<PageObject<MZ_AIMem>>.Success(rs);
        }
        public virtual async Task<BusResponse<int>> Delete(string[] ids, IUserInfo user)
        {
            if (user.OrgId <= 0)
            {
                return BusResponse<int>.Error(112, "非企业用户无法删除建模");
            }
            try
            {
                var tmparr = (await _aimemDAL.SelectList(x => x.OrgId == user.OrgId && ids.Contains(x.Id))).Select(x => x.MilvusId.Value).ToArray();
                int num = await _aimemDAL.Delete(x => x.OrgId == user.OrgId && ids.Contains(x.Id));
                await _provider.GetService<MilvusBLL>().DelFromIdsCollection(tmparr);
                return BusResponse<int>.Success(num);
            }
            catch (Exception ex)
            {
                return BusResponse<int>.Error(111, ex.Message);
            }
        }
        public virtual async Task<BusResponse<int>> Insert(MZ_AIMem data, IUserInfo user)
        {
            if (user.OrgId <= 0)
            {
                return BusResponse<int>.Error(112, "非企业用户无法建模");
            }
            var snowflake = _provider.GetService<SnowflakeHelper>();
            data.Id = snowflake.NextId().ToString();
            data.OrgId = user.OrgId;
            data.CreatedOn = DateTime.Now;
            data.MilvusId = 0;
            data.FStatus = 2;
            #region 建模
            try
            {
                var fileHelper = _provider.GetService<FileHelper>();
                var originalImage = await fileHelper.CreateRgb24FromUrl(data.FaceImg);
                var tbbx = _provider.GetService<FaceDetOnnxRunner>().Predict(originalImage);
                if (tbbx.Count > 0)
                {
                    var faceRecogRunner = _provider.GetService<FaceRecogRunner>();
                    var milBLL = _provider.GetService<MilvusBLL>();
                    var tmpimg = originalImage.CropByBox(tbbx[0].X1, tbbx[0].X2, tbbx[0].Y1, tbbx[0].Y2);
                    var faceSTNRunner = _provider.GetService<FaceSTNRunner>();
                    var tmpstn = faceSTNRunner.Predict(tmpimg);

                    Tensor<float> recogdata = faceRecogRunner.PredictTensor(tmpstn);
                    var res = await milBLL.InsertToMemberCollection(data.MemId.Value, data.HouseId, recogdata.ToArray());
                    if (res.IsSuccess())
                    {
                        data.MilvusId = res.Data;
                        data.FStatus = 1;
                        data.FaceImg = await fileHelper.UploadRgb24File(tmpstn.ToImage());
                    }
                }
            }
            catch { }
            #endregion



            return BusResponse<int>.Success(await _aimemDAL.Insert(data));
        }

    }
}
