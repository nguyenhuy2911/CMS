using Orchard.ContentManagement;
using Orchard.ContentManagement.Display.ContentDisplay;
using Orchard.DisplayManagement.ModelBinding;
using Orchard.DisplayManagement.Views;

namespace Orchard.ContentPreview.Drivers
{
    public class ContentPreviewDriver : ContentDisplayDriver
    {
        public override IDisplayResult Edit(ContentItem contentItem, IUpdateModel updater)
        {
            return Shape("ContentPreview_Button", contentItem).Location("Actions:after");
        }
    }
}