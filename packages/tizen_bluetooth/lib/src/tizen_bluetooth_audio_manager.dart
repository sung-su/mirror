import 'dart:async';
import 'dart:ffi';
import 'dart:typed_data';

import 'package:ffi/ffi.dart';
import 'package:flutter/foundation.dart';
import 'package:flutter/services.dart';
import 'package:tizen_interop/9.0/tizen.dart';
import 'package:tizen_interop_callbacks/tizen_interop_callbacks.dart';

import 'tizen_bluetooth_manager_type.dart';

class TizenBluetoothAudioManager {
  static final TizenInteropCallbacks callbacks = TizenInteropCallbacks();
  static bool initialized = false;

  static final methodChannel = const MethodChannel('tizen/bluetooth_audio');

  static void btInitialize() {
    if (initialized) return;

    int ret = tizen.bt_audio_initialize();
    if (ret != 0) {
      throw Exception(
        'Failed to bt_audio_initialize. Error code: ${tizen.get_error_message(ret).toDartString()}',
      );
    }

    initialized = true;
  }

  static void btDeinitialize() {
    if (!initialized) return;
    int ret = tizen.bt_audio_deinitialize();
    if (ret != 0) {
      throw Exception(
        'Failed to bt_audio_deinitialize. Error code: ${tizen.get_error_message(ret).toDartString()}',
      );
    }
    initialized = false;
  }

  static void btAudioConnect(
    String remoteAddress,
    BluetoothAudioProfileType type,
  ) {
    if (!initialized) return;

    final int ret = using((Arena arena) {
      final int connectResult = tizen.bt_audio_connect(
        remoteAddress.toNativeChar(allocator: arena),
        type.index,
      );
      if (connectResult != 0) {
        throw Exception(
          'Failed to bt_audio_connect. Error code: ${tizen.get_error_message(connectResult).toDartString()}',
        );
      }
      return connectResult;
    });
    if (ret != 0) {
      throw Exception('Failed to btAudioConnect.');
    }
  }

  static void btAudioDisconnect(
    String remoteAddress,
    BluetoothAudioProfileType type,
  ) {
    if (!initialized) return;

    final int ret = using((Arena arena) {
      final int connectResult = tizen.bt_audio_disconnect(
        remoteAddress.toNativeChar(allocator: arena),
        type.index,
      );
      return connectResult;
    });

    if (ret != 0) {
      throw Exception(
        'Failed to bt_audio_connect. Error code: ${tizen.get_error_message(ret).toDartString()}',
      );
    }
  }
}
