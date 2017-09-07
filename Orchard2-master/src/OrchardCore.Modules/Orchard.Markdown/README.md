# Markdown (Orchard.Markdown)

## Theming

These shapes are available for theming

### MarkdownPart

This shape is rendered when a `MarkdownPart` is attached to a content item.
The shape based class is of `MarkdownPartViewModel`.

The following properties are available on the `MarkdownPartViewModel` class.

| Property | Type | Description |
| --------- | ---- |------------ |
| `Markdown` | `string` | The Markdown value once all tokens have been processed |
| `ContentItem` | `ContentItem` | The content item of the part |
| `Html` | `string` | The HTML content resulting from the Mardkown source |
| `MarkdownPart` | `MarkdownPart` | The `MarkdownPart` instance|
| `TypePartSettings` | `MarkdownPartSettings` | The settings of the part |

### MarkdownField

This shape is rendered when a `MarkdownField` is attached to a content part.
The shape based class is of `MarkdownFieldViewModel`.

The following properties are available on the `MarkdownFieldViewModel` class.

| Property | Type | Description |
| --------- | ---- |------------ |
| `Markdown` | `string` | The Markdown value once all tokens have been processed |
| `Html` | `string` | The HTML content resulting from the Mardkown source |
| `Field` | `MarkdownField` | The `MarkdownField` instance|
| `Part` | `ContentPart` | The part this field attached to |
| `PartFieldDefinition` | `ContentPartFieldDefinition` | The part field definition |

## Editors

The __Markdown Part__ editor can be different for each content type. In the __Markdown Part__ settings of a 
content type, just select the one that needs to be used.

There are two predefined editor names:
- `Default` is the editor that is used by default
- `Wysiwyg` is the editor that provides a WYSIWYG experience

### Custom Editors

Customizing the editor can mean to replace the predefined one with different experiences, or to provide
new options for the user to choose from.

To create a new custom editor, it is required to provide two shape templates, one to provide
the name of the editor (optional if you want to override and existing one), and a shape to
render the actual HTML for the editor.

#### Declaration

To declare a new editor, create a shape named `Markdown_Option__{Name}` where `{Name}` is a value 
of your choosing. This will be represented by a file named `Markdown-{Name}.Option.cshtml`.

Sample content:

```csharp
@{
    string currentEditor = Model.Editor;
}
<option value="Wysiwyg" selected="@(currentEditor == "Wysiwyg")">@T["Wysiwyg editor"]</option>
```

#### HTML Editor

To define what HTML to render when the editor is selected from the settings, a shape named 
`Markdown_Editor__{Name}` corresponding to a file `Markdown-{Name}.Editor.cshtml` can be created.

Sample content:

```csharp
@using Orchard.Markdown.ViewModels;
@model MarkdownPartViewModel

<fieldset class="form-group">
    <label asp-for="Markdown">@T["Markdown"]</label>
    <textarea asp-for="Markdown" rows="5" class="form-control"></textarea>
    <span class="hint">@T["The markdown of the content item."]</span>
</fieldset>
```

### Overriding the predefined editors

You can override the HTML editor for the `Default` editor by creating a shape file named 
`Markdown.Editor.cshtml`. The Wysiwyg editor is defined by using the file named 
`Markdown-Wysiwyg.Editor.cshtml`.

## CREDITS

### Markdig
https://github.com/lunet-io/markdig
Copyright (c) 2016, Alexandre Mutel
BSD-2

### SimpleMDE
https://github.com/NextStepWebs/simplemde-markdown-editor
Copyright (c) 2015 Next Step Webs, Inc.
MIT