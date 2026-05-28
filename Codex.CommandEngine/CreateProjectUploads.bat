@echo off
setlocal

title Codex.CommandEngine Project Upload Builder

powershell.exe -NoProfile -ExecutionPolicy Bypass -File "%~dp0CreateProjectUploads.ps1"

echo.
pause
