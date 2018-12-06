using System;
using Abp.AspNetCore;
using Abp.AspNetCore.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using MarketingAsync.Configuration;
using MarketingAsync.Dapper;
using MarketingAsync.Dapper.Framework;
using MarketingAsync.Mongodb;
using MarketingAsync.Mtimes;
using MarketingAsync.Redis;
using MarketingAsync.Redis.Cache;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace MarketingAsync.Web.Startup
{
    [DependsOn(
        typeof(MarketingAsyncRedisModule),
        typeof(MarketingAsyncMongodbCoreModule),
        typeof(MarketingAsyncDapperModule),
        typeof(MarketingAsyncApplicationModule),
        typeof(AbpAspNetCoreModule))]
    public class MarketingAsyncWebModule : AbpModule
    {
        private readonly IConfigurationRoot _appConfiguration;

        public MarketingAsyncWebModule(IHostingEnvironment env)
        {
            _appConfiguration = AppConfigurations.Get(env.ContentRootPath, env.EnvironmentName);
        }

        public override void PreInitialize()
        {
            PersistentConfigurage.MasterConnectionString = _appConfiguration.GetConnectionString(MarketingAsyncConsts.MssqlConnectionStringName);
            DocumentConfigurage.ConnectionString = _appConfiguration.GetConnectionString(MarketingAsyncConsts.MongoConnectionStringName);
            CacheConfigurage.ConnectionString = _appConfiguration.GetConnectionString(MarketingAsyncConsts.Redis);

            DocumentConfigurage.DatabaseMongoDB = "fangwei_wxc_db"; 

            PersistentConfigurage.NotDatabase = _appConfiguration.GetConnectionString(MarketingAsyncConsts.NotDatabase);

            MtimeConfig.RecordCount = Convert.ToInt32(_appConfiguration.GetConnectionString(MarketingAsyncConsts.RecordCount));
            MtimeConfig.InputLog = Convert.ToBoolean(_appConfiguration.GetConnectionString(MarketingAsyncConsts.InputLog));

            Configuration.Navigation.Providers.Add<MarketingAsyncNavigationProvider>();

            Configuration.Modules.AbpAspNetCore()
                .CreateControllersForAppServices(
                    typeof(MarketingAsyncApplicationModule).GetAssembly()
                );

        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(MarketingAsyncWebModule).GetAssembly());
        }
    }
}