@echo off
cd /d "%~dp0"

::Visual Studio 2015 build environment
if not defined VS140COMNTOOLS goto err_no_vs
call "%VS140COMNTOOLS%vsvars32.bat"

::Compile Visual Studio solution
nuget restore TypedRest.sln
msbuild TypedRest.sln /nologo /t:Rebuild /p:Configuration=Release
if errorlevel 1 pause

::Create NuGet package
nuget pack TypedRest.csproj -Symbols -Properties Configuration=Release
if errorlevel 1 pause

goto end
rem Error messages

:err_no_vs
echo ERROR: No Visual Studio 2015 installation found. >&2
pause
goto end

:end
