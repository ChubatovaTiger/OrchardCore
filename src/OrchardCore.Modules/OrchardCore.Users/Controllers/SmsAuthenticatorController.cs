using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Fluid.Values;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using OrchardCore.Admin;
using OrchardCore.DisplayManagement.Notify;
using OrchardCore.Entities;
using OrchardCore.Liquid;
using OrchardCore.Modules;
using OrchardCore.Settings;
using OrchardCore.Sms.Abstractions;
using OrchardCore.Users.Events;
using OrchardCore.Users.Models;
using OrchardCore.Users.Services;
using OrchardCore.Users.ViewModels;

namespace OrchardCore.Users.Controllers;

[Authorize, Feature(UserConstants.Features.SmsAuthenticator)]
public class SmsAuthenticatorController : TwoFactorAuthenticationBaseController
{
    private readonly IUserService _userService;
    private readonly ISmsService _smsService;
    private readonly ILiquidTemplateManager _liquidTemplateManager;
    private readonly HtmlEncoder _htmlEncoder;

    public SmsAuthenticatorController(
        SignInManager<IUser> signInManager,
        UserManager<IUser> userManager,
        ISiteService siteService,
        IHtmlLocalizer<AccountController> htmlLocalizer,
        IStringLocalizer<AccountController> stringLocalizer,
        IOptions<TwoFactorOptions> twoFactorOptions,
        INotifier notifier,
        IDistributedCache distributedCache,
        IUserService userService,
        ISmsService smsService,
        ILiquidTemplateManager liquidTemplateManager,
        HtmlEncoder htmlEncoder,
        ITwoFactorAuthenticationHandlerCoordinator twoFactorAuthenticationHandlerCoordinator)
        : base(
            userManager,
            distributedCache,
            signInManager,
            twoFactorAuthenticationHandlerCoordinator,
            notifier,
            siteService,
            htmlLocalizer,
            stringLocalizer,
            twoFactorOptions)
    {
        _userService = userService;
        _smsService = smsService;
        _liquidTemplateManager = liquidTemplateManager;
        _htmlEncoder = htmlEncoder;
    }

    [Admin]
    public async Task<IActionResult> Index()
    {
        var user = await UserManager.GetUserAsync(User);
        if (user == null)
        {
            return UserNotFound();
        }

        if (await UserManager.IsPhoneNumberConfirmedAsync(user))
        {
            return RedirectToTwoFactorIndex();
        }

        return View();
    }

    [HttpPost, Admin]
    public async Task<IActionResult> RequestCode()
    {
        var user = await UserManager.GetUserAsync(User);
        if (user == null)
        {
            return UserNotFound();
        }

        if (await UserManager.IsPhoneNumberConfirmedAsync(user))
        {
            return RedirectToTwoFactorIndex();
        }

        var phoneNumber = await UserManager.GetPhoneNumberAsync(user);

        var code = await UserManager.GenerateChangePhoneNumberTokenAsync(user, phoneNumber);

        var setings = (await SiteService.GetSiteSettingsAsync()).As<SmsAuthenticatorLoginSettings>();
        var message = new SmsMessage()
        {
            To = phoneNumber,
            Body = await GetMessageAsync(setings, user, code),
        };

        if (await _smsService.SendAsync(message))
        {
            await Notifier.SuccessAsync(H["We have successfully sent an verification code to your phone number. Please retrieve the code from your device."]);
        }
        else
        {
            await Notifier.ErrorAsync(H["We are unable to send you an SMS message at this time. Please try again later."]);

            return RedirectToAction(nameof(Index));
        }

        var model = new EnableEmailAuthenticatorViewModel();

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> ValidateCode(EnableSmsAuthenticatorViewModel model)
    {
        var user = await UserManager.GetUserAsync(User);
        if (user == null)
        {
            return UserNotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var phoneNumber = await UserManager.GetPhoneNumberAsync(user);
        var result = await UserManager.VerifyChangePhoneNumberTokenAsync(user, StripToken(model.Code), phoneNumber);
        if (result)
        {
            await EnableTwoFactorAuthenticationAsync(user);

            await Notifier.SuccessAsync(H["Your phone number has been confirmed."]);

            return await RedirectToTwoFactorAsync(user);
        }

        await Notifier.ErrorAsync(H["Unable to confirm your phone at this time. Please try again."]);

        return View(nameof(RequestCode), model);
    }

    [HttpPost, Produces("application/json"), AllowAnonymous]
    public async Task<IActionResult> SendCode()
    {
        var user = await SignInManager.GetTwoFactorAuthenticationUserAsync();
        var errorMessage = S["The SMS message could not be sent. Please attempt to request the code at a later time."];

        if (user == null)
        {
            return BadRequest(new
            {
                success = false,
                message = errorMessage.Value,
            });
        }

        var settings = (await SiteService.GetSiteSettingsAsync()).As<SmsAuthenticatorLoginSettings>();
        var code = await UserManager.GenerateTwoFactorTokenAsync(user, TokenOptions.DefaultPhoneProvider);

        var message = new SmsMessage()
        {
            To = await UserManager.GetEmailAsync(user),
            Body = await GetMessageAsync(settings, user, code),
        };

        var result = await _smsService.SendAsync(message);

        return Ok(new
        {
            success = result,
            message = result ? S["A verification code has been sent to your phone number. Please check your device for the code."].Value
            : errorMessage.Value,
        });
    }

    private Task<string> GetMessageAsync(SmsAuthenticatorLoginSettings settings, IUser user, string code)
        => String.IsNullOrWhiteSpace(settings.Body)
        ? Task.FromResult(EmailAuthenticatorLoginSettings.DefaultBody)
        : GetContentAsync(settings.Body, user, code);

    private async Task<string> GetContentAsync(string message, IUser user, string code)
    {
        var result = await _liquidTemplateManager.RenderHtmlContentAsync(message, _htmlEncoder, null,
            new Dictionary<string, FluidValue>()
            {
                ["User"] = new ObjectValue(user),
                ["Code"] = new StringValue(code),
            });

        using var writer = new StringWriter();
        result.WriteTo(writer, _htmlEncoder);

        return writer.ToString();
    }
}
