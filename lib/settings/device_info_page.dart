import 'package:flutter/material.dart';
import 'package:tizen_fs/models/page_node.dart';
import 'package:device_info_plus_tizen/device_info_plus_tizen.dart';
import 'package:tizen_fs/settings/tizenfx.dart';

class DeviceInfoPage extends StatefulWidget {
  final PageNode node;
  final bool isEnabled;

  const DeviceInfoPage({super.key, required this.node, required this.isEnabled});

  @override
  State<DeviceInfoPage> createState() => DeviceInfoPageState();
}

class DeviceInfoPageState extends State<DeviceInfoPage> {
  double ram = -1;
  double width = -1;
  double height = -1;
  late Future<TizenDeviceInfo> _deviceInfo;

  Future<TizenDeviceInfo> _getDeviceInfo() async {
    final plugin = DeviceInfoPluginTizen();
    final info = await TizenFx.getDeviceInfo();
    ram = info['Total'] ?? 3969856;
    width = info['Width']?? 1280;
    height = info['Height']?? 720;
    return await plugin.tizenInfo;
  }

  @override
  void initState() {
    super.initState();
    _deviceInfo = _getDeviceInfo();
  }

  @override
  void didUpdateWidget(var oldWidget) {
    super.didUpdateWidget(oldWidget);
  }

  Widget createKeyValue(String key, String value) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(key, style: TextStyle(fontSize: 13)),
        Text(
          value,
          style: TextStyle(fontSize: 11, color: Color(0xFF979AA0)),
        ),
      ],
    );
  }

  @override
  Widget build(BuildContext context) {
    return FutureBuilder<TizenDeviceInfo>(
      future: _deviceInfo,
      builder: (context, snapshot) {
        if (snapshot.connectionState == ConnectionState.done) {
          final info = snapshot.data;
          return Padding(
            padding:
                widget.isEnabled
                    ? EdgeInsets.fromLTRB(120, 60, 0, 0)
                    : EdgeInsets.fromLTRB(80, 60, 0, 0),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              spacing: 20,
              children: [
                Text("Device info", style: TextStyle(fontSize: 35)),
                SizedBox(height: 5),
                createKeyValue("Name", info?.platformName ?? "Unknown"),
                createKeyValue("Model", info?.modelName ?? "Unknown"),
                createKeyValue("Tizen version", info?.platformVersion ?? "Unknown",),
                createKeyValue("CPU", info?.platformProcessor ?? "Unknown"),
                createKeyValue("RAM", (ram / (1024 * 1024)).toStringAsFixed(1) + 'GB',),
                createKeyValue("Resolution", width.toStringAsFixed(0) + 'x' + height.toStringAsFixed(0),),
              ],
            ),
          );
        } else {
          return Padding(
            padding:
                widget.isEnabled
                    ? EdgeInsets.fromLTRB(120, 60, 0, 0)
                    : EdgeInsets.fromLTRB(80, 60, 0, 0),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              spacing: 20,
              children: [
                Text("Device info", style: TextStyle(fontSize: 35)),
                SizedBox(height: 5),
                createKeyValue("Name", "Loading"),
                createKeyValue("Model", "Loading"),
                createKeyValue("Tizen version", "Loading",),
                createKeyValue("CPU", "Loading"),
                createKeyValue("RAM", 'Loading',),
                createKeyValue("Resolution", 'Loading'),
              ],
            ),
          );
        }
      },
    );
  }
}
