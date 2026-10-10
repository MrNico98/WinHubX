[CmdletBinding()]
param(
    [Parameter(Mandatory)]
    [string]$ArtifactDirectory,

    [string]$OutputFileName = 'SHA256SUMS.json'
)

$ErrorActionPreference = 'Stop'

$artifactRoot = (Resolve-Path -LiteralPath $ArtifactDirectory).Path
if (-not (Test-Path -LiteralPath $artifactRoot -PathType Container)) {
    throw "La directory artefatto non esiste: $artifactRoot"
}

if ([IO.Path]::GetFileName($OutputFileName) -ne $OutputFileName -or
    [string]::IsNullOrWhiteSpace($OutputFileName)) {
    throw 'Il nome del manifesto deve essere un semplice nome file.'
}

$manifestPath = Join-Path $artifactRoot $OutputFileName
$entries = @(
    Get-ChildItem -LiteralPath $artifactRoot -File -Recurse |
        Where-Object { $_.FullName -ne $manifestPath } |
        Sort-Object { [IO.Path]::GetRelativePath($artifactRoot, $_.FullName) } |
        ForEach-Object {
            $entry = [ordered]@{
                path = [IO.Path]::GetRelativePath($artifactRoot, $_.FullName).Replace('\', '/')
                sha256 = (Get-FileHash -LiteralPath $_.FullName -Algorithm SHA256).Hash.ToLowerInvariant()
                sizeBytes = $_.Length
            }

            if ([IO.Path]::GetExtension($_.Name) -in @('.exe', '.dll', '.msi', '.msix', '.msixbundle', '.appx', '.appxbundle')) {
                $signature = Get-AuthenticodeSignature -LiteralPath $_.FullName
                if ($signature.Status -notin @([Management.Automation.SignatureStatus]::Valid, [Management.Automation.SignatureStatus]::NotSigned)) {
                    throw "Firma Authenticode non valida per '$($_.Name)': $($signature.Status) — $($signature.StatusMessage)"
                }

                $entry.authenticodeStatus = $signature.Status.ToString()
                if ($signature.Status -eq [Management.Automation.SignatureStatus]::Valid) {
                    $entry.signerSubject = $signature.SignerCertificate.Subject
                    $entry.signerThumbprint = $signature.SignerCertificate.Thumbprint
                }
            }

            $entry
        }
)

if ($entries.Count -eq 0) {
    throw 'La directory artefatto non contiene file da verificare.'
}

$commit = $env:GITHUB_SHA
if ([string]::IsNullOrWhiteSpace($commit)) {
    $repositoryRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
    $commit = (git -C $repositoryRoot rev-parse HEAD).Trim()
}

$manifest = [ordered]@{
    algorithm = 'SHA-256'
    commit = $commit
    generatedAtUtc = [DateTime]::UtcNow.ToString('O')
    files = $entries
}

$temporaryPath = "$manifestPath.$([Guid]::NewGuid().ToString('N')).tmp"
try {
    $json = ConvertTo-Json -InputObject $manifest -Depth 5
    [IO.File]::WriteAllText($temporaryPath, $json, [Text.UTF8Encoding]::new($false))
    [IO.File]::Move($temporaryPath, $manifestPath, $true)
}
finally {
    if (Test-Path -LiteralPath $temporaryPath) {
        Remove-Item -LiteralPath $temporaryPath -Force
    }
}
