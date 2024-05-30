namespace OrchardCore.Testing.Tests;

public class UseCultureAttributeTests
{
    [Fact]
    public void UsesSuppliedCultureAndUICulture()
    {
        // Arrange
        var culture = "de-DE";
        var uiCulture = "fr-CA";

        // Act
        var usedCulture = new UseCultureAttribute(culture, uiCulture);

        // Assert
        Assert.Equal(new CultureInfo(culture), usedCulture.Culture);
        Assert.Equal(new CultureInfo(uiCulture), usedCulture.UICulture);
    }

    [Fact]
    public void UseCultureBeforeAndAfterTest()
    {
        // Arrange
        var originalCulture = CultureInfo.CurrentCulture;
        var originalUICulture = CultureInfo.CurrentUICulture;
        var culture = "de-DE";
        var uiCulture = "fr-CA";
        var usedCulture = new UseCultureAttribute(culture, uiCulture);

        // Act
        usedCulture.Before(methodUnderTest: null);

        // Assert
        Assert.Equal(new CultureInfo(culture), CultureInfo.CurrentCulture);
        Assert.Equal(new CultureInfo(uiCulture), CultureInfo.CurrentUICulture);

        // Act
        usedCulture.After(methodUnderTest: null);

        // Assert
        Assert.Equal(originalCulture, CultureInfo.CurrentCulture);
        Assert.Equal(originalUICulture, CultureInfo.CurrentUICulture);
    }

    [Fact]
    [UseCulture("ar-YE")]
    public void UseCultureAttributeUsesSuppliedCulture()
    {
        // Assert
        Assert.Equal(new CultureInfo("ar-YE"), CultureInfo.CurrentCulture);
        Assert.Equal(new CultureInfo("ar-YE"), CultureInfo.CurrentUICulture);
    }

    [Fact]
    [UseCulture("ar-YE", "ar-SA")]
    public void UseCultureAttributeUsesSuppliedCultureAndUICulture()
    {
        // Assert
        Assert.Equal(new CultureInfo("ar-YE"), CultureInfo.CurrentCulture);
        Assert.Equal(new CultureInfo("ar-SA"), CultureInfo.CurrentUICulture);
    }
}
