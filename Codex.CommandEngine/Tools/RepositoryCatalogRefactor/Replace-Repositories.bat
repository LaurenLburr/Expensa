@echo off
setlocal

powershell -NoProfile -ExecutionPolicy Bypass -File "D:\Git\CodexExpensa\Codex.CommandEngine\Tools\RepositoryCatalogRefactor\Replace-Repositories.ps1"

pause
