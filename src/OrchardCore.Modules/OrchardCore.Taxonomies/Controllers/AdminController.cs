using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Display;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.ContentManagement.Metadata.Models;
using OrchardCore.DisplayManagement.ModelBinding;
using OrchardCore.DisplayManagement.Notify;
using OrchardCore.Taxonomies.Models;
using YesSql;

namespace OrchardCore.Taxonomies.Controllers
{
    public class AdminController : Controller
    {
        private readonly IContentManager _contentManager;
        private readonly IAuthorizationService _authorizationService;
        private readonly IContentItemDisplayManager _contentItemDisplayManager;
        private readonly IContentDefinitionManager _contentDefinitionManager;
        private readonly ISession _session;
        protected readonly IHtmlLocalizer H;
        private readonly INotifier _notifier;
        private readonly IUpdateModelAccessor _updateModelAccessor;

        public AdminController(
            ISession session,
            IContentManager contentManager,
            IAuthorizationService authorizationService,
            IContentItemDisplayManager contentItemDisplayManager,
            IContentDefinitionManager contentDefinitionManager,
            INotifier notifier,
            IHtmlLocalizer<AdminController> localizer,
            IUpdateModelAccessor updateModelAccessor)
        {
            _contentManager = contentManager;
            _authorizationService = authorizationService;
            _contentItemDisplayManager = contentItemDisplayManager;
            _contentDefinitionManager = contentDefinitionManager;
            _session = session;
            _notifier = notifier;
            _updateModelAccessor = updateModelAccessor;
            H = localizer;
        }

        public async Task<IActionResult> Create(string id, string taxonomyContentItemId, string taxonomyItemId)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }

            if (_contentDefinitionManager.GetTypeDefinition(id) == null)
            {
                return NotFound();
            }

