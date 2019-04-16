$ErrorActionPreference = "Stop"
pushd $PSScriptRoot

dotnet test --configuration Release --no-build UnitTests\UnitTests.csproj

popd
