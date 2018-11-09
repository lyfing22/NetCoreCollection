using Abp.Application.Services;
using MarketingAsync.ActuatorApp;

namespace MarketingAsync.AnalysisApp
{
    public interface IAnalysisAppService : IApplicationService
    {

        void ExcuteWriteMongodb();


    }
}