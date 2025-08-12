
import 'package:flutter/material.dart';
import 'package:tizen_fs/models/page_node.dart';
import 'package:tizen_fs/poc/buttons_page.dart';
import 'package:tizen_fs/poc/empty_page.dart';

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
      // // builder: (context, node) => ButtonsPage(node: node),
      children: []
    );
    settings.children.add(
      PageNode(
        id: 'profile',
        icon: Icon(Icons.person_outlined, size: 25),
        title: 'Profile',
        // builder: (context, node) => ButtonsPage(node: node),
        children: [
          PageNode(
            id: 'profile_menu1',
            title: 'Tizen',
            // builder: (context, node) => ButtonsPage(node: node),
            children: [
              PageNode(
                id: 'profile_menu1_1',
                title: 'Active',
                // builder: (context, node) => EmptyPage(node: node),
              ),
              PageNode(
                id: 'profile_menu1_2',
                title: 'Remove',
                // builder: (context, node) => EmptyPage(node: node),
              ),
            ]
          ),
          PageNode(
            id: 'profile_menu2',
            title: 'Add a profile',
            // builder: (context, node) => EmptyPage(node: node),
          ),
        ],
      )
    );

    settings.children.add(
      PageNode(
        id: 'wifi',
        icon: Icon(Icons.wifi_outlined, size: 25),
        title: 'Wi-Fi',
        // builder: (context, node) => EmptyPage(node: node),
        children: [
          PageNode(
            id: 'wifi_menu1',
            title: 'Wi-Fi on/off',
            // builder: (context, node) => EmptyPage(node: node),
          ),
          PageNode(
            id: 'wifi_menu2',
            title: 'Advanced',
            // builder: (context, node) => EmptyPage(node: node),
          ),
        ],
      ),
    );

    settings.children.add(
      PageNode(
        id: 'bluetooth',
        icon: Icon(Icons.bluetooth_outlined, size: 25),
        title: 'Bluetooth',
        // builder: (context, node) => EmptyPage(node: node),
        children: [
          PageNode(
            id: 'bluetooth_menu1',
            title: 'Bluetooth',
            // builder: (context, node) => EmptyPage(node: node),
          ),
        ],
      ),
    );

    settings.children.add(
      PageNode(
        id: 'display',
        icon: Icon(Icons.light_mode_outlined, size: 25),
        title: 'Display',
        // builder: (context, node) => EmptyPage(node: node),
        children: [
          PageNode(
            id: 'display_menu1',
            title: 'Brightness',
            // builder: (context, node) => EmptyPage(node: node),
          ),
          PageNode(
            id: 'display_menu2',
            title: 'Font Size',
            // builder: (context, node) => EmptyPage(node: node),
          ),
        ],
      ),
    );

    settings.children.add(
      PageNode(
        id: 'wallpaper',
        icon: Icon(Icons.format_paint_outlined, size: 25),
        title: 'Wallpaper',
        // builder: (context, node) => EmptyPage(node: node),
      ),
    );

    settings.children.add(
      PageNode(
        id: 'sound',
        icon: Icon(Icons.volume_up_outlined, size: 25),
        title: 'Sound',
        // builder: (context, node) => EmptyPage(node: node),
      ),
    );

    settings.children.add(
      PageNode(
        id: 'date_time',
        icon: Icon(Icons.today_outlined, size: 25),
        title: 'Date & Time',
        // builder: (context, node) => EmptyPage(node: node),
        children: [
          PageNode(
            id: 'date_time_menu1',
            title: 'Auto update',
            // builder: (context, node) => EmptyPage(node: node),
          ),
          PageNode(
            id: 'date_time_menu2',
            title: 'Set time',
            // builder: (context, node) => EmptyPage(node: node),
          ),
        ],
      ),
    );

    settings.children.add(
      PageNode(
        id: 'language_input',
        icon: Icon(Icons.language_outlined, size: 25),
        title: 'Language & Input',
        // builder: (context, node) => EmptyPage(node: node),
      ),
    );

    settings.children.add(
      PageNode(
        id: 'accessibility',
        icon: Icon(Icons.accessibility_outlined, size: 25),
        title: 'Accessibility',
        // builder: (context, node) => EmptyPage(node: node),
      ),
    );


    settings.children.add(
      PageNode(
        id: 'about_device',
        icon: Icon(Icons.info_outline, size: 25),
        title: 'About device',
        // builder: (context, node) => EmptyPage(node: node),
        children: [
          PageNode(
            id: 'about_device_menu1',
            title: 'Device info',
            children: [
              PageNode(
                id: 'about_device_menu1_1',
                title: 'Name',
                description: "Tizen",
                // builder: (context, node) => EmptyPage(node: node),
              ),
              PageNode(
                id: 'about_device_menu1_2',
                title: 'Model',
                description: "RPI4",
                // builder: (context, node) => EmptyPage(node: node),
              ),
              PageNode(
                id: 'about_device_menu1_3',
                title: 'Tizen version',
                description: "10.0",
                // builder: (context, node) => EmptyPage(node: node),
              ),
              PageNode(
                id: 'about_device_menu1_4',
                title: 'CPU',
                description: "BCM2711",
                // builder: (context, node) => EmptyPage(node: node),
              ),
              PageNode(
                id: 'about_device_menu1_5',
                title: 'RAM',
                description: "3.8 GB",
                // builder: (context, node) => EmptyPage(node: node),
              ),
              PageNode(
                id: 'about_device_menu1_6',
                title: 'Resolution',
                description: "1920 x 1280",
                // builder: (context, node) => EmptyPage(node: node),
              ),
            ],
          ),
          PageNode(
            id: 'about_device_menu2',
            title: 'Open source license',
            // builder: (context, node) => EmptyPage(node: node),
          ),
          PageNode(
            id: 'about_device_menu3',
            title: 'Manage certificates',
            // builder: (context, node) => EmptyPage(node: node),
          ),
        ],
      ),
    );

    settings.children.add(
      PageNode(
        id: 'apps',
        icon: Icon(Icons.apps_outlined, size: 25),
        title: 'Apps',
        // builder: (context, node) => EmptyPage(node: node),
      ),
    );

    settings.children.add(
      PageNode(
        id: 'Storage',
        icon: Icon(Icons.archive_outlined, size: 25),
        title: 'Storage',
        // builder: (context, node) => EmptyPage(node: node),
      ),
    );

    settings.children.add(
      PageNode(
        id: '',
        // icon: Icon(Icons.settings_outlined, size: 25),
        title: '',
        // builder: (context, node) => EmptyPage(node: node),
      ),
    );


    return settings;
  }
}
