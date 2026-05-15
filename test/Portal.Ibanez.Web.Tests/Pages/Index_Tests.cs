using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace Portal.Ibanez.Pages;

public class Index_Tests : IbanezWebTestBase
{
    [Fact]
    public async Task Welcome_Page()
    {
        var response = await GetResponseAsStringAsync("/");
        response.ShouldNotBeNull();
    }
}
