using AuthService;
using AuthService.Controller;
using AuthService.Model;
using Common;
using Common.IdGenerator;
using Common.Share;
using Microsoft.Extensions.Logging;
using MonitorService.Business;
using MonitorService.Model;
using MyAccess.Aop;
using MyAccess.DB.Builder.WhereToSql;
using SKIT.FlurlHttpClient.Wechat.Work.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using TemplateAction.Core;
using WeiXinService.DAL;
using WeiXinService.Model;

namespace WeiXinService.Business
{
    public class CorpSyncBLL
    {
        private ITAServiceProvider _serviceProvider;
        private SnowflakeHelper _snowflake;
        private CorpSyncDAL _corpSyncDAL;
        private CorpTaskDAL _corpTaskDAL;
        private DeptDAL _deptDAL;
        private UserDAL _userDAL;
        private OrgDAL _orgDAL;
        private UserCropDAL _userCropDAL;
        private ILogger<CorpSyncBLL> _log;
        public CorpSyncBLL(ITAServiceProvider serviceProvider, SnowflakeHelper snowflake, CorpSyncDAL corpSyncDAL,
            CorpTaskDAL corpTaskDAL, DeptDAL deptDAL, UserDAL userDAL, OrgDAL orgDAL, UserCropDAL userCropDAL, ILoggerFactory logfactory)
        {
            _log = logfactory.CreateLogger<CorpSyncBLL>();
            _serviceProvider = serviceProvider;
            _snowflake = snowflake;
            _corpSyncDAL = corpSyncDAL;
            _corpTaskDAL = corpTaskDAL;
            _deptDAL = deptDAL;
            _userDAL = userDAL;
            _orgDAL = orgDAL;
            _userCropDAL = userCropDAL;
        }
        public virtual async Task TimerExecute(string appId, long jobId)
        {
            var syncData = (await _corpSyncDAL.SelectList(x => x.AppId == appId)).FirstOrDefault();
            if (syncData == null)
            {
                if (jobId > 0)
                {
                    await _serviceProvider.GetService<JobBLL>().DeleteJob(jobId);
                }
                return;
            }
            MZ_CorpTask newTask = new MZ_CorpTask();
            newTask.TaskId = _snowflake.NextId().ToString();
            newTask.AppId = appId;
            newTask.IsUpdateDept = false;
            newTask.IsAddDept = false;
            newTask.IsMoveDept = false;
            newTask.IsDelDept = false;
            newTask.IsUpdateMem = false;
            newTask.IsAddMem = false;
            newTask.IsDelMem = false;
            newTask.UpdateDeptErr = string.Empty;
            newTask.AddDeptErr = string.Empty;
            newTask.MoveDeptErr = string.Empty;
            newTask.DelDeptErr = string.Empty;
            newTask.UpdateMemErr = string.Empty;
            newTask.AddMemErr = string.Empty;
            newTask.DelMemErr = string.Empty;
            newTask.Status = 0;
            newTask.CreatedOn = DateTime.Now;
            await _corpTaskDAL.Insert(newTask);

            await ExecuteSync(newTask, syncData);
        }

