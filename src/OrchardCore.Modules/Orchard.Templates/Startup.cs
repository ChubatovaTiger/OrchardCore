using System;
using Fluid;
using Microsoft.AspNetCore.Modules;
using Microsoft.Extensions.DependencyInjection;
using Orchard.DisplayManagement;
using Orchard.DisplayManagement.Shapes;
using Orchard.DisplayManagement.Zones;
using Orchard.Environment.Navigation;
using Orchard.Security.Permissions;
using Orchard.Templates.Services;

namespace Orchard.Templates
{
    public class Startup : StartupBase
    {
        static Startup()
        {
            TemplateContext.GlobalMemberAccessStrategy.Register<Shape>((o, n) =>
            {
                switch (n)
                {
                    case "Id": return o.Id;
                    case "Attributes": return o.Attributes;
                    case "Classes": return o.Classes;
                    default:
                        if (o.Properties.TryGetValue(n, out object result))
                        {
                            return result;
                        }

                        return null;
                }
            });

            TemplateContext.GlobalMemberAccessStrategy.Register<ZoneHolding>((o, n) =>
            {
                switch (n)
                {
                    case "Id": return o.Id;
                    case "Attributes": return o.Attributes;
                    case "Classes": return o.Classes;
                    default:
                        if (o.Properties.TryGetValue(n, out object result))
                        {
                            return result;
                        }

                        return null;
                }
            });
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IShapeBindingResolver, TemplatesShapeBindingResolver>();
            services.AddScoped<TemplatesManager>();
            services.AddScoped<IPermissionProvider, Permissions>();
            services.AddScoped<INavigationProvider, AdminMenu>();
        }
    }
}
