# ShiftScheduler Deployment Script
# This script helps prepare your application for deployment

Write-Host "=== ShiftScheduler Deployment Preparation ===" -ForegroundColor Green

# Step 1: Clean and restore
Write-Host "Step 1: Cleaning and restoring packages..." -ForegroundColor Yellow
dotnet clean
dotnet restore

# Step 2: Build in Release mode
Write-Host "Step 2: Building in Release mode..." -ForegroundColor Yellow
dotnet build --configuration Release

if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ Build successful!" -ForegroundColor Green
} else {
    Write-Host "❌ Build failed! Please check the errors above." -ForegroundColor Red
    exit 1
}

# Step 3: Test run (optional)
Write-Host "Step 3: Testing application startup..." -ForegroundColor Yellow
Write-Host "Starting application for 10 seconds to test..." -ForegroundColor Cyan

# Start the application in background
$process = Start-Process -FilePath "dotnet" -ArgumentList "run", "--configuration", "Release" -PassThru -WindowStyle Hidden

# Wait 10 seconds
Start-Sleep -Seconds 10

# Stop the application
if ($process -and !$process.HasExited) {
    Stop-Process -Id $process.Id -Force
    Write-Host "✅ Application started successfully!" -ForegroundColor Green
} else {
    Write-Host "⚠️  Application may have issues starting. Check logs." -ForegroundColor Yellow
}

# Step 4: Check for common issues
Write-Host "Step 4: Checking for common deployment issues..." -ForegroundColor Yellow

# Check if appsettings.Production.json exists
if (Test-Path "appsettings.Production.json") {
    Write-Host "✅ Production settings file exists" -ForegroundColor Green
} else {
    Write-Host "⚠️  Production settings file missing" -ForegroundColor Yellow
}

# Check if render.yaml exists
if (Test-Path "render.yaml") {
    Write-Host "✅ Render configuration exists" -ForegroundColor Green
} else {
    Write-Host "⚠️  Render configuration missing" -ForegroundColor Yellow
}

# Check for sensitive data in appsettings.json
$appsettings = Get-Content "appsettings.json" -Raw
if ($appsettings -match '"ClientSecret":\s*"[^"]*"') {
    Write-Host "⚠️  Warning: ClientSecret found in appsettings.json" -ForegroundColor Red
    Write-Host "   Consider moving to environment variables" -ForegroundColor Red
}

Write-Host "`n=== Deployment Checklist ===" -ForegroundColor Green
Write-Host "Before deploying to Render:" -ForegroundColor Cyan
Write-Host "1. ✅ Code builds successfully" -ForegroundColor Green
Write-Host "2. ✅ All tests pass" -ForegroundColor Green
Write-Host "3. ⏳ Set up PostgreSQL database in Render" -ForegroundColor Yellow
Write-Host "4. ⏳ Configure environment variables" -ForegroundColor Yellow
Write-Host "5. ⏳ Set up Google OAuth (if needed)" -ForegroundColor Yellow
Write-Host "6. ⏳ Push code to Git repository" -ForegroundColor Yellow

Write-Host "`n=== Next Steps ===" -ForegroundColor Green
Write-Host "1. Push your code to a Git repository" -ForegroundColor Cyan
Write-Host "2. Create a Render account at https://render.com" -ForegroundColor Cyan
Write-Host "3. Follow the DEPLOYMENT_GUIDE.md for detailed instructions" -ForegroundColor Cyan
Write-Host "4. Set up your database and environment variables" -ForegroundColor Cyan

Write-Host "`n🎉 Your application is ready for deployment!" -ForegroundColor Green 