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

  List<WifiAP> get apList => _wifiManager.apList;
  bool get isInitialized => _wifiManager.initialized;
  bool get isActivated => _wifiManager.isActivated();

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

  bool? _lastConnectionResult;
  bool? get lastConnectionResult => _lastConnectionResult;

  bool? _lastDisconnectionResult;
  bool? get lastDisconnectionResult => _lastDisconnectionResult;

  bool? _isPluginInstalled;
  bool? get isPluginInstalled => _isPluginInstalled;

  WifiProvider() {
    _wifiManager = WifiManager();
    _setupCallbacks();
  }

  void _allConditionReset() {
    _connectedAp = null;
    _isActivating = false;
    _isConnecting = false;
    _isDisconnecting = false;
    _isDeactivating = false;
    _isScanning = false;
    _lastConnectionResult = null;
    _lastDisconnectionResult = null;
  }

  void _setupCallbacks() {
    WifiManager.onActivated = (int result) async {
      // print("@ onActivated[${result}]");
      _allConditionReset();
      notifyListeners();
      if (result == 0) {
        if (_isPluginInstalled == null) {
          _isPluginInstalled = true;
        }
        await scanAndRefresh();
      } else {
        if (_isPluginInstalled == null) {
          _isPluginInstalled = false;
        }
      }
    };

    WifiManager.onDeactivated = (int result) {
      // print("@ onDeactivated[${result}]");
      _isDeactivating = false;
      _connectedAp = null;
      notifyListeners();
    };

    WifiManager.onScanFinished = (int result) async {
      _isScanning = false;
      // print("onScanFinished[${result}]");
      notifyListeners();
      if (result == 0) {
        // print("onScanFinished success");
        _updateApListAndCurrentAp();
      }
    };

    WifiManager.onConnected = (int result) {
      _isConnecting = false;
      _lastConnectionResult = (result == 0 || result == -30277629);
      //print("onConnected[${result}] result=[${_lastConnectionResult}]");
      notifyListeners();
      if (_lastConnectionResult == true) {
        // print("onConnected success");
        _updateApListAndCurrentAp();
      }
    };

    WifiManager.onDisconnected = (int result) {
      _isDisconnecting = false;
      _lastDisconnectionResult = (result == 0);
      //print("onDisconnected[${result}] result=[${_lastDisconnectionResult}]");
      notifyListeners();
      if (_lastDisconnectionResult == true) {
        // print("onDisconnected success");
        _connectedAp = null;
        _updateApListAndCurrentAp();
      }
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

  bool get isSupported => _wifiManager.isSupported;

  Future<bool> wifiOn() async {
    if (isInitialized == false) {
      // print("wifiOn isInitialized false");
      return false;
    }
    if (_isActivating == true) {
      // print("wifiOn _isActivating true");
      return false;
    }

    try {
      // print("wifiOn scanAndRefresh before");
      scanAndRefresh();
      // print("wifiOn scanAndRefresh after");

      _isActivating = true;
      notifyListeners();

      // print("wifiOn activate await");
      await _wifiManager.activate();
      // print("wifiOn activate wake up");

      return true;
    } catch (e) {
      // print("wifiOn err");
      _isActivating = false;
      notifyListeners();
      return false;
    }
  }

  Future<bool> wifiOff() async {
    if (isInitialized == false) {
      // print("wifiOff isInitialized false");
      return false;
    }
    if (isActivated == false) {
      // print("wifiOff isActivated false");
      return true;
    }
    if (_isDeactivating == true) {
      // print("wifiOff _isDeactivating true");
      return false;
    }

    try {
      _isDeactivating = true;
      notifyListeners();

      // print("wifiOff deactivate await");
      await _wifiManager.deactivate();
      // print("wifiOff deactivate wake up");
      return true;
    } catch (e) {
      // print("wifiOff err");
      _isDeactivating = false;
      notifyListeners();
      return false;
    }
  }

  Future<void> scanAndRefresh() async {
    if (isInitialized == false) {
      // print("scanAndRefresh  isInitialized false");
      return;
    }
    if (isActivated == false) {
      // print("scanAndRefresh  isActivated false");
      return;
    }
    if (_isScanning) {
      // print("scanAndRefresh  _isScanning true");
      return;
    }

    try {
      _isScanning = true;
      // notifyListeners();

      _wifiManager.scan();
      // print("scanAndRefresh scan called");
    } catch (e) {
      // print("scanAndRefresh err");
      _isScanning = false;
      notifyListeners();
    }
  }

  void getCurrentAp() {
    final connectedApHandle = _wifiManager.getConnectedApHandle();
    // print("getCurrentAp getConnectedApHandle called");

    var id = _wifiManager.findIdByHandle(connectedApHandle);
    // print("getCurrentAp findIdByHandle called");

    if (connectedApHandle != nullptr) {
      for (var ap in apList) {
        if (ap.essid == id) {
          _connectedAp = ap;
          notifyListeners();
          // print("getCurrentAp ap found");
          return;
        }
      }
    }
    // print("getCurrentAp ap found not");
    _connectedAp = null;
    notifyListeners();
  }

  void _updateApListAndCurrentAp() {
    _wifiManager.updateApList();
    // print("_updateApListAndCurrentAp updateApList");
    final connectedApHandle = _wifiManager.getConnectedApHandle();
    // print("_updateApListAndCurrentAp getConnectedApHandle");

    getCurrentAp();
    notifyListeners();
  }

  Future<void> connectToAp(String essid, {String? password = ""}) async {
    if (_isConnecting) {
      // print("connectToAp _isConnecting true");
      return; //already connecting
    }
    if (_connectedAp?.essid == essid) {
      // print("connectToAp essid same");
      return; //already connecting
    }

    try {
      _isConnecting = true;
      notifyListeners();

      var apHandle = _wifiManager.findHandleByName(essid);
      if (apHandle == nullptr) {
        _isConnecting = false;
        // print("connectToAp apHandle not found");
        return; //not found
      }

      if (_wifiManager.passphraseRequired(apHandle)) {
        if (password == null || password!.isEmpty) {
          _isConnecting = false;
          // print("connectToAp passphraseRequired but empty");
          return;
        }
        if (!_wifiManager.setPassphrase(apHandle, password)) {
          _isConnecting = false;
          // print("connectToAp setPassphrase not matched");
          return; //cannot match password
        }
      }

      // print("connectToAp connect await");
      await _wifiManager.connect(apHandle);
      // print("connectToAp connect wake up");
    } catch (e) {
      // print("connectToAp err");
      _isConnecting = false;
      notifyListeners();
    }
  }

  Future<void> disconnectAp(WifiAP ap) async {
    if (ap == null) {
      // print("disconnectAp param ap null");
      _lastDisconnectionResult = false;
      notifyListeners();
      return;
    }
    if (_isDisconnecting) {
      // print("disconnectAp _isDisconnecting true");
      return;
    }
    if (_connectedAp?.essid != ap.essid) {
      // print("already disconnected or different AP",);
      _lastDisconnectionResult = true;
      notifyListeners();
      return; // this ap is not connected now
    }

    try {
      _isDisconnecting = true;
      notifyListeners();
      // print("disconnectAp disconnect await");
      await _wifiManager.disconnect(ap!.handle);
      // print("disconnectAp disconnect wake up");
    } catch (e) {
      // print("disconnectAp err");
      _isDisconnecting = false;
      _lastDisconnectionResult = false;
      notifyListeners();
    }
  }

  Future<void> disconnectFromCurrentAp() async {
    if (_connectedAp == null || _isDisconnecting) return;

    try {
      _isDisconnecting = true;
      notifyListeners();

      await _wifiManager.disconnect(_connectedAp!.handle);
    } catch (e) {
      _isDisconnecting = false;
      notifyListeners();
    }
  }
}
