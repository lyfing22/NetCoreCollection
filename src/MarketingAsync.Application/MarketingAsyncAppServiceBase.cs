using Abp.Application.Services;

namespace MarketingAsync
{
    /// <summary>
    /// Derive your application services from this class.
    /// </summary>
    public abstract class MarketingAsyncAppServiceBase : ApplicationService
    {
        protected MarketingAsyncAppServiceBase()
        {
            LocalizationSourceName = MarketingAsyncConsts.LocalizationSourceName;
        }
    }
}