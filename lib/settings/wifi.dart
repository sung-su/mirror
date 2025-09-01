import 'dart:ffi';
import 'dart:async';
import 'package:ffi/ffi.dart';
import 'package:tizen_interop/9.0/tizen.dart' as tz;
import 'package:tizen_interop_callbacks/tizen_interop_callbacks.dart';

//void (*wifi_manager_scan_finished_cb)(wifi_manager_error_e error_code, void *user_data)
// typedef WifiManagerScanFinishedCbNative = Void Function(Int32, Pointer<Void>);
// typedef WifiManagerScanFinishedCb = Pointer<NativeFunction<WifiManagerScanFinishedCbNative>>;

class WifiManager {
  late final Pointer<Pointer<Void>> wifiManagerHandle;
  late final Pointer<Pointer<Void>> connectionHandle;

  static final callbacks = TizenInteropCallbacks();

  WifiManager()
  {
    initializeWifi();
  }

  static void _deviceStateChangedCallback(int state, Pointer<Void> user_data) {
    print("@ wifi _deviceStateChangedCallback state=[${state}]");
  }

  static void _connectionStateChangedCallback(
    int state,
    Pointer<Void> ap,
    Pointer<Void> user_data,
  ) {
    print("@ wifi _connectionStateChangedCallback state=[${state}]");
  }

  static void _rssiLevelChangedCallback(int level, Pointer<Void> user_data) {
    print("@ wifi _rssiLevelChangedCallback level=[${level}]");
  }

  static void _scanFinishedCallback(int error_code, Pointer<Void> user_data) {
    print("@ wifi _scanFinishedCallback error=[${error_code}]");
  }

  static void _connectionTypeChangedCallback(
    int type,
    Pointer<Void> user_data,
  ) {
    print("@ wifi _connectionTypeChangedCallback type=[${type}]");
  }

  void initializeWifi() {
    return using((Arena arena) {
      wifiManagerHandle = arena<tz.wifi_manager_h>();
      final ret = tz.tizen.wifi_manager_initialize(wifiManagerHandle);
      if (ret == 0) {
        final callback = callbacks
            .register<Void Function(Int32, Pointer<Void>)>(
              'wifi_manager_device_state_changed_cb',
              Pointer.fromFunction(_deviceStateChangedCallback),
            );

        tz.tizen.wifi_manager_set_device_state_changed_cb(
          wifiManagerHandle.value,
          callback.interopCallback,
          callback.interopUserData,
        );

        final callback2 = callbacks
            .register<Void Function(Int32, Pointer<Void>, Pointer<Void>)>(
              'wifi_manager_connection_state_changed_cb',
              Pointer.fromFunction(_connectionStateChangedCallback),
            );

        tz.tizen.wifi_manager_set_connection_state_changed_cb(
          wifiManagerHandle.value,
          callback2.interopCallback,
          callback2.interopUserData,
        );

        final callback3 = callbacks
            .register<Void Function(Int32, Pointer<Void>)>(
              'wifi_manager_rssi_level_changed_cb',
              Pointer.fromFunction(_rssiLevelChangedCallback),
            );

        tz.tizen.wifi_manager_set_rssi_level_changed_cb(
          wifiManagerHandle.value,
          callback3.interopCallback,
          callback3.interopUserData,
        );

        final callback4 = callbacks
            .register<Void Function(Int32, Pointer<Void>)>(
              'wifi_manager_scan_finished_cb',
              Pointer.fromFunction(_scanFinishedCallback),
            );

        tz.tizen.wifi_manager_set_background_scan_cb(
          wifiManagerHandle.value,
          callback4.interopCallback,
          callback4.interopUserData,
        );

        // connectionHandle = arena<tz.connection_h>();
        // final ret = tz.tizen.connection_create(connectionHandle);

        // final callback5 = callbacks
        //     .register<Void Function(Int32, Pointer<Void>)>(
        //       '_connection_type_changed_cb',
        //       Pointer.fromFunction(_connectionTypeChangedCallback),
        //     );

        // tz.tizen.connection_set_type_changed_cb(
        //   connectionHandle.value,
        //   callback4.interopCallback,
        //   callback4.interopUserData,
        // );
      }

      //wlan_manager_set_message_callback(_wlan_event_handler)
      //_wlan_event_handler(wlan_mgr_event_info_t *event_info, void *user_data)

      // if (isActivated()) {
      //   scan();
      // } else {
      //   activate();
      // }
    });
  }

  bool isActivated() {
    return using((Arena arena) {
      final activated = arena<Bool>();
      final ret = tz.tizen.wifi_manager_is_activated(
        wifiManagerHandle.value,
        activated,
      );
      if (ret == 0 && activated.value == 1) {
        return true;
      }
      return false;
    });
  }

  static void _activatedCallback(int error_code, Pointer<Void> user_data) {
    print("@ wifi _activatedCallback error=[${error_code}]");
  }

  void activate() {
    //int 	wifi_manager_activate (wifi_manager_h wifi, wifi_manager_activated_cb callback, void *user_data)
    return using((Arena arena) {
      final callback = callbacks.register<Void Function(Int32, Pointer<Void>)>(
        'wifi_manager_activated_cb',
        Pointer.fromFunction(_activatedCallback),
      );

      final ret = tz.tizen.wifi_manager_activate(
        wifiManagerHandle.value,
        callback.interopCallback,
        callback.interopUserData,
      );
    });
  }

  void scan() {
    return using((Arena arena) {
      final callback = callbacks.register<Void Function(Int32, Pointer<Void>)>(
        'wifi_manager_scan_finished_cb',
        Pointer.fromFunction(_scanFinishedCallback),
      );

      final ret = tz.tizen.wifi_manager_scan(
        wifiManagerHandle.value,
        callback.interopCallback,
        callback.interopUserData,
      );
    });
  }

  //TODO:
  static void _connectedCallback(int error_code, Pointer<Void> user_data) {
    print("@ wifi _connectedCallback error=[${error_code}]");
  }

  Future<void> connect(Pointer<Void> ap) async {
    //int wifi_manager_connect (wifi_manager_h wifi, wifi_manager_ap_h ap, wifi_manager_connected_cb callback, void *user_data) async
    return using((Arena arena) {
      final handle = arena<tz.wifi_manager_h>();

      final callback = callbacks.register<Void Function(Int32, Pointer<Void>)>(
        'wifi_manager_connected_cb',
        Pointer.fromFunction(_scanFinishedCallback),
      );

      final ret = tz.tizen.wifi_manager_connect(
        handle.value,
        ap,
        callback.interopCallback,
        callback.interopUserData,
      );
    });
  }
}
