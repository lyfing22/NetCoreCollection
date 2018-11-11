using System.Collections.Generic;

namespace MarketingAsync.Mtimes
{
    public class Analysis : Mtime
    {
        public Analysis(int order, string @group, string actid, string message = "") : base(order, @group, actid, message)
        {

        }


        /// <summary>
        /// 分析项 当前锚点到任意锚点所需的时间  m1.m5 
        /// </summary>
        public KeyValuePair<string, Dictionary<string, int>> KeyValuePairs { get; set; }

        /// <summary>
        /// 锚点id
        /// </summary>
        public string Anchors { get; set; }

        public int TodoParent { get; set; }

        public int TodoChild { get; set; }

        public Analysis()
        {
            

        }

    }
}
