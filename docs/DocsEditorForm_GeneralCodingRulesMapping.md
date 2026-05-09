# DocsEditorForm Mapping Needed

Add this line to `DocsEditorForm.SelectDocumentByTag(...)`:

```csharp
"Docs.GeneralCodingRules" => "General_Coding_Rules.md",
```

Example:

```csharp
string? fileName = docTag switch
{
    "Docs.ExtensionManagerCatchUp" => "ExtensionManager_CatchUp_Latest.md",
    "Docs.GeneralCodingSpec" => "General_Coding_Spec.md",
    "Docs.GeneralCodingRules" => "General_Coding_Rules.md",
    "Docs.AddinDesignSpecTemplate" => "Addin_Design_Spec_Template.md",
    "Docs.ExpensaIntegrationSpecTemplate" => "Expensa_Integration_Design_Spec_Template.md",
    "Docs.AiAddinDesignWorkflow" => "AI_Addin_Design_Workflow.md",
    _ => null
};
```

Once this mapping exists, selecting `Docs > General Coding Rules` opens the Markdown editor.

The existing `AI Review` button in `DocsEditorForm` sends the document text to AI and updates the editor. Click `Save` after reviewing the AI changes.
