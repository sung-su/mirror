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
typedef BtAdapterSetStateChangedCallback = void Function(int, int);

typedef BtAdapterDeviceDiscoveryStateChangedCallback =
    void Function(int, int, DeviceDiscoveryInfo);
typedef BtDeviceSetBondCreatedCallback =
    void Function(int, BluetoothDeviceInfo);

typedef BtDeviceBondDestroyedCallback = void Function(int, String);

class TizenBluetoothManager {
  static final TizenInteropCallbacks callbacks = TizenInteropCallbacks();
  static bool initialized = false;

  static final Map<int, BtAdapterBondedDeviceCallback> _bondedDeviceCallbacks =
      {};
  static int _bondedDeviceCallbackIdCounter = 0;

  static final Map<int, BtAdapterSetStateChangedCallback>
  _btAdapterSetStateChangedCallbackCallbacks = {};
  static int _btAdapterSetStateChangedCallbackIdCounter = 0;

  static final Map<int, BtDeviceBondDestroyedCallback>
  _deviceBondDestroyedCallbacks = {};
  static int _deviceBondDestroyedIdCounter = 0;

  static final methodChannel = const MethodChannel('tizen/bluetooth');

  static void btInitialize() {
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

  static void btDeinitialize() {
    if (!initialized) return;
    int ret = tizen.bt_deinitialize();
    if (ret != 0) {
      throw Exception(
        'Failed to deinitialize Bluetooth. Error code: ${tizen.get_error_message(ret).toDartString()}',
      );
    }
    initialized = false;
  }

  static void btAdapterEnable() {
    if (!initialized) return;

    int ret = tizen.bt_adapter_enable();
    if (ret != 0) {
      throw Exception(
        'Failed to enable Bluetooth adapter. Error code: ${tizen.get_error_message(ret).toDartString()}',
      );
    }
  }

  static void btAdapterDisable() {
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

  static void btAdapterForeachBondedDevice(
    BtAdapterBondedDeviceCallback callback,
  ) {
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

  static Future<void> btAdapterSetDeviceDiscoveryStateChangedCallback(
    BtAdapterDeviceDiscoveryStateChangedCallback callback,
  ) async {
    if (!initialized) return;

    _btAdapterDeviceDiscoveryStateChangedCallback = callback;
    await methodChannel.invokeMethod<String>(
      'init_device_discovery_state_changed_cb',
    );
  }

  static void btAdapterUnsetDeviceDiscoveryStateChangedCallback() {
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

  static void btAdapterStartDeviceDiscovery() {
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

  static void btAdapterStopDeviceDiscovery() {
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
  }

  static void onBtAdapterSetStateChangedCallback(
    int result,
    int state,
    Pointer<Void> userData,
  ) {
    debugPrint('CalledonBtAdapterSetStateChangedCallback');

    final callbackId = TizenInteropCallbacks.getUserObject<int>(userData)!;

    final BtAdapterSetStateChangedCallback? callback =
        _btAdapterSetStateChangedCallbackCallbacks[callbackId];

    if (callback != null) {
      callback(result, state);
    } else {
      debugPrint('Callback not found for id: $callbackId');
    }

    return;
  }

  static void btAdapterSetStateChangedCallback(
    BtAdapterSetStateChangedCallback callback,
  ) {
    if (!initialized) return;

    final callbackId = _btAdapterSetStateChangedCallbackIdCounter++;
    _btAdapterSetStateChangedCallbackCallbacks[callbackId] = callback;

    final Pointer<Int> idPtr = calloc<Int>();
    idPtr.value = callbackId;

    final stateChangedCallback = callbacks
        .register<bt_adapter_state_changed_cbFunction>(
          'bt_adapter_state_changed_cb',
          Pointer.fromFunction(onBtAdapterSetStateChangedCallback),
          userObject: callbackId,
          blocking: false,
        );
    int ret = tizen.bt_adapter_set_state_changed_cb(
      stateChangedCallback.interopCallback,
      stateChangedCallback.interopUserData,
    );

    if (ret != 0) {
      _btAdapterSetStateChangedCallbackCallbacks.remove(callbackId);
      calloc.free(idPtr);
      throw Exception(
        'Failed to bt_adapter_set_state_changed_cb. Error code: ${tizen.get_error_message(ret).toDartString()}',
      );
    }
  }

  static void btAdapterUnsetStateChangedCallback() {
    if (!initialized) return;
    int ret = tizen.bt_adapter_unset_state_changed_cb();
    if (ret != 0) {
      throw Exception(
        'Failed to bt_adapter_unset_state_changed_cb. Error code: ${tizen.get_error_message(ret).toDartString()}',
      );
    }

    _btAdapterSetStateChangedCallbackIdCounter = 0;
    _btAdapterSetStateChangedCallbackCallbacks.clear();
  }

  static const EventChannel _deviceSetBondCreatedEventChannel = EventChannel(
    'tizen/bluetooth/device_bond_created',
  );

  static late StreamSubscription<BluetoothDeviceInfo>?
  _deviceSetBondCreatedSubscription;

  static late BtDeviceSetBondCreatedCallback? _btDeviceSetBondCreatedCallback;

  static Stream<BluetoothDeviceInfo> get deviceBondCreatedStream =>
      _deviceSetBondCreatedEventChannel.receiveBroadcastStream().map(
        (dynamic event) => BluetoothDeviceInfo.fromMap(
          (event as Map<dynamic, dynamic>).cast<String, dynamic>(),
        ),
      );

  static void btDeviceCreateBond(String remoteAddress) {
    if (!initialized) return;

    if (_btDeviceSetBondCreatedCallback == null) {
      debugPrint('No callback');
      return;
    }

    _deviceSetBondCreatedSubscription = deviceBondCreatedStream.listen((
      BluetoothDeviceInfo info,
    ) {
      debugPrint(
        'call _btAdapterDeviceDiscoveryStateChangedCallback result ${info.result}',
      );
      if (_btDeviceSetBondCreatedCallback != null) {
        _btDeviceSetBondCreatedCallback!(info.result, info);
      }
    });

    final int ret = using((Arena arena) {
      final int ret = tizen.bt_device_create_bond(
        remoteAddress.toNativeChar(allocator: arena),
      );
      return ret;
    });
    if (ret != 0) {
      debugPrint(
        'Failed to bt_device_create_bond. Error code: ${tizen.get_error_message(ret).toDartString()}',
      );
    }
  }

  static void btDeviceDestroyBond(String remoteAddress) {
    if (!initialized) return;

    final int ret = using((Arena arena) {
      final int ret = tizen.bt_device_destroy_bond(
        remoteAddress.toNativeChar(allocator: arena),
      );
      return ret;
    });
    if (ret != 0) {
      debugPrint(
        'Failed to bt_device_destroy_bond. Error code: ${tizen.get_error_message(ret).toDartString()}',
      );
    }
  }

  static Future<void> btDeviceSetBondCreatedCallback(
    BtDeviceSetBondCreatedCallback callback,
  ) async {
    if (!initialized) return;

    _btDeviceSetBondCreatedCallback = callback;
    await methodChannel.invokeMethod<String>(
      'init_bt_device_set_bond_created_cb',
    );
  }

  static void btDeviceUnsetBondCreatedCallback() {
    if (!initialized) return;
    int ret = tizen.bt_device_unset_bond_created_cb();
    if (ret != 0) {
      throw Exception(
        'Failed to bt_device_unset_bond_created_cb. Error code: ${tizen.get_error_message(ret).toDartString()}',
      );
    }

    _deviceSetBondCreatedSubscription?.cancel();
    _deviceSetBondCreatedSubscription = null;
    _btDeviceSetBondCreatedCallback = null;
  }

  /*
  static void onBtDeviceSetBondDestroyedCallback(
    int result,
    Pointer<Char> remoteAddress,
    Pointer<Void> userData,
  ) {
    debugPrint('[Start] onBtDeviceSetBondDestroyedCallback');

    final callbackId = TizenInteropCallbacks.getUserObject<int>(userData)!;

    final BtDeviceBondDestroyedCallback? callback =
        _deviceBondDestroyedCallbacks.remove(callbackId);
    String stringValue = '';
    debugPrint('check 1');
    if (remoteAddress != null) {
      debugPrint('check 2');
      stringValue = remoteAddress.toDartString();
      debugPrint('check 3');
    }
    debugPrint('check 4');
    if (callback != null) {
      callback(result, stringValue);
    } else {
      debugPrint('Callback not found for id: $callbackId');
    }

    return;
  }

  static void btDeviceSetBondDestroyedCallback(
    BtDeviceBondDestroyedCallback callback,
  ) {
    final callbackId = _deviceBondDestroyedIdCounter++;
    _deviceBondDestroyedCallbacks[callbackId] = callback;

    final Pointer<Int> idPtr = calloc<Int>();
    idPtr.value = callbackId;

    final bondDestroyedCallback = callbacks
        .register<bt_device_bond_destroyed_cbFunction>(
          'bt_device_bond_destroyed_cb',
          Pointer.fromFunction(onBtDeviceSetBondDestroyedCallback),
          userObject: callbackId,
        );
    int ret = tizen.bt_device_set_bond_destroyed_cb(
      bondDestroyedCallback.interopCallback,
      bondDestroyedCallback.interopUserData,
    );

    if (ret != 0) {
      _deviceBondDestroyedCallbacks.remove(callbackId);
      throw Exception(
        'Failed to bt_device_set_bond_created_cb. Error code: ${tizen.get_error_message(ret).toDartString()}',
      );
    }
  }*/
}
