# Main Navigation Tree Persistent Expansion Cleanup

This replacement removes the duplicate expansion-state helper methods and keeps one persistent implementation.

Expanded node names are saved to:

```text
%APPDATA%\Expensa\Extensions\MainFormSettings.json
```

The main tree restores by stable `TreeNode.Name`.
