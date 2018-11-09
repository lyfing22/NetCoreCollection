using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Abp.Domain.Entities;
using Newtonsoft.Json;

namespace MarketingAsync.Mtimes
{
    public class TimeControl : Entity<string>
    {
        public static Dictionary<string, List<Mtime>> DataArray { get; } = new Dictionary<string, List<Mtime>>();

        private List<Mtime> current = new List<Mtime>();


        /// <summary>
        /// 该程序总步骤
        /// </summary>
        private string Group { get; }

        private int index = 0;
        private int lastSaveIndex = 0;
        private int currentIndex = 0;

        public Dictionary<string, List<Mtime>> GetAllData()
        {
            return DataArray;
        }

        public TimeControl(string group)
        {
            this.Group = group;
        }
        public void Marke(string message, string actid = null)
        {
            index++;
            currentIndex++;
            if (!DataArray.ContainsKey(Group))
            {
                DataArray[Group] = current;
            }
            current.Add(new Mtime(currentIndex, Group, actid, message));
        }



        public void SaveData(int actId = 0)
        {
            Marke("-1.保存最后的数据");
            var writer = new StreamWriter("./f.log", true);
            var dt = current.GetRange(lastSaveIndex, index);
            if (dt.Any())
            {
                dt.ForEach(x => x.ActId = actId);
                writer.WriteLine(JsonConvert.SerializeObject(dt) + Environment.NewLine);
                //重置时间
                Mtime.ResetCurrentNow();
                Marke("0.开启监控");

                lastSaveIndex = currentIndex;
            }
            writer.Close();
            index = 0;
        }

        /// <summary>
        /// 获取某一锚点的数据在一定范围内记录的时间
        /// </summary>
        /// <param name="makeStart"></param>
        /// <param name="makeStop"></param>
        /// <returns></returns>
        public int GetRange(string makeStart, string makeStop)
        {
            var starttime = current.FirstOrDefault(x => x.Message.IndexOf(makeStart) == 0);
            var endtime = current.FirstOrDefault(x => x.Message.IndexOf(makeStop) == 0);
            if (starttime == null || endtime == null)
            {
                throw new TimeAnchorsException("锚点不正确");
            }
            var times = endtime.TotalTime - starttime.TotalTime;
            return times;
        }

        /// <summary>
        /// 统计某个锚点之间的平均值
        /// </summary>
        /// <returns></returns>
        public int GetRangeForAllAvg(string makeStart, string makeStop)
        {
            return 0;
        }

        /// <summary>
        /// 统计每个锚点之间的值并按时间倒序
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, int> GetIntermediate()
        {
            return null;

        }


    }
}