
import 'dart:developer';

import 'package:flutter/foundation.dart';
import 'package:tizen_bluetooth/tizen_bluetooth.dart';

class BtDevice {
  String remoteName;
  String remoteAddress;

  BtDevice({
    this.remoteName = '',
    this.remoteAddress = ''
  });
}

class BtModel extends ChangeNotifier {
  Map<String, Object> _data = {};
  Map<String, Object> get data => _data;

  List<String> _connectedDevices = [];
  List<String> _foundDevices = [];
  List<String> _devices = [];
  
  List<String> get devices => _devices;

  bool _isEnabled = false;
  bool get isEnabled => _isEnabled;
  
  BtModel.fromMock() {
    _connectedDevices = ["Device1", "Device2"];
    _foundDevices = ["QLED", "55\" Neo QLED", "AI Home REference", "MR Music Frame", "43\" Neo QLED"];

    _devices = [..._connectedDevices, ..._foundDevices];

    _data = {
      'Your device(Tizen) in currentrly visible to nearby devices.' : ['Bluetooth'],
      'Paired Devices': [..._connectedDevices],
      'Available Devices': [..._foundDevices],
    };
  }

  Future<void> enable() async {
    debugPrint('bt enable: TizenBluetoothManager.initialized=${TizenBluetoothManager.initialized}');
    Timeline.startSync('BtModel.enable ');
    if(!TizenBluetoothManager.initialized) {
      debugPrint('bt enable: initialize');
      await TizenBluetoothManager.btInitialize();
    }

    debugPrint('bt enable: enable call');
    await TizenBluetoothManager.btAdapterEnable();
    _isEnabled = true;
    Timeline.finishSync();
    notifyListeners();
  }
  
  Future<void> disable() async {
    debugPrint('bt disable: TizenBluetoothManager.initialized=${TizenBluetoothManager.initialized}');
    Timeline.startSync('BtModel.disable ');
    if(!TizenBluetoothManager.initialized) {
      debugPrint('bt enable: initialize');
      return;
    }

    debugPrint('bt enable: disable call');
    await TizenBluetoothManager.btAdapterDisable();
    _isEnabled = false;
    Timeline.finishSync();
    notifyListeners();
  }
}
