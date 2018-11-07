using Abp.Modules;
using Abp.Reflection.Extensions;

namespace MarketingAsync.Redis
{
    public class MarketingAsyncRedisModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Auditing.IsEnabledForAnonymousUsers = true;
             
        }

        public override void Initialize()
        {
             
            IocManager.RegisterAssemblyByConvention(typeof(MarketingAsyncRedisModule).GetAssembly()); 
             
        }
    }
}