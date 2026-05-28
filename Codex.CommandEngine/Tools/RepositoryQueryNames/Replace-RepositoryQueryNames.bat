@echo off
setlocal

powershell -NoProfile -ExecutionPolicy Bypass -File "D:\Git\CodexExpensa\Codex.CommandEngine\Tools\RepositoryQueryNames\Replace-RepositoryQueryNames.ps1"

pause
