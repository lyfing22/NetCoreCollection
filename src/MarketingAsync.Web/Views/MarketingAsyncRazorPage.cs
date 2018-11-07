using Abp.AspNetCore.Mvc.Views;

namespace MarketingAsync.Web.Views
{
    public abstract class MarketingAsyncRazorPage<TModel> : AbpRazorPage<TModel>
    {
        protected MarketingAsyncRazorPage()
        {
            LocalizationSourceName = MarketingAsyncConsts.LocalizationSourceName;
        }
    }
}
