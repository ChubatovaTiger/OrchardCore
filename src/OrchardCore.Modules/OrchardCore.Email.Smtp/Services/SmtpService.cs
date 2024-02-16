using System;
using System.Linq;
using System.Threading.Tasks;
using OrchardCore.Email.Smtp.Services;

namespace OrchardCore.Email.Services;

/// <summary>
/// Represents a SMTP service that allows to send emails.
/// </summary>
[Obsolete]
public class SmtpService : ISmtpService
{
    private readonly IEmailProviderResolver _emailProviderResolver;

    public SmtpService(IEmailProviderResolver emailProviderResolver)
    {
        _emailProviderResolver = emailProviderResolver;
    }

    public async Task<SmtpResult> SendAsync(MailMessage message)
    {
        var provider = await _emailProviderResolver.GetAsync(SmtpEmailProvider.TechnicalName);

        var result = await provider.SendAsync(message);

        if (result.Succeeded)
        {
            return SmtpResult.Success;
        }

        return SmtpResult.Failed(result.Errors.ToArray());
    }
}
