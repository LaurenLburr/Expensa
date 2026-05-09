# Extension Dev Host Refactor Step 4 - AI Designer Creates Project Space

## Scope

The AI Add-in Designer now creates the appropriate project space.

## Behavior

From:

```text
Project -> AI Add-in Designer
```

Enter an add-in name and click:

```text
Create Project Space
```

The designer now creates or ensures:

```text
Extensions/Modules/[ProjectName]
Extensions/Modules/[ProjectName]/Docs
Extensions/Modules/[ProjectName]/Docs/CatchUp.md
Extensions/Modules/[ProjectName]/Docs/Addin_Design_Spec.md
Extensions/Modules/[ProjectName]/Docs/Expensa_Integration_Design_Spec.md
```

It also registers the project with the Extension Manager registration store.

## Tree refresh

After a project space is created, `MainForm` rebuilds the tree so the project appears under:

```text
Add-in Projects
```

## Files

```text
UI/AiAddinDesignerPanelForm.cs
UI/AiAddinDesignerPanelForm.Designer.cs
UI/AiAddinDesignerPanelForm.resx
Extensions/MainForm.cs
```
