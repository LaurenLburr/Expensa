# MainForm Wiring for AI Add-in Designer

Add this command registration in `MainForm.RegisterCommands()`:

```csharp
_commandRegistry.Register(new OpenAiAddinDesignerCommand());
```

Add this under the `AI` tree node in `BuildNavigationTree()`:

```csharp
ai.Nodes.Add(CreateCommandNode("AI Add-in Designer", "Tools.AiAddinDesigner"));
```

Expected tree:

```text
AI
├── AI Add-in Designer
├── AI Test
└── AI Scaffold Files
```
