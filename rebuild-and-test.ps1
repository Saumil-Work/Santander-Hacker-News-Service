# Complete rebuild and test script
Write-Host "Starting complete rebuild and test..." -ForegroundColor Cyan

# Navigate to solution directory
Set-Location "C:\Users\saumi\source\repos\Santander-Hacker-News-Service"

# Step 1: Kill any running dotnet processes
Write-Host "Stopping any running dotnet processes..." -ForegroundColor Yellow
Get-Process dotnet -ErrorAction SilentlyContinue | Stop-Process -Force

# Step 2: Remove build artifacts
Write-Host "Removing build artifacts..." -ForegroundColor Yellow
Remove-Item -Recurse -Force ".\.vs" -ErrorAction SilentlyContinue
Remove-Item -Recurse -Force ".\*\bin" -ErrorAction SilentlyContinue
Remove-Item -Recurse -Force ".\*\obj" -ErrorAction SilentlyContinue

# Step 3: Clean solution
Write-Host "Running dotnet clean..." -ForegroundColor Yellow
dotnet clean

# Step 4: Restore packages
Write-Host "Restoring NuGet packages..." -ForegroundColor Yellow
dotnet restore

# Step 5: Build solution
Write-Host "Building solution..." -ForegroundColor Yellow
$buildResult = dotnet build
if ($LASTEXITCODE -ne 0) {
	Write-Host "Build failed!" -ForegroundColor Red
	exit 1
}

# Step 6: Run tests
Write-Host "`nRunning tests..." -ForegroundColor Cyan
dotnet test --verbosity normal --no-build

if ($LASTEXITCODE -eq 0) {
	Write-Host "`n✓ All tests passed!" -ForegroundColor Green
}
else {
	Write-Host "`n✗ Some tests failed. See output above." -ForegroundColor Red
	exit 1
}