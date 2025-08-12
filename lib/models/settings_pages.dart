
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
      builder: (context, node) => ButtonsPage(node: node),
      children: []
    );

    settings.children.add(
      PageNode(
        id: 'profiles',
        icon: Icon(Icons.settings_outlined, size: 25),
        title: 'Profiles',
        description: 'test',
        builder: (context, node) => ButtonsPage(node: node),
        children: [
          PageNode(
            id: 'profiles_menu1',
            title: 'Profile menu1',
            builder: (context, node) => ButtonsPage(node: node),
            children: [
              PageNode(
                id: 'test',
                title: 'test',
                builder: (context, node) => EmptyPage(node: node),
              ),
              PageNode(
                id: 'test2',
                title: 'test2',
                builder: (context, node) => EmptyPage(node: node),
              ),
              PageNode(
                id: 'test3',
                title: 'test3',
                builder: (context, node) => EmptyPage(node: node),
              )
            ]
          ),
          PageNode(
            id: 'profiles_menu12',
            title: 'Profile menu2',
            builder: (context, node) => EmptyPage(node: node),
          ),
        ],
      )
    );
    
    settings.children.add(
      PageNode(
        id: 'wifi',
        icon: Icon(Icons.settings_outlined, size: 25),
        title: 'Wi-Fi',
        builder: (context, node) => EmptyPage(node: node),
        children: [
          PageNode(
            id: 'wifi_menu1',
            title: 'Wi-Fi on/off',
            builder: (context, node) => EmptyPage(node: node),
          ),
        ],
      ),
    );

    settings.children.add(
      PageNode(
        id: 'about_device',
        icon: Icon(Icons.settings_outlined, size: 25),
        title: 'About Device',
        builder: (context, node) => EmptyPage(node: node),
        children: [
          PageNode(
            id: 'about_device_menu1',
            title: 'About Device menu1',
            builder: (context, node) => EmptyPage(node: node),
          ),
        ],
      ),
    );

    settings.children.add(
      PageNode(
        id: 'about_device2',
        icon: Icon(Icons.settings_outlined, size: 25),
        title: 'About Device2',
        builder: (context, node) => EmptyPage(node: node),
      ),
    );

    settings.children.add(
      PageNode(
        id: 'about_device3',
        icon: Icon(Icons.settings_outlined, size: 25),
        title: 'About Device3',
        builder: (context, node) => EmptyPage(node: node),
      ),
    );

    settings.children.add(
      PageNode(
        id: 'about_device4',
        icon: Icon(Icons.settings_outlined, size: 25),
        title: 'About Device4',
        builder: (context, node) => EmptyPage(node: node),
      ),
    );

    settings.children.add(
      PageNode(
        id: 'about_device5',
        icon: Icon(Icons.settings_outlined, size: 25),
        title: 'About Device5',
        builder: (context, node) => EmptyPage(node: node),
      ),
    );

    settings.children.add(
      PageNode(
        id: 'about_device6',
        icon: Icon(Icons.settings_outlined, size: 25),
        title: 'About Device6',
        builder: (context, node) => EmptyPage(node: node),
      ),
    );

    settings.children.add(
      PageNode(
        id: 'about_device7',
        icon: Icon(Icons.settings_outlined, size: 25),
        title: 'About Device7',
        builder: (context, node) => EmptyPage(node: node),
      ),
    );

    return settings;
  }
}
