#!/bin/sh
set -e
cd `dirname $0`

# Find dotnet
if command -v dotnet > /dev/null 2> /dev/null; then
    dotnet="dotnet"
else
    dotnet="../0install.sh run --version 9.0.200..!9.1 https://apps.0install.net/dotnet/sdk.xml"
fi

# Unit tests (without .NET Framework)
$dotnet test --no-build --logger trx --configuration Release --framework net9.0 UnitTests/UnitTests.csproj
