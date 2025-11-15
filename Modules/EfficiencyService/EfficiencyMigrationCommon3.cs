using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator;

namespace EfficiencyService
{
    [Migration(20250912175)]
    public class EfficiencyMigrationCommon3 : Migration
    {
        public override void Up()
        {
            Execute.Sql("ALTER TABLE t_com_product ADD COLUMN OrgId bigint;");
        }
        public override void Down()
        {
        }
    }
}
