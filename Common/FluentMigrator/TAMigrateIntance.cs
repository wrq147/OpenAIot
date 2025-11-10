using System;
using TemplateAction.Core;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Exceptions;

namespace Common.FluentMigrator
{
    /// <summary>
    /// 模块自动迁移数据库
    /// </summary>
    public class TAMigrateIntance
    {
        private ITAServiceProvider _serviceProvider;
        private TransientLifetimeFactory _lifeFactory;
        public TAMigrateIntance(ITAServiceProvider provider)
        {
            _serviceProvider = provider;
            _lifeFactory = new TransientLifetimeFactory();
        }
        /// <summary>
        /// 迁移到最新版
        /// </summary>
        public void ToLast()
        {
            try
            {
              
                //执行数据库迁移
                var runner = _serviceProvider.GetService<IMigrationRunner>(_lifeFactory);
                runner.MigrateUp();
            }
            catch (Exception ex) {}
        }
        /// <summary>
        /// 迁移到指定版本
        /// </summary>
        /// <param name="version"></param>
        public void MigrateTo(long version)
        {
            var runner = _serviceProvider.GetService<IMigrationRunner>(_lifeFactory);
            if (runner.HasMigrationsToApplyUp(version))
            {
                runner.MigrateUp(version);
            }
            else if (runner.HasMigrationsToApplyDown(version))
            {
                runner.MigrateDown(version);
            }
        }
    }
}
