
import 'package:flutter/foundation.dart';
import 'package:tizen_bluetooth/tizen_bluetooth.dart';
import 'package:tizen_fs/native/bt_manager.dart';
import 'package:tizen_interop/6.0/tizen.dart';

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
  
  bool _isOn = false;
  bool _isLoading = false;
  bool _initialized = false;

  
  List<String> get devices => _devices;

  // BtModel(this.contents);

  BtModel.fromMock() {
    _initialized = true;
    _isOn = false;
    _connectedDevices = ["Device1", "Device2"];
    _foundDevices = ["QLED", "55\" Neo QLED", "AI Home REference", "MR Music Frame", "43\" Neo QLED"];

    _devices = [..._connectedDevices, ..._foundDevices];

    _data = {
      'Bluetooth' : ['Bluetooth on/off'],
      'Connected Devices': [..._connectedDevices],
      'Available Devices': [..._foundDevices],
    };
    //notify
  }

  
  void initialize() async {
    if(_initialized) return;

    await TizenBluetoothManager.btInitialize();
    
    int state = BtManager.getState();
    debugPrint(' bet state = $state');

    _initialized = true;
  }

}
