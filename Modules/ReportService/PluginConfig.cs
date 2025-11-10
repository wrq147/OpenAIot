
using Common;
using Microsoft.Extensions.Configuration;
using ReportService.Business;
using ReportService.DAL;
using TemplateAction.Core;
using TemplateAction.NetCore;

namespace ReportService
{
    public class PluginConfig : TANetCorePluginConfig
    {
        protected override void ConfigureServices(IConfiguration config, IServiceCollection services)
        {
            services.AddBLL<ShareBLL>();
            services.AddBLL<ReportBLL>();
            services.AddBLL<WidgetBLL>();
            services.AddBLL<ThemeBLL>();
            services.AddBLL<DataSourceBLL>();
            services.AddBLL<ApiSourceBLL>();
            services.AddBLL<FileSourceBLL>();
            services.AddBLL<PrintBLL>();
            services.AddBLL<ReportWarnBLL>();
            services.AddBLL<ReportGroupBLL>();


            services.AddDAL<ReportDAL>();
            services.AddDAL<WidgetDAL>();
            services.AddDAL<ShareDAL>();
            services.AddDAL<ThemeDAL>();
            services.AddDAL<ApiSourceDAL>();
            services.AddDAL<FileSourceDAL>();
            services.AddDAL<DataSourceDAL>();
            services.AddDAL<PrintTemplateDAL>();
            services.AddDAL<PrintDataDAL>();
            services.AddDAL<ReportWarnDAL>();
            services.AddDAL<ReportGroupDAL>();
        }
        protected override void Configure(ITAApplication app, PluginObject plg)
        {
        }
    }
}
