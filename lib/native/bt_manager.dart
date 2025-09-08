
import 'dart:ffi';

import 'package:ffi/ffi.dart';
import 'package:flutter/foundation.dart';
import 'package:tizen_app_control/tizen_app_control.dart';
import 'package:tizen_fs/locator.dart';
import 'package:tizen_fs/models/app_data_model.dart';
import 'package:tizen_interop/9.0/tizen.dart';
import 'package:tizen_package_manager/tizen_package_manager.dart';

class BtManager {
  static int getState() {
    return using((Arena arena) {
      final state = arena<Int32>();
      int ret = tizen.bt_adapter_get_state(state);
      if (ret != 0) {
        throw Exception(
          'Failed to get the state of Bluetooth Adapter. Error code: ${tizen.get_error_message(ret).toDartString()}',
        );
      }
      return state.value;
    });
  }

}