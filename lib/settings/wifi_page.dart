import 'package:flutter/material.dart';
import 'package:tizen_fs/models/page_node.dart';
import 'package:tizen_fs/settings/wifi.dart';

class WifiPage extends StatefulWidget {
  final PageNode node;
  final bool isEnabled;

  const WifiPage({super.key, required this.node, required this.isEnabled});

  @override
  State<WifiPage> createState() => WifiPageState();
}

class WifiPageState extends State<WifiPage> {
  static late final WifiManager _wifiManager;

  @override
  void initState() {
    super.initState();
    _wifiManager = WifiManager();
    // _wifiManager.initializeWifi();
  }

  @override
  void didUpdateWidget(var oldWidget) {
    super.didUpdateWidget(oldWidget);
  }

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: EdgeInsets.fromLTRB(80, 60, 80, 60),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        spacing: 20,
        children: [
          Text("WiFi", style: TextStyle(fontSize: 35)),
          ElevatedButton(
            onPressed: () {
              _wifiManager.initializeWifi();
            },
            child: Text('init'),
          ),
          ElevatedButton(
            onPressed: () {
              _wifiManager.isActivated();
            },
            child: Text('isActivated'),
          ),
          ElevatedButton(
            onPressed: () {
              _wifiManager.activate();
            },
            child: Text('activate'),
          ),
          ElevatedButton(
            onPressed: () {
              _wifiManager.scan();
            },
            child: Text('san'),
          ),
        ],
      ),
    );
  }
}
