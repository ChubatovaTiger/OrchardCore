using System;
using System.Threading.Tasks;
using OrchardCore.DisplayManagement.Handlers;
using OrchardCore.DisplayManagement.ModelBinding;
using OrchardCore.DisplayManagement.Views;
using OrchardCore.Navigation;

namespace OrchardCore.Contents.AdminNodes
{
    public class ContentTypesAdminNodeDriver : DisplayDriver<MenuItem, ContentTypesAdminNode>
    {
        public override IDisplayResult Display(ContentTypesAdminNode treeNode)
        {
            return Combine(
                View("ContentTypesAdminNode_Fields_TreeSummary", treeNode).Location("TreeSummary", "Content"),
                View("ContentTypesAdminNode_Fields_TreeThumbnail", treeNode).Location("TreeThumbnail", "Content")
            );
        }

        public override IDisplayResult Edit(ContentTypesAdminNode treeNode)
        {
            return Initialize<ContentTypesAdminNodeViewModel>("ContentTypesAdminNode_Fields_TreeEdit", model =>
            {
                model.ShowAll = treeNode.ShowAll;
                model.ContentTypes = treeNode.ContentTypes;
            }).Location("Content");
        }

        public override async Task<IDisplayResult> UpdateAsync(ContentTypesAdminNode treeNode, IUpdateModel updater)
        {
            // Initializes the value to empty otherwise the model is not updated if no type is selected.
            treeNode.ContentTypes = Array.Empty<string>();

            var model = new ContentTypesAdminNodeViewModel();

            if (await updater.TryUpdateModelAsync(model, Prefix, x => x.ShowAll, x => x.ContentTypes)) {

                treeNode.ShowAll = model.ShowAll;
                treeNode.ContentTypes = model.ContentTypes;
            };

            return Edit(treeNode);
        }
    }
}
