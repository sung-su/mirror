import 'package:flutter/foundation.dart';
import 'package:device_info_plus_tizen/device_info_plus_tizen.dart';
import 'dart:ffi';
import 'package:ffi/ffi.dart';
import 'package:tizen_interop/9.0/tizen.dart' as tz;

int _fetchNativeSystemInfo(dynamic _) {
  return using((Arena arena) {
    final pMemInfo = arena<tz.runtime_memory_info_s>();
    if (tz.tizen.runtime_info_get_system_memory_info(pMemInfo) == 0) {
      return pMemInfo.ref.total;
    }
    return -1;
  });
}

class DeviceInfoProvider with ChangeNotifier {
  TizenDeviceInfo? _tizenDeviceInfo;
  int _ram = -1;
  bool _isLoading = true;
  Object? _error;

  TizenDeviceInfo? get tizenDeviceInfo => _tizenDeviceInfo;
  int get ram => _ram;
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
      _ram = await compute(_fetchNativeSystemInfo, null);
      final plugin = DeviceInfoPluginTizen();
      _tizenDeviceInfo = await plugin.tizenInfo;

    } catch (e) {
      _error = e;
      _tizenDeviceInfo = null;
      _ram = -1;
    } finally {
      _isLoading = false;
      notifyListeners();
    }
  }

  Future<void> refreshDeviceInfo() async {
    _tizenDeviceInfo = null;
    _ram = -1;
    _error = null;
    await loadDeviceInfo();
  }
}
