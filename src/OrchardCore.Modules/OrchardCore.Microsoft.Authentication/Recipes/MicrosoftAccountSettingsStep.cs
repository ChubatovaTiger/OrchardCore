using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using OrchardCore.Microsoft.Authentication.Services;
using OrchardCore.Microsoft.Authentication.Settings;
using OrchardCore.Recipes.Models;
using OrchardCore.Recipes.Services;

namespace OrchardCore.Microsoft.Authentication.Recipes
{
    /// <summary>
    /// This recipe step sets Microsoft Account settings.
    /// </summary>
    public class MicrosoftAccountSettingsStep : IRecipeStepHandler
    {
        private readonly IMicrosoftAccountService _microsoftAccountService;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public MicrosoftAccountSettingsStep(
            IMicrosoftAccountService microsoftAccountService,
            IOptions<JsonSerializerOptions> jsonSerializerOptions)
        {
            _microsoftAccountService = microsoftAccountService;
            _jsonSerializerOptions = jsonSerializerOptions.Value;
        }

        public async Task ExecuteAsync(RecipeExecutionContext context)
        {
            if (!string.Equals(context.Name, nameof(MicrosoftAccountSettings), StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var model = context.Step.ToObject<MicrosoftAccountSettingsStepModel>(_jsonSerializerOptions);
            var settings = await _microsoftAccountService.LoadSettingsAsync();

            settings.AppId = model.AppId;
            settings.AppSecret = model.AppSecret;
            settings.CallbackPath = model.CallbackPath;

            await _microsoftAccountService.UpdateSettingsAsync(settings);
        }
    }

    public class MicrosoftAccountSettingsStepModel
    {
        public string AppId { get; set; }
        public string AppSecret { get; set; }
        public string CallbackPath { get; set; }
    }
}
