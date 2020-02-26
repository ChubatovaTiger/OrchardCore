using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using Microsoft.Extensions.Localization;

namespace OrchardCore.OpenId.Validators
{
    public static class OpenIdUrlValidator
    {
        private static readonly AsyncLocal<IStringLocalizer> _localizer = new AsyncLocal<IStringLocalizer>();

        private static IStringLocalizer S => _localizer.Value;

        public static IEnumerable<ValidationResult> ValidateUrls(this ValidationContext context, string memberName, string member)
        {
            if (member != null)
            {
                if (_localizer.Value == null)
                {
                    _localizer.Value = (IStringLocalizer)context.GetService(typeof(IStringLocalizer<Startup>));
                }

                foreach (var url in member.Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) || !uri.IsWellFormedOriginalString())
                    {
                        yield return new ValidationResult(S["{0} is not wellformed", url], new[] { memberName });
                    }
                }
            }
        }
    }
}
