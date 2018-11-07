using Abp.Modules;
using Abp.Reflection.Extensions;

namespace MarketingAsync.RabbitMq
{
    [DependsOn(typeof(MarketingAsyncCoreModule))]
    public class MarketingAsyncRabbitMqCoreModule : AbpModule
    {
        public override void Initialize()
        {



            IocManager.RegisterAssemblyByConvention(typeof(MarketingAsyncRabbitMqCoreModule).GetAssembly());


        }
    }
}