        public virtual async Task<BusResponse<string>> StartSync(In_CorpSync data)
        {
            var corpItem = await _corpSyncDAL.Select(data.appid);
            bool isAdd = false;
            if (corpItem == null)
            {
                corpItem = new MZ_CorpSync();
                corpItem.AppId = data.appid;
                corpItem.DeptDict = string.Empty;
                corpItem.MemDict = string.Empty;
                isAdd = true;
            }
            else
            {
                if (corpItem.Status == "0")
                {
                    return BusResponse<string>.Error(111, "同步任务已启用");
                }
                corpItem = new MZ_CorpSync();
                corpItem.AppId = data.appid;
            }
            corpItem.Speed = data.speed;
            corpItem.UserName = data.username;
            corpItem.Status = "0";
            corpItem.UpdatedOn = DateTime.Now;

            MZ_Job job = new MZ_Job();
            job.concurrent = "0";
            job.createId = 0;
            job.create_time = DateTime.Now;
            job.updateId = 0;
            job.update_time = DateTime.Now;
            if (corpItem.Speed == 1)
            {
                job.cron_expression = "0 0/5 * * * ?";
            }
            else
            {
                job.cron_expression = "0 0 0/1 * * ? ";
            }
            job.invoke_target = typeof(CorpSyncBLL).FullName + ".TimerExecute('" + data.appid + "',$id)";
            job.job_group = "DEFAULT";
            job.job_name = "CorpSyncTimer-" + corpItem.AppId;
            job.misfire_policy = "0";
            job.status = "0";
            var rs = await _serviceProvider.GetService<JobBLL>().InsertJob(job);
            if (!rs.IsSuccess())
            {
                return BusResponse<string>.Error(rs.Code, rs.Message);
            }
            corpItem.JobId = rs.Data;
            if (isAdd)
            {
                await _corpSyncDAL.Insert(corpItem);
            }
            else
            {
                await _corpSyncDAL.Update(corpItem);
            }

            return BusResponse<string>.Success();
        }
        public virtual async Task<BusResponse<string>> StopSync(string appid)
        {
            var corpItem = await _corpSyncDAL.Select(appid);
            if (corpItem == null)
            {
                return BusResponse<string>.Error(111, "同步任务不存在，无法关闭");
            }

            await _serviceProvider.GetService<JobBLL>().DeleteJob(corpItem.JobId.Value);

            MZ_CorpSync newsync = new MZ_CorpSync();
            newsync.AppId = corpItem.AppId;
            newsync.Status = "1";
            newsync.UpdatedOn = DateTime.Now;
            newsync.JobId = 0;
            await _corpSyncDAL.Update(newsync);
            return BusResponse<string>.Success();
        }
        public virtual async Task<string> CreateTask(MZ_CorpSync data)
        {
            MZ_CorpTask task = new MZ_CorpTask();
            task.TaskId = _snowflake.NextId().ToString();
            task.AppId = data.AppId;
            task.IsUpdateDept = false;
            task.IsAddDept = false;
            task.IsMoveDept = false;
            task.IsDelDept = false;
            task.IsUpdateMem = false;
            task.IsAddMem = false;
            task.IsDelMem = false;
            task.UpdateDeptErr = string.Empty;
            task.AddDeptErr = string.Empty;
            task.MoveDeptErr = string.Empty;
            task.DelDeptErr = string.Empty;
            task.UpdateMemErr = string.Empty;
            task.AddMemErr = string.Empty;
            task.DelMemErr = string.Empty;
            task.Status = 0;
            task.CreatedOn = DateTime.Now;
            await _corpTaskDAL.Insert(task);

            _serviceProvider.GetService<CorpWxSyncThread>().Push(new CorpSyncItem()
            {
                data = data,
                task = task
            });

            return task.TaskId;
        }
        public virtual async Task<MZ_CorpSync> Info(string appid, string username = null)
        {
            var syncdata = await _corpSyncDAL.Select(appid);
            if (syncdata == null)
            {
                syncdata = new MZ_CorpSync();
                syncdata.AppId = appid;
                syncdata.Status = "1";
                syncdata.DeptDict = string.Empty;
                syncdata.MemDict = string.Empty;
                syncdata.JobId = 0;
                syncdata.Speed = 2;
                syncdata.UserName = username;
                syncdata.UpdatedOn = DateTime.Now;
                await _corpSyncDAL.Insert(syncdata);
            }
            else
            {

                if (username != null)
                {
                    syncdata.UserName = username;
                    MZ_CorpSync newsync = new MZ_CorpSync();
                    newsync.AppId = appid;
                    newsync.UserName = username;
                    await _corpSyncDAL.Update(newsync);
                }
            }
            return syncdata;
        }
        public virtual async Task<MZ_CorpTask> TaskInfo(string id)
        {
            return await _corpTaskDAL.Select(id);
        }
        public virtual async Task<PageObject<MZ_CorpTask>> TaskListPage(In_TaskListPage query)
        {
            Expression<Func<MZ_CorpTask, bool>> expression = x => true;
            if (query.beginTime != null)
            {
                expression = expression.And(x => x.CreatedOn >= query.beginTime);
            }
            if (query.endTime != null)
            {
                expression = expression.And(x => x.CreatedOn <= query.endTime);
            }
            return await _corpTaskDAL.SelectPage(expression, query, "CreatedOn desc");
        }
        public virtual async Task ExecuteSync(MZ_CorpTask task, MZ_CorpSync data)
        {
            try
            {


                Dictionary<long, long> deptDict;
                if (string.IsNullOrEmpty(data.DeptDict))
                {
                    deptDict = new Dictionary<long, long>();
                }
                else
                {
                    deptDict = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<long, long>>(data.DeptDict);
                }
                Dictionary<string, long> userDict;
                if (string.IsNullOrEmpty(data.MemDict))
                {
                    userDict = new Dictionary<string, long>();
                }
                else
                {
                    userDict = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, long>>(data.MemDict);
                }
                var apiHelper = _serviceProvider.GetService<WxApiHelper>();
                var accountInfo = await apiHelper.AccountInfo(data.AppId);
                var deptRspt = await apiHelper.GetDepartmentList(data.AppId);
                var rootDept = await _deptDAL.SelectRoot(accountInfo.OrgId);
                if (rootDept == null)
                {
                    throw new Exception("企业部门异常");
                }
                var oldDeptList = await _deptDAL.SelectChildrenByRoot(rootDept);
                if (deptRspt.IsSuccess())
                {
                    //完成更新部门
                    await FinishUpdateDept(task, deptDict, deptRspt.Data, oldDeptList);

                    //完成新增部门
                    await FinishAddDept(task, deptDict, deptRspt.Data, data, rootDept);

                    //完成移动部门
                    await FinishMoveDept(task, deptDict, deptRspt.Data, oldDeptList);

                    //完成删除部门
                    await FinishDelDept(task, deptDict, deptRspt.Data, oldDeptList);
                }
                else
                {
                    throw new Exception(deptRspt.Message);
                }
                List<CgibinUserListResponse.Types.User> userlist = new List<CgibinUserListResponse.Types.User>();
                foreach (var deptItem in deptRspt.Data)
                {
                    var tmpusersRsps = await apiHelper.GetDepartmentMembers(data.AppId, deptItem.DepartmentId);
                    var tmpusers = tmpusersRsps.Data.Where(x => x.Status == 1 || x.Status == 4);
                    userlist.AddRange(tmpusers);
                }

                userlist = userlist.DistinctBy(x => x.UserId).ToList();
                var dbMemList = await _userDAL.GetMembersByOrgId(accountInfo.OrgId);
                var manuserIds = await _userDAL.SelectManUserIds(accountInfo.OrgId);
                HashSet<long> hsManUids = new HashSet<long>();
                foreach (var uid in manuserIds)
                {
                    hsManUids.Add(uid);
                }
                //完成更新人员
                await FinishUpdateMember(task, userDict, deptDict, userlist, dbMemList, accountInfo, data);

                //完成新增人员
                await FinishAddMember(task, userDict, deptDict, userlist, accountInfo, data);

                //完成删除人员
                await FinishDelMember(task, userDict, userlist, dbMemList, accountInfo, hsManUids);

            }
            catch (Exception ex)
            {
                _log.LogError(ex, ex.Message + ex.StackTrace);
            }
            MZ_CorpTask newtask = new MZ_CorpTask();
            newtask.TaskId = task.TaskId;
            newtask.Status = 1;
            await _corpTaskDAL.Update(newtask);
        }
        private async Task FinishUpdateDept(MZ_CorpTask task, Dictionary<long, long> deptDict, CgibinDepartmentListResponse.Types.Department[] departments, List<MZ_Dept> oldDeptList)
        {
            try
            {
                using (BLLTranScope scope = new BLLTranScope())
                {
                    foreach (var deptkvp in deptDict)
                    {
                        var newdpt = departments.Where(x => x.DepartmentId == deptkvp.Key).FirstOrDefault();
                        if (newdpt != null)
                        {
                            var olddept = oldDeptList.Where(x => x.dept_id == deptkvp.Value).FirstOrDefault();
                            int newsort = Convert.ToInt32(newdpt.DepartmentOrder > 999999999 ? (999999999 - newdpt.DepartmentOrder % 999999999) : (999999999 - newdpt.DepartmentOrder));
                            if (olddept != null && (olddept.dept_name != newdpt.Name || olddept.order_num != newsort))
                            {
                                MZ_Dept dpt = new MZ_Dept();
                                dpt.dept_id = deptkvp.Value;
                                dpt.dept_name = newdpt.Name;
                                dpt.order_num = newsort;
                                await _deptDAL.UpdateDept(dpt);
                            }
                        }
                    }
                    MZ_CorpTask newtask = new MZ_CorpTask();
                    newtask.TaskId = task.TaskId;
                    newtask.IsUpdateDept = true;
                    await _corpTaskDAL.Update(newtask);
                    // 完成
                    await scope.CompleteAsync();
                }
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message + ex.StackTrace);
                MZ_CorpTask newtask = new MZ_CorpTask();
                newtask.TaskId = task.TaskId;
                newtask.UpdateDeptErr = ex.Message;
                await _corpTaskDAL.Update(newtask);
            }

        }
        private async Task<long> FindIterDeptId(CgibinDepartmentListResponse.Types.Department newdpt, CgibinDepartmentListResponse.Types.Department[] departments, Dictionary<long, long> deptDict, MZ_Dept root)
        {
            if (deptDict.TryGetValue(newdpt.DepartmentId, out var deptId))
            {
                return deptId;
            }
            else
            {
                var addDept = new MZ_Dept();
                addDept.dept_id = _snowflake.NextId();
                addDept.createId = 2;
                addDept.updateId = 2;
                addDept.update_time = addDept.create_time = DateTime.Now;
                MZ_Dept parentDept;
                if (newdpt.ParentDepartmentId == 1)
                {
                    addDept.parent_id = root.dept_id;
                    parentDept = root;
                }
                else
                {
                    var tmpdeee = departments.Where(x => x.DepartmentId == newdpt.ParentDepartmentId).FirstOrDefault();
                    if (tmpdeee == null)
                    {
                        addDept.parent_id = root.dept_id;
                        parentDept = root;
                    }
                    else
                    {
                        addDept.parent_id = await FindIterDeptId(tmpdeee, departments, deptDict, root);
                        parentDept = await _deptDAL.SelectById(addDept.parent_id.Value);
                    }

                }
                addDept.status = "0";
                addDept.del_flag = "0";
                addDept.email ??= string.Empty;
                addDept.phone ??= string.Empty;
                addDept.OrgId = root.OrgId;
                addDept.ancestors = parentDept.ancestors + addDept.dept_id + ",";
                addDept.dept_name = newdpt.Name;
                addDept.order_num = Convert.ToInt32(newdpt.DepartmentOrder > 999999999 ? (999999999 - newdpt.DepartmentOrder % 999999999) : (999999999 - newdpt.DepartmentOrder));
                await _deptDAL.InsertDept(addDept);
                deptDict.Add(newdpt.DepartmentId, addDept.dept_id.Value);
                return addDept.dept_id.Value;
            }
        }
        private async Task FinishAddDept(MZ_CorpTask task, Dictionary<long, long> deptDict, CgibinDepartmentListResponse.Types.Department[] departments, MZ_CorpSync data, MZ_Dept rootDept)
        {
            try
            {
                using (BLLTranScope scope = new BLLTranScope())
                {
                    foreach (var newdpt in departments)
                    {
                        await FindIterDeptId(newdpt, departments, deptDict, rootDept);
                    }

                    MZ_CorpTask newtask = new MZ_CorpTask();
                    newtask.TaskId = task.TaskId;
                    newtask.IsAddDept = true;
                    await _corpTaskDAL.Update(newtask);

                    MZ_CorpSync newSync = new MZ_CorpSync();
                    newSync.AppId = task.AppId;
                    newSync.DeptDict = Newtonsoft.Json.JsonConvert.SerializeObject(deptDict);
                    newSync.UpdatedOn = DateTime.Now;
                    await _corpSyncDAL.Update(newSync);
                    // 完成
                    await scope.CompleteAsync();
                }
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message + ex.StackTrace);
                MZ_CorpTask newtask = new MZ_CorpTask();
                newtask.TaskId = task.TaskId;
                newtask.AddDeptErr = ex.Message;
                await _corpTaskDAL.Update(newtask);
                throw;
            }
        }
        private async Task FinishMoveDept(MZ_CorpTask task, Dictionary<long, long> deptDict, CgibinDepartmentListResponse.Types.Department[] departments, List<MZ_Dept> oldDeptList)
        {
            try
            {
                using (BLLTranScope scope = new BLLTranScope())
                {

                    foreach (var deptkvp in deptDict)
                    {
                        var newdpt = departments.Where(x => x.DepartmentId == deptkvp.Key).FirstOrDefault();
                        if (newdpt != null)
                        {
                            var olddept = oldDeptList.Where(x => x.dept_id == deptkvp.Value).FirstOrDefault();
                            if (olddept != null)
                            {
                                if (deptDict.TryGetValue(newdpt.ParentDepartmentId, out long newval))
                                {
                                    if (olddept.parent_id != newval)
                                    {
                                        MZ_Dept dpt = new MZ_Dept();
                                        dpt.dept_id = deptkvp.Value;
                                        dpt.parent_id = newval;
                                        await _deptDAL.UpdateDept(dpt);
                                    }
                                }
                            }

                        }
                    }

                    MZ_CorpTask newtask = new MZ_CorpTask();
                    newtask.TaskId = task.TaskId;
                    newtask.IsMoveDept = true;
                    await _corpTaskDAL.Update(newtask);

                    // 完成
                    await scope.CompleteAsync();
                }
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message + ex.StackTrace);
                MZ_CorpTask newtask = new MZ_CorpTask();
                newtask.TaskId = task.TaskId;
                newtask.MoveDeptErr = ex.Message;
                await _corpTaskDAL.Update(newtask);
                throw;
            }
        }
        private async Task FinishDelDept(MZ_CorpTask task, Dictionary<long, long> deptDict, CgibinDepartmentListResponse.Types.Department[] departments, List<MZ_Dept> oldDeptList)
        {
            try
            {
                using (BLLTranScope scope = new BLLTranScope())
                {

                    foreach (var iteDept in oldDeptList)
                    {
                        var tmpkvpList = deptDict.Where(x => x.Value == iteDept.dept_id).ToList();
                        if (tmpkvpList.Count == 0)
                        {
                            await _deptDAL.DeleteDeptById(iteDept.dept_id.Value);
                            continue;
                        }
                        var newdpt = departments.Where(x => x.DepartmentId == tmpkvpList[0].Key).FirstOrDefault();
                        if (newdpt == null)
                        {
                            deptDict.Remove(tmpkvpList[0].Key);
                            await _deptDAL.DeleteDeptById(tmpkvpList[0].Value);
                            continue;
                        }
                    }

                    MZ_CorpTask newtask = new MZ_CorpTask();
                    newtask.TaskId = task.TaskId;
                    newtask.IsDelDept = true;
                    await _corpTaskDAL.Update(newtask);


                    MZ_CorpSync newSync = new MZ_CorpSync();
                    newSync.AppId = task.AppId;
                    newSync.DeptDict = Newtonsoft.Json.JsonConvert.SerializeObject(deptDict);
                    newSync.UpdatedOn = DateTime.Now;
                    await _corpSyncDAL.Update(newSync);

                    // 完成
                    await scope.CompleteAsync();
                }
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message + ex.StackTrace);
                MZ_CorpTask newtask = new MZ_CorpTask();
                newtask.TaskId = task.TaskId;
                newtask.DelDeptErr = ex.Message;
                await _corpTaskDAL.Update(newtask);
                throw;
            }
        }


        private async Task FinishUpdateMember(MZ_CorpTask task, Dictionary<string, long> userDict, Dictionary<long, long> deptDict, List<CgibinUserListResponse.Types.User> memberList, List<MZ_AdminInfo> dbMemList, MZ_WXAccount account, MZ_CorpSync data)
        {
            try
            {
                using (BLLTranScope scope = new BLLTranScope())
                {
                    foreach (var userItem in userDict)
                    {
                        var newMem = memberList.Where(x => x.UserId == userItem.Key).FirstOrDefault();
                        if (newMem != null)
                        {
                            var userInfo = dbMemList.Where(x => x.Id == userItem.Value && x.IsPrimary == true).FirstOrDefault();
                            if (userInfo == null)
                            {
                                continue;
                            }
                            int rawgender = 0;
                            if (userInfo.Sex == "0")
                            {
                                rawgender = 1;
                            }
                            else if (userInfo.Sex == "1")
                            {
                                rawgender = 2;
                            }
                            bool isChange = false;
                            bool isChangePost = false;
                            MZ_AdminInfo newuser = new MZ_AdminInfo();
                            newuser.Id = userItem.Value;
                            if (userInfo.RealName != newMem.Name || rawgender != newMem.Gender)
                            {
                                newuser.RealName = newMem.Name;
                                if (newMem.Gender == 1)
                                {
                                    newuser.Sex = "0";
                                }
                                else if (newMem.Gender == 2)
                                {
                                    newuser.Sex = "1";
                                }
                                else
                                {
                                    newuser.Sex = "2";
                                }
                                isChange = true;
                            }
                            if (!string.IsNullOrEmpty(data.UserName) && newMem.ExtendedAttribute != null)
                            {
                                var attrfield = newMem.ExtendedAttribute.AttributeList.Where(x => x.Name == data.UserName && x.Type == 0).FirstOrDefault();
                                if (attrfield != null && attrfield.Text != null && !string.IsNullOrEmpty(attrfield.Text.Value))
                                {
                                    if (userInfo.UserName != attrfield.Text.Value)
                                    {
                                        newuser.UserName = attrfield.Text.Value;
                                        isChange = true;
                                    }
                                }
                            }
                            if (!string.IsNullOrEmpty(newMem.MobileNumber) && userInfo.Mobile != newMem.MobileNumber)
                            {
                                if (await _userDAL.CheckPhoneUnique(newMem.MobileNumber) != null)
                                {
                                    throw new Exception($"手机号{newMem.MobileNumber}已存在");
                                }
                                newuser.Mobile = newMem.MobileNumber;
                                isChange = true;
                            }
                            if (!string.IsNullOrEmpty(newMem.Email) && userInfo.Email != newMem.Email)
                            {
                                newuser.Email = newMem.Email;
                                newuser.EmailActive = false;
                                isChange = true;
                            }
                            if (!string.IsNullOrEmpty(newMem.Position) && userInfo.post_name != newMem.Position)
                            {
                                isChangePost = true;
                            }
                            long mainDeptId = 0;
                            if (newMem.MainDepartmentId != null && deptDict.TryGetValue(newMem.MainDepartmentId.Value, out mainDeptId))
                            {
                                if (userInfo.dept_id != mainDeptId)
                                {
                                    isChangePost = true;
                                }
                            }
                            if (isChange)
                            {
                                await _userDAL.Update(newuser);
                            }

                            //更新主部门信息
                            if (isChangePost)
                            {
                                MZ_User_Org newuserorg = new MZ_User_Org();
                                newuserorg.UserId = userInfo.Id;
                                newuserorg.OrgId = account.OrgId;
                                newuserorg.post_name = newMem.Position;
                                if (mainDeptId > 0)
                                {
                                    newuserorg.dept_id = mainDeptId;
                                }
                                await _orgDAL.DeleteUserOrgByDept(userInfo.Id.Value, mainDeptId);
                                await _orgDAL.UpdateUserOrg(newuserorg);
                            }
                            //更新分身部门信息
                            var userOtherDeptIds = dbMemList.Where(x => x.Id == userItem.Value && x.IsPrimary == false).Select(x => x.dept_id.Value).ToArray();
                            List<long> newDeptIds = new List<long>();
                            bool canUpdateOther = false;
                            foreach (var newdptid in newMem.DepartmentIdList)
                            {
                                if (newdptid != newMem.MainDepartmentId)
                                {
                                    if (deptDict.TryGetValue(newdptid, out var nnid))
                                    {
                                        if (!userOtherDeptIds.Contains(nnid))
                                        {
                                            canUpdateOther = true;
                                            break;
                                        }
                                        newDeptIds.Add(nnid);
                                    }
                                }
                            }

                            if (newDeptIds.Count != userOtherDeptIds.Length)
                            {
                                canUpdateOther = true;
                            }

                            if (canUpdateOther)
                            {
                                await _orgDAL.DeleteAllOtherUserOrgByOrg(userItem.Value, account.OrgId);
                                int idx = 0;
                                foreach (var titem in newDeptIds)
                                {
                                    MZ_User_Org newuserorg = new MZ_User_Org();
                                    newuserorg.UserId = userItem.Value;
                                    newuserorg.OrgId = account.OrgId;
                                    newuserorg.dept_id = titem;
                                    newuserorg.post_name = string.Empty;
                                    if (newMem.DepartmentLeaderStatusList.Length > idx)
                                    {
                                        newuserorg.IsLeader = newMem.DepartmentLeaderStatusList[idx] == 1 ? true : false;
                                    }
                                    else
                                    {
                                        newuserorg.IsLeader = false;
                                    }
                                    newuserorg.IsPrimary = false;
                                    if (!await _orgDAL.CheckExistDept(newuserorg.UserId.Value, newuserorg.dept_id.Value))
                                    {
                                        await _orgDAL.InsertUserOrg(newuserorg);
                                    }
                                    ++idx;
                                }
                            }
                        }
                    }

                    MZ_CorpTask newtask = new MZ_CorpTask();
                    newtask.TaskId = task.TaskId;
                    newtask.IsUpdateMem = true;
                    await _corpTaskDAL.Update(newtask);

                    // 完成
                    await scope.CompleteAsync();
                }
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message + ex.StackTrace);
                MZ_CorpTask newtask = new MZ_CorpTask();
                newtask.TaskId = task.TaskId;
                newtask.UpdateMemErr = ex.Message;
                await _corpTaskDAL.Update(newtask);
                throw;
            }
        }


        private async Task FinishAddMember(MZ_CorpTask task, Dictionary<string, long> userDict, Dictionary<long, long> deptDict, List<CgibinUserListResponse.Types.User> memberList, MZ_WXAccount account, MZ_CorpSync data)
        {
            var configBLL = _serviceProvider.GetService<ConfigBLL>();
            var roleVal = long.Parse(await configBLL.SelectConfigByKey("sys.reg.roleId"));

            var fileHelper = _serviceProvider.GetService<Common.FileHelper>();
            try
            {
                using (BLLTranScope scope = new BLLTranScope())
                {
                    foreach (var newuser in memberList)
                    {
                        if (!userDict.ContainsKey(newuser.UserId))
                        {
                            var memInfo = await _userDAL.GetAdminByMobile(newuser.MobileNumber);
                            if (memInfo == null)
                            {
                                memInfo = new MZ_AdminInfo();
                                if (string.IsNullOrEmpty(newuser.AvatarUrl))
                                {
                                    memInfo.Avatar = string.Empty;
                                }
                                else
                                {
                                    memInfo.Avatar = await fileHelper.DownFileToLocal(newuser.AvatarUrl);
                                }
                                if (!string.IsNullOrEmpty(data.UserName) && newuser.ExtendedAttribute != null)
                                {
                                    var attrfield = newuser.ExtendedAttribute.AttributeList.Where(x => x.Name == data.UserName && x.Type == 0).FirstOrDefault();
                                    if (attrfield != null && attrfield.Text != null && !string.IsNullOrEmpty(attrfield.Text.Value))
                                    {
                                        if (memInfo.UserName != attrfield.Text.Value)
                                        {
                                            memInfo.UserName = attrfield.Text.Value;
                                        }
                                    }
                                }
                                if (string.IsNullOrEmpty(newuser.MobileNumber))
                                {
                                    if (StringHelper.IsMobile(newuser.UserId))
                                    {
                                        memInfo.Mobile = newuser.UserId;
                                    }
                                    else
                                    {
                                        memInfo.Mobile = string.Empty;
                                    }
                                }
                                else
                                {
                                    memInfo.Mobile = newuser.MobileNumber;
                                }

                                memInfo.Email = string.IsNullOrEmpty(newuser.Email) ? "" : newuser.Email;
                                memInfo.EmailActive = false;
                                if (string.IsNullOrEmpty(newuser.Address))
                                {
                                    memInfo.Introduction = string.Empty;
                                }
                                else
                                {
                                    memInfo.Introduction = newuser.Address;
                                }
                                memInfo.RealName = newuser.Name;
                                if (newuser.Gender == 1)
                                {
                                    memInfo.Sex = "0";
                                }
                                else if (newuser.Gender == 2)
                                {
                                    memInfo.Sex = "1";
                                }
                                else
                                {
                                    memInfo.Sex = "2";
                                }
                                memInfo.OrgId = account.OrgId;
                                memInfo.Salt = MyAccess.Core.StringTool.GetEnglishChar(16);
                                string tmppassword = MyAccess.Core.StringTool.GetEnglishChar(12);
                                memInfo.Password = MyAccess.Core.Crypter.MD5(string.Concat(tmppassword, memInfo.Salt));
                                memInfo.del_flag = "0";
                                memInfo.status = "0";
                                memInfo.Id = _snowflake.NextId();
                                memInfo.UserName = "$" + memInfo.Id;
                                memInfo.Signature ??= string.Empty;
                                memInfo.WaitSignature ??= string.Empty;
                                memInfo.createId = 2;
                                memInfo.create_time = DateTime.Now;
                                memInfo.updateId = 2;
                                memInfo.update_time = memInfo.create_time;
                                await _userDAL.Insert(memInfo);

                                MZ_UserRole ur = new MZ_UserRole();
                                ur.UserId = memInfo.Id;
                                ur.RoleID = roleVal;
                                ur.OrgId = 0;
                                await _userDAL.AddUserRoleItem(ur);
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(newuser.Email))
                                {
                                    memInfo.Email = newuser.Email;
                                    memInfo.EmailActive = false;
                                }
                                memInfo.RealName = newuser.Name;
                                if (newuser.Gender == 1)
                                {
                                    memInfo.Sex = "0";
                                }
                                else if (newuser.Gender == 2)
                                {
                                    memInfo.Sex = "1";
                                }
                                else
                                {
                                    memInfo.Sex = "2";
                                }
                                await _userDAL.Update(memInfo);
                                await _orgDAL.DeleteUserOrg(memInfo.Id.Value, account.OrgId);
                            }


                            //添加企业微信的登录openid
                            if (!await _userCropDAL.Some(x => x.Id == memInfo.Id && x.AppId == account.AppId))
                            {
                                MZ_AdminCrop adminCrop = new MZ_AdminCrop();
                                adminCrop.Id = memInfo.Id;
                                adminCrop.AppId = account.AppId;
                                adminCrop.CropUserId = newuser.UserId;
                                adminCrop.UpdatedOn = DateTime.Now;
                                await _userCropDAL.Insert(adminCrop);
                            }

                            //添加用户部门信息
                            List<long> newDeptIds = new List<long>();
                            foreach (var newdptid in newuser.DepartmentIdList)
                            {
                                if (deptDict.TryGetValue(newdptid, out var nnid))
                                {
                                    newDeptIds.Add(nnid);
                                }
                            }
                            int idx = 0;
                            long mainDeptid = 0;
                            if (newuser.MainDepartmentId != null)
                            {
                                deptDict.TryGetValue(newuser.MainDepartmentId.Value, out mainDeptid);
                            }
                            else
                            {
                                if (newDeptIds.Count > 0)
                                {
                                    mainDeptid = newDeptIds[0];
                                }
                            }


                            foreach (var deptid in newDeptIds)
                            {
                                MZ_User_Org newuserorg = new MZ_User_Org();
                                newuserorg.UserId = memInfo.Id;
                                newuserorg.OrgId = account.OrgId;
                                newuserorg.dept_id = deptid;

                                if (newuser.DepartmentLeaderStatusList.Length > idx)
                                {
                                    newuserorg.IsLeader = newuser.DepartmentLeaderStatusList[idx] == 1 ? true : false;
                                }
                                else
                                {
                                    newuserorg.IsLeader = false;
                                }

                                newuserorg.IsPrimary = deptid == mainDeptid ? true : false;
                                if (newuserorg.IsPrimary == true)
                                {
                                    newuserorg.post_name = newuser.Position;
                                }
                                else
                                {
                                    newuserorg.post_name = string.Empty;
                                }
                                if (!await _orgDAL.CheckExistDept(newuserorg.UserId.Value, newuserorg.dept_id.Value))
                                {
                                    await _orgDAL.InsertUserOrg(newuserorg);
                                }
                            }

                            userDict.Add(newuser.UserId, memInfo.Id.Value);
                        }
                    }

                    MZ_CorpTask newtask = new MZ_CorpTask();
                    newtask.TaskId = task.TaskId;
                    newtask.IsAddMem = true;
                    await _corpTaskDAL.Update(newtask);


                    MZ_CorpSync newSync = new MZ_CorpSync();
                    newSync.AppId = task.AppId;
                    newSync.MemDict = Newtonsoft.Json.JsonConvert.SerializeObject(userDict);
                    newSync.UpdatedOn = DateTime.Now;
                    await _corpSyncDAL.Update(newSync);

                    // 完成
                    await scope.CompleteAsync();
                }
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message + ex.StackTrace);
                MZ_CorpTask newtask = new MZ_CorpTask();
                newtask.TaskId = task.TaskId;
                newtask.AddMemErr = ex.Message;
                await _corpTaskDAL.Update(newtask);
                throw;
            }
        }

        private async Task FinishDelMember(MZ_CorpTask task, Dictionary<string, long> userDict, List<CgibinUserListResponse.Types.User> memberList, List<MZ_AdminInfo> dbMemList, MZ_WXAccount account, HashSet<long> mans)
        {
            try
            {
                using (BLLTranScope scope = new BLLTranScope())
                {
                    foreach (var meminfo in dbMemList)
                    {
                        //禁止删除管理人员
                        if (mans.Contains(meminfo.Id.Value))
                        {
                            continue;
                        }
                        var tmpkvpList = userDict.Where(x => x.Value == meminfo.Id).ToList();
                        if (tmpkvpList.Count == 0)
                        {
                            await _orgDAL.DeleteUserOrg(meminfo.Id.Value, account.OrgId);
                            continue;
                        }
                        var newusr = memberList.Where(x => x.UserId == tmpkvpList[0].Key).FirstOrDefault();
                        if (newusr == null)
                        {
                            userDict.Remove(tmpkvpList[0].Key);
                            await _orgDAL.DeleteUserOrg(tmpkvpList[0].Value, account.OrgId);
                            continue;
                        }

                    }

                    MZ_CorpTask newtask = new MZ_CorpTask();
                    newtask.TaskId = task.TaskId;
                    newtask.IsDelMem = true;
                    await _corpTaskDAL.Update(newtask);


                    MZ_CorpSync newSync = new MZ_CorpSync();
                    newSync.AppId = task.AppId;
                    newSync.MemDict = Newtonsoft.Json.JsonConvert.SerializeObject(userDict);
                    newSync.UpdatedOn = DateTime.Now;
                    await _corpSyncDAL.Update(newSync);

                    // 完成
                    await scope.CompleteAsync();
                }
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message + ex.StackTrace);
                MZ_CorpTask newtask = new MZ_CorpTask();
                newtask.TaskId = task.TaskId;
                newtask.DelMemErr = ex.Message;
                await _corpTaskDAL.Update(newtask);
                throw;
            }
        }
    }
}
