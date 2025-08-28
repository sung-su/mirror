import 'package:flutter/foundation.dart';
import 'package:device_info_plus_tizen/device_info_plus_tizen.dart';
import 'dart:ffi';
import 'package:ffi/ffi.dart';
import 'package:tizen_interop/9.0/tizen.dart' as tz;

List<int> _fetchNativeSystemInfo(dynamic _) {
  int ramResult = -1;
  int widthResult = -1;
  int heightResult = -1;

  ramResult = using((Arena arena) {
    Pointer<tz.runtime_memory_info_s> pMemInfo = arena();
    if (tz.tizen.runtime_info_get_system_memory_info(pMemInfo) == 0) {
      return pMemInfo.ref.total;
    }
    return -1;
  });

  widthResult = using((Arena arena) {
    Pointer<Char> ptrKey = "tizen.org/feature/screen.width".toNativeChar(allocator: arena);
    Pointer<Int> ptrValue = arena();
    if (tz.tizen.system_info_get_platform_int(ptrKey, ptrValue) == 0) {
      return ptrValue.value ?? -1;
    }
    return -1;
  });

  heightResult = using((Arena arena) {
    Pointer<Char> ptrKey = "tizen.org/feature/screen.height".toNativeChar(allocator: arena);
    Pointer<Int> ptrValue = arena();
    if (tz.tizen.system_info_get_platform_int(ptrKey, ptrValue) == 0) {
      return ptrValue.value ?? -1;
    }
    return -1;
  });

  return [ramResult, widthResult, heightResult];
}

class DeviceInfoProvider with ChangeNotifier {
  TizenDeviceInfo? _tizenDeviceInfo;
  int _ram = -1;
  int _width = -1;
  int _height = -1;
  bool _isLoading = true;
  Object? _error;

  TizenDeviceInfo? get tizenDeviceInfo => _tizenDeviceInfo;
  int get ram => _ram;
  int get width => _width;
  int get height => _height;
  bool get isLoading => _isLoading;
  Object? get error => _error;

  Future<void> loadDeviceInfo() async {
    if (!_isLoading && (_tizenDeviceInfo != null || _error != null)) {
      return;
    }

    _isLoading = true;
    _error = null;
    notifyListeners();

    try {
      final List<int> nativeInfo = await compute(_fetchNativeSystemInfo, null);
      _ram = nativeInfo[0];
      _width = nativeInfo[1];
      _height = nativeInfo[2];

      final plugin = DeviceInfoPluginTizen();
      _tizenDeviceInfo = await plugin.tizenInfo;

    } catch (e) {
      _error = e;
    } finally {
      _isLoading = false;
      notifyListeners();
    }
  }

  Future<void> refreshDeviceInfo() async {
    _tizenDeviceInfo = null;
    _ram = -1;
    _width = -1;
    _height = -1;
    _error = null;
    await loadDeviceInfo();
  }
}
