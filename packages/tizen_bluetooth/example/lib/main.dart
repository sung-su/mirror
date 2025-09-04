import 'package:flutter/material.dart';
import 'dart:async';

import 'package:flutter/services.dart';
import 'package:tizen_bluetooth/tizen_bluetooth.dart';

void main() {
  runApp(const MyApp());
}

class MyApp extends StatefulWidget {
  const MyApp({super.key});

  @override
  State<MyApp> createState() => _MyAppState();
}

class _MyAppState extends State<MyApp> {
  // final _tizenBluetoothManager = TizenBluetoothManager();

  @override
  void initState() {
    super.initState();
    initPlatformState();
  }

  // Platform messages are asynchronous, so we initialize in an async method.
  Future<void> initPlatformState() async {}

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      home: Scaffold(
        appBar: AppBar(title: const Text('Plugin example app')),
        body: Center(
          child: Column(
            children: [
              Row(
                mainAxisAlignment: MainAxisAlignment.center,
                children: <Widget>[
                  TextButton(
                    onPressed: () async {
                      await TizenBluetoothManager.btInitialize();
                    },
                    child: const Text('btInitialize'),
                  ),
                  const SizedBox(width: 10),
                  TextButton(
                    onPressed: () async {
                      await TizenBluetoothManager.btDeinitialize();
                    },
                    child: const Text('btDeinitialize'),
                  ),
                  const SizedBox(width: 10),
                  TextButton(
                    onPressed: () async {
                      await TizenBluetoothManager.btAdapterEnable();
                    },
                    child: const Text('btAdapterEnable'),
                  ),
                  const SizedBox(width: 10),
                  TextButton(
                    onPressed: () async {
                      await TizenBluetoothManager.btAdapterDisable();
                    },
                    child: const Text('btAdapterDisable'),
                  ),
                ],
              ),
              const SizedBox(height: 10),
              Row(
                mainAxisAlignment: MainAxisAlignment.center,
                children: <Widget>[
                  TextButton(
                    onPressed: () async {
                      await TizenBluetoothManager.btAdapterStartDeviceDiscovery();
                    },
                    child: const Text('btAdapterStartDeviceDiscovery'),
                  ),
                  const SizedBox(width: 10),
                  TextButton(
                    onPressed: () async {
                      await TizenBluetoothManager.btAdapterStopDeviceDiscovery();
                    },
                    child: const Text('btAdapterStopDeviceDiscovery'),
                  ),
                  const SizedBox(width: 10),
                  TextButton(
                    onPressed: () async {
                      await TizenBluetoothManager.btAdapterForeachBondedDevice((
                        BluetoothDeviceInfo deviceInfo,
                      ) {
                        debugPrint(
                          'bonded device: ${deviceInfo.remoteName}(${deviceInfo.remoteAddress})',
                        ); //정상 동작
                      });
                    },
                    child: const Text('btAdapterForeachBondedDevice'),
                  ),
                  const SizedBox(width: 10),
                  TextButton(
                    onPressed: () async {
                      await TizenBluetoothManager.btAdapterSetDeviceDiscoveryStateChangedCallback((
                        int result,
                        int discoveryState,
                        DeviceDiscoveryInfo deviceDiscoveryInfo,
                      ) {
                        if (discoveryState == 2) {
                          debugPrint(
                            'Discovery => remoteName : ${deviceDiscoveryInfo.remoteName} / remoteAddress : ${deviceDiscoveryInfo.remoteAddress} / '
                            'btClass : ${deviceDiscoveryInfo.btClass.minorDeviceClass}.'
                            '${deviceDiscoveryInfo.btClass.majorDeviceClass}.'
                            '${deviceDiscoveryInfo.btClass.majorServiceClassMask} /'
                            'isBonded :${deviceDiscoveryInfo.isBonded} /'
                            'rssi :${deviceDiscoveryInfo.rssi} /'
                            'appearance :${deviceDiscoveryInfo.appearance} /'
                            'serviceUuid :${deviceDiscoveryInfo.serviceUuid} /'
                            'manufacturerData :${deviceDiscoveryInfo.manufacturerData} /'
                            'manufacturerDataLen :${deviceDiscoveryInfo.manufacturerDataLen} /',
                          );
                        }
                      });
                    },
                    child: const Text(
                      'btAdapterSetDeviceDiscoveryStateChangedCallback',
                    ),
                  ),
                  TextButton(
                    onPressed: () async {
                      await TizenBluetoothManager.btAdapterUnsetDeviceDiscoveryStateChangedCallback();
                    },
                    child: const Text(
                      'btAdapterUnsetDeviceDiscoveryStateChangedCallback',
                    ),
                  ),
                ],
              ),
            ],
          ),
        ),
      ),
    );
  }
}
//D4:20:9E:A5:E8:9E