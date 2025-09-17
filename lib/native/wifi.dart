import 'dart:ffi';
import 'dart:async';
import 'package:ffi/ffi.dart';
import 'package:tizen_interop/9.0/tizen.dart' as tz;
import 'package:tizen_interop_callbacks/tizen_interop_callbacks.dart';
import 'package:tizen_fs/providers/wifi_provider.dart';

class WifiManager {
  static late final Pointer<Pointer<Void>> wifiManagerHandle;

  static final callbacks = TizenInteropCallbacks();
  bool _initialized = false;
  bool get initialized => _initialized;
  bool _isSupported = true;
  bool get isSupported => _isSupported;

  static List<WifiAP> _apList = [];
  List<WifiAP> get apList => _apList;

  static Function(int result)? onActivated;
  static Function(int result)? onDeactivated;
  static Function(int result)? onScanFinished;
  static Function(int result)? onConnected;
  static Function(int result)? onDisconnected;

  static Function(int state)? onDeviceStateChanged;
  static Function(int state)? onScanStateChanged;
  static Function(int state)? onBackgroundScan;
  static Function(int state)? onConnectionStateChanged;
  static Function(int state)? onRssiLevelChanged;

  late final activatedCallack;
  late final deactivatedCallack;
  late final scanFinishedCallback;
  late final foreachFoundApCallback;
  late final connectedCallback;
  late final disconnectedCallback;

  late final deviceStateChangedCallback;
  late final scanStateChangedCallback;
  late final backgroundScanCallback;
  late final connectionStateChangedCallback;
  late final rssiLevelChangedCallback;

  WifiManager() {
    _initialized = initializeWifi();
    registerCallbacks();
    setCallbacks();
  }

  @override
  void dispose() {
    if (_initialized) {

      unsetCallbacks();

      callbacks.unregister(activatedCallack);
      callbacks.unregister(deactivatedCallack);
      callbacks.unregister(scanFinishedCallback);
      callbacks.unregister(foreachFoundApCallback);
      callbacks.unregister(connectedCallback);
      callbacks.unregister(disconnectedCallback);

      callbacks.unregister(deviceStateChangedCallback);
      callbacks.unregister(scanStateChangedCallback);
      callbacks.unregister(backgroundScanCallback);
      callbacks.unregister(connectionStateChangedCallback);
      callbacks.unregister(rssiLevelChangedCallback);

      tz.tizen.wifi_manager_deinitialize(wifiManagerHandle.value);
      calloc.free(wifiManagerHandle);

      _clearStaticResources();

      _initialized = false;
    }
  }

  bool initializeWifi() {
    if (_initialized) {
      //print("@ Wi-Fi Native Initialized Already");
      return _initialized;
    }

    wifiManagerHandle = calloc<tz.wifi_manager_h>();
    final ret = tz.tizen.wifi_manager_initialize(wifiManagerHandle);
    //print("@ Wi-Fi Native Initialized [${ret == 0}]");
    if (ret == tz.wifi_manager_error_e.WIFI_MANAGER_ERROR_NOT_SUPPORTED) {
      _isSupported = false;
    }
    return ret == 0;
  }

  void setCallbacks() {
    setDeviceStateChangedCallback();
    setScanStateChangedCallback();
    setBackgroundScanCallback();
    setConnectionStateChangedCallback();
    setRssiLevelChangedCallback();
  }

  void unsetCallbacks() {
    unsetDeviceStateChangedCallback();
    unsetScanStateChangedCallback();
    unsetBackgroundScanCallback();
    unsetConnectionStateChangedCallback();
    unsetRssiLevelChangedCallback();
  }

  static void _clearStaticResources() {
    onActivated = null;
    onDeactivated = null;
    onScanFinished = null;
    onConnected = null;
    onDisconnected = null;
    onDeviceStateChanged = null;
    onScanStateChanged = null;
    onBackgroundScan = null;
    onConnectionStateChanged = null;
    onRssiLevelChanged = null;
    _apList.clear();
  }

  static void _deviceStateChangedCallback(int state, Pointer<Void> user_data) {
    //print("@### Wi-Fi Native _deviceStateChangedCallback=[${state}]");
    //activated / deactivated
    onDeviceStateChanged?.call(state);
  }

  int setDeviceStateChangedCallback() {
    final ret = tz.tizen.wifi_manager_set_device_state_changed_cb(
      wifiManagerHandle.value,
      deviceStateChangedCallback.interopCallback,
      deviceStateChangedCallback.interopUserData,
    );
    //print("@### Wi-Fi Native setDeviceStateChangedCallback=[${ret}]");
    return ret;
  }

