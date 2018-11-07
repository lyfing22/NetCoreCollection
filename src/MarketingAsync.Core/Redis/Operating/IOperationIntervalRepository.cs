namespace MarketingAsync.Redis.Operating
{
    /// <summary>
    /// 检测运行间隔
    /// </summary>
    public interface IOperationIntervalRepository : IRedisHelper
    {
        /// <summary>
        /// 检测该用户的运行间隔时间并执行
        /// </summary>
        /// <param name="userId"></param>
        void Check(string userId);

        /// <summary>
        /// 设置用户的间隔时间
        /// </summary>
        /// <param name="userId"></param>
        void Setting(string userId);
    }
}
