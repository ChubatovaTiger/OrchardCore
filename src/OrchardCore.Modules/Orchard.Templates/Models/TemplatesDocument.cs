using System.Collections.Generic;

namespace Orchard.Templates.Models
{
    public class TemplatesDocument
    {
        public Dictionary<string, Template> Templates { get; } = new Dictionary<string, Template>();
    }

    public class Template
    {
        public string Content { get; set; }
        public string Description { get; set; }
    }
}
