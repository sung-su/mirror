
import 'dart:ffi';
import 'package:flutter/foundation.dart';
import 'package:ffi/ffi.dart';
import 'package:tizen_interop/6.0/tizen.dart';

class PackageManager {
  static int id = 0;

  static void _uninstallCallback(int id, Pointer<Char> type, Pointer<Char> package, int event_type, int event_state, int progress, int error, Pointer<Void> user_data) {
    // Issue: callback is not invoked
    debugPrint('_uninstallCallback called');
  }

  static void _eventCallback(Pointer<Char> type, Pointer<Char> package, int event_type, int event_state, int progress, int error, Pointer<Void> user_data) {
    // Issue: callback is not invoked
    debugPrint('_callback called');
  }

  static void initialize() {
    using((Arena arena) {
      final pkgmgrHandlerPtr = arena<package_manager_h>();
      int ret = tizen.package_manager_create(pkgmgrHandlerPtr);
      if(ret != 0) return false;

      final callback2 = Pointer.fromFunction<package_manager_event_cbFunction>(_eventCallback);
      tizen.package_manager_set_event_cb(pkgmgrHandlerPtr.value, callback2, nullptr);

    });  
  }

  static Future<bool> _uninstall(String pkgName) async {
    final result = using((Arena arena) {
      final requestHandlePtr = arena<package_manager_request_h>();
      int ret = tizen.package_manager_request_create(requestHandlePtr);
      if(ret != 0) return false;

      final requestHandle = requestHandlePtr.value;
      final callback = Pointer.fromFunction<package_manager_request_event_cbFunction>(_uninstallCallback);

      ret = tizen.package_manager_request_uninstall_with_cb(
        requestHandle,
        pkgName.toNativeChar(),
        callback,
        nullptr,
        arena<Int>(id++)
      );
      if(ret != 0) return false;

      tizen.package_manager_request_destroy(requestHandle);
      return true;
    });

    return result;
  }

  static Future<bool> uninstallPackage(String pkgName) async {
    final ret = await compute(_uninstall, pkgName);
    return ret;
  }
}
