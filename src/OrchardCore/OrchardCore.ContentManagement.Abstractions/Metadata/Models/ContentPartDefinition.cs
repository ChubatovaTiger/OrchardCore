using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;

namespace OrchardCore.ContentManagement.Metadata.Models
{
    public class ContentPartDefinition : ContentDefinition
    {
        public ContentPartDefinition(string name)
        {
            Name = name;
            Fields = Enumerable.Empty<ContentPartFieldDefinition>();
            Settings = new JsonObject();
        }

        public ContentPartDefinition(string name, IEnumerable<ContentPartFieldDefinition> fields, JsonObject settings)
        {
            Name = name;
            Fields = fields.ToList();
            Settings = new JsonObject(settings);

            foreach (var field in Fields)
            {
                field.PartDefinition = this;
            }
        }

        public IEnumerable<ContentPartFieldDefinition> Fields { get; private set; }
    }
}
