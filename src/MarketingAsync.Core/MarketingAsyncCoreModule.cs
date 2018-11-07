using Abp.Modules;
using Abp.Reflection.Extensions;
using MarketingAsync.Localization;

namespace MarketingAsync
{
    public class MarketingAsyncCoreModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Auditing.IsEnabledForAnonymousUsers = true;

            MarketingAsyncLocalizationConfigurer.Configure(Configuration.Localization);
        }

        public override void Initialize()
        {
             
            IocManager.RegisterAssemblyByConvention(typeof(MarketingAsyncCoreModule).GetAssembly()); 
             
        }
    }
}