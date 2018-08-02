using GraphQL.Types;
using OrchardCore.Title.Model;

namespace OrchardCore.Title.GraphQL
{
    public class TitleQueryObjectType : ObjectGraphType<TitlePart>
    {
        public TitleQueryObjectType()
        {
            Name = "TitlePart";

            Field(x => x.Title);
        }
    }
}