  int unsetDeviceStateChangedCallback() {
    final ret = tz.tizen.wifi_manager_unset_device_state_changed_cb(
      wifiManagerHandle.value,
    );
    //print("@### Wi-Fi Native unsetDeviceStateChangedCallback=[${ret}]");
    return ret;
  }

  static void _scanStateChangedCallback(int state, Pointer<Void> user_data) {
    //print("@### Wi-Fi Native _scanStateChangedCallback=[${state}]");
    //not scanning / scanning
    onScanStateChanged?.call(state);
  }

  int setScanStateChangedCallback() {
    final ret = tz.tizen.wifi_manager_set_scan_state_changed_cb(
      wifiManagerHandle.value,
      scanStateChangedCallback.interopCallback,
      scanStateChangedCallback.interopUserData,
    );
    //print("@### Wi-Fi Native setScanStateChangedCallback=[${ret}]");
    return ret;
  }

  int unsetScanStateChangedCallback() {
    final ret = tz.tizen.wifi_manager_unset_scan_state_changed_cb(
      wifiManagerHandle.value,
    );
    //print("@### Wi-Fi Native unsetScanStateChangedCallback=[${ret}]");
    return ret;
  }

  static void _backgroundScanCallback(int state, Pointer<Void> user_data) {
    //print("@### Wi-Fi Native _backgroundScanCallback=[${state}]");
    onBackgroundScan?.call(state);
  }

  int setBackgroundScanCallback() {
    final ret = tz.tizen.wifi_manager_set_background_scan_cb(
      wifiManagerHandle.value,
      backgroundScanCallback.interopCallback,
      backgroundScanCallback.interopUserData,
    );
    //scan by bg
    //print("@### Wi-Fi Native setBackgroundScanCallback=[${ret}]");
    return ret;
  }

  int unsetBackgroundScanCallback() {
    final ret = tz.tizen.wifi_manager_unset_background_scan_cb(
      wifiManagerHandle.value,
    );
    //print("@### Wi-Fi Native unsetBackgroundScanCallback=[${ret}]");
    return ret;
  }

  static void _connectionStateChangedCallback(
    int state,
    Pointer<Void> user_data,
  ) {
    //print("@### Wi-Fi Native _connectionStateChangedCallback=[${state}]");
    //connection state changed in ap list
    //0:disconnect 3:connect
    onConnectionStateChanged?.call(state);
  }

  int setConnectionStateChangedCallback() {
    final ret = tz.tizen.wifi_manager_set_connection_state_changed_cb(
      wifiManagerHandle.value,
      connectionStateChangedCallback.interopCallback,
      connectionStateChangedCallback.interopUserData,
    );
    //print("@### Wi-Fi Native setConnectionStateChangedCallback=[${ret}]");
    return ret;
  }

  int unsetConnectionStateChangedCallback() {
    final ret = tz.tizen.wifi_manager_unset_connection_state_changed_cb(
      wifiManagerHandle.value,
    );
    //print("@### Wi-Fi Native unsetConnectionStateChangedCallback=[${ret}]");
    return ret;
  }

  static void _rssiLevelChangedCallback(int state, Pointer<Void> user_data) {
    //print("@### Wi-Fi Native _rssiLevelChangedCallback=[${state}]");
    //4:very_strong(-63), 3:strong(-74), 2:weak(-82), 1:very_weak 0:no_signal
    onRssiLevelChanged?.call(state);
  }

  int setRssiLevelChangedCallback() {
    final ret = tz.tizen.wifi_manager_set_rssi_level_changed_cb(
      wifiManagerHandle.value,
      rssiLevelChangedCallback.interopCallback,
      rssiLevelChangedCallback.interopUserData,
    );
    //print("@### Wi-Fi Native setRssiLevelChangedCallback=[${ret}]");
    return ret;
  }

  int unsetRssiLevelChangedCallback() {
    final ret = tz.tizen.wifi_manager_unset_rssi_level_changed_cb(
      wifiManagerHandle.value,
    );
    //print("@### Wi-Fi Native unsetRssiLevelChangedCallback=[${ret}]");
    return ret;
  }

