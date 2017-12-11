using OrchardCore.Modules;
using Microsoft.Extensions.DependencyInjection;
using GraphQL.Types;
using OrchardCore.Body.Model;
using OrchardCore.Apis.GraphQL.Queries;
using OrchardCore.ContentManagement;

namespace OrchardCore.Body.GraphQL
{
    [RequireFeatures("OrchardCore.Apis.GraphQL")]
    public class Startup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<InputObjectGraphType<BodyPart>, BodyInputObjectType>();
            services.AddScoped<IInputObjectGraphType, BodyInputObjectType>();

            services.AddScoped<ObjectGraphType<BodyPart>, BodyQueryObjectType>();
            services.AddScoped<IObjectGraphType, BodyQueryObjectType>();

            services.AddScoped<IGraphQLFilter<ContentItem>, BodyGraphQLFilter>();
        }
    }
}
