using System.Collections.Generic;
using System.IO;
using MarketingAsync.Mtimes;
using Newtonsoft.Json;

namespace MarketingAsync.AnalysisApp
{

    public class AnalysisAppService : MarketingAsyncAppServiceBase, IAnalysisAppService
    {

        private readonly IMtimeRepository _mtimeRepository;

        public AnalysisAppService(IMtimeRepository mtimeRepository)
        {
            _mtimeRepository = mtimeRepository;
        }

        //分析日志插入mongodb

        public void ExcuteWriteMongodb()
        {
            using (var reader = new StreamReader("./f.log"))
            {
                string input;
                while ((input = reader.ReadLine()) != null)
                {
                    var source = JsonConvert.DeserializeObject<IEnumerable<Mtime>>(input);
                    _mtimeRepository.InsertList(source);
                }
            }
        }



    }
}