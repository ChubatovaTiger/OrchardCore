using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using OrchardCore.Recipes.Models;
using OrchardCore.Recipes.Services;
using OrchardCore.Sitemaps.Models;
using OrchardCore.Sitemaps.Services;

namespace OrchardCore.Sitemaps.Recipes
{
    /// <summary>
    /// This recipe step creates a set of sitemaps.
    /// </summary>
    public class SitemapsStep : IRecipeStepHandler
    {
        private static readonly JsonSerializerOptions _options = new()
        {
            TypeNameHandling = TypeNameHandling.Auto
        };

        private readonly ISitemapManager _sitemapManager;

        public SitemapsStep(ISitemapManager sitemapManager)
        {
            _sitemapManager = sitemapManager;
        }

        public async Task ExecuteAsync(RecipeExecutionContext context)
        {
            if (!string.Equals(context.Name, "Sitemaps", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var model = context.GetStep<SitemapStepModel>();

            foreach (var token in model.Data.Cast<JsonObject>())
            {
                var sitemap = token.Deserialize<SitemapType>(_options);
                await _sitemapManager.UpdateSitemapAsync(sitemap);
            }
        }

        public class SitemapStepModel
        {
            public JsonArray Data { get; set; }
        }
    }
}
