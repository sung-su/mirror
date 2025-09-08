
import 'dart:async';
import 'dart:developer';
import 'dart:ffi';

import 'package:ffi/ffi.dart';
import 'package:flutter/foundation.dart';
import 'package:tizen_bluetooth/tizen_bluetooth.dart';
import 'package:tizen_fs/native/bt_manager.dart';
import 'package:tizen_interop/9.0/tizen.dart';

class BtDevice {
  final String remoteName;
  final String remoteAddress;
  int rssi;
  bool isBonded;
  bool isConnected;
  late List<String> serviceUuid = [];
  final int serviceCount;
  bool get isAudioSupported => ((serviceMask & BtServiceType.a2dp) == BtServiceType.a2dp || ((serviceMask & BtServiceType.hsp) == BtServiceType.hsp));
  bool get isHidSupported => ((serviceMask & BtServiceType.hid) == BtServiceType.hid);
  late int serviceMask;

  BtDevice({
    required this.remoteName,
    required this.remoteAddress,
    this.rssi = 0,
    this.isBonded = false,
    this.isConnected = false,
    required List<String> serviceUuid,
    required this.serviceCount,
  }) : serviceMask = getMaskFromUuid(serviceUuid, serviceCount, remoteName);
  
  static int getMaskFromUuid(List<String> serviceUuid, int serviceCount, String name) {
    debugPrint('!!!!!!!!!!!!!!!!!!!!!!!name: $name');

    return using((Arena arena) {
      final uuidsPtr = arena<Pointer<Char>>(serviceCount);

      for (int i = 0; i < serviceCount; i++) {
        final strPtr = serviceUuid[i].toNativeUtf8(allocator: arena).cast<Char>();
        uuidsPtr[i] = strPtr;
      }

      final serviceMask = arena<Int32>();
      tizen.bt_device_get_service_mask_from_uuid_list(uuidsPtr, serviceCount, serviceMask);  

      debugPrint('mask: ${serviceMask.value}');

      return serviceMask.value;
    }) ?? 0;
  }
}

class Item {
  final bool isKey;
  final Object item;

  Item({required this.isKey, required this.item});
}

class BtModel extends ChangeNotifier {
  Map<String, List> _data = {};
  // Map<String, Object> get model => _data;
  
  List<Item> _flattened = [];
  List<Item> get data => _flattenData();

  List get _connectedDevices => _data['Paired Devices'] as List;
  List get _foundDevices => _data['Available Devices'] as List;

  bool _isEnabled = false;
  bool get isEnabled => _isEnabled;

  List<Item> _flattenData() {
    _flattened = <Item>[];

    _data.forEach((key, list) {
      if(list.isNotEmpty) {
         _flattened.add(Item(isKey: true, item: key));
        for(var item in list) {
          _flattened.add(Item(isKey: false, item: item));
        }
      }
    });
    
    return _flattened;
  }

  BtDevice? getDevice(int index) {
    if(_flattened.length > index)
      return _flattened[index].item as BtDevice;

    return null;
  }

  BtModel.fromMock() {
    // Timeline.startSync('BtModel_fromMock');

    _data = {
      'Your device(Tizen) in currentrly visible to nearby devices.' : ['Bluetooth'],
      'Paired Devices': [],
      'Available Devices': [],
    };

    debugPrint('BtModel.fromMock call: btInitialize');
    TizenBluetoothManager.btInitialize();

  }

  BtModel.init() {
    if(_isEnabled) return;

    _data = {
      'Your device(Tizen) in currentrly visible to nearby devices.' : ['Bluetooth'],
      'Paired Devices': [],
      'Available Devices': [],
    };

    debugPrint('BtModel.fromMock call: btInitialize');
    TizenBluetoothManager.btInitialize();    
  }

  // 0 - disabled, 1-enabled
  void _btAdapterStateChanged(int result, int state) async {
    if(state == 1) {
      updateConnectedDevices();
    }
  }

  void _deviceDiscoveryStateChanged(int result, int discoveryState, DeviceDiscoveryInfo deviceInfo) {
    debugPrint('_deviceDiscoveryStateChanged: resut=$result, state=$discoveryState, name=${deviceInfo.remoteName}');

    if(discoveryState == 2) {
      _foundDevices.add(
        BtDevice(
          remoteName: deviceInfo.remoteName,
          remoteAddress: deviceInfo.remoteAddress,
          serviceCount: deviceInfo.serviceCount,
          serviceUuid: deviceInfo.serviceUuid
        )
      );
      notifyListeners();
    }
  }

  Future<void> setCallback() async {
    // Timeline.startSync('BtModel_setCallback');
    debugPrint('set callback');

    TizenBluetoothManager.btAdapterSetStateChangedCallback(_btAdapterStateChanged);
    TizenBluetoothManager.btAdapterSetDeviceDiscoveryStateChangedCallback(_deviceDiscoveryStateChanged);
    // Timeline.finishSync();
  }

  void _safeCall(VoidCallback callback) {
    try {
      callback();
    }
    catch(e, stack) {
      debugPrint('$e, $stack');
    }
  }


