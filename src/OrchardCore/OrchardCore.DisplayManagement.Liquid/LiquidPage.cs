using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace OrchardCore.DisplayManagement.Liquid
{
    public class LiquidPage : Razor.RazorPage<dynamic>
    {
        public override Task ExecuteAsync()
        {
            if (RenderAsync != null && ViewContext.ExecutingFilePath == LiquidViewsFeatureProvider.DefaultRazorViewPath)
            {
                var viewContextAccessor = Context.RequestServices.GetRequiredService<ViewContextAccessor>();
                viewContextAccessor.ViewContext = ViewContext;

                return RenderAsync(ViewContext.Writer);
            }

            return LiquidViewTemplate.RenderAsync(this);
        }

        public System.Func<TextWriter, Task> RenderAsync { get; set; }
    }
}
