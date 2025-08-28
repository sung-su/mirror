
import 'dart:ffi';
import 'package:flutter/foundation.dart';
import 'package:ffi/ffi.dart';
import 'package:tizen_interop/6.0/tizen.dart';

class AppControlOperations {
  static String defaultOperation = 'http://tizen.org/apcontrol/operation/default';
}

class AppLauncher {
  
  static void _launchCallback(Pointer<app_control_s> app_control, Pointer<app_control_s> reply, int result, Pointer<void> usr_data) {
    // Issue: callback is not invoked
    debugPrint('_launchCallback called');
  }

  static Future<bool> _launch(String appId) async {
    final result = using((Arena arena) {
      final appControlHandlePtr = arena<app_control_h>();
      int ret = tizen.app_control_create(appControlHandlePtr);
      if(ret != 0) return false;

      final appControlHandle = appControlHandlePtr.value;       
      ret = tizen.app_control_set_app_id(appControlHandle, appId.toNativeChar());
      if(ret != 0) return false;

      ret = tizen.app_control_set_operation(appControlHandle, AppControlOperations.defaultOperation.toNativeChar());
      if(ret != 0) return false;
      
      final callback = Pointer.fromFunction<app_control_reply_cbFunction>(_launchCallback);
      ret = tizen.app_control_send_launch_request(appControlHandle, callback, nullptr);
      if(ret != 0) return false;

      return true;
    });

    return result;
  }

  static Future<bool> launch(String appid) async {
    final ret = await compute(_launch, appid);
    return ret;
  }
}