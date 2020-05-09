#if NETCOREAPP
using System;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using TypedRest.Endpoints;
using Xunit;

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
#endif
