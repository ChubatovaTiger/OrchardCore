using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OrchardCore.Admin;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Display.ContentDisplay;
using OrchardCore.Data.Migration;
using OrchardCore.DisplayManagement;
using OrchardCore.Entities;
using OrchardCore.Modules;
using OrchardCore.Mvc.Core.Utilities;
using OrchardCore.Navigation;
using OrchardCore.Routing;
using OrchardCore.Security.Permissions;
using OrchardCore.Sitemaps.Builders;
using OrchardCore.Sitemaps.Cache;
using OrchardCore.Sitemaps.Controllers;
using OrchardCore.Sitemaps.Drivers;
using OrchardCore.Sitemaps.Handlers;
using OrchardCore.Sitemaps.Models;
using OrchardCore.Sitemaps.Routing;
using OrchardCore.Sitemaps.Services;

namespace OrchardCore.Sitemaps
{
    public class Startup : StartupBase
    {
        private readonly AdminOptions _adminOptions;

        public Startup(IOptions<AdminOptions> adminOptions)
        {
            _adminOptions = adminOptions.Value;
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IDataMigration, Migrations>();
            services.AddScoped<INavigationProvider, AdminMenu>();
            services.AddScoped<IPermissionProvider, Permissions>();
            services.AddIdGeneration();

            services.Configure<SitemapsOptions>(options =>
            {
                if (options.GlobalRouteValues.Count == 0)
                {
                    options.GlobalRouteValues = new RouteValueDictionary
                    {
                        {"Area", "OrchardCore.Sitemaps"},
                        {"Controller", "Sitemap"},
                        {"Action", "Index"}
                    };

                    options.SitemapIdKey = "sitemapId";
                }
            });

            services.AddSingleton<IShellRouteValuesAddressScheme, SitemapValuesAddressScheme>();
            services.AddSingleton<SitemapsTransformer>();
            services.AddSingleton<SitemapEntries>();

            services.AddScoped<ISitemapIdGenerator, SitemapIdGenerator>();
            services.AddScoped<IPermissionProvider, Permissions>();
            services.AddScoped<ISitemapManager, SitemapManager>();
            services.AddScoped<ISitemapHelperService, SitemapHelperService>();
            services.AddScoped<IDisplayManager<SitemapSource>, DisplayManager<SitemapSource>>();
            services.AddScoped<ISitemapBuilder, DefaultSitemapBuilder>();
            services.AddScoped<ISitemapTypeBuilder, SitemapTypeBuilder>();
            services.AddScoped<ISitemapCacheProvider, DefaultSitemapCacheProvider>();
            services.AddScoped<ISitemapCacheManager, DefaultSitemapCacheManager>();
            services.AddScoped<ISitemapTypeCacheManager, SitemapTypeCacheManager>();
            services.AddScoped<ISitemapTypeBuilder, SitemapIndexTypeBuilder>();
            services.AddScoped<ISitemapTypeCacheManager, SitemapIndexTypeCacheManager>();
            services.AddScoped<ISitemapModifiedDateProvider, DefaultSitemapModifiedDateProvider>();
            services.AddScoped<IRouteableContentTypeCoordinator, DefaultRouteableContentTypeCoordinator>();

            services.AddScoped<ISitemapPartContentItemValidationProvider, SitemapPartContentItemValidationProvider>();
            services.AddScoped<ISitemapContentItemValidationProvider>(serviceProvider =>
                serviceProvider.GetRequiredService<ISitemapPartContentItemValidationProvider>());

            // Sitemap Part.
            services.AddContentPart<SitemapPart>()
                .UseDisplayDriver<SitemapPartDisplay>()
                .AddHandler<SitemapPartHandler>();
        }

