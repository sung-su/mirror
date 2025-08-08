import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:tizen_fs/settings/master_detail_page.dart';
import 'package:tizen_fs/settings/master_page.dart';
import 'package:tizen_fs/settings/menu.dart';

class Settings extends StatefulWidget {
  const Settings({super.key});

  @override
  State<Settings> createState() => _Settings();
}

class _Settings extends State<Settings> {
  Menu settings = Menu(
    title: "Settings",
    items: [
      MenuItem(title: "Profile", icon: Icons.person_outlined),
      MenuItem(title: "Wi-Fi", icon: Icons.wifi_outlined),
      MenuItem(title: "Bluetooth", icon: Icons.bluetooth_outlined),
      MenuItem(title: "Display", icon: Icons.light_mode_outlined),
      MenuItem(title: "Wallpaper", icon: Icons.format_paint_outlined),
      MenuItem(title: "Sound", icon: Icons.volume_up_outlined),
      MenuItem(title: "Date & Time", icon: Icons.today_outlined),
      MenuItem(title: "Language & Input", icon: Icons.language_outlined),
      MenuItem(title: "Accessibility", icon: Icons.accessibility_outlined),
      MenuItem(title: "About device", icon: Icons.info_outline),
      MenuItem(title: "Apps", icon: Icons.apps_outlined),
      MenuItem(title: "Storage", icon: Icons.archive_outlined),
    ],
  );

  Menu profiles = Menu(
    title: "Profile",
    items: [
      MenuItem(title: "Tizen", icon: Icons.person_outlined),
      MenuItem(title: "Add an account"),
    ],
  );

  Menu wifi = Menu(
    title: "Wi-Fi",
    items: [
      MenuItem(
        title: "Wi-Fi",
        Toggle: false,
      ),
      MenuItem(title: "Advanced"),
    ],
  );

  Menu empty = Menu(title: "empty", items: []);

  FocusNode masterNode = FocusNode();
  GlobalKey<MasterDetailPageState> masterKey =
      GlobalKey<MasterDetailPageState>();

  KeyEventResult _onKeyEvent(FocusNode node, KeyEvent event) {
    if (event is KeyDownEvent) {
      if (event.logicalKey == LogicalKeyboardKey.arrowRight) {
        print("@main _onKeyEvent.arrowRight pageIndex=[$pageIndex]");
        masterKey.currentState?.moveNext();
        return KeyEventResult.handled;
      } else if (event.logicalKey == LogicalKeyboardKey.arrowLeft) {
        print("@main _onKeyEvent.arrowLeft pageIndex=[$pageIndex]");
        masterKey.currentState?.movePrev();
        return KeyEventResult.handled;
      } else if (event.logicalKey == LogicalKeyboardKey.enter ||
          event.logicalKey == LogicalKeyboardKey.select) {
        return KeyEventResult.handled;
      }
      return KeyEventResult.ignored;
    }
    return KeyEventResult.ignored;
  }

  int focusedIndex = 0;
  int pageIndex = 0;

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: MasterDetailPage(
        key: masterKey,
        onPageChanged: (changedPageIndex) {
          pageIndex = changedPageIndex;
        },
      ),
    );
  }
}
