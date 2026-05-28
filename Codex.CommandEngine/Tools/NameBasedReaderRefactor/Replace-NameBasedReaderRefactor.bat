@echo off
setlocal

powershell -NoProfile -ExecutionPolicy Bypass -File "D:\Git\CodexExpensa\Codex.CommandEngine\Tools\NameBasedReaderRefactor\Replace-NameBasedReaderRefactor.ps1"

pause
