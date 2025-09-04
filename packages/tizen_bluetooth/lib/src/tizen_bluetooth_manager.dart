import 'dart:async';
import 'dart:ffi';
import 'dart:typed_data';

import 'package:ffi/ffi.dart';
import 'package:flutter/foundation.dart';
import 'package:flutter/services.dart';
import 'package:tizen_interop/9.0/tizen.dart';
import 'package:tizen_interop_callbacks/tizen_interop_callbacks.dart';

import 'tizen_bluetooth_manager_type.dart';

typedef BtAdapterBondedDeviceCallback = void Function(BluetoothDeviceInfo);
typedef BtAdapterDeviceDiscoveryStateChangedCallback =
    void Function(int, int, DeviceDiscoveryInfo);

class TizenBluetoothManager {
  static final TizenInteropCallbacks callbacks = TizenInteropCallbacks();
  static bool initialized = false;

  static final Map<int, BtAdapterBondedDeviceCallback> _bondedDeviceCallbacks =
      {};
  static int _bondedDeviceCallbackIdCounter = 0;

  static final methodChannel = const MethodChannel('tizen/bluetooth');

  static Future<void> btInitialize() async {
    if (initialized) return;
    // await methodChannel.invokeMethod<String>('bt_initialize');

    int ret = tizen.bt_initialize();
    if (ret != 0) {
      throw Exception(
        'Failed to initialize Bluetooth. Error code: ${tizen.get_error_message(ret).toDartString()}',
      );
    }

    initialized = true;
  }

  static Future<void> btDeinitialize() async {
    if (!initialized) return;
    int ret = tizen.bt_deinitialize();
    if (ret != 0) {
      throw Exception(
        'Failed to deinitialize Bluetooth. Error code: ${tizen.get_error_message(ret).toDartString()}',
      );
    }
    initialized = false;
  }

  static Future<void> btAdapterEnable() async {
    if (!initialized) return;

    int ret = tizen.bt_adapter_enable();
    if (ret != 0) {
      throw Exception(
        'Failed to enable Bluetooth adapter. Error code: ${tizen.get_error_message(ret).toDartString()}',
      );
    }
  }

  static Future<void> btAdapterDisable() async {
    if (!initialized) return;

    int ret = tizen.bt_adapter_disable();
    if (ret != 0) {
      throw Exception(
        'Failed to disable Bluetooth adapter. Error code: ${tizen.get_error_message(ret).toDartString()}',
      );
    }
  }

  static bool onBtAdapterBondedDeviceCallback(
    Pointer<bt_device_info_s> deviceInfo,
    Pointer<Void> userData,
  ) {
    debugPrint('[Start] onBtAdapterBondedDeviceCallback');

    final BluetoothDeviceInfo bluetoothDeviceInfo =
        BluetoothDeviceInfo.deviceInfoSToBluetoothDeviceInfo(deviceInfo.ref);

    final Pointer<Int> idPtr = userData.cast<Int>();
    final callbackId = idPtr.value;

    final BtAdapterBondedDeviceCallback? callback = _bondedDeviceCallbacks
        .remove(callbackId);
    calloc.free(idPtr);

    if (callback != null) {
      callback(bluetoothDeviceInfo);
    } else {
      debugPrint('Callback not found for id: $callbackId');
    }

    return true;
  }

  static Future<void> btAdapterForeachBondedDevice(
    BtAdapterBondedDeviceCallback callback,
  ) async {
    final callbackId = _bondedDeviceCallbackIdCounter++;
    _bondedDeviceCallbacks[callbackId] = callback;

    final Pointer<Int> idPtr = calloc<Int>();
    idPtr.value = callbackId;

    int ret = tizen.bt_adapter_foreach_bonded_device(
      Pointer.fromFunction<bt_adapter_bonded_device_cbFunction>(
        onBtAdapterBondedDeviceCallback,
        false,
      ),
      idPtr.cast<Void>(),
    );

    if (ret != 0) {
      final error = tizen.get_error_message(ret).toDartString();
      debugPrint('Failed to bt_adapter_foreach_bonded_device: $error');
      _bondedDeviceCallbacks.remove(callbackId);
      calloc.free(idPtr);
    }
  }

