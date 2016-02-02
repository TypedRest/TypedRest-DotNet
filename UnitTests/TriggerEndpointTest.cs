﻿using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using RichardSzalay.MockHttp;

namespace TypedRest
{
    [TestFixture]
    public class TriggerEndpointTest : EndpointTestBase
    {
        private TriggerEndpoint _endpoint;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            _endpoint = new TriggerEndpoint(EntryEndpoint, "endpoint");
        }

        [Test]
        public async Task TestProbe()
        {
            Mock.Expect(HttpMethod.Options, "http://localhost/endpoint")
                .Respond(new StringContent("") {Headers = {Allow = {"POST"}}});

            await _endpoint.ProbeAsync();

            _endpoint.TriggerAllowed.Should().BeTrue();
        }

        [Test]
        public async Task TestTrigger()
        {
            Mock.Expect(HttpMethod.Post, "http://localhost/endpoint")
                .Respond(HttpStatusCode.NoContent);

            await _endpoint.TriggerAsync();
        }
    }
}