using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrchardCore.Environment.Extensions;
using OrchardCore.Environment.Extensions.Features;

namespace OrchardCore.Testing.Stubs;

public class NullExtensionManager : IExtensionManager
{
    public IEnumerable<IFeatureInfo> GetDependentFeatures(string featureId) => Enumerable.Empty<IFeatureInfo>();

    public IEnumerable<Type> GetExportedExtensionTypes(IExtensionInfo extensionInfo) => Enumerable.Empty<Type>();

    public IExtensionInfo GetExtension(string extensionId) => null;

    public IEnumerable<IExtensionInfo> GetExtensions() => Enumerable.Empty<IExtensionInfo>();

    public IEnumerable<IFeatureInfo> GetFeatureDependencies(string featureId) => Enumerable.Empty<IFeatureInfo>();

    public IEnumerable<IFeatureInfo> GetFeatures() => Enumerable.Empty<IFeatureInfo>();

    public IEnumerable<IFeatureInfo> GetFeatures(string[] featureIdsToLoad) => Enumerable.Empty<IFeatureInfo>();

    public Task<ExtensionEntry> LoadExtensionAsync(IExtensionInfo extensionInfo) => Task.FromResult(new ExtensionEntry());

    public Task<IEnumerable<IFeatureInfo>> LoadFeaturesAsync() => Task.FromResult(Enumerable.Empty<IFeatureInfo>());

    public Task<IEnumerable<IFeatureInfo>> LoadFeaturesAsync(string[] featureIdsToLoad) => Task.FromResult(Enumerable.Empty<IFeatureInfo>());
}
