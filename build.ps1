Param ([Parameter(Mandatory=$True)][string]$Version)
$ErrorActionPreference = "Stop"

pushd "$(Split-Path -Path $MyInvocation.MyCommand.Definition -Parent)"

dotnet clean
dotnet msbuild /t:Restore /p:Configuration=Release /p:Version=$Version
dotnet msbuild /t:Build /p:Configuration=Release /p:Version=$Version
dotnet test --configuration Release --no-build "UnitTests/UnitTests.csproj"

popd
