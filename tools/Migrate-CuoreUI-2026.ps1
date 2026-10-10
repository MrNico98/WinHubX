[CmdletBinding(SupportsShouldProcess = $true, ConfirmImpact = 'Medium')]
param(
    [string]$Root = (Join-Path $PSScriptRoot '..\Project\WinHubX')
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
$obsolete = 'ImageAutoCenter|ImageOffset|TextOffset|CheckedSymbolColor|UncheckedSymbolColor'

Get-ChildItem -LiteralPath $Root -Recurse -Filter '*.Designer.cs' | ForEach-Object {
    $path = $_.FullName
    $content = Get-Content -Raw -LiteralPath $path
    $updated = [regex]::Replace($content, "(?m)^\s*[^\r\n]*\.($obsolete)\s*=.*\r?\n", '')
    if ($updated -cne $content -and $PSCmdlet.ShouldProcess($path, 'Remove obsolete Designer properties')) {
        [System.IO.File]::WriteAllText($path, $updated, [System.Text.UTF8Encoding]::new($false))
    }
}
