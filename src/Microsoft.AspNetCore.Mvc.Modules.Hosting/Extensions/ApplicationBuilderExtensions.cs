using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Orchard;
using Orchard.Environment.Extensions;
using Orchard.Hosting;
using Orchard.Hosting.Web.Routing;
using System.IO;
using System.Linq;

namespace Microsoft.AspNetCore.Mvc.Modules.Hosting
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseModules(this IApplicationBuilder builder)
        {
            var extensionManager = builder.ApplicationServices.GetRequiredService<IExtensionManager>();
            var hostingEnvironment = builder.ApplicationServices.GetRequiredService<IHostingEnvironment>();
            var loggerFactory = builder.ApplicationServices.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger("Default");

            if (hostingEnvironment.IsDevelopment())
            {
                builder.UseDeveloperExceptionPage();
            }

            // Add static files to the request pipeline.
            builder.UseStaticFiles();

            // TODO: configure the location and parameters (max-age) per module.
            var availableExtensions = extensionManager.GetExtensions();
            foreach (var extension in availableExtensions)
            {
                var contentPath = Path.Combine(extension.ExtensionFileInfo.PhysicalPath, "Content");
                if (Directory.Exists(contentPath))
                {
                    builder.UseStaticFiles(new StaticFileOptions
                    {
                        RequestPath = "/" + extension.Id,
                        FileProvider = new PhysicalFileProvider(contentPath)
                    });
                }
            }

            // Ensure the shell tenants are loaded when a request comes in
            // and replaces the current service provider for the tenant's one.
            builder.UseMiddleware<OrchardContainerMiddleware>();

            // Route the request to the correct tenant specific pipeline
            builder.UseMiddleware<OrchardRouterMiddleware>();

            // Load controllers
            var applicationPartManager = builder.ApplicationServices.GetRequiredService<ApplicationPartManager>();

            using (logger.BeginScope("Loading extensions"))
            {
                availableExtensions.InvokeAsync(async ae =>
                {
                    var extensionEntry = await extensionManager.LoadExtensionAsync(ae);

                    if (!extensionEntry.IsError)
                    {
                        var assemblyPart = new AssemblyPart(extensionEntry.Assembly);
                        applicationPartManager.ApplicationParts.Add(assemblyPart);
                    }
                }, logger).Wait();
            }

            return builder;
        }
    }
}