using MarketingAsync.Redis;
using Shouldly;
using Xunit;

namespace MarketingAsync.Tests.TestDatas
{
    public class RedisTest : MarketingAsyncTestBase
    {
        private readonly IRedisHelper _redisHelper;

        public RedisTest()
        {
            _redisHelper = LocalIocManager.Resolve<IRedisHelper>();
        }


        [Fact]
        public void SetTest()
        {


            var my1 = new MyClass();
            my1.A = "1";
            my1.B = "2";

            var my2 = new MyClass1();
            my2.A = "1";
            my2.B = "2";

           my1.ShouldBe(my1);

            //_redisHelper.StringSet("a", "b");
            //var data = _redisHelper.StringGet("a");
            //data.ShouldBe("b");
            //ExportAct(537);

            my1.ShouldSatisfyAllConditions(
                () => my1.A.ShouldBe(my2.A, ignoreOrder: true),
                () => my1.B.ShouldBe(my2.B, ignoreOrder: true)
            );

        }


    }

    public class MyClass
    {
        public string A { get; set; }

        public string B { get; set; }

    }

    public class MyClass1
    {
        public string A { get; set; }

        public string B { get; set; }
    }
}