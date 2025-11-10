using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeveloperService
{
    [Migration(20231202001)]
    public class DeveloperMigrator2 : Migration
    {

        public override void Up()
        {

            Insert.IntoTable("mz_role").Row(new
            {
                RoleID = 5,
                RoleName = "企业接口开发者",
                RoleSort = 0,
                RoleDesc = "提供企业接口开发相关的服务",
                IsSystem = "1",
                Status = "0",
                OrgId = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                RoleID = 6,
                RoleName = "个人接口开发者",
                RoleSort = 0,
                RoleDesc = "提供个人接口开发相关的服务",
                IsSystem = "1",
                Status = "0",
                OrgId = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            });

        }
        public override void Down()
        {
        }
    }
}
