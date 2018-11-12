using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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

        //分析日志插入mongodb的店铺
        public void ExcuteWriteMongodb()
        {
            using (var reader = new StreamReader("./f.log"))
            {
                string input;
                while ((input = reader.ReadLine()) != null)
                {
                    if (!string.IsNullOrWhiteSpace(input))
                    {
                        var source = JsonConvert.DeserializeObject<IEnumerable<Mtime>>(input);

                        AnalysisAnchors(source.ToArray());
                    }
                }
            }
        }

        /// <summary>
        /// 锚点分析
        /// </summary>
        /// <param name="mtimes">一个actid中的锚点情况</param>
        /// <returns></returns>
        public void AnalysisAnchors(Mtime[] nodes)
        {

            var list = new List<Analysis>();
            var three = nodes.Length;

            int tract = 0;
            for (var index = 0; index < nodes.Length; index++)
            {

                var m = nodes[index];
                var c1 = JsonConvert.SerializeObject(m);
                var thisTime = JsonConvert.DeserializeObject<Analysis>(c1);
                var m1 = GetM1(thisTime.Message);
                m1 = m1.Substring(0, m1.Length - 1).Replace('.', '-');
                thisTime.KeyValuePairs = new KeyValuePair<string, Dictionary<string, int>>("x" + m1, new Dictionary<string, int>());

                if (tract != 0)
                {
                    var parent = nodes[tract - 1];
                    thisTime.TodoParent = thisTime.CurrentTime - parent.CurrentTime;
                }

                if (tract < three - 1)
                {
                    var next = nodes[tract + 1];
                    thisTime.TodoChild = next.CurrentTime - thisTime.CurrentTime;
                }

                thisTime.Total = nodes[nodes.Length - 1].CurrentTime - nodes[0].CurrentTime;
                tract++;

                //比较
                for (var current = tract; current < three; current++)
                {
                    var self = thisTime.KeyValuePairs.Value;
                    var point = nodes[current];
                    var m2 = GetM1(point.Message);
                    m2 = m2.Substring(0, m2.Length - 1).Replace('.', '-');
                    int childs = point.CurrentTime - thisTime.CurrentTime;
                    var key = "x" + m1 + "y" + m2;
                    var pindex = 1;

                    if (self.ContainsKey(key))
                    {
                    reset:
                        if (!self.ContainsKey(key + "-" + pindex))
                        {
                            key = key + "-" + pindex;
                        }
                        else
                        {
                            pindex++;
                            goto reset;
                        }
                    }

                    thisTime.KeyValuePairs.Value.Add(key, childs);
                }
                _mtimeRepository.updateData(thisTime);
               // _mtimeRepository.Insert(m);
            }


        }


        public string GetM1(string message)
        {

            var m1 = Regex.Match(message, "^-?[\\d]+\\.?[\\d]*[\\.]").Value;
            return m1;

        }

    }
}