using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json.Linq;
using OrchardCore.Entities;

namespace OrchardCore.Settings
{
    // When updating class also update SiteSettingsDeploymentSource and SettingsStep.
    public class SiteSettings : Entity, ISite
    {
        public int Id { get; set; }
        public string BaseUrl { get; set; }
        public string Calendar { get; set; }
        public int MaxPagedCount { get; set; }
        public int MaxPageSize { get; set; }
        public int PageSize { get; set; }
        public string TimeZoneId { get; set; }
        public ResourceDebugMode ResourceDebugMode { get; set; }
        public string SiteName { get; set; }
        public string SiteSalt { get; set; }
        public string PageTitleFormat { get; set; }
        public string SuperUser { get; set; }
        public bool UseCdn { get; set; } = true;
        public string CdnBaseUrl { get; set; }
        public RouteValueDictionary HomeRoute { get; set; } = new RouteValueDictionary();
        public bool AppendVersion { get; set; } = true;

        public SiteSettings Clone()
        {
            var settings = MemberwiseClone() as SiteSettings;

            settings.Properties = new JObject(Properties);
            settings.HomeRoute = new RouteValueDictionary(HomeRoute);

            return settings;
        }
    }
}
