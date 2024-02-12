using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OrchardCore.DisplayManagement.Handlers;
using OrchardCore.Email.Drivers;
using OrchardCore.Email.Services;
using OrchardCore.Modules;
using OrchardCore.Navigation;
using OrchardCore.Security.Permissions;
using OrchardCore.Settings;

namespace OrchardCore.Email
{
    public class Startup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddEmailServices()
                .AddScoped<IDisplayDriver<ISite>, EmailSettingsDisplayDriver>()
                .AddScoped<IPermissionProvider, Permissions>()
                .AddScoped<INavigationProvider, AdminMenu>();

            services.AddSmtpEmailProvider()
                .AddScoped<IDisplayDriver<ISite>, SmtpSettingsDisplayDriver>()
                .AddTransient<IConfigureOptions<SmtpOptions>, SmtpOptionsConfiguration>();
        }
    }
}
