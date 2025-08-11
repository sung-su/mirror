
import 'package:tizen_fs/models/page_node.dart';
import 'package:tizen_fs/settings/buttons_page.dart';
import 'package:tizen_fs/settings/empty_page.dart';

class SettingPages {
  late final PageNode _root;

  SettingPages() {
    _root = _buildPageTree();
  }

  PageNode getRoot() => _root;
  
  PageNode _buildPageTree() {
    return PageNode (
      id: 'settings',
      title: 'Settings',
      builder: (context, node) => ButtonsPage(node: node), 
      children: [
        PageNode(
          id: 'profiles',
          title: 'Profiles',
          builder: (context, node) => ButtonsPage(node: node),
          children: [
            PageNode(
              id: 'profiles_menu1',
              title: 'Profile menu1',
              builder: (context, node) => EmptyPage(node: node),
            ),
            PageNode(
              id: 'profiles_menu12',
              title: 'Profile menu2',
              builder: (context, node) => EmptyPage(node: node),
            ),
          ],
        ),
        PageNode(
          id: 'wifi',
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
        PageNode(
          id: 'about_device',
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
      ],
    );
  }
}
