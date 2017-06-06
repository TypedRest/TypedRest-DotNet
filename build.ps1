Param ([Parameter(Mandatory=$True)][string]$Version)
$ErrorActionPreference = "Stop"

pushd "$(Split-Path -Path $MyInvocation.MyCommand.Definition -Parent)"

dotnet msbuild /t:Restore /t:Build /p:Configuration=Release /p:Version=$Version
dotnet test --configuration Release --no-build "UnitTests/UnitTests.csproj"

popd
