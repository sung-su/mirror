Name:       org.tizen.cssettings
Summary:    org.tizen.cssettings
Version:    1.1.19
Release:    1
Group:      N/A
License:    Apache-2.0
Source0:    %{name}-%{version}.tar.gz

BuildRequires:  pkgconfig(libtzplatform-config)
Requires(post):  /usr/bin/tpk-backend

%define viewer_name org.tizen.cssettings
%define gadgets_name org.tizen.settings.main
%define preload_tpk_path %{TZ_SYS_RO_APP}/.preload-tpk
%define preload_rpk_path %{TZ_SYS_RO_APP}/.preload-rpk

%description
This is a NUI Setting Application including viewer and internal gadgets

%prep
mkdir -p %{buildroot}%{TZ_SYS_SHARE}/settings
%setup -q

%build

%install
rm -rf %{buildroot}
mkdir -p %{buildroot}/%{preload_tpk_path}
mkdir -p %{buildroot}/%{preload_rpk_path}
mkdir -p %{buildroot}/%{TZ_SYS_GLOBALUSER_DATA}/settings/
install packaging/%{gadgets_name}-%{version}.rpk %{buildroot}/%{preload_rpk_path}/
install packaging/%{viewer_name}-%{version}.tpk %{buildroot}/%{preload_tpk_path}/
cp -r resource/media/settings %{buildroot}%{TZ_SYS_GLOBALUSER_DATA}/

%post

#resetSound
DEFAULT_CALL_TONE="%{TZ_SYS_SHARE}/settings/Ringtones/ringtone_sdk.mp3"
DEFAULT_NOTI_TONE="%{TZ_SYS_SHARE}/Alerts/General notification_sdk.wav"

#resetImages
DEFAULT_HOME="%{TZ_SYS_SHARE}/Wallpapers/home_001.png"
DEFAULT_LOCK="%{TZ_SYS_SHARE}/Wallpapers/home_003.png"

%files
%defattr(-,root,root,-)
%{preload_tpk_path}/*
%{preload_rpk_path}/*
%{TZ_SYS_GLOBALUSER_DATA}/settings/*
%manifest org.tizen.cssettings.manifest
%license LICENSE
