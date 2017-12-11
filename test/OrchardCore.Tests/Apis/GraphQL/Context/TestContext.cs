using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.FunctionalTests;
using OrchardCore.Apis.GraphQL.Client;
using OrchardCore.Tests.Apis.Sources;
using Xunit;

namespace OrchardCore.Tests.Apis.GraphQL.Context
{
    public class TestContext : IAsyncLifetime, IDisposable
    {
        public OrchardTestFixture<SiteStartup> Site { get; }

        public OrchardGraphQLClient Client { get; }

        public TestContext()
        {
            Site = new OrchardTestFixture<SiteStartup>(EnvironmentHelpers.GetApplicationPath());

            Client = new OrchardGraphQLClient(Site.Client);
        }

        public void Dispose()
        {
            Site.Dispose();
        }

        public virtual Task InitializeAsync()
        {
            return Task.CompletedTask;
        }

        public virtual Task DisposeAsync()
        {
            return Task.CompletedTask;
        }
    }
}
