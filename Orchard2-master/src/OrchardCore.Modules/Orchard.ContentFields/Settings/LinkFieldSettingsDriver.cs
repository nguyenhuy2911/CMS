﻿using System.Threading.Tasks;
using Orchard.ContentFields.Fields;
using Orchard.ContentManagement.Metadata.Models;
using Orchard.ContentTypes.Editors;
using Orchard.DisplayManagement.Views;

namespace Orchard.ContentFields.Settings
{
    public class LinkFieldSettingsDriver : ContentPartFieldDisplayDriver<LinkField>
    {
        public override IDisplayResult Edit(ContentPartFieldDefinition partFieldDefinition)
        {
            return Shape<LinkFieldSettings>("LinkFieldSettings_Edit", model => partFieldDefinition.Settings.Populate(model))
                .Location("Content");
        }

        public override async Task<IDisplayResult> UpdateAsync(ContentPartFieldDefinition partFieldDefinition, UpdatePartFieldEditorContext context)
        {
            var model = new LinkFieldSettings();

            await context.Updater.TryUpdateModelAsync(model, Prefix);

            context.Builder.MergeSettings(model);

            return Edit(partFieldDefinition);
        }
    }
}
