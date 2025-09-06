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

  static List<WifiAP> _apList = [];
  static List<WifiAP> get apList => _apList;

  static Function(int result)? onActivated;
  static Function(int result)? onDeactivated;
  static Function(int result)? onScanFinished;
  static Function(int result)? onConnected;
  static Function(int result)? onDisconnected;

  WifiManager() {
    initializeWifi();
  }

  @override
  void dispose() {
    if (_initialized) {
      tz.tizen.wifi_manager_deinitialize(wifiManagerHandle.value);
    }
  }

  void initializeWifi() {
    if (_initialized) {
      print("@ Wi-Fi Native Initialized Already");
      return;
    }

    wifiManagerHandle = calloc<tz.wifi_manager_h>();
    final ret = tz.tizen.wifi_manager_initialize(wifiManagerHandle);
    _initialized = true;
    print("@ Wi-Fi Native Initialized");
  }

  bool isActivated() {
    final activated = calloc<Bool>();
    final ret = tz.tizen.wifi_manager_is_activated(
      wifiManagerHandle.value,
      activated,
    );

    var result = false;
    if (ret == 0 && activated.value == true) {
      result = true;
    }
    print("@ Wi-Fi Native isActivated=[${result}]");
    return result;
  }

  static void _activatedCallback(int result, Pointer<Void> user_data) {
    print("@ Wi-Fi Native Activated CallBack=[${result}]");
    onActivated?.call(result);
  }

  Future<void> activate() async {
    final callback = callbacks.register<Void Function(Int32, Pointer<Void>)>(
      'wifi_manager_activated_cb',
      Pointer.fromFunction(_activatedCallback),
    );
    print("@ Wi-Fi Native Activate Call");
    final ret = await tz.tizen.wifi_manager_activate(
      wifiManagerHandle.value,
      callback.interopCallback,
      callback.interopUserData,
    );
    print("@ Wi-Fi Native Activate Called");
  }

  static void _deactivatedCallback(int result, Pointer<Void> user_data) {
    print("@ Wi-Fi Native Deactivated CallBack=[${result}]");
    onDeactivated?.call(result);
  }

  Future<void> deactivate() async {
    final callback = callbacks.register<Void Function(Int32, Pointer<Void>)>(
      'wifi_manager_deactivated_cb',
      Pointer.fromFunction(_deactivatedCallback),
    );
    print("@ Wi-Fi Native Deactivate Call");
    final ret = await tz.tizen.wifi_manager_deactivate(
      wifiManagerHandle.value,
      callback.interopCallback,
      callback.interopUserData,
    );
    print("@ Wi-Fi Native Deactivate Called");
  }

  static void _scanFinishedCallback(int error_code, Pointer<Void> user_data) {
    print("@ Wi-Fi Native ScanFinished CallBack=[${error_code}]");
    onScanFinished?.call(error_code);
  }

  void scan() {
    final callback2 = callbacks.register<Void Function(Int32, Pointer<Void>)>(
      'wifi_manager_scan_finished_cb',
      Pointer.fromFunction(_scanFinishedCallback),
    );
    print("@ Wi-Fi Native Scan Call");
    final ret = tz.tizen.wifi_manager_scan(
      wifiManagerHandle.value,
      callback2.interopCallback,
      callback2.interopUserData,
    );
    print("@ Wi-Fi Native Scan Called");
  }

  static bool _foreachFoundApCallback(
    Pointer<Void> ap,
    Pointer<Void> user_data,
  ) {
    if (ap == nullptr) return false;
    Pointer<Pointer<Char>> id = calloc<Pointer<Char>>();
    var ret = tz.tizen.wifi_manager_ap_get_essid(ap, id);

    Pointer<Int32> state = calloc<Int32>();
    ret = tz.tizen.wifi_manager_ap_get_connection_state(ap, state);

    Pointer<Int> frequency = calloc<Int>();
    ret = tz.tizen.wifi_manager_ap_get_frequency(ap, frequency);

    Pointer<Int> rssi = calloc<Int>();
    ret = tz.tizen.wifi_manager_ap_get_rssi(ap, rssi);

    print(
      "@ Wi-Fi Native UpdateAP Callback handle=[${ap}] id=[${id.value.toDartString()}] state=[${state.value}] frequency=[${frequency.value}] rssi=[${rssi.value}]",
    );

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
  }

  static void updateApList() {
    _apList.clear();

    final callback = callbacks
        .register<Bool Function(Pointer<Void>, Pointer<Void>)>(
          'wifi_manager_found_ap_cb',
          Pointer.fromFunction(_foreachFoundApCallback, false),
        );

    print("@ Wi-Fi Native UpdateAPList Call");
    final ret = tz.tizen.wifi_manager_foreach_found_ap(
      wifiManagerHandle.value,
      callback.interopCallback,
      callback.interopUserData,
    );
    print("@ Wi-Fi Native Update APList Called");
  }

  Pointer<Void> getConnectedAp() {
    final ap = calloc<tz.wifi_manager_ap_h>();
    final ret = tz.tizen.wifi_manager_get_connected_ap(
      wifiManagerHandle.value,
      ap,
    );
    var result = ap?.value ?? nullptr;
    print("@ Wi-Fi Native Current Connected AP=[${result}]");
    return result;
  }

  bool passphraseRequired(Pointer<Void> apHandle) {
    Pointer<Bool> isRequired = calloc<Bool>();
    final ret = tz.tizen.wifi_manager_ap_is_passphrase_required(
      apHandle,
      isRequired,
    );
    bool result = false;
    if (ret == 0 && isRequired.value == true) result = true;
    print("@ Wi-Fi Native PassphraseRequired=[${result}]");
    return result;
  }

  bool setPassphrase(Pointer<Void> ap, String passphrase) {
    final ret = tz.tizen.wifi_manager_ap_set_passphrase(
      ap,
      passphrase.toNativeChar(),
    );
    bool result = false;
    if (ret == 0) result = true;
    print("@ Wi-Fi Native SetPassphrase=[${result}]");
    return result;
  }

  static void _connectedCallback(int error_code, Pointer<Void> user_data) {
    print("@ Wi-Fi Native ConnectedCallback=[${error_code}]");
    onConnected?.call(error_code);
  }

  Pointer<Void> _findHandleByName(String name) {
    Pointer<Void> handle = nullptr;
    for (var ap in apList) {
      if (ap.essid == name) {
        print("@ Wi-Fi Native Found AP=[${name}]");
        return ap.handle;
      }
    }
    print("@ Wi-Fi Native Cannot Found AP");
    return handle;
  }

  Future<void> connect(String apName, String passwd) async {
    final callback = callbacks.register<Void Function(Int32, Pointer<Void>)>(
      'wifi_manager_connected_cb',
      Pointer.fromFunction(_connectedCallback),
    );
    var apHandle = _findHandleByName(apName);
    if (apHandle == nullptr) {
      return;
    }

    if (passphraseRequired(apHandle)) {
      if (!setPassphrase(apHandle, passwd)) {
        return;
      }
    }
    print("@ Wi-Fi Native Connect call");
    final ret = await tz.tizen.wifi_manager_connect(
      wifiManagerHandle.value,
      apHandle,
      callback.interopCallback,
      callback.interopUserData,
    );
    print("@ Wi-Fi Native Connect called");
  }

  static void _disconnectedCallback(int error_code, Pointer<Void> user_data) {
    print("@ Wi-Fi Native Disconnected Callback=[${error_code}]");
    onDisconnected?.call(error_code);
  }

  Future<void> disconnect() async {
    final apHandle = getConnectedAp();
    final callback = callbacks.register<Void Function(Int32, Pointer<Void>)>(
      'wifi_manager_disconnected_cb',
      Pointer.fromFunction(_connectedCallback),
    );

    print("@ Wi-Fi Native Disconnect call");
    final ret = await tz.tizen.wifi_manager_disconnect(
      wifiManagerHandle.value,
      apHandle,
      callback.interopCallback,
      callback.interopUserData,
    );
    print("@ Wi-Fi Native Disconnect called");
  }
}