            if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageTaxonomies))
            {
                return Forbid();
            }

            var contentItem = await _contentManager.NewAsync(id);
            contentItem.Weld<TermPart>();
            contentItem.Alter<TermPart>(t => t.TaxonomyContentItemId = taxonomyContentItemId);

            dynamic model = await _contentItemDisplayManager.BuildEditorAsync(contentItem, _updateModelAccessor.ModelUpdater, true);

            model.TaxonomyContentItemId = taxonomyContentItemId;
            model.TaxonomyItemId = taxonomyItemId;

            return View(model);
        }

        [HttpPost]
        [ActionName("Create")]
        public async Task<IActionResult> CreatePost(string id, string taxonomyContentItemId, string taxonomyItemId)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }

            if (_contentDefinitionManager.GetTypeDefinition(id) == null)
            {
                return NotFound();
            }

            if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageTaxonomies))
            {
                return Forbid();
            }

            ContentItem taxonomy;

            var contentTypeDefinition = _contentDefinitionManager.GetTypeDefinition("Taxonomy");

            if (!contentTypeDefinition.IsDraftable())
            {
                taxonomy = await _contentManager.GetAsync(taxonomyContentItemId, VersionOptions.Latest);
            }
            else
            {
                taxonomy = await _contentManager.GetAsync(taxonomyContentItemId, VersionOptions.DraftRequired);
            }

            if (taxonomy == null)
            {
                return NotFound();
            }

            var contentItem = await _contentManager.NewAsync(id);
            contentItem.Weld<TermPart>();
            contentItem.Alter<TermPart>(t => t.TaxonomyContentItemId = taxonomyContentItemId);

            dynamic model = await _contentItemDisplayManager.UpdateEditorAsync(contentItem, _updateModelAccessor.ModelUpdater, true);

            if (!ModelState.IsValid)
            {
                model.TaxonomyContentItemId = taxonomyContentItemId;
                model.TaxonomyItemId = taxonomyItemId;

                return View(model);
            }

            if (taxonomyItemId == null)
            {
                // Use the taxonomy as the parent if no target is specified.
                taxonomy.Alter<TaxonomyPart>(part => part.Terms.Add(contentItem));
            }
            else
            {
                // Look for the target taxonomy item in the hierarchy.
                var parentTaxonomyItem = FindTaxonomyItem(taxonomy.As<TaxonomyPart>(), taxonomyItemId);

                // Couldn't find targeted taxonomy item.
                if (parentTaxonomyItem == null)
                {
                    return NotFound();
                }

                if (parentTaxonomyItem.Terms is not JsonArray taxonomyItems)
                {
                    taxonomyItems = new JsonArray();
                    parentTaxonomyItem["Terms"] = taxonomyItems;
                }

                taxonomyItems.Add(JsonSerializer.SerializeToNode(contentItem));
            }

            _session.Save(taxonomy);

            return RedirectToAction(nameof(Edit), "Admin", new { area = "OrchardCore.Contents", contentItemId = taxonomyContentItemId });
        }

        public async Task<IActionResult> Edit(string taxonomyContentItemId, string taxonomyItemId)
        {
            if (string.IsNullOrWhiteSpace(taxonomyContentItemId) || string.IsNullOrWhiteSpace(taxonomyItemId))
            {
                return NotFound();
            }

            var taxonomy = await _contentManager.GetAsync(taxonomyContentItemId, VersionOptions.Latest);

            if (taxonomy == null)
            {
                return NotFound();
            }

            if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageTaxonomies, taxonomy))
            {
                return Forbid();
            }

            // Look for the target taxonomy item in the hierarchy.
            var taxonomyItem = FindTaxonomyItem(taxonomy.As<TaxonomyPart>(), taxonomyItemId);

            // Couldn't find targeted taxonomy item.
            if (taxonomyItem == null)
            {
                return NotFound();
            }

            var contentItem = taxonomyItem.Deserialize<ContentItem>();
            contentItem.Weld<TermPart>();
            contentItem.Alter<TermPart>(t => t.TaxonomyContentItemId = taxonomyContentItemId);

            dynamic model = await _contentItemDisplayManager.BuildEditorAsync(contentItem, _updateModelAccessor.ModelUpdater, false);

            model.TaxonomyContentItemId = taxonomyContentItemId;
            model.TaxonomyItemId = taxonomyItemId;

            return View(model);
        }

        [HttpPost]
        [ActionName("Edit")]
        public async Task<IActionResult> EditPost(string taxonomyContentItemId, string taxonomyItemId)
        {
            if (string.IsNullOrWhiteSpace(taxonomyContentItemId) || string.IsNullOrWhiteSpace(taxonomyItemId))
            {
                return NotFound();
            }

            if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageTaxonomies))
            {
                return Forbid();
            }

            ContentItem taxonomy;

            var contentTypeDefinition = _contentDefinitionManager.GetTypeDefinition("Taxonomy");

            if (!contentTypeDefinition.IsDraftable())
            {
                taxonomy = await _contentManager.GetAsync(taxonomyContentItemId, VersionOptions.Latest);
            }
            else
            {
                taxonomy = await _contentManager.GetAsync(taxonomyContentItemId, VersionOptions.DraftRequired);
            }

            if (taxonomy == null)
            {
                return NotFound();
            }

            // Look for the target taxonomy item in the hierarchy.
            var taxonomyItem = FindTaxonomyItem(taxonomy.As<TaxonomyPart>(), taxonomyItemId);

            // Couldn't find targeted taxonomy item.
            if (taxonomyItem == null)
            {
                return NotFound();
            }

            var existing = taxonomyItem.Deserialize<ContentItem>();

            // Create a new item to take into account the current type definition.
            var contentItem = await _contentManager.NewAsync(existing.ContentType);

            contentItem.ContentItemId = existing.ContentItemId;
            contentItem.Merge(existing);
            contentItem.Weld<TermPart>();
            contentItem.Alter<TermPart>(t => t.TaxonomyContentItemId = taxonomyContentItemId);

            dynamic model = await _contentItemDisplayManager.UpdateEditorAsync(contentItem, _updateModelAccessor.ModelUpdater, false);

            if (!ModelState.IsValid)
            {
                model.TaxonomyContentItemId = taxonomyContentItemId;
                model.TaxonomyItemId = taxonomyItemId;

                return View(model);
            }

            taxonomyItem.Merge(contentItem.Content, new JsonMergeSettings
            {
                MergeArrayHandling = MergeArrayHandling.Replace,
                MergeNullValueHandling = MergeNullValueHandling.Merge
            });

            // Merge doesn't copy the properties.
            taxonomyItem[nameof(ContentItem.DisplayText)] = contentItem.DisplayText;

            _session.Save(taxonomy);

            return RedirectToAction(nameof(Edit), "Admin", new { area = "OrchardCore.Contents", contentItemId = taxonomyContentItemId });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string taxonomyContentItemId, string taxonomyItemId)
        {
            if (string.IsNullOrWhiteSpace(taxonomyContentItemId) || string.IsNullOrWhiteSpace(taxonomyItemId))
            {
                return NotFound();
            }

            if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageTaxonomies))
            {
                return Forbid();
            }

            ContentItem taxonomy;

            var contentTypeDefinition = _contentDefinitionManager.GetTypeDefinition("Taxonomy");

            if (!contentTypeDefinition.IsDraftable())
            {
                taxonomy = await _contentManager.GetAsync(taxonomyContentItemId, VersionOptions.Latest);
            }
            else
            {
                taxonomy = await _contentManager.GetAsync(taxonomyContentItemId, VersionOptions.DraftRequired);
            }

            if (taxonomy == null)
            {
                return NotFound();
            }

            // Look for the target taxonomy item in the hierarchy.
            var taxonomyItem = FindTaxonomyItem(taxonomy.As<TaxonomyPart>(), taxonomyItemId);

            // Couldn't find targeted taxonomy item.
            if (taxonomyItem == null)
            {
                return NotFound();
            }

            taxonomyItem.Remove();
            _session.Save(taxonomy);

            await _notifier.SuccessAsync(H["Taxonomy item deleted successfully."]);

            return RedirectToAction(nameof(Edit), "Admin", new { area = "OrchardCore.Contents", contentItemId = taxonomyContentItemId });
        }

        private JsonObject FindTaxonomyItem(ContentElement contentItem, string taxonomyItemId) =>
            FindTaxonomyItem((JsonObject)contentItem.Content, taxonomyItemId);

        private JsonObject FindTaxonomyItem(JsonObject data, string taxonomyItemId)
        {
            if (data["ContentItemId"]?.GetValue<string>() == taxonomyItemId)
            {
                return data;
            }

            if (data["Terms"] is not JsonArray taxonomyItems)
            {
                return null;
            }

            foreach (var taxonomyItem in taxonomyItems.Cast<JsonObject>())
            {
                // Search in inner taxonomy items.
                var result = FindTaxonomyItem(taxonomyItem, taxonomyItemId);

                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }
    }
}
