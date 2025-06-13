import 'package:flutter/material.dart';
import 'package:tizen_fs/poc/immersive_carousel_poc.dart';
import 'package:tizen_fs/poc/list_poc.dart';
import 'package:tizen_fs/poc/media_card_poc.dart';
import 'package:tizen_fs/poc/media_db_parser_poc.dart';
import 'package:tizen_fs/poc/video_player_poc.dart';
import 'package:tizen_fs/router.dart';
import 'package:tizen_fs/widgets/home_screen_size_wrapper.dart';
import 'media_list_poc.dart';
import 'setting_panel_poc.dart';
import 'immersive_list_poc.dart';
import 'tab_poc.dart';
import 'youtube_poc.dart';

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
        child: ListView(
          children: <Widget>[
            ListTile(
              title: const Text('List Poc'),
              subtitle: const Text('Test for ensureVisible of the Scroller'),
              leading: const Icon(Icons.subscriptions),
              onTap: () {
                Navigator.push(
                    context,
                    MaterialPageRoute(
                      builder: (context) =>
                          HomeScreenSizeWrapper(const ListPocPage()),
                    ));
              },
            ),
            ListTile(
              title: const Text('Go to Home'),
              subtitle: const Text('Back to the main screen'),
              leading: const Icon(Icons.home),
              onTap: () {
                AppRouter.router.go(ScreenPaths.home);
              },
            ),
            ListTile(
              title: const Text('video player Poc'),
              subtitle: const Text('video player without DRM'),
              leading: const Icon(Icons.home),
              onTap: () {
                Navigator.push(
                    context,
                    MaterialPageRoute(
                      builder: (context) =>
                          HomeScreenSizeWrapper(const VideoPlayerPocPage()),
                    ));
              },
            ),
            ListTile(
              title: const Text('Tab Poc'),
              subtitle: const Text('Appbar and Pages'),
              leading: const Icon(Icons.subscriptions),
              onTap: () {
                Navigator.push(
                    context,
                    MaterialPageRoute(
                      builder: (context) =>
                          HomeScreenSizeWrapper(const TabPocPage()),
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
              subtitle: const Text('Media List Views'),
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
            ListTile(
              title: const Text('YouTube Data Poc'),
              subtitle: const Text('Fetching the YouTube data and show tiles'),
              leading: const Icon(Icons.video_library),
              onTap: () {
                Navigator.push(
                    context,
                    MaterialPageRoute(
                      builder: (context) =>
                          HomeScreenSizeWrapper(const YoutubePocPage()),
                    ));
              },
            ),
            ListTile(
              title: const Text('Media Card Poc'),
              subtitle: const Text('Card for Media contents'),
              leading: const Icon(Icons.video_library),
              onTap: () {
                Navigator.push(
                    context,
                    MaterialPageRoute(
                      builder: (context) => MediaCardPocPage(),
                    ));
              },
            ),
          ],
        ),
      ),
    );
  }
}
