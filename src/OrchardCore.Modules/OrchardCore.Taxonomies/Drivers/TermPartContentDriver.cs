using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Display.ContentDisplay;
using OrchardCore.ContentManagement.Records;
using OrchardCore.DisplayManagement.Handlers;
using OrchardCore.DisplayManagement.ModelBinding;
using OrchardCore.DisplayManagement.Views;
using OrchardCore.Navigation;
using OrchardCore.Settings;
using OrchardCore.Taxonomies.Indexing;
using OrchardCore.Taxonomies.Models;
using OrchardCore.Taxonomies.Services;
using OrchardCore.Taxonomies.ViewModels;
using YesSql;

namespace OrchardCore.Taxonomies.Drivers
{
    public class TermPartContentDriver : ContentDisplayDriver
    {
        private readonly ISession _session;
        private readonly ISiteService _siteService;
        private readonly IContentManager _contentManager;
        private readonly ITaxonomyFieldService _taxonomyFieldService;

        public TermPartContentDriver(
            ISession session,
            ISiteService siteService,
            IContentManager contentManager,
            ITaxonomyFieldService taxonomyFieldService)
        {
            _session = session;
            _siteService = siteService;
            _contentManager = contentManager;
            _taxonomyFieldService = taxonomyFieldService;
        }

        public override Task<IDisplayResult> DisplayAsync(ContentItem model, BuildDisplayContext context)
        {
            var part = model.As<TermPart>();
            if (part != null)
            {
                return Task.FromResult<IDisplayResult>(Initialize<TermPartViewModel>("TermPart", async m =>
                {
                    var enableOrdering = (await _contentManager.GetAsync(part.TaxonomyContentItemId, VersionOptions.Latest)).As<TaxonomyPart>().EnableOrdering;
                    var pageSize = part.OrderingPageSize;
                    if (part.OrderingPageSize == 0)
                    {
                        var siteSettings = await _siteService.GetSiteSettingsAsync();
                        pageSize = siteSettings.PageSize;
                    }
                    var pager = await GetPagerAsync(context.Updater, pageSize);
                    m.TaxonomyContentItemId = part.TaxonomyContentItemId;
                    m.ContentItem = part.ContentItem;
                    m.ContentItems = (await _taxonomyFieldService.QueryCategorizedItemsAsync(part, enableOrdering, pager)).ToArray();
                    m.Pager = await context.New.PagerSlim(pager);
                    
                })
                .Location("Detail", "Content:5"));
            }

            return Task.FromResult<IDisplayResult>(null);
        }

        private static async Task<PagerSlim> GetPagerAsync(IUpdateModel updater, int pageSize)
        {
            var pagerParameters = new PagerSlimParameters();
            await updater.TryUpdateModelAsync(pagerParameters);

            var pager = new PagerSlim(pagerParameters, pageSize);

            return pager;
        }
    }
}
