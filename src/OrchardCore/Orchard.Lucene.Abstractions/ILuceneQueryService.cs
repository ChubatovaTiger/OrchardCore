using Lucene.Net.Search;
using Newtonsoft.Json.Linq;

namespace Orchard.Lucene
{
    public interface ILuceneQueryService
    {
        TopDocs Search(LuceneQueryContext context, JObject queryObj);
        Query CreateQueryFragment(LuceneQueryContext context, JObject queryObj);
    }
}