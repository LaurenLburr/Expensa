# Step 2 – Wire TreeView Context

## Update MainForm.cs

Replace your menu click wiring with:

```csharp
menuItem.Click += (s, e) =>
{
    var context = new DefaultCommandContext
    {
        SelectedNode = treeView1.SelectedNode,
        ProjectName = null,
        ActiveDatabasePath = null,
        Services = null
    };

    executor.Execute(command, context);
};
```

## Test Command

In any command (example):

```csharp
public override void Execute(ICommandContext context)
{
    var node = context.SelectedNode as TreeNode;

    MessageBox.Show(node?.Text ?? "No node selected");

    // existing logic continues...
}
```

## Expected Result

- Clicking a node then running a command shows node text
- No crashes
- Existing behavior unchanged