  void registerCallbacks() {
    deviceStateChangedCallback = callbacks
        .register<Void Function(Int32, Pointer<Void>)>(
          'wifi_manager_device_state_changed_cb',
          Pointer.fromFunction(_deviceStateChangedCallback),
        );
    scanStateChangedCallback = callbacks
        .register<Void Function(Int32, Pointer<Void>)>(
          'wifi_manager_scan_state_changed_cb',
          Pointer.fromFunction(_scanStateChangedCallback),
        );
    backgroundScanCallback = callbacks
        .register<Void Function(Int32, Pointer<Void>)>(
          'wifi_manager_scan_finished_cb',
          Pointer.fromFunction(_backgroundScanCallback),
        );
    connectionStateChangedCallback = callbacks
        .register<Void Function(Int32, Pointer<Void>)>(
          'wifi_manager_connection_state_changed_cb',
          Pointer.fromFunction(_connectionStateChangedCallback),
        );
    rssiLevelChangedCallback = callbacks
        .register<Void Function(Int32, Pointer<Void>)>(
          'wifi_manager_rssi_level_changed_cb',
          Pointer.fromFunction(_rssiLevelChangedCallback),
        );

    activatedCallack = callbacks.register<Void Function(Int32, Pointer<Void>)>(
      'wifi_manager_activated_cb',
      Pointer.fromFunction(_activatedCallback),
    );

    deactivatedCallack = callbacks
        .register<Void Function(Int32, Pointer<Void>)>(
          'wifi_manager_deactivated_cb',
          Pointer.fromFunction(_deactivatedCallback),
        );

    scanFinishedCallback = callbacks
        .register<Void Function(Int32, Pointer<Void>)>(
          'wifi_manager_scan_finished_cb',
          Pointer.fromFunction(_scanFinishedCallback),
        );

    foreachFoundApCallback = callbacks
        .register<Bool Function(Pointer<Void>, Pointer<Void>)>(
          'wifi_manager_found_ap_cb',
          Pointer.fromFunction(_foreachFoundApCallback, false),
        );

    connectedCallback = callbacks.register<Void Function(Int32, Pointer<Void>)>(
      'wifi_manager_connected_cb',
      Pointer.fromFunction(_connectedCallback),
    );

    disconnectedCallback = callbacks
        .register<Void Function(Int32, Pointer<Void>)>(
          'wifi_manager_disconnected_cb',
          Pointer.fromFunction(_disconnectedCallback),
        );
  }

  bool isActivated() {
    final activated = calloc<Bool>();
    try {
      final ret = tz.tizen.wifi_manager_is_activated(
        wifiManagerHandle.value,
        activated,
      );

      var result = false;
      if (ret == 0) {
        result = activated.value;
      }
      //print("@ Wi-Fi Native isActivated=[${result}]");
      return result;
    } finally {
      calloc.free(activated);
    }
  }

  static void _activatedCallback(int result, Pointer<Void> user_data) {
    //print("@ Wi-Fi Native Activated CallBack=[${result}]");
    onActivated?.call(result);
  }

  Future<void> activate() async {
    var ret = await tz.tizen.wifi_manager_activate(
      wifiManagerHandle.value,
      activatedCallack.interopCallback,
      activatedCallack.interopUserData,
    );
    //print("@ Wi-Fi Native activate Called [${ret}]");
  }

  static void _deactivatedCallback(int result, Pointer<Void> user_data) {
    //print("@ Wi-Fi Native Deactivated CallBack=[${result}]");
    onDeactivated?.call(result);
  }

  Future<void> deactivate() async {
    final ret = await tz.tizen.wifi_manager_deactivate(
      wifiManagerHandle.value,
      deactivatedCallack.interopCallback,
      deactivatedCallack.interopUserData,
    );
    //print("@ Wi-Fi Native Deactivate Called [${ret}]");
  }

  static void _scanFinishedCallback(int error_code, Pointer<Void> user_data) {
    //print("@ Wi-Fi Native ScanFinished CallBack=[${error_code}]");
    onScanFinished?.call(error_code);
  }

  void scan() {
    final ret = tz.tizen.wifi_manager_scan(
      wifiManagerHandle.value,
      scanFinishedCallback.interopCallback,
      scanFinishedCallback.interopUserData,
    );
    //print("@ Wi-Fi Native Scan Called [${ret}]");
  }

  String findIdByHandle(Pointer<Void> apHandle) {
    Pointer<Pointer<Char>> essid = calloc<Pointer<Char>>();
    try {
      var ret = tz.tizen.wifi_manager_ap_get_essid(apHandle, essid);
      String id = "";
      if (ret == 0) {
        id = essid.value.toDartString();
      }
      return id;
    } finally {
      calloc.free(essid);
    }
  }

  int _findStateByHandle(Pointer<Void> apHandle) {
    Pointer<Int32> state = calloc<Int32>();
    try {
      var ret = tz.tizen.wifi_manager_ap_get_connection_state(apHandle, state);
      ret = state.value;
      return ret;
    } finally {
      calloc.free(state);
    }
  }

