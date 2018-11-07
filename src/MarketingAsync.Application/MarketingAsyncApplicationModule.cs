using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;

namespace MarketingAsync
{
    [DependsOn(
        typeof(MarketingAsyncCoreModule), 
        typeof(AbpAutoMapperModule))]
    public class MarketingAsyncApplicationModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(MarketingAsyncApplicationModule).GetAssembly());
        }
    }
}