Param ($Version = "1.0-dev")
$ErrorActionPreference = "Stop"
pushd $PSScriptRoot

dotnet msbuild -t:Restore -t:Build -p:Configuration=Release -p:Version=$Version

popd