  static const EventChannel _deviceDiscoveryStateChangedEventChannel =
      EventChannel('tizen/bluetooth/device_discovery_state_changed');

  static late StreamSubscription<DeviceDiscoveryInfo>?
  _deviceDiscoveryStateChangedSubscription;

  static late BtAdapterDeviceDiscoveryStateChangedCallback?
  _btAdapterDeviceDiscoveryStateChangedCallback;

  static Stream<DeviceDiscoveryInfo> get deviceDiscoveryStateChangedStream =>
      _deviceDiscoveryStateChangedEventChannel.receiveBroadcastStream().map(
        (dynamic event) => DeviceDiscoveryInfo.fromMap(
          (event as Map<dynamic, dynamic>).cast<String, dynamic>(),
        ),
      );

  /// TODO: service_uuid and manufacturer_data are not supported yet.
  static Future<void> btAdapterSetDeviceDiscoveryStateChangedCallback(
    BtAdapterDeviceDiscoveryStateChangedCallback callback,
  ) async {
    if (!initialized) return;

    _btAdapterDeviceDiscoveryStateChangedCallback = callback;
    await methodChannel.invokeMethod<String>(
      'init_device_discovery_state_changed_cb',
    );
  }

  static Future<void>
  btAdapterUnsetDeviceDiscoveryStateChangedCallback() async {
    if (!initialized) return;
    int ret = tizen.bt_adapter_unset_device_discovery_state_changed_cb();
    if (ret != 0) {
      throw Exception(
        'Failed to bt_adapter_unset_device_discovery_state_changed_cb. Error code: ${tizen.get_error_message(ret).toDartString()}',
      );
    }

    _deviceDiscoveryStateChangedSubscription?.cancel();
    _deviceDiscoveryStateChangedSubscription = null;
    _btAdapterDeviceDiscoveryStateChangedCallback = null;
  }

  static Future<void> btAdapterStartDeviceDiscovery() async {
    if (!initialized) return;

    if (_btAdapterDeviceDiscoveryStateChangedCallback == null) {
      debugPrint('No callback');
      return;
    }

    _deviceDiscoveryStateChangedSubscription = deviceDiscoveryStateChangedStream
        .listen((DeviceDiscoveryInfo info) {
          debugPrint(
            'call _btAdapterDeviceDiscoveryStateChangedCallback result ${info.result} state ${info.state}',
          );
          if (_btAdapterDeviceDiscoveryStateChangedCallback != null) {
            _btAdapterDeviceDiscoveryStateChangedCallback!(
              info.result,
              info.state,
              info,
            );
            if (info.state == 1) {
              _deviceDiscoveryStateChangedSubscription?.cancel();
            }
          }
        });

    int ret = tizen.bt_adapter_start_device_discovery();
    if (ret != 0) {
      throw Exception(
        'Failed to bt_adapter_start_device_discovery. Error code: ${tizen.get_error_message(ret).toDartString()}',
      );
    }
  }

  static Future<void> btAdapterStopDeviceDiscovery() async {
    if (!initialized) return;

    if (_btAdapterDeviceDiscoveryStateChangedCallback == null) {
      debugPrint('No callback');
      return;
    }

    int ret = tizen.bt_adapter_stop_device_discovery();
    if (ret != 0) {
      throw Exception(
        'Failed to bt_adapter_stop_device_discovery. Error code: ${tizen.get_error_message(ret).toDartString()}',
      );
    }

    // _deviceDiscoveryStateChangedSubscription = null;
    // _btAdapterDeviceDiscoveryStateChangedCallback = null;
  }
}
