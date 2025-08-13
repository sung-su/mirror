
import 'package:flutter/material.dart';
import 'package:tizen_fs/models/page_node.dart';
import 'package:tizen_fs/poc/buttons_page.dart';
import 'package:tizen_fs/poc/empty_page.dart';
import 'package:tizen_fs/settings/about_device.dart';

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
      // // builder: (context, node, isEnabled) => ButtonsPage(node: node),
      children: []
    );
    settings.children.add(
      PageNode(
        id: 'profile',
        icon: Icons.person_outlined,
        title: 'Profile',
        // builder: (context, node, isEnabled) => ButtonsPage(node: node),
        children: [
          PageNode(
            id: 'profile_menu1',
            title: 'Tizen',
            icon: IconData(0x0054, fontFamily: 'MaterialIcons'), //unicode T: 0054
            // builder: (context, node, isEnabled) => ButtonsPage(node: node),
            children: [
              PageNode(
                id: 'profile_menu1_1',
                title: 'Active',
                // builder: (context, node, isEnabled) => EmptyPage(node: node, isEnabled: isEnabled),
              ),
              PageNode(
                id: 'profile_menu1_2',
                title: 'Remove',
                // builder: (context, node, isEnabled) => EmptyPage(node: node, isEnabled: isEnabled),
              ),
            ]
          ),
          PageNode(
            id: 'profile_menu2',
            title: 'Add a profile',
            // builder: (context, node, isEnabled) => EmptyPage(node: node, isEnabled: isEnabled),
          ),
        ],
      )
    );

    settings.children.add(
      PageNode(
        id: 'about_device',
        icon: Icons.info_outline,
        title: 'About device',
        children: [
          PageNode(
            id: 'about_device_menu1',
            title: 'Device info',
            builder: (context, node, isEnabled) => AboutDevice(node: node, isEnabled: true),
          ),
          PageNode(
            id: 'about_device_menu2',
            title: 'Open source license',
            builder: (context, node, isEnabled) => EmptyPage(node: node, isEnabled: isEnabled),
          ),
          PageNode(
            id: 'about_device_menu3',
            title: 'Manage certificates',
            // builder: (context, node, isEnabled) => EmptyPage(node: node, isEnabled: isEnabled),
          ),
        ],
      ),
    );

    settings.children.add(
      PageNode(
        id: 'wifi',
        icon: Icons.wifi_outlined,
        title: 'Wi-Fi',
        // builder: (context, node, isEnabled) => EmptyPage(node: node, isEnabled: isEnabled),
        children: [
          PageNode(
            id: 'wifi_menu1',
            title: 'Wi-Fi on/off',
            // builder: (context, node, isEnabled) => EmptyPage(node: node, isEnabled: isEnabled),
          ),
          PageNode(
            id: 'wifi_menu2',
            title: 'Advanced',
            // builder: (context, node, isEnabled) => EmptyPage(node: node, isEnabled: isEnabled),
          ),
        ],
      ),
    );

    settings.children.add(
      PageNode(
        id: 'bluetooth',
        icon: Icons.bluetooth_outlined,
        title: 'Bluetooth',
        // builder: (context, node, isEnabled) => EmptyPage(node: node, isEnabled: isEnabled),
        children: [
          PageNode(
            id: 'bluetooth_menu1',
            title: 'Bluetooth',
            // builder: (context, node, isEnabled) => EmptyPage(node: node, isEnabled: isEnabled),
          ),
        ],
      ),
    );

    settings.children.add(
      PageNode(
        id: 'display',
        icon: Icons.light_mode_outlined,
        title: 'Display',
        // builder: (context, node, isEnabled) => EmptyPage(node: node, isEnabled: isEnabled),
        children: [
          PageNode(
            id: 'display_menu1',
            title: 'Brightness',
            // builder: (context, node, isEnabled) => EmptyPage(node: node, isEnabled: isEnabled),
          ),
          PageNode(
            id: 'display_menu2',
            title: 'Font Size',
            // builder: (context, node, isEnabled) => EmptyPage(node: node, isEnabled: isEnabled),
          ),
        ],
      ),
    );

    settings.children.add(
      PageNode(
        id: 'wallpaper',
        icon: Icons.format_paint_outlined,
        title: 'Wallpaper',
        // builder: (context, node, isEnabled) => EmptyPage(node: node, isEnabled: isEnabled),
      ),
    );

    settings.children.add(
      PageNode(
        id: 'sound',
        icon: Icons.volume_up_outlined,
        title: 'Sound',
        // builder: (context, node, isEnabled) => EmptyPage(node: node, isEnabled: isEnabled),
      ),
    );

    settings.children.add(
      PageNode(
        id: 'date_time',
        icon: Icons.today_outlined,
        title: 'Date & Time',
        // builder: (context, node, isEnabled) => EmptyPage(node: node, isEnabled: isEnabled),
        children: [
          PageNode(
            id: 'date_time_menu1',
            title: 'Auto update',
            // builder: (context, node, isEnabled) => EmptyPage(node: node, isEnabled: isEnabled),
          ),
          PageNode(
            id: 'date_time_menu2',
            title: 'Set time',
            // builder: (context, node, isEnabled) => EmptyPage(node: node, isEnabled: isEnabled),
          ),
        ],
      ),
    );

    settings.children.add(
      PageNode(
        id: 'language_input',
        icon: Icons.language_outlined,
        title: 'Language & Input',
        // builder: (context, node, isEnabled) => EmptyPage(node: node, isEnabled: isEnabled),
      ),
    );

    settings.children.add(
      PageNode(
        id: 'accessibility',
        icon: Icons.accessibility_outlined,
        title: 'Accessibility',
        // builder: (context, node, isEnabled) => EmptyPage(node: node, isEnabled: isEnabled),
      ),
    );

    settings.children.add(
      PageNode(
        id: 'apps',
        icon: Icons.apps_outlined,
        title: 'Apps',
        // builder: (context, node, isEnabled) => EmptyPage(node: node, isEnabled: isEnabled),
      ),
    );

    settings.children.add(
      PageNode(
        id: 'Storage',
        icon: Icons.archive_outlined,
        title: 'Storage',
        // builder: (context, node, isEnabled) => EmptyPage(node: node, isEnabled: isEnabled),
      ),
    );

    return settings;
  }
}
