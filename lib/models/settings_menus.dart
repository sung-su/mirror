import 'package:flutter/material.dart';
import 'package:tizen_fs/models/page_node.dart';
// import 'package:tizen_fs/settings/bluetooth_page.dart';
import 'package:tizen_fs/settings/date_time_page.dart';
import 'package:tizen_fs/settings/device_info_page.dart';
import 'package:tizen_fs/settings/end_page.dart';
import 'package:tizen_fs/settings/language_input_page.dart';
import 'package:tizen_fs/settings/language_page.dart';
import 'package:tizen_fs/settings/profile_active_page.dart';
import 'package:tizen_fs/settings/set_date_page.dart';
import 'package:tizen_fs/settings/set_time_page.dart';
import 'package:tizen_fs/settings/set_timezone_page.dart';
import 'package:tizen_fs/settings/wifi_page.dart';
import 'package:tizen_fs/settings/about_device_page.dart';

class SettingPages {
  late final PageNode _root;

  SettingPages() {
    _root = _buildPageTree();
  }

  PageNode getRoot() => _root;

  PageNode _buildPageTree() {
    final PageNode settings = PageNode(
      id: 'settings',
      title: 'Settings',
      children: [],
    );

    settings.children.add(
      PageNode(
        id: 'date_time',
        icon: Icons.today_outlined,
        title: 'Date & Time',
        builder: (context, node, isEnabled, onItemSelected) => DateTimePage(
          node: node,
          isEnabled: isEnabled,
          onSelectionChanged: onItemSelected,
        ),
        children: [
          PageNode(
            id: 'date_time_auto_update',
            title: 'Auto update',
            isEnd: true,
            builder: (context, node, isEnabled, onItemSelected) => EndPage(),
          ),
          PageNode(
            id: 'date_time_set_date',
            title: 'Set date',
            builder: (context, node, isEnabled, onItemSelected) =>
                SetDatePage(node: node, isEnabled: isEnabled),
          ),
          PageNode(
            id: 'date_time_set_time',
            title: 'Set time',
            builder: (context, node, isEnabled, onItemSelected) =>
                SetTimePage(node: node, isEnabled: isEnabled),
          ),
          PageNode(
            id: 'date_time_time_zone',
            title: 'Time zone',
            builder: (context, node, isEnabled, onItemSelected) =>
                SetTimezonePage(node: node, isEnabled: isEnabled),
          ),
          PageNode(
            id: 'date_time_24_hour_clock',
            title: '24 hour clock',
            isEnd: true,
            builder: (context, node, isEnabled, onItemSelected) => EndPage(),
          ),
        ],
      ),
    );

    settings.children.add(
      PageNode(
        id: 'language_input',
        icon: Icons.language_outlined,
        title: 'Language & Input',
        builder: (context, node, isEnabled, onItemSelected) => LanguageInputPage(
          node: node,
          isEnabled: isEnabled,
          onSelectionChanged: onItemSelected,
        ),
        children: [
          PageNode(
            id: 'language_input_display_language',
            title: 'Display language',
            builder: (context, node, isEnabled, onItemSelected) => LanguagePage(
              node: node,
              isEnabled: isEnabled,
              onSelectionChanged: onItemSelected,
            ),
          ),
          PageNode(
            id: 'language_input_keyboard',
            title: 'Keyboard',
            isEnd: true,
            builder: (context, node, isEnabled, onItemSelected) => EndPage(),
          ),
          PageNode(
            id: 'language_input_autofill_service',
            title: 'Autofill service',
            isEnd: true,
            builder: (context, node, isEnabled, onItemSelected) => EndPage(),
          ),
          PageNode(
            id: 'language_input_voice_control',
            title: 'Voice control',
            isEnd: true,
            builder: (context, node, isEnabled, onItemSelected) => EndPage(),
          ),
          PageNode(
            id: 'language_input_tts',
            title: 'Text-to-speech',
            isEnd: true,
            builder: (context, node, isEnabled, onItemSelected) => EndPage(),
          ),
          PageNode(
            id: 'language_input_stt',
            title: 'Speech-to-text',
            isEnd: true,
            builder: (context, node, isEnabled, onItemSelected) => EndPage(),
          ),
        ],
      ),
    );

    settings.children.add(
      PageNode(
        id: 'profile',
        icon: Icons.person_outlined,
        title: 'Profile',
        children: [
          PageNode(
            id: 'profile_tizen',
            title: 'Tizen',
            icon: const IconData(0x0054), // unicode T
            isEnd: true,
            builder: (context, node, isEnabled, onItemSelected) =>
                ProfileActivePage(node: node, isEnabled: isEnabled),
          ),
        ],
      ),
    );

    settings.children.add(
      PageNode(
        id: 'about_device',
        icon: Icons.info_outline,
        title: 'About Device',
        builder: (context, node, isEnabled, onItemSelected) => AboutDevicePage(
          node: node,
          isEnabled: isEnabled,
          onSelectionChanged: onItemSelected,
        ),
        children: [
          PageNode(
            id: 'about_device_device_info',
            title: 'Device info',
            isEnd: true,
            builder: (context, node, isEnabled, onItemSelected) =>
                DeviceInfoPage(node: node, isEnabled: isEnabled),
          ),
          PageNode(
            id: 'about_device_opensource_license',
            title: 'Open source license',
            isEnd: true,
            builder: (context, node, isEnabled, onItemSelected) => EndPage(),
          ),
        ],
      ),
    );

    settings.children.add(
      PageNode(
        id: 'wifi',
        icon: Icons.wifi_outlined,
        title: 'Wi-Fi',
        builder: (context, node, isEnabled, onItemSelected) => WifiPage(
          node: node,
          isEnabled: isEnabled,
          onSelectionChanged: onItemSelected,
        ),
        children: [
          PageNode(id: 'wi_fi_empty', title: 'Wi-Fi', isEnd: true),
        ],
      ),
    );

    return settings;
  }
}
