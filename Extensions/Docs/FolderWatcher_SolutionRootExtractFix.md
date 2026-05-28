# Folder Watcher Solution Root Extract Fix

## Problem

The watcher was extracting to:

```text
D:\Git\CodexExpensa\Extensions
```

But the generated zip is rooted like:

```text
Extensions\Host\...
```

That means the correct extraction target is the solution root:

```text
D:\Git\CodexExpensa
```

## Fix

Changed the default Extract To folder to:

```text
D:\Git\CodexExpensa
```

The default Watch Folder remains:

```text
D:\Git\CodexExpensa\Extensions\AI_Replies
```

## Result

Zip entry:

```text
Extensions\Host\CodexExpensa.ExtensionDevHost\UI\File.cs
```

extracts to:

```text
D:\Git\CodexExpensa\Extensions\Host\CodexExpensa.ExtensionDevHost\UI\File.cs
```

## Important

If your settings file already exists, it may still contain the old value. Update the Extract To box once in the UI to:

```text
D:\Git\CodexExpensa
```

It will persist after that.
