#!/bin/sh
set -e
cd `dirname $0`

dotnet clean
dotnet msbuild -t:Restore -t:Build -p:Configuration=Release -p:Version=$1
