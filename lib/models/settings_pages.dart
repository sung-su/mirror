
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
        icon: Icon(Icons.settings_outlined, size: 25),
        title: 'Profile',
        description: 'test',
        // builder: (context, node) => ButtonsPage(node: node),
        children: [
          PageNode(
            id: 'Tizen',
            title: 'Tizen',
            // builder: (context, node) => ButtonsPage(node: node),
            children: [
              PageNode(
                id: 'menu1',
                title: 'menu1',
                // builder: (context, node) => EmptyPage(node: node),
              ),
              PageNode(
                id: 'menu2',
                title: 'menu2',
                // builder: (context, node) => EmptyPage(node: node),
              ),
              PageNode(
                id: 'menu3',
                title: 'menu3',
                // builder: (context, node) => EmptyPage(node: node),
              )
            ]
          ),
          PageNode(
            id: 'user',
            title: 'user',
            // builder: (context, node) => EmptyPage(node: node),
          ),
        ],
      )
    );
    
    settings.children.add(
      PageNode(
        id: 'wifi',
        icon: Icon(Icons.settings_outlined, size: 25),
        title: 'Wi-Fi',
        // builder: (context, node) => EmptyPage(node: node),
        children: [
          PageNode(
            id: 'wifi_menu1',
            title: 'Wi-Fi on/off',
            // builder: (context, node) => EmptyPage(node: node),
          ),
          PageNode(
            id: 'wifi_advanced',
            title: 'Advanced',
            // builder: (context, node) => EmptyPage(node: node),
          ),
        ],
      ),
    );

    settings.children.add(
      PageNode(
        id: 'bluetooth',
        icon: Icon(Icons.settings_outlined, size: 25),
        title: 'Bluetooth',
        // builder: (context, node) => EmptyPage(node: node),
        children: [
          PageNode(
            id: 'bluetooth_menu1',
            title: 'Bluetooth on/off',
            // builder: (context, node) => EmptyPage(node: node),
          ),
        ],
      ),
    );

    settings.children.add(
      PageNode(
        id: 'about_device2',
        icon: Icon(Icons.settings_outlined, size: 25),
        title: 'About Device2',
        // builder: (context, node) => EmptyPage(node: node),
      ),
    );

    settings.children.add(
      PageNode(
        id: 'display',
        icon: Icon(Icons.settings_outlined, size: 25),
        title: 'Display',
        // builder: (context, node) => EmptyPage(node: node),
        children: [
          PageNode(
            id: 'display_brightness',
            title: 'Brightness',
            // builder: (context, node) => EmptyPage(node: node),
          ),
          PageNode(
            id: 'display_font',
            title: 'Font Size',
            // builder: (context, node) => EmptyPage(node: node),
          ),
        ],
      ),
    );

    settings.children.add(
      PageNode(
        id: 'date_time',
        icon: Icon(Icons.settings_outlined, size: 25),
        title: 'Date & Time',
        // builder: (context, node) => EmptyPage(node: node),
        children: [
          PageNode(
            id: 'date_time_autoupdate',
            title: 'Auto update',
            // builder: (context, node) => EmptyPage(node: node),
          ),
          PageNode(
            id: 'date_time_set_data',
            title: 'Set time',
            // builder: (context, node) => EmptyPage(node: node),
          ),
        ],
      ),
    );

    settings.children.add(
      PageNode(
        id: 'language_input',
        icon: Icon(Icons.settings_outlined, size: 25),
        title: 'Language & Input',
        // builder: (context, node) => EmptyPage(node: node),
      ),
    );

    settings.children.add(
      PageNode(
        id: 'about_device6',
        icon: Icon(Icons.settings_outlined, size: 25),
        title: 'About Device',
        // builder: (context, node) => EmptyPage(node: node),
      ),
    );

    settings.children.add(
      PageNode(
        id: 'apps',
        icon: Icon(Icons.settings_outlined, size: 25),
        title: 'Apps',
        // builder: (context, node) => EmptyPage(node: node),
      ),
    );

    return settings;
  }
}
