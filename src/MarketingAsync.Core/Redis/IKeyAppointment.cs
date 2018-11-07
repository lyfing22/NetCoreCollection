namespace MarketingAsync.Redis
{
    public interface IKeyAppointment
    {
        //至少需要一个合并Key
        string Merge(string key);

        string CurrentKey { get; }

    }
}