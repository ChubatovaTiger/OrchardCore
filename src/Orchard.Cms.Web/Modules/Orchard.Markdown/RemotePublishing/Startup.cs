﻿using OrchardCore.Modules;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Extensions.Features.Attributes;
using Orchard.MetaWeblog;

namespace Orchard.Markdown.RemotePublishing
{

    [OrchardFeature("Orchard.RemotePublishing")]
    public class Startup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IMetaWeblogDriver, MarkdownMetaWeblogDriver>();
        }
    }
}
