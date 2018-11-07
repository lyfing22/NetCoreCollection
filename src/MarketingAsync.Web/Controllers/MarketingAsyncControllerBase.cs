using Abp.AspNetCore.Mvc.Controllers;

namespace MarketingAsync.Web.Controllers
{
    public abstract class MarketingAsyncControllerBase: AbpController
    {
        protected MarketingAsyncControllerBase()
        {
            LocalizationSourceName = MarketingAsyncConsts.LocalizationSourceName;
        }
    }
}