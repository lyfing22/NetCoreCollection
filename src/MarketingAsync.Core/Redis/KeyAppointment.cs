namespace MarketingAsync.Redis
{
    public abstract class KeyAppointment : IKeyAppointment
    {


        public string Merge(string key)
        {
            //统计key调用次数 
            return CurrentKey + ":" + key;
        }

        public abstract string CurrentKey { get; }
    }
}
