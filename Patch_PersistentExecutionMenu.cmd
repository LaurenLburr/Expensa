@echo off
setlocal
title Patch Persistent Execution Menu
powershell -NoProfile -ExecutionPolicy Bypass -File "%~dp0Patch_PersistentExecutionMenu.ps1"
exit /b %errorlevel%
