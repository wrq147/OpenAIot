using AuthService;
using FlowService.FlowNode.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
namespace FlowService.FlowNode.Conditions
{
    public class UserCondition : BaseCondition
    {
        public ObjData[] value { get; set; }


        /// <summary>
        /// 判断用户是否是指定字符串里的
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userlist"></param>
        /// <returns></returns>
        private bool UserInUser(long userId, string userlist)
        {
            string[] strs = userlist.Split(',', StringSplitOptions.RemoveEmptyEntries);
            return strs.Contains(userId.ToString());
        }


        /// <summary>
        /// 判断用户是否是指定部门里的
        /// </summary>
        /// <param name="context"></param>
        /// <param name="userId"></param>
        /// <param name="deptlist"></param>
        /// <returns></returns>
        private bool UserInDept(BuilderContext context, long userId, string deptlist)
        {
            string[] strs = deptlist.Split(',', StringSplitOptions.RemoveEmptyEntries);
            if (context.UserDeptList == null) return false;
            var dept = context.UserDeptList.Where(x => x.Id == userId).FirstOrDefault();
            if (dept == null) return false;
            string[] desss = dept.ancestors.Split(',', StringSplitOptions.RemoveEmptyEntries); ;
            foreach (string s in strs)
            {
                if (desss.Contains(s))
                {
                    return true;
                }
            }
            return false;
        }
        public override string ToExpressionString(BuilderContext context)
        {
            if (this.id == "root")
            {
                this.id = "$initiator";
            }
            var targetUser = context.GetFormObject(this.id);
            var targetUserLong = targetUser as long?;
            if (targetUserLong == null)
            {
                var selectedlist = targetUser as IEnumerable<object>;
                if (selectedlist == null)
                {
                    return "false";
                }
                var dictobj = selectedlist.First() as IDictionary<string, object>;
                var jk = dictobj["id"];
                if (jk == null)
                {
                    return "false";
                }
                targetUserLong = Convert.ToInt64(jk);
            }

            var ulist = value.Where(x => x.type == "user").Select(x => x.id).ToList();
            var dlist = value.Where(x => x.type == "dept").Select(x => x.id).ToList();

            if (ulist.Count > 0 && dlist.Count > 0)
            {
                return (UserInUser(targetUserLong.Value, string.Join(',', ulist)) || UserInDept(context, targetUserLong.Value, string.Join(',', dlist))) ? "true" : "false";
            }
            else if (ulist.Count > 0)
            {
                return UserInUser(targetUserLong.Value, string.Join(',', ulist)) ? "true" : "false";
            }
            else if (dlist.Count > 0)
            {
                return UserInUser(targetUserLong.Value, string.Join(',', dlist)) ? "true" : "false";
            }
            return "false";
        }
    }
}
