@echo off
pushd "%~dp0"

::Visual Studio 2015 build environment
if not defined VS140COMNTOOLS (
  echo ERROR: No Visual Studio 2015 installation found. >&2
  popd
  exit /b 1
)
call "%VS140COMNTOOLS%vsvars32.bat"

::Compile Visual Studio solution
nuget restore TypedRest.sln
msbuild TypedRest.sln /nologo /t:Rebuild /p:Configuration=Release
if errorlevel 1 pause

::Create NuGet packages
mkdir build\Packages
nuget pack TypedRest\TypedRest.csproj -Properties Configuration=Release -IncludeReferencedProjects -Symbols -OutputDirectory build\Packages
if errorlevel 1 pause
nuget pack TypedRest.CommandLine\TypedRest.CommandLine.csproj -Properties Configuration=Release -IncludeReferencedProjects -Symbols -OutputDirectory build\Packages
if errorlevel 1 pause
nuget pack TypedRest.Wpf\TypedRest.Wpf.csproj -Properties Configuration=Release -IncludeReferencedProjects -Symbols -OutputDirectory build\Packages
if errorlevel 1 pause

pushd templates
call build.cmd
popd

popd