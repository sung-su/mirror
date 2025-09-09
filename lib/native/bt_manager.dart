
import 'dart:ffi';

import 'package:ffi/ffi.dart';
import 'package:flutter/foundation.dart';
import 'package:tizen_interop/9.0/tizen.dart';

class BtManager {
  static int getState() {
    return using((Arena arena) {
      final state = arena<Int32>();
      int ret = tizen.bt_adapter_get_state(state);
      if (ret != 0) {
        debugPrint('Failed to get bt adapter state: $ret: ${tizen.get_error_message(ret).toDartString()}');
        return ret;
      }
      return state.value;
    });
  }

}