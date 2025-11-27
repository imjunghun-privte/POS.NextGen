param(
    [string]$SolutionRoot = "$(Split-Path -Parent $MyInvocation.MyCommand.Path)"
)

Write-Host "=== Install Dev Environment ===" -ForegroundColor Cyan

# 1. Visual Studio 설정 적용
powershell -ExecutionPolicy Bypass -File (Join-Path $SolutionRoot "Configure-VSSettings.ps1")

# 2. VS Code 설정 처리
$vscodeSrc = Join-Path $SolutionRoot "..\.vscode"
$vscodeDst = Join-Path (Join-Path $SolutionRoot "../") ".vscode"

# 대상과 소스 경로가 동일하면 복사하지 않음
if ($vscodeSrc -ne $vscodeDst) {

    if (!(Test-Path $vscodeDst)) {
        New-Item -ItemType Directory -Path $vscodeDst | Out-Null
    }

    Write-Host "Copying VS Code Workspace Settings..."
    Copy-Item "$vscodeSrc\*" $vscodeDst -Recurse -Force
    Write-Host "✓ VS Code Settings Installed" -ForegroundColor Green
}
else {
    Write-Host "VS Code settings already in place. (Skip copy)" -ForegroundColor Yellow
}

Write-Host "✓ Dev Environment setup completed" -ForegroundColor Green
