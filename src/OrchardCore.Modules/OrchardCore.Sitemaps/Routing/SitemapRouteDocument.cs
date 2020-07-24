using System;
using System.Collections.Generic;
using MessagePack;
using Newtonsoft.Json;
using OrchardCore.Data.Documents;

namespace OrchardCore.Sitemaps.Routing
{
    public class SitemapRouteDocument : Document, IMessagePackSerializationCallbackReceiver
    {
        [IgnoreMember]
        public Dictionary<string, string> SitemapIds { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        [JsonIgnore]
        public Dictionary<string, string> SitemapIdsValues { get; set; }

        public Dictionary<string, string> SitemapPaths { get; set; } = new Dictionary<string, string>();

        // 'MessagePack' doesn't preserve the 'OrdinalIgnoreCase' comparison when deserializing.
        public virtual void OnAfterDeserialize() => SitemapIds = new Dictionary<string, string>(SitemapIdsValues, StringComparer.OrdinalIgnoreCase);
        public virtual void OnBeforeSerialize() => SitemapIdsValues = SitemapIds;
    }
}
