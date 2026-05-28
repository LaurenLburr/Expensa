@echo off
setlocal
title Patch Persisted Record Actions Menu
powershell -NoProfile -ExecutionPolicy Bypass -File "%~dp0Patch_PersistedRecordActionsMenu.ps1"
exit /b %errorlevel%
