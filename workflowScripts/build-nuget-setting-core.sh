#!/bin/bash
PROJECT=SettingCore/SettingCore.csproj
CONFIGURATION=Debug
OUTDIR=artifacts

MAJOR=1
MINOR=0
PATCH=$(git rev-list --count HEAD)

VERSION=$MAJOR.$MINOR.$PATCH

dotnet clean $PROJECT
rm -rf SettingCore/bin SettingCore/obj artifacts

dotnet restore --force $PROJECT

dotnet build --no-restore -c $CONFIGURATION $PROJECT

dotnet pack --no-restore --no-build --nologo \
    $PROJECT \
    -o $OUTDIR \
    -p:Version=$VERSION

