import 'dart:ffi';
import 'dart:async';
import 'package:flutter/material.dart';
import 'package:tizen_fs/native/wifi.dart';
import 'package:tizen_interop/9.0/tizen.dart';

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
  bool _isDisposed = false;

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

  void _notifyListenersSafe() {
    if (!_isDisposed) {
      notifyListeners();
    }
  }

  void _setupCallbacks() {
    WifiManager.onActivated = (int result) async {
      if (_isDisposed) return;

      //print("@ onActivated[${result}]");
      _allConditionReset();
      _notifyListenersSafe();
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
      if (_isDisposed) return;

      //print("@ onDeactivated[${result}]");
      _isDeactivating = false;
      _connectedAp = null;
      _notifyListenersSafe();
    };

    WifiManager.onScanFinished = (int result) async {
      if (_isDisposed) return;

      _isScanning = false;
      //print("onScanFinished[${result}]");
      _notifyListenersSafe();
      if (result == 0) {
        //print("onScanFinished success");
        _updateApListAndCurrentAp();
      }
    };

    WifiManager.onConnected = (int result) {
      if (_isDisposed) return;

      _isConnecting = false;
      _notifyListenersSafe();
      if (result == 0 || result == -30277629) {
        //print("onConnected success");
        _updateApListAndCurrentAp();
      }
    };

    WifiManager.onDisconnected = (int result) {
      if (_isDisposed) return;

      _isDisconnecting = false;
      _notifyListenersSafe();
      if (result == 0) {
        //print("onDisconnected success");
        _connectedAp = null;
        _updateApListAndCurrentAp();
      }
    };

    WifiManager.onBackgroundScan = (int result) async {
      if (_isDisposed) return;

      //print("onBackgroundScan [${result}]");
      if (result == 0) {
        _updateApListAndCurrentAp();
      }
    };

    WifiManager.onConnectionStateChanged = (int result) async {
      if (_isDisposed) return;

      //print("onConnectionStateChanged [${result}]");
      if (result ==
          wifi_manager_connection_state_e
              .WIFI_MANAGER_CONNECTION_STATE_CONNECTED) {
        //print("onConnectionStateChanged connect[${result}] //3");
        _isConnecting = false;
        _lastConnectionResult = true;
        _notifyListenersSafe();
        _updateApListAndCurrentAp();
      } else if (result ==
          wifi_manager_connection_state_e
              .WIFI_MANAGER_CONNECTION_STATE_DISCONNECTED) {
        //print("onConnectionStateChanged disconnect[${result}] //0");
        _isDisconnecting = false;
        _lastDisconnectionResult = true;
        _notifyListenersSafe();
        _updateApListAndCurrentAp();
      } else if (result ==
          wifi_manager_connection_state_e
              .WIFI_MANAGER_CONNECTION_STATE_ASSOCIATION) {
        //print("onConnectionStateChanged associateion[${result}]");
        _lastConnectionResult = true;
        _notifyListenersSafe();
      } else if (result ==
          wifi_manager_connection_state_e
              .WIFI_MANAGER_CONNECTION_STATE_CONFIGURATION) {
        //print("onConnectionStateChanged configuration[${result}]");
        _lastConnectionResult = true;
        _notifyListenersSafe();
      } else if (result ==
          wifi_manager_connection_state_e
              .WIFI_MANAGER_CONNECTION_STATE_FAILURE) {
        //print("onConnectionStateChanged fail[${result}]");
        _lastDisconnectionResult = false;
        _lastConnectionResult = false;
        _notifyListenersSafe();
      }
    };
  }

  @override
  void dispose() {
    _isDisposed = true;

    WifiManager.onActivated = null;
    WifiManager.onDeactivated = null;
    WifiManager.onScanFinished = null;
    WifiManager.onConnected = null;
    WifiManager.onDisconnected = null;
    WifiManager.onBackgroundScan = null;
    WifiManager.onConnectionStateChanged = null;

    _wifiManager.dispose();
    _allConditionReset();
    _connectedAp = null;
    _isPluginInstalled = null;

    super.dispose();
  }

  bool get isSupported => _wifiManager.isSupported;

  Future<bool> wifiOn() async {
    if (_isDisposed || isInitialized == false) {
      //print("wifiOn isDisposed or isInitialized false");
      return false;
    }
    if (_isActivating == true) {
      //print("wifiOn _isActivating true");
      return false;
    }

    try {
      //print("wifiOn scanAndRefresh before");
      scanAndRefresh();
      //print("wifiOn scanAndRefresh after");

      _isActivating = true;
      _notifyListenersSafe();

      //print("wifiOn activate await");
      await _wifiManager.activate();
      //print("wifiOn activate wake up");

      return true;
    } catch (e) {
      //print("wifiOn err");
      _isActivating = false;
      _notifyListenersSafe();
      return false;
    }
  }

  Future<bool> wifiOff() async {
    if (_isDisposed || isInitialized == false) {
      //print("wifiOff isDisposed or isInitialized false");
      return false;
    }
    if (isActivated == false) {
      //print("wifiOff isActivated false");
      return true;
    }
    if (_isDeactivating == true) {
      //print("wifiOff _isDeactivating true");
      return false;
    }

    try {
      _isDeactivating = true;
      _notifyListenersSafe();

      //print("wifiOff deactivate await");
      await _wifiManager.deactivate();
      //print("wifiOff deactivate wake up");
      return true;
    } catch (e) {
      //print("wifiOff err");
      _isDeactivating = false;
      _notifyListenersSafe();
      return false;
    }
  }

  Future<void> scanAndRefresh() async {
    if (_isDisposed || isInitialized == false) {
      //print("scanAndRefresh  _isDisposed or isInitialized false");
      return;
    }
    if (isActivated == false) {
      //print("scanAndRefresh  isActivated false");
      return;
    }
    if (_isScanning) {
      //print("scanAndRefresh  _isScanning true");
      return;
    }

    try {
      _isScanning = true;
      // _notifyListenersSafe();

      _wifiManager.scan();
      //print("scanAndRefresh scan called");
    } catch (e) {
      //print("scanAndRefresh err");
      _isScanning = false;
      _notifyListenersSafe();
    }
  }

  void getCurrentAp() {
    if (_isDisposed) return;

    final connectedApHandle = _wifiManager.getConnectedApHandle();
    //print("getCurrentAp getConnectedApHandle called");

    var id = _wifiManager.findIdByHandle(connectedApHandle);
    //print("getCurrentAp findIdByHandle called");

    if (connectedApHandle != nullptr) {
      for (var ap in apList) {
        if (ap.essid == id) {
          _connectedAp = ap;
          _notifyListenersSafe();
          //print("getCurrentAp ap found");
          return;
        }
      }
    }
    //print("getCurrentAp ap found not");
    _connectedAp = null;
    _notifyListenersSafe();
  }

  void _updateApListAndCurrentAp() {
    if (_isDisposed) return;

    _wifiManager.updateApList();
    //print("_updateApListAndCurrentAp updateApList");
    final connectedApHandle = _wifiManager.getConnectedApHandle();
    //print("_updateApListAndCurrentAp getConnectedApHandle");

    getCurrentAp();
    _notifyListenersSafe();
  }

  Future<void> connectToAp(String essid, {String? password = ""}) async {
    if (_isDisposed) return;

    if (_isConnecting) {
      //print("connectToAp _isConnecting true");
      return; //already connecting
    }
    if (_connectedAp?.essid == essid) {
      //print("connectToAp essid same");
      return; //already connecting
    }

    try {
      _isConnecting = true;
      _notifyListenersSafe();

      var apHandle = _wifiManager.findHandleByName(essid);
      if (apHandle == nullptr) {
        _isConnecting = false;
        //print("connectToAp apHandle not found");
        return; //not found
      }

      if (_wifiManager.passphraseRequired(apHandle)) {
        if (password == null || password!.isEmpty) {
          _isConnecting = false;
          //print("connectToAp passphraseRequired but empty");
          return;
        }
        if (!_wifiManager.setPassphrase(apHandle, password)) {
          _isConnecting = false;
          //print("connectToAp setPassphrase not matched");
          return; //cannot match password
        }
      }

      //print("connectToAp connect await");
      await _wifiManager.connect(apHandle);
      //print("connectToAp connect wake up");
    } catch (e) {
      //print("connectToAp err");
      _isConnecting = false;
      _notifyListenersSafe();
    }
  }

  Future<void> disconnectAp(WifiAP ap) async {
    if (_isDisposed) return;

    if (ap == null) {
      //print("disconnectAp param ap null");
      // _lastDisconnectionResult = false;
      _notifyListenersSafe();
      return;
    }
    if (_isDisconnecting) {
      //print("disconnectAp _isDisconnecting true");
      return;
    }
    if (_connectedAp?.essid != ap.essid) {
      //print("already disconnected or different AP");
      // _lastDisconnectionResult = true;
      _notifyListenersSafe();
      return; // this ap is not connected now
    }

    try {
      _isDisconnecting = true;
      _notifyListenersSafe();
      //print("disconnectAp disconnect await");
      await _wifiManager.disconnect(ap!.handle);
      //print("disconnectAp disconnect wake up");
    } catch (e) {
      //print("disconnectAp err");
      _isDisconnecting = false;
      // _lastDisconnectionResult = false;
      _notifyListenersSafe();
    }
  }

  Future<void> disconnectFromCurrentAp() async {
    if (_isDisposed || _connectedAp == null || _isDisconnecting) return;

    try {
      _isDisconnecting = true;
      _notifyListenersSafe();

      await _wifiManager.disconnect(_connectedAp!.handle);
    } catch (e) {
      _isDisconnecting = false;
      _notifyListenersSafe();
    }
  }
}