        public override void Configure(IApplicationBuilder app, IEndpointRouteBuilder routes, IServiceProvider serviceProvider)
        {
            var adminControllerName = typeof(AdminController).ControllerName();

            routes.MapAreaControllerRoute(
                name: "SitemapsList",
                areaName: "OrchardCore.Sitemaps",
                pattern: _adminOptions.AdminUrlPrefix + "/Sitemaps/List",
                defaults: new { controller = adminControllerName, action = nameof(AdminController.List) }
            );

            routes.MapAreaControllerRoute(
                name: "SitemapsDisplay",
                areaName: "OrchardCore.Sitemaps",
                pattern: _adminOptions.AdminUrlPrefix + "/Sitemaps/Display/{id}",
                defaults: new { controller = adminControllerName, action = nameof(AdminController.Display) }
            );

            routes.MapAreaControllerRoute(
                name: "SitemapsCreate",
                areaName: "OrchardCore.Sitemaps",
                pattern: _adminOptions.AdminUrlPrefix + "/Sitemaps/Create",
                defaults: new { controller = adminControllerName, action = nameof(AdminController.Create) }
            );

            routes.MapAreaControllerRoute(
                name: "SitemapsEdit",
                areaName: "OrchardCore.Sitemaps",
                pattern: _adminOptions.AdminUrlPrefix + "/Sitemaps/Edit/{sitemapId}",
                defaults: new { controller = adminControllerName, action = nameof(AdminController.Edit) }
            );

            routes.MapAreaControllerRoute(
                name: "SitemapsDelete",
                areaName: "OrchardCore.Sitemaps",
                pattern: _adminOptions.AdminUrlPrefix + "/Sitemaps/Delete/{sitemapId}",
                defaults: new { controller = adminControllerName, action = nameof(AdminController.Delete) }
            );

            routes.MapAreaControllerRoute(
                name: "SitemapsToggle",
                areaName: "OrchardCore.Sitemaps",
                pattern: _adminOptions.AdminUrlPrefix + "/Sitemaps/Toggle/{sitemapId}",
                defaults: new { controller = adminControllerName, action = nameof(AdminController.Toggle) }
            );

            var sitemapIndexController = typeof(SitemapIndexController).ControllerName();

            routes.MapAreaControllerRoute(
                 name: "SitemapsIndexList",
                 areaName: "OrchardCore.Sitemaps",
                 pattern: _adminOptions.AdminUrlPrefix + "/SitemapsIndexes/List",
                 defaults: new { controller = sitemapIndexController, action = nameof(SitemapIndexController.List) }
            );

            routes.MapAreaControllerRoute(
                 name: "SitemapsIndexCreate",
                 areaName: "OrchardCore.Sitemaps",
                 pattern: _adminOptions.AdminUrlPrefix + "/SitemapsIndexes/Create",
                 defaults: new { controller = sitemapIndexController, action = nameof(SitemapIndexController.Create) }
            );

            routes.MapAreaControllerRoute(
                 name: "SitemapsIndexEdit",
                 areaName: "OrchardCore.Sitemaps",
                 pattern: _adminOptions.AdminUrlPrefix + "/SitemapsIndexes/Edit/{sitemapId}",
                 defaults: new { controller = sitemapIndexController, action = nameof(SitemapIndexController.Edit) }
            );

            routes.MapAreaControllerRoute(
                 name: "SitemapsIndexDelete",
                 areaName: "OrchardCore.Sitemaps",
                 pattern: _adminOptions.AdminUrlPrefix + "/SitemapsIndexes/Delete/{sitemapId}",
                 defaults: new { controller = sitemapIndexController, action = nameof(SitemapIndexController.Delete) }
            );

            routes.MapAreaControllerRoute(
                 name: "SitemapsIndexToggle",
                 areaName: "OrchardCore.Sitemaps",
                 pattern: _adminOptions.AdminUrlPrefix + "/SitemapsIndexes/Toggle/{sitemapId}",
                 defaults: new { controller = sitemapIndexController, action = nameof(SitemapIndexController.Toggle) }
            );

            var sourceController = typeof(SourceController).ControllerName();

            routes.MapAreaControllerRoute(
                 name: "SitemapsSourceCreate",
                 areaName: "OrchardCore.Sitemaps",
                 pattern: _adminOptions.AdminUrlPrefix + "/SitemapsSource/Create/{sitemapId}/{sourceType}",
                 defaults: new { controller = sourceController, action = nameof(SourceController.Create) }
            );

            routes.MapAreaControllerRoute(
                 name: "SitemapsSourceEdit",
                 areaName: "OrchardCore.Sitemaps",
                 pattern: _adminOptions.AdminUrlPrefix + "/SitemapsSource/Edit/{sitemapId}/{sourceId}",
                 defaults: new { controller = sourceController, action = nameof(SourceController.Edit) }
            );

            routes.MapAreaControllerRoute(
                 name: "SitemapsSourceDelete",
                 areaName: "OrchardCore.Sitemaps",
                 pattern: _adminOptions.AdminUrlPrefix + "/SitemapsSource/Delete/{sitemapId}/{sourceId}",
                 defaults: new { controller = sourceController, action = nameof(SourceController.Delete) }
            );

            routes.MapDynamicControllerRoute<SitemapsTransformer>("/{**sitemap}");
            var sitemapManager = serviceProvider.GetService<ISitemapManager>();
            sitemapManager.BuildAllSitemapRouteEntriesAsync().GetAwaiter().GetResult();
        }
    }

    [Feature("OrchardCore.Sitemaps.RazorPages")]
    public class SitemapsRazorPagesStartup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions<SitemapsRazorPagesOptions>();
            services.AddScoped<IRouteableContentTypeProvider, RazorPagesContentTypeProvider>();
        }
    }
}
