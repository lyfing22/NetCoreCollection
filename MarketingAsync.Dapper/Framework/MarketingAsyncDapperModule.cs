using System.Reflection;
using Abp.Modules;

namespace MarketingAsync.Dapper.Framework
{
    [DependsOn(typeof(MarketingAsyncCoreModule))]
    public class MarketingAsyncDapperModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
