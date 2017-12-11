using System;
using System.Collections.Generic;
using System.Linq;
using GraphQL.Types;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Metadata;

namespace OrchardCore.Contents.GraphQL.Mutations.Types
{
    public class CreateContentItemInputType : InputObjectGraphType<ContentItem>
    {
        public CreateContentItemInputType()
        {
            Field(ci => ci.ContentType, false);
            Field(ci => ci.Author, true);
            Field(ci => ci.Owner, true);

            Field(ci => ci.Published, true);
            Field(ci => ci.Latest, true);

            Field<ContentPartsInputType>("ContentParts");
        }
    }

    public class ContentPartsInputType : InputObjectGraphType
    {
        public ContentPartsInputType(
            IServiceProvider serviceProvider,
            IContentDefinitionManager contentDefinitionManager,
            IEnumerable<ContentPart> _contentParts,
            IEnumerable<IInputObjectGraphType> inputGraphTypes)
        {
            Name = "ContentPartsInput";

            foreach (var contentPartDefinition in contentDefinitionManager.ListPartDefinitions())
            {
                var partName = contentPartDefinition.Name; // BagPart

                var contentPart = _contentParts.FirstOrDefault(x => x.GetType().Name == partName);

                if (contentPart != null)
                {
                    var inputGraphType =
                        typeof(InputObjectGraphType<>).MakeGenericType(contentPart.GetType());

                    var inputGraphTypeResolved = (IInputObjectGraphType)serviceProvider.GetService(inputGraphType);

                    if (inputGraphTypeResolved != null)
                    {
                        var name = contentPart.GetType().Name; // About

                        Field(
                            inputGraphTypeResolved.GetType(),
                            name);
                    }
                }
            }
        }
    }
}
