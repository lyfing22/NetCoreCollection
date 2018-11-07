using Abp.Modules;
using Abp.Reflection.Extensions;

namespace MarketingAsync.Mongodb
{
    [DependsOn(
        typeof(MarketingAsyncCoreModule))]
    public class MarketingAsyncMongodbCoreModule : AbpModule
    {
        public override void Initialize()
        {



            IocManager.RegisterAssemblyByConvention(typeof(MarketingAsyncMongodbCoreModule).GetAssembly());


        }
    }
}