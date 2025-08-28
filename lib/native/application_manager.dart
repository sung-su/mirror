
import 'dart:ffi';
import 'package:flutter/foundation.dart';
import 'package:tizen_fs/models/app_info.dart';
import 'package:ffi/ffi.dart';
import 'package:tizen_interop/6.0/tizen.dart';

class ApplicationManager {
  static List<AppInfo> appInfos = [];

  static bool _getappInfowithFilter(Pointer<app_info_s> app_info, Pointer<Void> user_data) {
    final appInfo = app_info.cast<app_info_s>();
    using((Arena arena) {

      AppInfo appinfo = AppInfo();
      final pId = arena<Pointer<Char>>();
      if (tizen.app_info_get_app_id(appInfo, pId) == 0) {
        appinfo.appId = pId.value.toDartString();
      }

      final label = arena<Pointer<Char>>();
      if(tizen.app_info_get_label(app_info, label) == 0) {
        appinfo.name = label.value.toDartString();
      }

      final pkgName = arena<Pointer<Char>>();
      if(tizen.app_info_get_package(app_info, pkgName) == 0) {
        appinfo.packageName = pkgName.value.toDartString();
      }

      final icon = arena<Pointer<Char>>();
      if(tizen.app_info_get_icon(app_info, icon) == 0) {
        appinfo.icon = icon.value.toDartString();
      }

      appInfos.add(appinfo);
    });
    return true;
  }

  static Future<List<AppInfo>> _loadApps(dynamic _) async {
    using((Arena arena) {
      final filterHandlePtr = arena<app_info_filter_h>();
      int ret = tizen.app_info_filter_create(filterHandlePtr);
      if(ret != 0) return false;

      final filterHandle = filterHandlePtr.value;
      ret = tizen.app_info_filter_add_bool(filterHandle, PACKAGE_INFO_PROP_APP_NODISPLAY.toNativeChar(), false);
      if(ret != 0) return false;

      final filterCallback = Pointer.fromFunction<app_info_filter_cbFunction>(_getappInfowithFilter, false);
      ret = tizen.app_info_filter_foreach_appinfo(
        filterHandle,
        filterCallback,
        nullptr
      );
      if(ret != 0) return false;

      tizen.app_info_filter_destroy(filterHandle);
      return true;
    });
    return appInfos;
  }

  static Future<List<AppInfo>> loadApps() async {
    appInfos.clear();
    final applist = await compute(_loadApps, null);
    return applist;
  }
}