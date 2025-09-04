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

  String targetBondAdress = 'none';

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
                    onPressed: () {
                      debugPrint('call btInitialize()');
                      TizenBluetoothManager.btInitialize();
                      debugPrint('call btAdapterSetStateChangedCallback()');
                      TizenBluetoothManager.btAdapterSetStateChangedCallback((
                        int result,
                        int state,
                      ) {
                        debugPrint(
                          'btAdapterSetStateChangedCallback : result $result / state $state',
                        );
                      });
                    },
                    child: const Text('btInitialize'),
                  ),
                  const SizedBox(width: 10),
                  TextButton(
                    onPressed: () {
                      debugPrint('call btDeinitialize()');
                      TizenBluetoothManager.btDeinitialize();
                    },
                    child: const Text('btDeinitialize'),
                  ),
                  const SizedBox(width: 10),
                  TextButton(
                    onPressed: () {
                      debugPrint('call btAdapterEnable()');
                      TizenBluetoothManager.btAdapterEnable();
                    },
                    child: const Text('btAdapterEnable'),
                  ),
                  const SizedBox(width: 10),
                  TextButton(
                    onPressed: () {
                      debugPrint('call btAdapterDisable()');
                      TizenBluetoothManager.btAdapterDisable();
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
                    onPressed: () {
                      debugPrint('call btAdapterStartDeviceDiscovery()');
                      TizenBluetoothManager.btAdapterStartDeviceDiscovery();
                    },
                    child: const Text('btAdapterStartDeviceDiscovery'),
                  ),
                  const SizedBox(width: 10),
                  TextButton(
                    onPressed: () {
                      debugPrint('call btAdapterStopDeviceDiscovery()');
                      TizenBluetoothManager.btAdapterStopDeviceDiscovery();
                    },
                    child: const Text('btAdapterStopDeviceDiscovery'),
                  ),
                  const SizedBox(width: 10),
                  TextButton(
                    onPressed: () {
                      debugPrint('call btAdapterForeachBondedDevice()');
                      TizenBluetoothManager.btAdapterForeachBondedDevice((
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
                      debugPrint(
                        'call btAdapterSetDeviceDiscoveryStateChangedCallback()',
                      );
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
                    onPressed: () {
                      debugPrint(
                        'call btAdapterUnsetDeviceDiscoveryStateChangedCallback()',
                      );
                      TizenBluetoothManager.btAdapterUnsetDeviceDiscoveryStateChangedCallback();
                    },
                    child: const Text(
                      'btAdapterUnsetDeviceDiscoveryStateChangedCallback',
                    ),
                  ),
                  // TextButton(
                  //   onPressed: () async {
                  //     TizenBluetoothManager.btAdapterSetStateChangedCallback((
                  //       int result,
                  //       int state,
                  //     ) {
                  //       debugPrint(
                  //         'btAdapterSetStateChangedCallback : result $result / state $state',
                  //       );
                  //     });
                  //   },
                  //   child: const Text('btAdapterSetStateChangedCallback'),
                  // ),
                ],
              ),
              const SizedBox(height: 10),
              Row(
                mainAxisAlignment: MainAxisAlignment.center,
                children: <Widget>[
                  TextButton(
                    onPressed: () {
                      debugPrint('call btInitialize()');
                      TizenBluetoothAudioManager.btInitialize();
                    },
                    child: const Text('btInitialize'),
                  ),
                  const SizedBox(width: 10),
                  TextButton(
                    onPressed: () {
                      debugPrint('call btDeinitialize()');
                      TizenBluetoothAudioManager.btDeinitialize();
                    },
                    child: const Text('btDeinitialize'),
                  ),
                  const SizedBox(width: 10),
                  TextButton(
                    onPressed: () {
                      debugPrint(
                        'call btAudioConnect(\'$targetBondAdress\', )',
                      );
                      TizenBluetoothAudioManager.btAudioConnect(
                        targetBondAdress,
                        BluetoothAudioProfileType.profileTypeAll,
                      );
                    },
                    child: Text('btAudioConnect(\'$targetBondAdress\', All)'),
                  ),
                  const SizedBox(width: 10),
                  TextButton(
                    onPressed: () {
                      debugPrint(
                        'call btAudioDisconnect(\'$targetBondAdress\', )',
                      );
                      TizenBluetoothAudioManager.btAudioDisconnect(
                        targetBondAdress,
                        BluetoothAudioProfileType.profileTypeAll,
                      );
                    },
                    child: Text(
                      'btAudioDisconnect(\'$targetBondAdress\', All)',
                    ),
                  ),
                  const SizedBox(width: 10),
                  TextButton(
                    onPressed: () {
                      debugPrint('call btDeviceBondCreatedCallback()');
                      TizenBluetoothManager.btDeviceSetBondCreatedCallback((
                        int result,
                        BluetoothDeviceInfo deviceInfo,
                      ) {
                        debugPrint(
                          'btDeviceBondCreatedCallback: result : $result / ${deviceInfo.remoteName}(${deviceInfo.remoteAddress})',
                        );
                        setState(() {
                          targetBondAdress = deviceInfo.remoteAddress; //정상 동작
                        });
                      });

                      debugPrint(
                        'call btDeviceCreateBond(\'54:10:4F:D2:74:4F\', )',
                      );
                      TizenBluetoothManager.btDeviceCreateBond(
                        '54:10:4F:D2:74:4F',
                      );
                    },
                    child: const Text(
                      'btDeviceCreateBond(\'54:10:4F:D2:74:4F\')',
                    ),
                  ),
                  const SizedBox(width: 10),
                  TextButton(
                    onPressed: () {
                      debugPrint('call btDeviceSetBondDestroyedCallback()');
                      /*TizenBluetoothManager.btDeviceSetBondDestroyedCallback((
                        int result,
                        String remoteAddress,
                      ) {
                        debugPrint(
                          'btDeviceSetBondDestroyedCallback: result : $result / $remoteAddress)',
                        );
                      });*/

                      debugPrint(
                        'call btDeviceDestroyBond(\'54:10:4F:D2:74:4F\', )',
                      );
                      TizenBluetoothManager.btDeviceDestroyBond(
                        '54:10:4F:D2:74:4F',
                      );
                    },
                    child: const Text(
                      'btDeviceDestroyBond(\'54:10:4F:D2:74:4F\')',
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
