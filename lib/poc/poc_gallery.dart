import 'package:flutter/material.dart';
import 'package:tizen_fs/poc/immersive_carousel_poc.dart';
import 'package:tizen_fs/poc/media_db_parser_poc.dart';
import 'package:tizen_fs/widgets/home_screen_size_wrapper.dart';
import 'media_list_poc.dart';
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
            ListTile(
              title: const Text('ImmersiveCarousel Poc'),
              subtitle: const Text('Immersive Carousel'),
              leading: const Icon(Icons.subscriptions),
              onTap: () {
                Navigator.push(
                    context,
                    MaterialPageRoute(
                      builder: (context) => HomeScreenSizeWrapper(
                          const ImmersiveCarouselPocPage()),
                    ));
              },
            ),
            ListTile(
              title: const Text('Media List Poc'),
              leading: const Icon(Icons.subscriptions),
              onTap: () {
                Navigator.push(
                    context,
                    MaterialPageRoute(
                      builder: (context) =>
                          HomeScreenSizeWrapper(const MediaListPocPage()),
                    ));
              },
            ),
            ListTile(
              title: const Text('Media DB Poc'),
              subtitle: const Text(
                  'Parsing the media db to get categories and tiles'),
              leading: const Icon(Icons.table_view),
              onTap: () {
                Navigator.push(
                    context,
                    MaterialPageRoute(
                      builder: (context) =>
                          HomeScreenSizeWrapper(const MediaDBParserPocPage()),
                    ));
              },
            ),
          ],
        ),
      ),
    );
  }
}
