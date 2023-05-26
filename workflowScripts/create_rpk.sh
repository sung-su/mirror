#!/bin/bash

csproj_file=SettingMainGadget/SettingMainGadget/SettingMainGadget.csproj
sed -i '/^\s*<Target Name="PostBuild".*/d' $csproj_file
sed -i '/^\s*<Exec Command.*/d' $csproj_file
sed -i '/^\s*<\/Target.*/d' $csproj_file
build_targets=SettingMainGadget/SettingMainGadget/Directory.Build.targets
sed -i '/^\s*<Exec Command=.*/d' $build_targets

dotnet build SettingMainGadget/

rm -rf SettingMainGadget/SettingMainGadget/create_rpk
mkdir SettingMainGadget/SettingMainGadget/create_rpk
cp -pr SettingMainGadget/SettingMainGadget/res/ SettingMainGadget/SettingMainGadget/create_rpk
cp SettingMainGadget/SettingMainGadget/bin/Debug/netcoreapp3.1/SettingMainGadget.dll SettingMainGadget/SettingMainGadget/create_rpk/res/allowed/
cp -pr SettingMainGadget/SettingMainGadget/bin/Debug/netcoreapp3.1/en-US/ SettingMainGadget/SettingMainGadget/create_rpk/res/allowed/
cp -pr SettingMainGadget/SettingMainGadget/bin/Debug/netcoreapp3.1/ko-KR/ SettingMainGadget/SettingMainGadget/create_rpk/res/allowed/
cp SettingMainGadget/SettingMainGadget/tizen-manifest.xml SettingMainGadget/SettingMainGadget/create_rpk/

/home/user/tizen-studio/tools/ide/bin/tizen package -t rpk -- SettingMainGadget/SettingMainGadget/create_rpk -s platform_profile
