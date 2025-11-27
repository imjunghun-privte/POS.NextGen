Write-Host "=== Configure Visual Studio Settings ===" -ForegroundColor Cyan
$settingsDir = Join-Path $env:APPDATA "Microsoft\VisualStudio\Settings"
if (!(Test-Path $settingsDir)) { New-Item -ItemType Directory -Path $settingsDir | Out-Null }
$vsSettingsFile = Join-Path $settingsDir "POS.VSSettings.json"
$json = @{
 Editor=@{TabSize=4;IndentSize=4;InsertSpaces=$true;TrimTrailingWhitespace=$true;InsertFinalNewline=$true;}
 CSharp=@{OrganizeUsingsOnFormat=$true;FormatOnSave=$true;SortUsings=$true;}
} | ConvertTo-Json -Depth 10
$json | Set-Content -Path $vsSettingsFile -Encoding UTF8
Write-Host "✓ VS Settings Written"
