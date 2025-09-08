Name:       org.tizen.tizen-fs
Summary:    org.tizen.tizen-fs
Version:    1.0.0
Release:    1
Group:      N/A
License:    Apache-2.0
Source0:    %{name}-%{version}.tar.gz

%define preload_tpk_path /usr/apps/.preload-rw-tpk

%description
This is a homescreen flutter application

%prep
mkdir -p %{buildroot}%{TZ_SYS_SHARE}/settings
%setup -q

%build

%install
rm -rf %{buildroot}
mkdir -p %{buildroot}/%{preload_tpk_path}
install packaging/%{name}-%{version}.tpk %{buildroot}/%{preload_tpk_path}/

%files
%defattr(-,root,root,-)
%{preload_tpk_path}/*
%manifest %{name}.manifest
%license LICENSE
