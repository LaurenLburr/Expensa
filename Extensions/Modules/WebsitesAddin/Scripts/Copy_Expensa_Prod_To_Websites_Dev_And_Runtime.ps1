
function Resolve-DefaultWebsitesDevDatabasePath {
    $scriptFolder = Split-Path -Parent $PSCommandPath
    $moduleFolder = Split-Path -Parent $scriptFolder

    return Join-Path $moduleFolder "DevDatabase\websites.current.db"
}

function Resolve-DefaultRuntimeDatabasePath {
    return Join-Path $env:APPDATA "Expensa\Extensions\Runtime\WebsitesAddin\websites.current.db"
}

function New-ArchivedDatabaseName {
    param(
        [string] $Folder,
        [string] $Prefix
    )

    $timestamp = Get-Date -Format "yyyyMMdd_HHmmss"

    return Join-Path $Folder "$Prefix.$timestamp.db"
}

# Example archive name:
# websites.current.before-prod-copy.20260601_143522.db
