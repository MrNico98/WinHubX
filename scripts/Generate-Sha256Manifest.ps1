[CmdletBinding()]
param(
    [string]$RepositoryRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path,
    [string]$OutputPath = (Join-Path (Resolve-Path (Join-Path $PSScriptRoot '..')).Path 'docs/SHA256-SOURCES.json')
)

$ErrorActionPreference = 'Stop'
$extensions = @('.cs', '.csproj', '.resx', '.json', '.ps1', '.cmd', '.yml', '.yaml', '.manifest', '.config')
$relativeOutputPath = [IO.Path]::GetRelativePath($RepositoryRoot, $OutputPath).Replace('\', '/').ToLowerInvariant()

Push-Location $RepositoryRoot
try {
    $commit = (git rev-parse HEAD).Trim()
    $files = git ls-files | Where-Object {
        $extension = [IO.Path]::GetExtension($_).ToLowerInvariant()
        $extensions -contains $extension -and $_.Replace('\', '/').ToLowerInvariant() -ne $relativeOutputPath
    }

    $entries = foreach ($relativePath in $files) {
        $fullPath = Join-Path $RepositoryRoot $relativePath
        if (-not (Test-Path -LiteralPath $fullPath -PathType Leaf)) {
            throw "File versionato non trovato: $relativePath"
        }

        $hash = (Get-FileHash -LiteralPath $fullPath -Algorithm SHA256).Hash.ToLowerInvariant()
        [ordered]@{
            path = $relativePath.Replace('\', '/')
            sha256 = $hash
        }
    }

    $manifest = [ordered]@{
        algorithm = 'SHA-256'
        commit = $commit
        generatedAtUtc = [DateTime]::UtcNow.ToString('O')
        files = @($entries)
    }

    $parent = Split-Path -Parent $OutputPath
    New-Item -ItemType Directory -Path $parent -Force | Out-Null
    $manifest | ConvertTo-Json -Depth 4 | Set-Content -LiteralPath $OutputPath -Encoding utf8NoBOM
}
finally {
    Pop-Location
}