  Future<void> unsetCallback() async {
    debugPrint('unset callback');

    TizenBluetoothManager.btAdapterUnsetStateChangedCallback();
    TizenBluetoothManager.btAdapterUnsetDeviceDiscoveryStateChangedCallback();
  }

  Future<void> updateConnectedDevices() async {
    _connectedDevices.clear();

    TizenBluetoothManager.btAdapterForeachBondedDevice((deviceInfo){
      //BluetoothDeviceInfo
      debugPrint("get bonded device: ${deviceInfo.remoteName} : ${deviceInfo.remoteAddress}");
      _connectedDevices.add( BtDevice(
        remoteName: deviceInfo.remoteName,
        remoteAddress: deviceInfo.remoteAddress,
        isBonded: deviceInfo.isBonded,
        isConnected: deviceInfo.isConnected,
        serviceCount: deviceInfo.serviceCount,
        serviceUuid: deviceInfo.serviceUuid
      ));
    });

    notifyListeners();
  }

  Future<void> enable() async {
    debugPrint('bt enable: TizenBluetoothManager.initialized=${TizenBluetoothManager.initialized}');

    setCallback();

    // 0-disable, 1-enable
    final state = BtManager.getState();
    if (state == 1) {
      debugPrint('bt enable: device aleary enabled!!');
      updateConnectedDevices();
    } 
    else {
      if(!TizenBluetoothManager.initialized) {
        debugPrint('bt enable: initialize');
        TizenBluetoothManager.btInitialize();
      }
      
      debugPrint('bt enable: enable call');
      // Timeline.startSync('BtModel.enable');

      _safeCall((){
        TizenBluetoothManager.btAdapterEnable();
      });
      // Timeline.finishSync();
    }

    _isEnabled = true;
    notifyListeners();

    debugPrint('bt enable: scan start');
    _foundDevices.clear();
    _safeCall((){
      TizenBluetoothManager.btAdapterStartDeviceDiscovery();
    });
  }

  Future<void> stopDiscovery() async {
    debugPrint('bt stopDiscovery');
    if(!TizenBluetoothManager.initialized) {
      debugPrint('bt not initialized');
      return;
    }

    _safeCall((){
      TizenBluetoothManager.btAdapterStopDeviceDiscovery();
    });    
  }
  
  Future<void> disable() async {
    debugPrint('bt disable: TizenBluetoothManager.initialized=${TizenBluetoothManager.initialized}');
    // Timeline.startSync('BtModel.disable ');
    if(!TizenBluetoothManager.initialized) {
      debugPrint('bt enable: initialize');
      return;
    }

    _safeCall((){
      TizenBluetoothManager.btAdapterStopDeviceDiscovery();
    });

    unsetCallback();

    if(_hidInitialized)
      deinitializeHid();

    debugPrint('bt enable: disable call');
    TizenBluetoothManager.btAdapterDisable();
    
    _isEnabled = false;

    _connectedDevices.clear();
    _foundDevices.clear();

    notifyListeners();
  }

  Future<bool> pair(BtDevice device) async {
    final completer = Completer<bool>();
    // 0 - complete, 1 - failed
    TizenBluetoothManager.btDeviceSetBondCreatedCallback((result, deviceInfo)
    {
      debugPrint('BondCreatedCallback: result : $result / ${deviceInfo.remoteName}(${deviceInfo.remoteAddress})',);

      TizenBluetoothManager.btDeviceUnsetBondCreatedCallback();
      if (result == 0) {
        //remove available
        //add paired
        final device = _foundDevices.where((item) => (item as BtDevice)?.remoteAddress == deviceInfo.remoteAddress).first;
        device.isBonded = true;
        _connectedDevices.add(device);
        _foundDevices.remove(device);
        completer.complete(true);
      }
      else {
        completer.complete(false);
      }
    });
    debugPrint('bond');
    TizenBluetoothManager.btDeviceCreateBond(device.remoteAddress);
    return completer.future;
  }

  Future<bool> connect(BtDevice device) async {
    final completer = Completer<bool>();
    debugPrint(' connect to device : ${device.remoteName}');

    bool paired = device.isBonded;
    if(!paired) {
      paired = await pair(device);
    } else {
      debugPrint('device already paired');
    }

    if(paired) {
      if(device.isAudioSupported) {
        await connectAudio(device);
      }
      else if(device.isHidSupported) {
        debugPrint('### connect to hid');
      }
      else {
        debugPrint('### jus paired');  
      }
      
      completer.complete(true);
    } else {
      completer.complete(false);
    }

    return completer.future;
  }

