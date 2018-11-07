using System.Threading.Tasks;
using MarketingAsync.Web.Controllers;
using Shouldly;
using Xunit;

namespace MarketingAsync.Web.Tests.Controllers
{
    public class HomeController_Tests: MarketingAsyncWebTestBase
    {
        [Fact]
        public async Task Index_Test()
        {
            //Act
            var response = await GetResponseAsStringAsync(
                GetUrl<HomeController>(nameof(HomeController.Index))
            );

            //Assert
            response.ShouldNotBeNullOrEmpty();
        }
    }
}
