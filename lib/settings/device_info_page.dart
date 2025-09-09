import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:tizen_fs/models/page_node.dart';
import 'package:tizen_fs/providers/device_info_provider.dart';

class DeviceInfoPage extends StatefulWidget {
  final PageNode node;
  final bool isEnabled;

  const DeviceInfoPage({
    super.key,
    required this.node,
    required this.isEnabled,
  });

  @override
  State<DeviceInfoPage> createState() => DeviceInfoPageState();
}

class DeviceInfoPageState extends State<DeviceInfoPage> {
  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addPostFrameCallback((_) {
      if (mounted) {
        Provider.of<DeviceInfoProvider>(
          context,
          listen: false,
        ).loadDeviceInfo();
      }
    });
  }

  Widget createKeyValue(String key, String value) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(key, style: TextStyle(fontSize: 13)),
        Text(value, style: TextStyle(fontSize: 11, color: Color(0xFF979AA0))),
      ],
    );
  }

  @override
  Widget build(BuildContext context) {
    return Consumer<DeviceInfoProvider>(
      builder: (context, deviceInfoProvider, child) {
        final padding =
            widget.isEnabled
                ? EdgeInsets.fromLTRB(120, 60, 0, 0)
                : EdgeInsets.fromLTRB(80, 60, 0, 0);

        if (deviceInfoProvider.isLoading) {
          return Padding(
            padding: padding,
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              spacing: 20,
              children: [
                Text("Device info", style: TextStyle(fontSize: 35)),
                SizedBox(height: 5),
                createKeyValue("Name", "Loading"),
                createKeyValue("Model", "Loading"),
                createKeyValue("Tizen version", "Loading"),
                createKeyValue("CPU", "Loading"),
                createKeyValue("RAM", 'Loading'),
                createKeyValue("Resolution", 'Loading'),
              ],
            ),
          );
        } else if (deviceInfoProvider.error != null) {
          return Padding(
            padding: padding,
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              spacing: 20,
              children: [
                Text("Device info", style: TextStyle(fontSize: 35)),
                SizedBox(height: 5),
                Text(
                  "Error loading device information.",
                  style: TextStyle(color: Colors.red),
                ),
                Text(deviceInfoProvider.error.toString()),
              ],
            ),
          );
        } else {
          final info = deviceInfoProvider.tizenDeviceInfo;
          final ram = deviceInfoProvider.ram;
          final width = deviceInfoProvider.tizenDeviceInfo?.screenWidth ?? 1;
          final height = deviceInfoProvider.tizenDeviceInfo?.screenHeight ?? 1;
          return Padding(
            padding: padding,
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              spacing: 20,
              children: [
                Text("Device info", style: TextStyle(fontSize: 35)),
                SizedBox(height: 5),
                createKeyValue("Name", info?.platformName ?? "Unknown"),
                createKeyValue("Model", info?.modelName ?? "Unknown"),
                createKeyValue(
                  "Tizen version",
                  info?.platformVersion ?? "Unknown",
                ),
                createKeyValue("CPU", info?.platformProcessor ?? "Unknown"),
                createKeyValue(
                  "RAM",
                  ram > 0
                      ? (ram / (1024 * 1024)).toStringAsFixed(1) + 'GB'
                      : 'Unknown',
                ),
                createKeyValue(
                  "Resolution",
                  width > 0 && height > 0
                      ? '${width.toStringAsFixed(0)}x${height.toStringAsFixed(0)}'
                      : 'Unknown',
                ),
              ],
            ),
          );
        }
      },
    );
  }
}
