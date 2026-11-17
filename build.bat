@echo off
setlocal enabledelayedexpansion

echo ========================================================
echo               PoeRedux - Build Executable
echo ========================================================
echo.

:: Check if dotnet CLI is available
where dotnet >nul 2>&1
if %ERRORLEVEL% neq 0 (
    echo [ERROR] .NET SDK was not found in PATH.
    echo Please install the .NET 10 SDK: https://dotnet.microsoft.com/download
    echo.
    set "EXIT_CODE=1"
    goto :error
)

set "CONFIGURATION=Release"
set "RUNTIME=win-x64"
set "OUTPUT_DIR=%~dp0publish"
set "PROJECT_PATH=%~dp0PoeRedux\PoeRedux.csproj"

echo [INFO] Project: %PROJECT_PATH%
echo [INFO] Configuration: %CONFIGURATION%
echo [INFO] Runtime: %RUNTIME%
echo [INFO] Output: %OUTPUT_DIR%\PoeRedux.exe
echo.

echo [INFO] Publishing self-contained single-file executable...
dotnet publish "%PROJECT_PATH%" -c %CONFIGURATION% -r %RUNTIME% --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o "%OUTPUT_DIR%"
set "EXIT_CODE=%ERRORLEVEL%"

if %EXIT_CODE% neq 0 (
    echo.
    echo [ERROR] dotnet publish failed with exit code %EXIT_CODE%.
    goto :error
)

echo.
echo ========================================================
echo [SUCCESS] Build completed successfully!
echo [OUTPUT] %OUTPUT_DIR%\PoeRedux.exe
echo ========================================================
echo.
goto :end

:error
echo ========================================================
echo [FAILED] Build process terminated with errors.
echo ========================================================
echo.

:end
if "%~1"=="" pause
exit /b %EXIT_CODE%
