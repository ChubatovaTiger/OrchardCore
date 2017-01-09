﻿using Microsoft.AspNetCore.Mvc.Modules;
using Microsoft.Extensions.DependencyInjection;
using Orchard.MetaWeblog;

namespace Orchard.Autoroute.RemotePublishing
{

    [OrchardFeature("Orchard.RemotePublishing")]
    public class RemotePublishingStartup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IMetaWeblogDriver, AutorouteMetaWeblogDriver>();
        }
    }
}
