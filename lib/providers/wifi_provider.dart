import 'dart:ffi';
import 'dart:async';
import 'package:flutter/material.dart';
import 'package:tizen_fs/native/wifi.dart';

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

  WifiAP? _connectedAp = null;
  WifiAP? get connectedAp => _connectedAp;

  WifiProvider() {
    _wifiManager = WifiManager();
    _isActivated = _wifiManager.isActivated();
    _setupCallbacks();
  }

  void _setupCallbacks() {
    WifiManager.onActivated = (int result) async {
      print("@ onActivated[${result}]");
      _isActivating = false;
      _isActivated = result == 0;
      if (_isActivated) await scanAndRefresh();
      else notifyListeners();
    };

    WifiManager.onDeactivated = (int result) {
      _isDeactivating = false;
      _isActivated = false;
      _connectedAp = null;
      WifiManager.apList.clear();
      notifyListeners();
    };

    WifiManager.onScanFinished = (int result) async {
      _isScanning = false;
      if (result == 0) _updateApListAndCurrentAp();
      else notifyListeners();
    };

    WifiManager.onConnected = (int result) {
      _isConnecting = false;
      if (result == 0) _updateApListAndCurrentAp();
      else notifyListeners();
    };

    WifiManager.onDisconnected = (int result) {
      _isDisconnecting = false;
      if (result == 0) {
        _connectedAp = null;
        _updateApListAndCurrentAp();
      }
      else notifyListeners();
    };
  }

  @override
  void dispose() {
    WifiManager.onActivated = null;
    WifiManager.onDeactivated = null;
    WifiManager.onScanFinished = null;
    WifiManager.onConnected = null;
    WifiManager.onDisconnected = null;
    _wifiManager.dispose();
    super.dispose();
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

  void getCurrentAp() {
    final connectedApHandle = _wifiManager.getConnectedAp();
    var id = WifiManager.findIdByHandle(connectedApHandle);
    print("@ getCurrentAp1 handle[${connectedApHandle}]");
    print("@ getCurrentAp1 id[${id}]");

    if (connectedApHandle != nullptr) {
      for (var ap in apList) {
        if (ap.essid == id) {
          _connectedAp = ap;
          print("@ getCurrentAp2 handle[${_connectedAp?.essid}]");
          print("@ getCurrentAp2 id[${_connectedAp?.handle}]");
          notifyListeners();
          return;
        }
      }
    }
    print("@ getCurrentAp3 fail [${_connectedAp?.handle}]");

    _connectedAp = null;
    notifyListeners();
    return;
  }

  Future waitDisconnect() async {
    print("@ connetToAp 4-1");

    final completer = Completer();
    bool fired = false;

    final prev = WifiManager.onDisconnected;
    WifiManager.onDisconnected = (int result) {
      if (!fired) {
        fired = true;
        completer.complete();
      }
      prev?.call(result);
    };
    print("@ connetToAp 4-2");

    try {
      await disconnectFromCurrentAp();
    print("@ connetToAp 4-3");

      await completer.future.timeout(Duration(seconds: 3));
    print("@ connetToAp 4-4");

    } finally {
    print("@ connetToAp 4-5");

      WifiManager.onDisconnected = prev;
    }

  }

  void _updateApListAndCurrentAp() {
    WifiManager.updateApList();
    final connectedApHandle = _wifiManager.getConnectedAp();
    getCurrentAp();
    notifyListeners();
  }

  Future<bool> connectToAp(String essid, {String? password}) async {
    print("@ connetToAp 1 [${_isConnecting}]");

    if (_isConnecting) return false;
    print("@ connetToAp 2");

    try {
      if(_connectedAp?.essid == essid) {
        return true;
      }

      _isConnecting = true;
      notifyListeners();

      if (_connectedAp != null && _connectedAp?.essid != essid) {
        print("@ connetToAp 3");
        await waitDisconnect();
      }
      print("@ connetToAp 5");
      await _wifiManager.connect(essid, password ?? "");
      print("@ connetToAp 6");
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