  static bool _foreachFoundApCallback(
    Pointer<Void> ap,
    Pointer<Void> user_data,
  ) {
    if (ap == nullptr) return false;

    Pointer<Pointer<Char>> id = calloc<Pointer<Char>>();
    Pointer<Int32> state = calloc<Int32>();
    Pointer<Int> frequency = calloc<Int>();
    Pointer<Int> rssi = calloc<Int>();

    try {
      var ret = tz.tizen.wifi_manager_ap_get_essid(ap, id);
      ret = tz.tizen.wifi_manager_ap_get_connection_state(ap, state);
      ret = tz.tizen.wifi_manager_ap_get_frequency(ap, frequency);
      ret = tz.tizen.wifi_manager_ap_get_rssi(ap, rssi);

      //print("@ Wi-Fi Native UpdateAP Callback handle=[${ap}] id=[${id.value.toDartString()}] state=[${state.value}] frequency=[${frequency.value}] rssi=[${rssi.value}]",);

      _apList.add(
        WifiAP(
          essid: id.value.toDartString(),
          state: state.value,
          frequency: frequency.value,
          rssi: rssi.value,
          handle: ap,
        ),
      );
      return true;
    } finally {
      calloc.free(id);
      calloc.free(state);
      calloc.free(frequency);
      calloc.free(rssi);
    }
  }

  void updateApList() {
    if (!isActivated()) {
      //print("@ Wi-Fi Native UpdateAPList Skipped - WiFi not activated");
      return;
    }

    _apList.clear();

    final ret = tz.tizen.wifi_manager_foreach_found_ap(
      wifiManagerHandle.value,
      foreachFoundApCallback.interopCallback,
      foreachFoundApCallback.interopUserData,
    );
    //print("@ Wi-Fi Native Update APList Called, Found [${_apList.length}] APs");
  }

  Pointer<Void> getConnectedApHandle() {
    final ap = calloc<tz.wifi_manager_ap_h>();
    try {
      final ret = tz.tizen.wifi_manager_get_connected_ap(
        wifiManagerHandle.value,
        ap,
      );
      var result = ap?.value ?? nullptr;
      //print("@ Wi-Fi Native Current Connected AP=[${result}] [${ret}]");
      return result;
    } finally {
      calloc.free(ap);
    }
  }

  bool passphraseRequired(Pointer<Void> apHandle) {
    Pointer<Bool> isRequired = calloc<Bool>();
    try {
      final ret = tz.tizen.wifi_manager_ap_is_passphrase_required(
        apHandle,
        isRequired,
      );
      bool result = false;
      if (ret == 0 && isRequired.value == true) result = true;
      //print("@ Wi-Fi Native PassphraseRequired=[${result}] [${ret}]");
      return result;
    } finally {
      calloc.free(isRequired);
    }
  }

  bool setPassphrase(Pointer<Void> ap, String passphrase) {
    var password = passphrase.toNativeChar();
    try {
      final ret = tz.tizen.wifi_manager_ap_set_passphrase(ap, password);
      bool result = false;
      if (ret == 0) result = true;
      //print("@ Wi-Fi Native SetPassphrase=[${result}] [${ret}]");
      return result;
    } finally {
      calloc.free(password);
    }
  }

  Pointer<Void> findHandleByName(String name) {
    Pointer<Void> handle = nullptr;
    for (var ap in apList) {
      if (ap.essid == name) {
        //print("@ Wi-Fi Native Found AP=[${name}]");
        return ap.handle;
      }
    }
    //print("@ Wi-Fi Native Cannot Found AP");
    return handle;
  }

  static void _connectedCallback(int error_code, Pointer<Void> user_data) {
    //print("@ Wi-Fi Native ConnectedCallback=[${error_code}]");
    onConnected?.call(error_code);
  }

  Future<void> connect(Pointer<Void> apHandle) async {
    final ret = await tz.tizen.wifi_manager_connect(
      wifiManagerHandle.value,
      apHandle,
      connectedCallback.interopCallback,
      connectedCallback.interopUserData,
    );
    //print("@ Wi-Fi Native Connect called [${ret}]");
  }

  static void _disconnectedCallback(int error_code, Pointer<Void> user_data) {
    //print("@ Wi-Fi Native Disconnected Callback=[${error_code}]");
    onDisconnected?.call(error_code);
  }

  Future<void> disconnect(Pointer<Void> apHandle) async {
    final ret = await tz.tizen.wifi_manager_disconnect(
      wifiManagerHandle.value,
      apHandle,
      disconnectedCallback.interopCallback,
      disconnectedCallback.interopUserData,
    );
    //print("@ Wi-Fi Native Disconnect called [${ret}]");
  }
}
