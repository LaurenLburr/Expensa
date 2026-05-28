# Design Preview Handle Fix

## Problem

`LoadWorkspace()` called:

```csharp
BeginInvoke(new MethodInvoker(RenderDesignSpecPreview));
```

before the form had a window handle.

That caused:

```text
InvalidOperationException:
Invoke or BeginInvoke cannot be called on a control until the window handle has been created.
```

## Fix

Added handle-aware preview rendering.

The form now:

- tracks whether preview rendering is pending
- waits for `HandleCreated`
- only renders immediately when the form handle already exists
- avoids calling `BeginInvoke` before handle creation

## Files

```text
UI/DesignSpecConversationForm.cs
UI/DesignSpecConversationForm.Designer.cs
UI/DesignSpecConversationForm.resx
```