  Future<bool> connectAudio(BtDevice device) async {
    debugPrint('### connect to audio');
    final completer = Completer<bool>();
    TizenBluetoothAudioManager.btInitialize();

    //All, hsp_hep, a2dp, ag
    TizenBluetoothAudioManager.btAudioSetConnectionStateChangedCallback(
      (result, connected, remoteAddress, profileType) {
        debugPrint('btAudioSetConnectionStateChangedCallback : result=$result, connected=$connected');

        TizenBluetoothAudioManager.btAudioUnsetConnectionStateChangedCallback();
        if (result == 0) {
          device.isConnected = true;
          notifyListeners();
          completer.complete(true);
        }
        else {
          completer.complete(false);
        }
      }
    );

    TizenBluetoothAudioManager.btAudioConnect(device.remoteAddress, BluetoothAudioProfileType.profileTypeA2DP);
    return completer.future;
  }

  Future<bool> disconnectAudio(BtDevice device) async {
    debugPrint('### disconnect to audio');
    final completer = Completer<bool>();
    TizenBluetoothAudioManager.btInitialize();

    //All, hsp_hep, a2dp, ag
    TizenBluetoothAudioManager.btAudioSetConnectionStateChangedCallback(
      (result, connected, remoteAddress, profileType) {
        debugPrint('btAudioSetConnectionStateChangedCallback : result=$result, connected=$connected');

        TizenBluetoothAudioManager.btAudioUnsetConnectionStateChangedCallback();
        if (result == 0) {
          device.isConnected = connected;
          notifyListeners();
          completer.complete(true);
        }
        else {
          completer.complete(false);
        }
      }
    );

    debugPrint('###### disconnect to audio call: device-${device.remoteName}');
    TizenBluetoothAudioManager.btAudioDisconnect(device.remoteAddress, BluetoothAudioProfileType.profileTypeA2DP);
    return completer.future;
  }

  static void _onHidConnectStateChanged(int result, bool connected, String remoteAddress) {
    debugPrint('hid host initialized');

      if(result == 0) {
        if (_hidCallbackCompleter.keys.contains(remoteAddress)) {
          _hidCallbackCompleter[remoteAddress]?.complete(true);
          _hidCallbackCompleter.remove(remoteAddress);
        }

        // completer.complete(true);
      }
      else {
        if (_hidCallbackCompleter.keys.contains(remoteAddress)) {
          _hidCallbackCompleter[remoteAddress]?.complete(false);
          _hidCallbackCompleter.remove(remoteAddress);
        }
        // completer.complete(false);
      }
  }

  Future<void> initializeHid() async {
    debugPrint('### initializeHid');
    // final completer = Completer<bool>();

    TizenBluetoothHidHostManager.btInitialize(_onHidConnectStateChanged);
    _hidInitialized = true;

    // return completer.future;
  }

  static Map<String, Completer> _hidCallbackCompleter = {};
  bool _hidInitialized = false;

  Future<bool> connectHid(String remoteAddress) async {
    debugPrint('### connect to hid');
    final completer = Completer<bool>();

    if(!_hidInitialized) {
      await initializeHid();
    }

    _hidCallbackCompleter[remoteAddress] = completer;

    TizenBluetoothHidHostManager.btConnect(remoteAddress);
    
    return completer.future;
  }

  void deinitializeHid() {
    debugPrint('### deinitializeHid');
    TizenBluetoothHidHostManager.btDeinitialize();
    _hidInitialized = false;    
  }

  Future<bool> disconnectHid(String remoteAddress) async {
    debugPrint('### disconnect to hid');
    final completer = Completer<bool>();

    if(!_hidInitialized) {
      await initializeHid();
    }

    _hidCallbackCompleter[remoteAddress] = completer;

    TizenBluetoothHidHostManager.btDisconnect(remoteAddress);
    
    return completer.future;
  }

  Future<void> disconnect(BtDevice device) async {
    debugPrint(' disconnect to device : ${device.remoteName}'); 

    if(device.isAudioSupported) {
      await disconnectAudio(device);
    }
    else if(device.isHidSupported) {
      final result = await disconnectHid(device.remoteAddress);
      if(result) {
        device.isConnected = false;
      }
    }

  }

  Future<bool> Unpair(BtDevice device) async {
    final completer = Completer<bool>();
    debugPrint('unpair to device : ${device.remoteName}');

    TizenBluetoothManager.btDeviceSetBondDestroyedCallback((result, remoteaddress)
    {
      debugPrint('BondDestroyedCallback: result : $result / address:$remoteaddress',);

      TizenBluetoothManager.btDeviceUnsetBondDestroyedCallback();
      if (result == 0) {
        //remove paired
        final device = _connectedDevices.where((item) => (item as BtDevice)?.remoteAddress == remoteaddress).first;
        device.isConnected = false;
        device.isBonded= false;
        _foundDevices.insert(0, device);
        _connectedDevices.remove(device);
        completer.complete(true);
      }
      else {
        completer.complete(false);
      }
    });
    debugPrint('bond destroy');
    TizenBluetoothManager.btDeviceDestroyBond(device.remoteAddress); 
    return completer.future;
  }

  void moveToPaired(String remoteAddress) {
    
    notifyListeners();
  }

}

class BtServiceType
{
  static const int none = 0;
  static const int a2dp = 0x00040000;
  static const int hsp = 0x00000020;
  static const int hid = 0x00200000;
  static const int all = 0x00FFFFFF;
}