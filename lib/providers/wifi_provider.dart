import 'dart:ffi';
import 'package:flutter/material.dart';
import 'package:tizen_fs/settings/wifi.dart';

class WifiAP {
  final Pointer<Void> handle;
  final String essid;
  final int state;
  final int frequency;
  final int rssi;

  WifiAP({
    required this.handle,
    required this.essid,
    required this.state,
    required this.frequency,
    required this.rssi,
  });
}

class WifiProvider with ChangeNotifier {
  late WifiManager _wifiManager;
  WifiManager get wifiManager => _wifiManager;

  List<WifiAP> get apList => WifiManager.apList;

  bool get isInitialized => _wifiManager.initialized;

  bool _isActivated = false;
  bool get isActivated => _isActivated;

  bool _isActivating = false;
  bool get isActivating => _isActivating;

  bool _isDeactivating = false;
  bool get isDeactivating => _isDeactivating;

  bool _isScanning = false;
  bool get isScanning => _isScanning;

  bool _isConnecting = false;
  bool get isConnecting => _isConnecting;

  bool _isDisconnecting = false;
  bool get isDisconnecting => _isDisconnecting;

  WifiAP? _connectedAp;
  WifiAP? get connectedAp => _connectedAp;

  WifiProvider() {
    _wifiManager = WifiManager();
    _isActivated = _wifiManager.isActivated();
    _setupCallbacks();
  }

  void _setupCallbacks() {
    WifiManager.onActivated = (int result) {
      _isActivating = false;
      _isActivated = true;
      scanAndRefresh();
      notifyListeners();
    };

    WifiManager.onDeactivated = (int result) {
      _isDeactivating = false;
      _isActivated = false;
      notifyListeners();
    };

    WifiManager.onScanFinished = (int result) {
      if (result == 0) {
        _updateApListAndCurrentAp();
      }
      _isScanning = false;
      notifyListeners();
    };

    WifiManager.onConnected = (int result) {
      if (result == 0) {
        _updateApListAndCurrentAp();
      }
      _isConnecting = false;
      notifyListeners();
    };

    WifiManager.onDisconnected = (int result) {
      if (result == 0) {
        _connectedAp = null;
        _updateApListAndCurrentAp();
      }
      _isDisconnecting = false;
      notifyListeners();
    };
  }

  @override
  void dispose() {
    super.dispose();
    _wifiManager.dispose();
  }

  Future<bool> wifiOn() async {
    if (_isActivating == true || isActivated == true) return false;

    try {
      _isActivating = true;
      notifyListeners();

      await wifiManager.activate();
      return true;
    } catch (e) {
      _isActivating = false;
      notifyListeners();
      return false;
    }
  }

  Future<bool> wifiOff() async {
    if (_isDeactivating == true || isActivated == false) return false;

    try {
      _isDeactivating = true;
      notifyListeners();

      await wifiManager.deactivate();
      return true;
    } catch (e) {
      _isDeactivating = false;
      notifyListeners();
      return false;
    }
  }

  Future<void> scanAndRefresh() async {
    if (_isScanning) return;

    try {
      _isScanning = true;
      notifyListeners();

      _wifiManager.scan();
    } catch (e) {
      _isScanning = false;
      notifyListeners();
    }
  }

  void _updateApListAndCurrentAp() {
    final oldApList = List<WifiAP>.from(WifiManager.apList);
    final oldConnectedAp = _connectedAp;

    WifiManager.updateApList();
    final connectedApHandle = _wifiManager.getConnectedAp();

    // AP 목록이 변경되었는지 확인
    final apListChanged = oldApList.length != WifiManager.apList.length ||
        !oldApList.every((oldAp) => WifiManager.apList.any((newAp) => newAp.handle == oldAp.handle));

    if (connectedApHandle != nullptr) {
      for (var ap in WifiManager.apList) {
        if (ap.handle == connectedApHandle) {
          _connectedAp = ap;
          // 연결 상태가 변경되었거나 AP 목록이 변경되었을 때 notify
          if (oldConnectedAp?.handle != _connectedAp?.handle || apListChanged) {
            notifyListeners();
          }
          return;
        }
      }
    }

    // 연결이 해제되었거나 AP 목록이 변경되었을 때 notify
    final connectionChanged = oldConnectedAp != null && _connectedAp == null;
    if (connectionChanged || apListChanged) {
      _connectedAp = null;
      notifyListeners();
    }
  }

  Future<bool> connectToAp(String essid, {String? password}) async {
    print("@ connetToAp 1 [${_isConnecting}]");

    if (_isConnecting) return false;
    print("@ connetToAp 2");

    try {
      _isConnecting = true;
      notifyListeners();

      if (_connectedAp != null) await disconnectFromCurrentAp();
      await _wifiManager.connect(essid, password ?? "");

      return true;
    } catch (e) {
      _isConnecting = false;
      notifyListeners();
      return false;
    }
  }

  Future<void> disconnectFromCurrentAp() async {
    if (_connectedAp == null || _isDisconnecting) return;

    try {
      _isDisconnecting = true;
      notifyListeners();

      await _wifiManager.disconnect();
    } catch (e) {
      _isDisconnecting = false;
      notifyListeners();
    }
  }
}
