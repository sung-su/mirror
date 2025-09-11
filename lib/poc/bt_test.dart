import 'package:flutter/material.dart';
import 'package:tizen_bluetooth/tizen_bluetooth.dart';
import 'package:tizen_fs/models/bt_model.dart';

class BtTestPage extends StatefulWidget {
  @override
  State<BtTestPage> createState() => _BtTestPageState();
}

class _BtTestPageState extends State<BtTestPage> {
  BtModel _model = BtModel.init();

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('BT Test Apps')),
      body: Column(
        children: [
          // ElevatedButton(
          //   onPressed: () {
          //     BtManager.getState(); // 0 -diabeld, 1-enabled
          //   },
          //   child: Text('get state')
          // ),
          ElevatedButton(
            onPressed: () {
              try {
                TizenBluetoothManager.btInitialize();
              } catch (e, stack) {
                debugPrint('e=$e, stack=$stack');
              }
            },
            child: Text('init'),
          ),
          // ElevatedButton(
          //   onPressed: () {
          //     _model.setCallback();
          //   },
          //   child: Text('setcallback')
          // ),
          // ElevatedButton(
          //   onPressed: () {
          //     _model.unsetCallback();
          //   },
          //   child: Text('unsetCallback')
          // ),
          // ElevatedButton(
          //   onPressed: () {
          //     try {
          //       // TizenBluetoothManager.btAdapterEnable();
          //       _model.enable();
          //     }
          //     catch(e, stack) {
          //       debugPrint('e=$e, stack=$stack');
          //     }
          //   },
          //   child: Text('enable')
          // ),
          ElevatedButton(
            onPressed: () {
              TizenBluetoothManager.btAdapterStartDeviceDiscovery();
            },
            child: Text('start scan'),
          ),
          ElevatedButton(
            onPressed: () {
              TizenBluetoothManager.btAdapterStopDeviceDiscovery();
            },
            child: Text('stop scan'),
          ),
          ElevatedButton(
            onPressed: () {
              TizenBluetoothManager.btAdapterDisable();
            },
            child: Text('disable'),
          ),
          ElevatedButton(
            onPressed: () {
              TizenBluetoothManager.btDeinitialize();
            },
            child: Text('deinit'),
          ),
        ],
      ),
    );
  }
}
