using System;
using System.Collections.Generic;
using System.Linq;

namespace MarketingAsync.Mtimes
{
    public class TimeControl
    {
        private static Dictionary<string, List<Mtime>> Dictionary = new Dictionary<string, List<Mtime>>();

        private List<Mtime> current = new List<Mtime>();

        /// <summary>
        /// 该程序总步骤
        /// </summary>
        private string Group { get; }

        public TimeControl(string group)
        {
            this.Group = group;
        }
        public void Make(string message, string actid = null)
        {
            if (!Dictionary.ContainsKey(Group))
            {
                Dictionary[Group] = current;
            }
            current.Add(new Mtime(Group, actid, message));
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
            var times = endtime.Timespan - starttime.Timespan;
            return times.Milliseconds;
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
            return 0;

        }


    }
}