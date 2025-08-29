
import 'package:flutter/material.dart';
import 'package:tizen_fs/models/page_node.dart';
import 'package:tizen_fs/poc/empty_page.dart';
import 'package:tizen_fs/settings/device_info_page.dart';

class SettingPages {
  late final PageNode _root;

  SettingPages() {
    _root = _buildPageTree();
  }

  PageNode getRoot() => _root;
  PageNode _buildPageTree() {
    PageNode settings = PageNode (
      id: 'settings',
      title: 'Settings',
      children: []
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
            // icon: IconData(0x0054, fontFamily: 'MaterialIcons'), //unicode T: 0054
            children: [
              PageNode(
                id: 'profile_tizen_active',
                title: 'Active',
              ),
              PageNode(
                id: 'profile_tizen_remove',
                title: 'Remove',
              ),
            ]
          ),
          PageNode(
            id: 'profile_add',
            title: 'Add a profile',
          ),
        ],
      )
    );

    settings.children.add(
      PageNode(
        id: 'about_device',
        icon: Icons.info_outline,
        title: 'About Device',
        children: [
          PageNode(
            id: 'about_device_device_info',
            title: 'Device info',
            builder: (context, node, isEnabled) => DeviceInfoPage(node: node, isEnabled: isEnabled),
          ),
          PageNode(
            id: 'about_device_opensource_license',
            title: 'Open source license',
            // builder: (context, node, isEnabled) => EmptyPage(node: node, isEnabled: isEnabled),
          ),
          PageNode(
            id: 'about_device_certificates',
            title: 'Manage certificates',
            children: [
              PageNode(
                id: 'about_device_certificates_AAA',
                title: 'AAA service',
              ),
              PageNode(
                id: 'about_device_certificates_ANF',
                title: 'ANF server',
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

        children: [
          PageNode(
            id: 'wifi_menu1',
            title: 'Wi-Fi',
          ),
          PageNode(
            id: 'wifi_menu2',
            title: 'Advanced',
          ),
        ],
      ),
    );

    settings.children.add(
      PageNode(
        id: 'bluetooth',
        icon: Icons.bluetooth_outlined,
        title: 'Bluetooth',

        children: [
          PageNode(
            id: 'bluetooth_menu1',
            title: 'Bluetooth',
          ),
        ],
      ),
    );

    settings.children.add(
      PageNode(
        id: 'display',
        icon: Icons.light_mode_outlined,
        title: 'Display',
        children: [
          PageNode(
            id: 'display_brigetness',
            title: 'Brightness',
          ),
          PageNode(
            id: 'display_font_size',
            title: 'Font size',
          ),
          PageNode(
            id: 'display_font_type',
            title: 'Font type',
          ),
          PageNode(
            id: 'display_screen_timeout',
            title: 'Screen timeout',
          ),
        ],
      ),
    );

    settings.children.add(
      PageNode(
        id: 'date_time',
        icon: Icons.today_outlined,
        title: 'Date & Time',
        children: [
          PageNode(
            id: 'date_time_auto_update',
            title: 'Auto update',
          ),
          PageNode(
            id: 'date_time_set_date',
            title: 'Set date',
          ),
          PageNode(
            id: 'date_time_set_time',
            title: 'Set time',
          ),
          PageNode(
            id: 'date_time_time_zone',
            title: 'Time zone',
          ),
          PageNode(
            id: 'date_time_24_hour_clock',
            title: '24 hour clock',
          ),
        ],
      ),
    );

    settings.children.add(
      PageNode(
        id: 'language_input',
        icon: Icons.language_outlined,
        title: 'Language & Input',
        children: [
          PageNode(
            id: 'language_input_display_language',
            title: 'Language',
          ),
          PageNode(
            id: 'language_input_keyboard',
            title: 'Keyboard',
          ),
          PageNode(
            id: 'language_input_autofill_service',
            title: 'Autofill',
          ),
          PageNode(
            id: 'language_input_voice_control',
            title: 'Voice control',
          ),
          // PageNode(
          //   id: 'language_input_tts',
          //   title: 'TTS (TBD)',
          // ),
          // PageNode(
          //   id: 'language_input_stt',
          //   title: 'STT (TBD)',
          // ),
        ],
      ),
    );

    settings.children.add(
      PageNode(
        id: 'apps',
        icon: Icons.apps_outlined,
        title: 'Apps',
        children: [
          PageNode(
            id: 'apps_application_manager',
            title: 'Application Manager',
          ),
          PageNode(
            id: 'apps_default_application',
            title: 'Default application',
          ),
        ],
      ),
    );

    settings.children.add(
      PageNode(
        id: 'Storage',
        icon: Icons.archive_outlined,
        title: 'Storage',
        children: [
          PageNode(
            id: 'storage_internal',
            title: 'Internal',
          ),
          PageNode(
            id: 'storage_external',
            title: 'External',
          ),
          PageNode(
            id: 'storage_default_settings',
            title: 'Default settings',
          ),
        ],
      ),
    );

    return settings;
  }
}
