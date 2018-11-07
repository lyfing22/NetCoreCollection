using Abp.AspNetCore.TestBase;
using Abp.Modules;
using Abp.Reflection.Extensions;
using MarketingAsync.Web.Startup;
namespace MarketingAsync.Web.Tests
{
    [DependsOn(
        typeof(MarketingAsyncWebModule),
        typeof(AbpAspNetCoreTestBaseModule)
        )]
    public class MarketingAsyncWebTestModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.UnitOfWork.IsTransactional = false; //EF Core InMemory DB does not support transactions.
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(MarketingAsyncWebTestModule).GetAssembly());
        }
    }
}