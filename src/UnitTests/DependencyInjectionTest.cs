using Microsoft.Extensions.DependencyInjection;
using TypedRest.Endpoints;

namespace TypedRest
{
    public class DependencyInjectionTest
    {
        [Fact]
        public void TestGetRequiredService()
        {
            var services = new ServiceCollection();
            services.AddTypedRest(new Uri("http://example.com/"));
            var provider = services.BuildServiceProvider();

            provider.GetRequiredService<EntryEndpoint>()
                    .Uri.Should().Be(new Uri("http://example.com/"));
        }
    }
}
