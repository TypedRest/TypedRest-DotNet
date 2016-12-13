#!/bin/sh
cd `dirname $0`

#Handle Windows-style paths in project files
export MONO_IOMAP=all

#Avoid "access denied" errors on minibuildd
if [ ! -d "$HOME" ]; then
  mkdir work
  export HOME=`pwd`/work
fi

echo Restoring NuGet packages...
nuget restore -NonInteractive

echo Compiling solution...
xbuild /nologo /tv:12.0 /p:TargetFrameworkVersion=v4.5 /t:Rebuild /p:Configuration=Release /p:DeployOnBuild=True /p:PublishProfile=Release

if [ -d work ]; then
  rm -rf work
fi
