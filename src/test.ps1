$ErrorActionPreference = "Stop"
pushd $PSScriptRoot

dotnet test --configuration Release --no-build UnitTests\UnitTests.csproj
if ($LASTEXITCODE -ne 0) {throw "Exit Code: $LASTEXITCODE"}

popd
