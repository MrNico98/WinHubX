[CmdletBinding(SupportsShouldProcess = $true, ConfirmImpact = 'Medium')]
param(
    [Parameter(Mandatory)]
    [string]$Path
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$resolved = (Resolve-Path -LiteralPath $Path).Path
$document = New-Object System.Xml.XmlDocument
$document.PreserveWhitespace = $true
$document.Load($resolved)
$root = $document.DocumentElement
$seen = @{}
$dataNodes = @($root.SelectNodes('./data'))
$removedCount = 0

for ($index = $dataNodes.Count - 1; $index -ge 0; $index--) {
    $node = $dataNodes[$index]
    $name = $node.GetAttribute('name')
    if ($seen.ContainsKey($name)) {
        [void]$root.RemoveChild($node)
        $removedCount++
    } else {
        $seen[$name] = $true
    }
}

if ($removedCount -gt 0 -and $PSCmdlet.ShouldProcess($resolved, "Remove $removedCount duplicate RESX entries")) {
    $settings = New-Object System.Xml.XmlWriterSettings
    $settings.Indent = $false
    $settings.OmitXmlDeclaration = $false
    $settings.Encoding = New-Object System.Text.UnicodeEncoding($false, $true)
    $temporary = Join-Path ([System.IO.Path]::GetDirectoryName($resolved)) ([System.IO.Path]::GetRandomFileName())
    try {
        $writer = [System.Xml.XmlWriter]::Create($temporary, $settings)
        try { $document.Save($writer) } finally { $writer.Dispose() }
        Move-Item -LiteralPath $temporary -Destination $resolved -Force
    } finally {
        if (Test-Path -LiteralPath $temporary) { Remove-Item -LiteralPath $temporary -Force }
    }
}
