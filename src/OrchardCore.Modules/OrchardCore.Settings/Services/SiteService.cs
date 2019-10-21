using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using OrchardCore.Environment.Cache;
using OrchardCore.Environment.Shell.Scope;
using OrchardCore.Modules;
using YesSql;

namespace OrchardCore.Settings.Services
{
    /// <summary>
    /// Implements <see cref="ISiteService"/> by storing the site as a Content Item.
    /// </summary>
    public class SiteService : ISiteService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ISignal _signal;
        private readonly IClock _clock;

        private const string SiteCacheKey = "SiteService";

        public SiteService(
            IMemoryCache memoryCache,
            ISignal signal,
            IClock clock)
        {
            _memoryCache = memoryCache;
            _signal = signal;
            _clock = clock;
        }

        /// <inheritdoc/>
        public IChangeToken ChangeToken => _signal.GetToken(SiteCacheKey);

        private SiteSettingsCache ScopedCache => ShellScope.Services.GetRequiredService<SiteSettingsCache>();

        /// <inheritdoc/>
        public async Task<ISite> GetSiteSettingsAsync()
        {
            var scopedCache = ScopedCache;

            if (scopedCache.SiteSettings != null)
            {
                return scopedCache.SiteSettings;
            }

            SiteSettings site;

            if (!_memoryCache.TryGetValue(SiteCacheKey, out site))
            {
                var session = Session;

                // First get a new token.
                var changeToken = ChangeToken;

                // The cache is always updated with the actual persisted data.
                site = await session.Query<SiteSettings>().FirstOrDefaultAsync();

                if (site == null)
                {
                    site = new SiteSettings
                    {
                        SiteSalt = Guid.NewGuid().ToString("N"),
                        SiteName = "My Orchard Project Application",
                        PageTitleFormat = "{% page_title Site.SiteName, position: \"after\", separator: \" - \" %}",
                        PageSize = 10,
                        MaxPageSize = 100,
                        MaxPagedCount = 0,
                        TimeZoneId = _clock.GetSystemTimeZone().TimeZoneId,
                    };

                    // Persists new data.
                    Session.Save(site);

                    // Invalidates the cache after the session is committed.
                    _signal.DeferredSignalToken(SiteCacheKey);
                }
                else
                {
                    // Cache a clone to not be mutated by the current scope.
                    _memoryCache.Set(SiteCacheKey, site.Clone(), changeToken);
                }

                // Here no cloning, the instance needs to stay tied to the session.
                return scopedCache.SiteSettings = site;
            }

            // Each scope uses a cloned value of the cache.
            return scopedCache.SiteSettings = site.Clone();
        }

        /// <inheritdoc/>
        public Task UpdateSiteSettingsAsync(ISite site)
        {
            // Check if it is the same instance.
            if (ScopedCache.SiteSettings != site)
            {
                ScopedCache.SiteSettings.UpdateFrom(site);
            }

            // Persists new data.
            Session.Save(ScopedCache.SiteSettings);

            // Cache invalidation after committing the session.
            _signal.DeferredSignalToken(SiteCacheKey);

            return Task.CompletedTask;
        }

        private ISession Session => ShellScope.Services.GetService<ISession>();
    }
}
