using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace TronBox.API.Tests.Config
{
    public class TronBoxAppFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)

        {
            builder.UseStartup<TStartup>();
            builder.UseEnvironment("Development");
        }
    }
}
