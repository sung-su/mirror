import 'package:flutter/material.dart';
import 'package:tizen_fs/models/page_node.dart';
import 'package:tizen_fs/settings/bluetooth_page.dart';
import 'package:tizen_fs/settings/device_info_page.dart';
import 'package:tizen_fs/settings/end_page.dart';
import 'package:tizen_fs/settings/profile_active_page.dart';
import 'package:tizen_fs/settings/wifi_page.dart';
import 'package:tizen_fs/settings/about_device_page.dart';

class SettingPages {
  late final PageNode _root;

  SettingPages() {
    _root = _buildPageTree();
  }

  PageNode getRoot() => _root;
  PageNode _buildPageTree() {
    PageNode settings = PageNode(
      id: 'settings',
      title: 'Settings',
      children: [],
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
            icon: const IconData(0x0054), //unicode T: 0054
            isEnd: true,
            builder:
                (context, node, isEnabled, onItemSelected) =>
                    ProfileActivePage(node: node, isEnabled: isEnabled),
            // children: [
            //   // PageNode(
            //   //   id: 'profile_tizen_active',
            //   //   title: 'Active',
            //   //   isEnd: true,
            //   //   builder:
            //   //       (context, node, isEnabled, onItemSelected) =>
            //   //           ProfileActivePage(node: node, isEnabled: isEnabled),
            //   // ),
            //   // PageNode(
            //   //   id: 'profile_tizen_remove',
            //   //   title: 'Remove',
            //   //   isEnd: true,
            //   // ),
            // ],
          ),
          // PageNode(id: 'profile_add', title: 'Add a profile'),
        ],
      ),
    );

    settings.children.add(
      PageNode(
        id: 'about_device',
        icon: Icons.info_outline,
        title: 'About Device',
        builder:
            (context, node, isEnabled, onItemSelected) => AboutDevicePage(
              node: node,
              isEnabled: isEnabled,
              onSelectionChanged: onItemSelected,
            ),
        children: [
          PageNode(
            id: 'about_device_device_info',
            title: 'Device info',
            builder:
                (context, node, isEnabled, onItemSelected) =>
                    DeviceInfoPage(node: node, isEnabled: isEnabled),
          ),
          PageNode(
            id: 'about_device_opensource_license',
            title: 'Open source license',
            builder: (context, node, isEnabled, onItemSelected) => EndPage(),
            isEnd: true,
          ),
          PageNode(
            id: 'about_device_certificates',
            title: 'Manage certificates',
            children: [
              PageNode(
                id: 'about_device_certificates_AAA',
                title: 'AAA certificates',
                builder:
                    (context, node, isEnabled, onItemSelected) => EndPage(),
              ),
              PageNode(
                id: 'about_device_certificates_ANF',
                title: 'ANF certificates',
                builder:
                    (context, node, isEnabled, onItemSelected) => EndPage(),
              ),
            ],
          ),
        ],
      ),
    );

    settings.children.add(
      PageNode(
        id: 'wifi',
        icon: Icons.wifi_outlined,
        title: 'Wi-Fi',
        builder:
            (context, node, isEnabled, onItemSelected) => WifiPage(
              node: node,
              isEnabled: isEnabled,
              onSelectionChanged: onItemSelected,
            ),
        children: [
          // to make empty area for custom page
          PageNode(id: 'wi_fi_empty', title: 'wifi ', isEnd: true),
        ],
      ),
    );

    settings.children.add(
      PageNode(
        id: 'bluetooth',
        icon: Icons.bluetooth_outlined,
        title: 'Bluetooth',
        builder:
            (context, node, isEnabled, onItemSelected) => BluetoothPage(
              node: node,
              isEnabled: isEnabled,
              onSelectionChanged: onItemSelected,
            ),
        // isEnd: true,
        children: [
          // to make empty area for custom page
          PageNode(id: 'bt_empty', title: 'Bluetooth 1 ', isEnd: true),
        ],
      ),
    );

    // settings.children.add(
    //   PageNode(
    //     id: 'display',
    //     icon: Icons.light_mode_outlined,
    //     title: 'Display',
    //     children: [
    //       PageNode(id: 'display_brigetness', title: 'Brightness'),
    //       PageNode(id: 'display_font_size', title: 'Font size'),
    //       PageNode(id: 'display_font_type', title: 'Font type'),
    //       PageNode(id: 'display_screen_timeout', title: 'Screen timeout'),
    //     ],
    //   ),
    // );

    // settings.children.add(
    //   PageNode(
    //     id: 'date_time',
    //     icon: Icons.today_outlined,
    //     title: 'Date & Time',
    //     children: [
    //       PageNode(id: 'date_time_auto_update', title: 'Auto update'),
    //       PageNode(id: 'date_time_set_date', title: 'Set date'),
    //       PageNode(id: 'date_time_set_time', title: 'Set time'),
    //       PageNode(id: 'date_time_time_zone', title: 'Time zone'),
    //       PageNode(id: 'date_time_24_hour_clock', title: '24 hour clock'),
    //     ],
    //   ),
    // );

    // settings.children.add(
    //   PageNode(
    //     id: 'language_input',
    //     icon: Icons.language_outlined,
    //     title: 'Language & Input',
    //     children: [
    //       PageNode(id: 'language_input_display_language', title: 'Language'),
    //       PageNode(id: 'language_input_keyboard', title: 'Keyboard'),
    //       PageNode(id: 'language_input_autofill_service', title: 'Autofill'),
    //       PageNode(id: 'language_input_voice_control', title: 'Voice control'),
    //       // PageNode(
    //       //   id: 'language_input_tts',
    //       //   title: 'TTS (TBD)',
    //       // ),
    //       // PageNode(
    //       //   id: 'language_input_stt',
    //       //   title: 'STT (TBD)',
    //       // ),
    //     ],
    //   ),
    // );

    // settings.children.add(
    //   PageNode(
    //     id: 'apps',
    //     icon: Icons.apps_outlined,
    //     title: 'Apps',
    //     children: [
    //       PageNode(
    //         id: 'apps_application_manager',
    //         title: 'Application Manager',
    //       ),
    //       PageNode(
    //         id: 'apps_default_application',
    //         title: 'Default application',
    //       ),
    //     ],
    //   ),
    // );

    // settings.children.add(
    //   PageNode(
    //     id: 'Storage',
    //     icon: Icons.archive_outlined,
    //     title: 'Storage',
    //     children: [
    //       PageNode(id: 'storage_internal', title: 'Internal'),
    //       PageNode(id: 'storage_external', title: 'External'),
    //       PageNode(id: 'storage_default_settings', title: 'Default settings'),
    //     ],
    //   ),
    // );

    return settings;
  }
}
