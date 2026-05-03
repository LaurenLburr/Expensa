# MainForm ApplySafeSplitterLayout Fix

## Problem

`MainForm.cs` referenced `ApplySafeSplitterLayout()` from the constructor events, but the method was missing.

## Fix

Added the missing method back to `MainForm.cs`.
