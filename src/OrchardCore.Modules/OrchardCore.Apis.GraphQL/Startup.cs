using System;
using GraphQL;
using GraphQL.Http;
using GraphQL.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OrchardCore.Apis.GraphQL.Mutations;
using OrchardCore.Apis.GraphQL.Queries;
using OrchardCore.Apis.GraphQL.Services;
using OrchardCore.Apis.GraphQL.Subscriptions;
using OrchardCore.Environment.Navigation;
using OrchardCore.Modules;
using OrchardCore.Security.Permissions;

namespace OrchardCore.Apis.GraphQL
{
    public class Startup : StartupBase
    {
        public override int Order => 10;

        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IDependencyResolver, InternalDependencyResolver>();
            services.AddSingleton<IDocumentExecuter, DocumentExecuter>();
            services.AddSingleton<IDocumentWriter, DocumentWriter>();

            services.AddGraphQLQueries();
            services.AddGraphQLMutations();
            services.AddGraphQLSubscriptions();

            // Schema

            services.AddScoped<ISchemaService, SchemaService>();

            services.AddScoped<IPermissionProvider, Permissions>();
            services.AddTransient<INavigationProvider, AdminMenu>();

            services.TryAddScoped<IGraphQLSchemaHashService, NullGraphQLSchemaHashService>();
        }

        public override void Configure(IApplicationBuilder app, IRouteBuilder routes, IServiceProvider serviceProvider)
        {
            app.UseMiddleware<GraphQLMiddleware>(new GraphQLSettings
            {
                BuildUserContext = ctx => new GraphQLUserContext
                {
                    User = ctx.User
                }
            });
        }
    }
}
