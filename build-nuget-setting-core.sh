#!/bin/bash
PROJECT=SettingCore/SettingCore.csproj
CONFIGURATION=Debug
OUTDIR=artifacts

VERSION_SUFFIX=$((10000+$(git rev-list --count HEAD)))
VERSION_BASE=1.0.0
VERSION=$VERSION_BASE-$VERSION_SUFFIX

dotnet clean $PROJECT
rm -rf SettingCore/bin SettingCore/obj artifacts

dotnet restore --force $PROJECT

dotnet build --no-restore -c $CONFIGURATION $PROJECT

dotnet pack --no-restore --no-build --nologo \
    $PROJECT \
    -o $OUTDIR \
    -p:Version=$VERSION

