import 'dart:ffi';
import 'package:flutter/material.dart';
import 'package:ffi/ffi.dart';
import 'package:tizen_interop/9.0/tizen.dart';

void appInfoCallback(Pointer<Void> appInfoHandlePtr, Pointer<Void> userData) {
  final appInfo = appInfoHandlePtr.cast<app_info_s>();

  using((Arena arena) {
    final pId = arena<Pointer<Char>>();
    if (tizen.app_info_get_app_id(appInfo, pId) == 0) {
      final appId = pId.value.toDartString();
      print('App ID: $appId');
      calloc.free(pId.value);
    }
  });

  tizen.app_info_destroy(appInfo);
}

class AppListPage extends StatefulWidget {
  @override
  State<AppListPage> createState() => _AppListPageState();
}

class _AppListPageState extends State<AppListPage> {
  static List<String> appIds = [];

  @override
  void initState() {
    super.initState();
    _loadApps();
  }

  static bool _getappInfo(Pointer<app_info_s> app_info, Pointer<Void> user_data) {
    final appInfo = app_info.cast<app_info_s>();
    using((Arena arena) {
      final pId = arena<Pointer<Char>>();
      if (tizen.app_info_get_app_id(appInfo, pId) == 0) {
        final id = pId.value.toDartString();
        appIds.add(id);
      }
    });
    return true;
  }

  static bool _getappInfowithFilter(Pointer<app_info_s> app_info, Pointer<Void> user_data) {
    final appInfo = app_info.cast<app_info_s>();
    using((Arena arena) {
      final pId = arena<Pointer<Char>>();
      if (tizen.app_info_get_app_id(appInfo, pId) == 0) {
        final id = pId.value.toDartString();
      }

      final label = arena<Pointer<Char>>();
      if(tizen.app_info_get_label(app_info, label) == 0) {
        final name = label.value.toDartString();
        appIds.add(name);
      }

      final icon = arena<Pointer<Char>>();
      if(tizen.app_info_get_icon(app_info, icon) == 0) {
        final iconpath = icon.value.toDartString();
      }
    });
    return true;
  }

  void _loadApps() {
    using((Arena arena) {
      final filterHandlePtr = arena<app_info_filter_h>();

      final result = tizen.app_info_filter_create(filterHandlePtr);

      if (result == 0) { // 0 == TIZEN_ERROR_NONE
        final filterHandle = filterHandlePtr.value;

        tizen.app_info_filter_add_bool(filterHandlePtr.value, PACKAGE_INFO_PROP_APP_NODISPLAY.toNativeChar(), false);
        final filterCallback = Pointer.fromFunction<app_info_filter_cbFunction>(_getappInfowithFilter, false);

        tizen.app_info_filter_foreach_appinfo(
          filterHandle,
          filterCallback,
          nullptr
        );

        tizen.app_info_filter_destroy(filterHandle);
      }
      else {
        print('Failed to create filter, error: $result');
      }
    });

    setState(() {});
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Installed Apps')),
      body: ListView.builder(
        itemCount: appIds.length,
        itemBuilder: (context, index) {
          return ListTile(title: Text(appIds[index]));
        },
      ),
    );
  }
}