param(
    [string]$SolutionRoot = "$(Split-Path -Parent $MyInvocation.MyCommand.Path)"
)

Write-Host "=== POS NextGen Snippet Installer ===" -ForegroundColor Cyan

$vsSnippetSource = Join-Path $SolutionRoot "../snippets/vs"
$vscodeSnippetSource = Join-Path $SolutionRoot "../snippets/vscode"

$vsUserSnippetPath = Join-Path $env:USERPROFILE "Documents\Visual Studio 2026\Code Snippets\Visual C#\My Code Snippets"
$vscodePath = Join-Path $env:APPDATA "Code\User\snippets"

if (!(Test-Path $vsUserSnippetPath)) { New-Item -ItemType Directory -Path $vsUserSnippetPath | Out-Null }
Copy-Item "$vsSnippetSource\*.snippet" $vsUserSnippetPath -Force

if (!(Test-Path $vscodePath)) { New-Item -ItemType Directory -Path $vscodePath | Out-Null }
Copy-Item "$vscodeSnippetSource\*" $vscodePath -Force

Write-Host "✓ Snippet install completed" -ForegroundColor Green
