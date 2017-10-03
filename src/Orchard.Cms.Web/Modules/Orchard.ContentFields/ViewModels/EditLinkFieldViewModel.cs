﻿using Orchard.ContentFields.Fields;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Metadata.Models;

namespace Orchard.ContentFields.ViewModels
{
    public class EditLinkFieldViewModel
    {
        public string Value { get; set; }
        public string Text { get; set; }
        public string Target { get; set; }
        public LinkField Field { get; set; }
        public ContentPart Part { get; set; }
        public ContentPartFieldDefinition PartFieldDefinition { get; set; }
    }
}
