# Main Navigation Tree Persistent Expansion State

Main tree expanded node names are now stored in:

```text
%APPDATA%\Expensa\Extensions\MainFormSettings.json
```

Behavior:
- First run defaults collapsed unless saved state exists.
- Expand/collapse updates the saved state.
- Rebuilding and restarting restores expanded nodes by stable `TreeNode.Name`.
