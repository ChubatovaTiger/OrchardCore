# Email (`OrchardCore.Email`)

This module facilitates the configuration of email settings and will automatically activate upon demand when utilizing at least one email provider service. For further details, refer to the documentation for the [Azure Email](../Email.Azure/README.md) and [SMTP Email](../Email.Smtp/README.md) modules.

## Email Settings

Enabling the `OrchardCore.Email` module will allow the user to set the following settings:

| Setting | Description |
| --- | --- |
| `DefaultSender` | The email of the sender.

!!! note
    When [OrchardCore.Email.Azure](../Email.Azure/README.md) or [OrchardCore.Email.Smtp](../Email.Smtp/README.md) is enabled you can override the `DefaultSender` setting from the module-specific settings, otherwise, it will fall back to this setting.

## Email Settings Configuration

The `OrchardCore.Email` module allows the user to use configuration values to override the settings configured from the admin area by calling the `ConfigureEmailSettings()` extension method on `OrchardCoreBuilder` when initializing the app.

The following configuration values can be customized:

```json
    "OrchardCore_Email": {
      "DefaultSender": "",
    }
```

For more information please refer to [Configuration](../../core/Configuration/README.md).
