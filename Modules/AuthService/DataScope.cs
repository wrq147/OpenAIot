using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AuthService
{
    /// <summary>
    /// 数据权限类
    /// </summary>
    public class DataScope
    {
        /// <summary>
        /// 部门过滤
        /// </summary>
        public List<long> DeptList { get; set; }
        /// <summary>
        /// 用户过滤
        /// </summary>
        public List<long> UserList { get; set; }

        /// <summary>
        /// 多个数据权限组合成一个
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static DataScope Contact(List<DataScope> list)
        {
            HashSet<long> finalDeptList = new HashSet<long>();
            HashSet<long> finalUserList = new HashSet<long>();
            foreach (DataScope ds in list)
            {
                if (ds.DeptList == null && ds.UserList == null)
                {
                    return ds;
                }
                else
                {
                    if (ds.DeptList != null)
                    {
                        foreach (long deptid in ds.DeptList)
                        {
                            finalDeptList.Add(deptid);
                        }
                    }

                    if (ds.UserList != null)
                    {
                        foreach (long uid in ds.UserList)
                        {
                            finalUserList.Add(uid);
                        }
                    }

                }
            }
            DataScope newds = new DataScope();
            if (finalDeptList.Count > 0)
            {
                newds.DeptList = finalDeptList.ToList();
            }
            if (finalUserList.Count > 0)
            {
                newds.UserList = finalUserList.ToList();
            }
            return newds;
        }

        /// <summary>
        /// 生成过滤条件
        /// </summary>
        /// <param name="deptAlias"></param>
        /// <param name="userAlias"></param>
        /// <param name="initOr"></param>
        /// <param name="deptAliasIsMulti">deptAlias对应的字段是否是全文索引</param>
        /// <param name="userAliasIsMulti">userAlias对应的字段是否是全文索引</param>
        /// <param name="allAnd"></param>
        /// <returns></returns>
        public string GenerateFilter(string deptAlias, string userAlias, string initOr = "", bool deptAliasIsMulti = false, bool userAliasIsMulti = false, bool allAnd = true)
        {
            string datafilter = initOr;
            //初始化数据权限

            if (UserList == null && DeptList == null)
            {
                if (!string.IsNullOrEmpty(datafilter))
                {
                    //有全部权限
                    if (allAnd)
                    {
                        return " AND 1=1";
                    }
                    else
                    {
                        return "1=1";
                    }
                }
            }

            if (UserList != null && UserList.Count > 0)
            {
                if (userAliasIsMulti)
                {
                    if (string.IsNullOrEmpty(datafilter))
                    {
                        datafilter = " match(" + userAlias + ") against('" + string.Join(' ', UserList) + "')";
                    }
                    else
                    {
                        datafilter = datafilter + " OR match(" + userAlias + ") against('" + string.Join(' ', UserList) + "')";
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(datafilter))
                    {
                        datafilter = userAlias + " in (" + string.Join(',', UserList) + ")";
                    }
                    else
                    {
                        datafilter = datafilter + " OR " + userAlias + " in (" + string.Join(',', UserList) + ")";
                    }
                }
            }
            if (DeptList != null && DeptList.Count > 0)
            {
                if (deptAliasIsMulti)
                {
                    if (string.IsNullOrEmpty(datafilter))
                    {
                        datafilter = " match(" + deptAlias + ") against('" + string.Join(' ', DeptList) + "')";
                    }
                    else
                    {
                        datafilter = datafilter + " OR match(" + deptAlias + ") against('" + string.Join(' ', DeptList) + "')";
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(datafilter))
                    {
                        datafilter = deptAlias + " in (" + string.Join(',', DeptList) + ")";
                    }
                    else
                    {
                        datafilter = datafilter + " OR " + deptAlias + " in (" + string.Join(',', DeptList) + ")";
                    }
                }
            }

            if (allAnd)
            {
                if (!string.IsNullOrEmpty(datafilter))
                {
                    datafilter = " AND (" + datafilter + ")";
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(datafilter))
                {
                    datafilter = "(" + datafilter + ")";
                }
            }
            return datafilter;
        }
        /// <summary>
        /// 判断是否有指定部门权限
        /// </summary>
        /// <param name="deptId"></param>
        /// <returns></returns>
        public bool CheckDept(long deptId)
        {
            if (DeptList == null) return false;
            return DeptList.Contains(deptId);
        }
        /// <summary>
        /// 判断是否有指定的用户权限
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public bool CheckUser(long uid)
        {
            if (UserList == null) return false;
            return UserList.Contains(uid);
        }
    }
}
