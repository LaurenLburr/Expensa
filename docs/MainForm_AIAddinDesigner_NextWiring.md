# Next Step: Wire AI Add-in Designer into MainForm

## Goal

The `AI Add-in Designer` form and command files have been added, but the command must be registered and added to the main TreeView.

## Project

```text
CodexExpensa.ExtensionDevHost
```

## File to modify

```text
Extensions/Host/CodexExpensa.ExtensionDevHost/MainForm.cs
```

---

## 1. Register the command

Find:

```csharp
private void RegisterCommands()
{
```

Add this inside the method with the other `_commandRegistry.Register(...)` calls:

```csharp
_commandRegistry.Register(new OpenAiAddinDesignerCommand());
```

Recommended placement:

```csharp
_commandRegistry.Register(new OpenAiApiKeyCommand());
_commandRegistry.Register(new AiTestCommand());
_commandRegistry.Register(new AiScaffoldFilesCommand());
_commandRegistry.Register(new OpenAiAddinDesignerCommand());
```

---

## 2. Add the TreeView node

Find the section where the `AI` tree node is built. It should look similar to this:

```csharp
TreeNode ai = new("AI");
ai.Nodes.Add(CreateCommandNode("AI Test", "Tools.AiTest"));
ai.Nodes.Add(CreateCommandNode("AI Scaffold Files", "Tools.AiScaffoldFiles"));
```

Change it to:

```csharp
TreeNode ai = new("AI");
ai.Nodes.Add(CreateCommandNode("AI Add-in Designer", "Tools.AiAddinDesigner"));
ai.Nodes.Add(CreateCommandNode("AI Test", "Tools.AiTest"));
ai.Nodes.Add(CreateCommandNode("AI Scaffold Files", "Tools.AiScaffoldFiles"));
```

---

## 3. Expected tree

After rebuilding and running:

```text
AI
├── AI Add-in Designer
├── AI Test
└── AI Scaffold Files
```

---

## 4. Test workflow

1. Rebuild solution.
2. Run `CodexExpensa.ExtensionDevHost`.
3. Open:

```text
AI → AI Add-in Designer
```

4. Click `New Session`.
5. Enter an add-in idea, for example:

```text
Create an add-in for tracking useful websites by tag. It should integrate with Expensa navigation and have project-specific docs.
```

6. Click `Send to AI`.

---

## 5. If it does not appear

Check these two things:

### Command registration exists

```csharp
_commandRegistry.Register(new OpenAiAddinDesignerCommand());
```

### Tree node exists

```csharp
ai.Nodes.Add(CreateCommandNode("AI Add-in Designer", "Tools.AiAddinDesigner"));
```

---

## 6. Notes

The designer feature currently supports:

- saved AI design sessions
- conversation history
- editable add-in design spec
- editable Expensa integration spec
- export docs into the project folder

It does **not** generate code yet. That should be the next milestone after the design workflow is stable.
