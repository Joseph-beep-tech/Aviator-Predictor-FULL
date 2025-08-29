@echo off
echo Building Aviator Predictor...
echo.

REM Check for .NET SDK
where dotnet >nul 2>&1
if %errorlevel% equ 0 (
    echo .NET SDK found, building with dotnet...
    dotnet build --configuration Release
    goto :end
)

REM Check for MSBuild
where msbuild >nul 2>&1
if %errorlevel% equ 0 (
    echo MSBuild found, building with MSBuild...
    msbuild Aviator-Hack.csproj /p:Configuration=Release
    goto :end
)

REM Check for Visual Studio
if exist "C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe" (
    echo Visual Studio 2019 Community found...
    "C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe" Aviator-Hack.csproj /p:Configuration=Release
    goto :end
)

if exist "C:\Program Files (x86)\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe" (
    echo Visual Studio 2022 Community found...
    "C:\Program Files (x86)\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe" Aviator-Hack.csproj /p:Configuration=Release
    goto :end
)

if exist "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe" (
    echo Visual Studio 2022 Community found...
    "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe" Aviator-Hack.csproj /p:Configuration=Release
    goto :end
)

echo No build tools found!
echo Please install one of the following:
echo - .NET 6.0 SDK
echo - Visual Studio 2019/2022
echo - MSBuild tools
echo.
echo You can download .NET 6.0 SDK from: https://dotnet.microsoft.com/download/dotnet/6.0
echo.

:end
echo.
echo Build process completed.
pause
