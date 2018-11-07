using System;
using System.Collections.Generic;
using System.Text;

namespace MarketingAsync.Act.Keys
{
    public static class SignKeys
    {
        /// <summary>
        /// 签到活动当天的签到人数key {actid} {day}
        /// </summary>
        public static readonly string SignActTodayCount_Key = "SignActTodayCount_Key_{0}_{1}";

        /// <summary>
        /// 用户补签卡信息 缓存 IEnumerable 集合 {actid} {openid}
        /// </summary>
        public static readonly string SignatureCard_Key = "SignatureCard_Key_{0}_{1}";

        /// <summary>
        ///签到活动奖项发放数量递增key {0}奖项设置prizeid  {1}prizetype周建峰 2018.10.31
        /// </summary>
        public static readonly string signactprizegrantnum_key = "signactprizegrantnum_key{0}_{1}";



        #region 老活动Key

        

        #endregion

    }
}
