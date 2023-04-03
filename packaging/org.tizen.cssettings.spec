Name:       org.tizen.cssettings
Summary:    org.tizen.cssettings
Version:    1.1.5
Release:    1
Group:      N/A
License:    Apache-2.0
Source0:    %{name}-%{version}.tar.gz

BuildRequires:  pkgconfig(libtzplatform-config)
Requires(post):  /usr/bin/tpk-backend

%define viewer_name org.tizen.SettingView
%define gadgets_name org.tizen.settings.main
%define preload_tpk_path %{TZ_SYS_RO_APP}/.preload-tpk
%define preload_rpk_path %{TZ_SYS_RO_APP}/.preload-rpk

%description
This is a NUI Setting Application including viewer and internal gadgets

%prep
%setup -q

%build

%install
rm -rf %{buildroot}
mkdir -p %{buildroot}/%{preload_tpk_path}
mkdir -p %{buildroot}/%{preload_rpk_path}
install packaging/%{gadgets_name}-%{version}.rpk %{buildroot}/%{preload_rpk_path}/
install packaging/%{viewer_name}-%{version}.tpk %{buildroot}/%{preload_tpk_path}/

%post

%files
%defattr(-,root,root,-)
%{preload_tpk_path}/*
%{preload_rpk_path}/*
%license LICENSE
