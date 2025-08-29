@echo off
echo Starting Aviator Predictor...
echo.

REM Check if the application is already built
if not exist "bin\Release\net6.0-windows\Aviator-Hack.exe" (
    echo Building application first...
    "C:\Program Files\dotnet\dotnet.exe" build --configuration Release
    if %errorlevel% neq 0 (
        echo Build failed! Please check for errors.
        pause
        exit /b 1
    )
)

echo Launching Aviator Predictor...
echo.
echo No login required - direct access to prediction tool!
echo.

REM Run the application
"C:\Program Files\dotnet\dotnet.exe" run --configuration Release

echo.
echo Application closed.
pause
