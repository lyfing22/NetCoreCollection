using System.Collections.Generic;
using Abp.Application.Services;

namespace MarketingAsync.ActuatorApp
{
    public interface IActuatorAppService : IApplicationService
    {
        /// <summary>
        /// 启动一个任务
        /// </summary>
        void StartWork();

       void ExcuteWorker(IEnumerable<int> actIdList);

        /// <summary>
        /// 导出活动以及活动设置数据
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        bool ExportAct(int Id);


        void StartWorkFor(int id);
        void BackToLastExport();

        void DeleteActData(int rowId, string actId);
    }
}