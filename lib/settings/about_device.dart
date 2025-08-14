import 'package:flutter/material.dart';
import 'package:tizen_fs/models/page_node.dart';
import 'package:device_info_plus_tizen/device_info_plus_tizen.dart';
import 'package:tizen_fs/settings/setting_page.dart';
import 'package:tizen_fs/settings/tizenfx.dart';

class AboutDevice extends StatefulWidget {
  final PageNode node;
  final bool isEnabled;

  const AboutDevice({super.key, required this.node, required this.isEnabled});

  @override
  State<AboutDevice> createState() => AboutDeviceState();
}

class AboutDeviceState extends State<AboutDevice> {
  double ram = 3969856;
  double width = 1280;
  double height = 720;
  void GetAboutDeviceInfo() async {
    final ret = await TizenFx.getAboutDeviceInfo();
    setState(() {
      ram = ret['Total'] ?? 0;
      width = ret['width'] ?? 0;
      height = ret['height'] ?? 0;
    });
  }

  Future<TizenDeviceInfo> GetDeviceInfo() async {
    final plugin = DeviceInfoPluginTizen();
    return await plugin.tizenInfo;
  }

  @override
  void initState() {
    super.initState();
    GetAboutDeviceInfo();
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
        Padding(
          padding: EdgeInsetsGeometry.only(left: 10, bottom: 5),
          child: Text(
            value,
            style: TextStyle(fontSize: 11, color: Color(0xFF979AA0)),
          ),
        ),
      ],
    );
  }

  @override
  Widget build(BuildContext context) {
    return FutureBuilder<TizenDeviceInfo>(
      future: GetDeviceInfo(),
      builder: (context, snapshot) {
        if (snapshot.connectionState == ConnectionState.done) {
          final info = snapshot.data;

          return Padding(
            padding:
                widget.isEnabled
                    ? EdgeInsets.fromLTRB(120, 70, 0, 0)
                    : EdgeInsets.fromLTRB(80, 70, 0, 0),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text("Device info", style: TextStyle(fontSize: 24)),
                Padding(padding: EdgeInsets.only(top: 10)),
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
          //return Center(child: CircularProgressIndicator());
          return Padding(
            padding:
                widget.isEnabled
                    ? EdgeInsets.fromLTRB(120, 70, 0, 0)
                    : EdgeInsets.fromLTRB(80, 70, 0, 0),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text("Device info", style: TextStyle(fontSize: 24)),
                Padding(padding: EdgeInsets.only(top: 10)),
                createKeyValue("Name", "Tizen"),
                createKeyValue("Model", "rpi4"),
                createKeyValue("Tizen version", "10.0",),
                createKeyValue("CPU", "BCM2711"),
                createKeyValue("RAM", (ram / (1024 * 1024)).toStringAsFixed(1) + 'GB',),
                createKeyValue("Resolution", width.toStringAsFixed(0) + 'x' + height.toStringAsFixed(0),),
              ],
            ),
          );
        }
      },
    );
  }
}
