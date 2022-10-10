Name:       org.tizen.cssettings
Summary:    org.tizen.cssettings
Version:    1.0.0
Release:    1
Group:      N/A
License:    Apache-2.0
Source0:    %{name}-%{version}.tar.gz

BuildRequires:  pkgconfig(libtzplatform-config)
Requires(post):  /usr/bin/tpk-backend

%define viewer_name org.tizen.SettingView
%define widgets_name org.tizen.cssettings
%define preload_tpk_path %{TZ_SYS_RO_APP}/.preload-tpk

%description
This is a NUI Setting Application including viewer and internal widgets

%prep
%setup -q

%build

%install
rm -rf %{buildroot}
mkdir -p %{buildroot}/%{preload_tpk_path}
install packaging/%{viewer_name}-%{version}.tpk %{buildroot}/%{preload_tpk_path}/
install packaging/%{widgets_name}-%{version}.tpk %{buildroot}/%{preload_tpk_path}/

%post

%files
%defattr(-,root,root,-)
%{preload_tpk_path}/*
%license LICENSE
