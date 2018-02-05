using System.Threading.Tasks;
using OrchardCore.ContentManagement;
using OrchardCore.Contents.Workflows.Activities;
using OrchardCore.Contents.Workflows.ViewModels;
using OrchardCore.DisplayManagement.ModelBinding;
using OrchardCore.DisplayManagement.Views;
using OrchardCore.Workflows.Display;
using OrchardCore.Workflows.Models;

namespace OrchardCore.Contents.Workflows.Drivers
{
    public class PublishContentTaskDisplay : ActivityDisplayDriver<PublishContentTask>
    {
        public override IDisplayResult Display(PublishContentTask activity)
        {
            return Combine(
                Shape("PublishContentTask_Fields_Thumbnail", activity).Location("Thumbnail", "Content"),
                Shape("PublishContentTask_Fields_Design", activity).Location("Design", "Content")
            );
        }

        public override IDisplayResult Edit(PublishContentTask activity)
        {
            return Shape<PublishContentTaskViewModel>("PublishContentTask_Fields_Edit", model =>
            {
                model.Expression = activity.Content.Expression;
            }).Location("Content");
        }

        public async override Task<IDisplayResult> UpdateAsync(PublishContentTask activity, IUpdateModel updater)
        {
            var viewModel = new PublishContentTaskViewModel();
            if (await updater.TryUpdateModelAsync(viewModel, Prefix, x => x.Expression))
            {
                activity.Content = new WorkflowExpression<IContent>(viewModel.Expression);
            }
            return Edit(activity);
        }
    }
}
