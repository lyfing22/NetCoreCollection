using Abp.Dependency;

namespace MarketingAsync.Redis.Cache
{
    public static class CacheConfigurage
    {

        public static string ConnectionString { get; set; }

        public static string[] ConnStrings { get; set; }

        public static int Database { get; set; } = 0;

        public static int TimeDefaultValidTime = 1800;


    } 
}