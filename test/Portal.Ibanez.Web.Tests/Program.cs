using Microsoft.AspNetCore.Builder;
using Portal.Ibanez;
using Volo.Abp.AspNetCore.TestBase;

var builder = WebApplication.CreateBuilder();

builder.Environment.ContentRootPath = GetWebProjectContentRootPathHelper.Get("Portal.Ibanez.Web.csproj");
await builder.RunAbpModuleAsync<IbanezWebTestModule>(applicationName: "Portal.Ibanez.Web" );

public partial class Program
{
}
