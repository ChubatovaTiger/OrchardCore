namespace OrchardCore.Email.Azure.ViewModels;

public class AzureEmailSettingsViewModel
{
    [EmailAddress]
    public string DefaultSender { get; set; }

    public string ConnectionString { get; set; }

    public bool ConfigurationExists { get; set; }

    public bool FileConfigurationExists { get; set; }

    public bool PreventAdminSettingsOverride { get; set; }
}
