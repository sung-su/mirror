import 'package:flutter/material.dart';
import 'package:tizen_fs/widgets/home_screen_size_wrapper.dart';
import 'setting_panel_poc.dart';
import 'immersive_list_poc.dart';
import 'home_poc.dart';

class PocGalleryPage extends StatelessWidget {
  const PocGalleryPage({super.key, required this.title});

  final String title;
  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        backgroundColor: Theme.of(context).colorScheme.surfaceContainerHighest,
        title: Text(title),
      ),
      body: Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: <Widget>[
            ListTile(
              title: const Text('Home Poc'),
              subtitle: const Text('Appbar and lists'),
              leading: const Icon(Icons.subscriptions),
              onTap: () {
                Navigator.push(
                    context,
                    MaterialPageRoute(
                      builder: (context) =>
                          HomeScreenSizeWrapper(const HomePocPage()),
                    ));
              },
            ),
            ListTile(
              title: const Text('ImmersiveList Poc'),
              subtitle: const Text('google tv home screen'),
              leading: const Icon(Icons.subscriptions),
              onTap: () {
                Navigator.push(
                    context,
                    MaterialPageRoute(
                      builder: (context) =>
                          HomeScreenSizeWrapper(const ImmersiveListPocPage()),
                    ));
              },
            ),
            ListTile(
              title: const Text('Setting panel drawer Poc'),
              subtitle: const Text('Left side setting panel'),
              leading: const Icon(Icons.subscriptions),
              onTap: () {
                Navigator.push(
                    context,
                    MaterialPageRoute(
                      builder: (context) =>
                          HomeScreenSizeWrapper(const SettingPanelPocPage()),
                    ));
              },
            ),
          ],
        ),
      ),
    );
  }
}
