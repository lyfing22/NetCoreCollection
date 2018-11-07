namespace MarketingAsync.Dapper
{
    public static class PersistentConfigurage
    {
        public static string MasterConnectionString { get; set; } 

        public static string SlaveConnectionString { get; set; }

        public static string NotDatabase { get; set; }

        public static string CardStaticDBName { get; set; }
    }

    public static class OraclePersistentConfigurage
    {
        public static string MasterConnectionString { get; set; }

        public static string SlaveConnectionString { get; set; }
    }
}
