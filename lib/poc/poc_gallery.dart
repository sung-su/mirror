import 'package:flutter/material.dart';
import 'package:tizen_fs/poc/adaptive_grid.dart';
import 'package:tizen_fs/poc/disable_auto_scroll.dart';
import 'package:tizen_fs/poc/color_table.dart';
import 'package:tizen_fs/poc/list_poc.dart';
import 'package:tizen_fs/poc/infinite_list.dart';
import 'package:tizen_fs/poc/reoder_grid_test.dart';
import 'package:tizen_fs/poc/reoder_grid_test2.dart';
import 'package:tizen_fs/poc/shimmerloading.dart';
import 'package:tizen_fs/poc/two_page_navi.dart';
import 'package:tizen_fs/screen/main_screen.dart';
import 'package:tizen_fs/widgets/home_screen_size_wrapper.dart';

class PocGalleryPage extends StatelessWidget {
  const PocGalleryPage({super.key, required this.title});

  final String title;
  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        automaticallyImplyLeading: false,
        backgroundColor: Theme.of(context).colorScheme.surfaceContainerHighest,
        title: Text(title),
      ),
      body: Center(
        child: ListView(
          children: <Widget>[
            ListTile(
              title: const Text('Home'),
              subtitle: const Text('go to home'),
              leading: const Icon(Icons.subscriptions),
              onTap: () {
                Navigator.push(
                    context,
                    MaterialPageRoute(
                      builder: (context) =>
                          HomeScreenSizeWrapper(const MainScreen()),
                    ));
              },
            ),
            ListTile(
              title: const Text('Color Table'),
              subtitle: const Text('theme colors for home'),
              leading: const Icon(Icons.subscriptions),
              onTap: () {
                Navigator.push(
                    context,
                    MaterialPageRoute(
                      builder: (context) =>
                          HomeScreenSizeWrapper(const ColorTablePage()),
                    ));
              },
            ),
            ListTile(
              title: const Text('Grid Test 1'),
              subtitle: const Text('Reorderable grid 1'),
              leading: const Icon(Icons.subscriptions),
              onTap: () {
                Navigator.push(
                    context,
                    MaterialPageRoute(
                      builder: (context) =>
                          HomeScreenSizeWrapper(GridTestPage()),
                    ));
              },
            ),
            ListTile(
              title: const Text('Grid Test 2'),
              subtitle: const Text('Reorderable grid 2'),
              leading: const Icon(Icons.subscriptions),
              onTap: () {
                Navigator.push(
                    context,
                    MaterialPageRoute(
                      builder: (context) =>
                          HomeScreenSizeWrapper(GridTestPage2()),
                    ));
              },
            ),
            ListTile(
              title: const Text('H/V List Test'),
              subtitle: const Text('Simple ListView changed layout for user input'),
              leading: const Icon(Icons.subscriptions),
              onTap: () {
                Navigator.push(
                    context,
                    MaterialPageRoute(
                      builder: (context) =>
                          HomeScreenSizeWrapper(ListPocPage()),
                    ));
              },
            ),
            ListTile(
              title: const Text('Shimmer loading Test'),
              subtitle: const Text('page loading test'),
              leading: const Icon(Icons.subscriptions),
              onTap: () {
                Navigator.push(
                    context,
                    MaterialPageRoute(
                      builder: (context) =>
                          HomeScreenSizeWrapper(ShimmerLoadingScreen()),
                    ));
              },
            ),
            ListTile(
              title: const Text('InfiniteScollList'),
              subtitle: const Text('InfiniteScollList test'),
              leading: const Icon(Icons.subscriptions),
              onTap: () {
                Navigator.push(
                    context,
                    MaterialPageRoute(
                      builder: (context) =>
                          HomeScreenSizeWrapper(InfiniteScollList()),
                    ));
              },
            ),
            ListTile(
              title: const Text('Adaptive Grid'),
              subtitle: const Text('Adaptive Grid test'),
              leading: const Icon(Icons.subscriptions),
              onTap: () {
                Navigator.push(
                    context,
                    MaterialPageRoute(
                      builder: (context) =>
                          HomeScreenSizeWrapper(AdaptiveGridPage()),
                    ));
              },
            ),
            ListTile(
              title: const Text('NoAutoScrollFocusView'),
              subtitle: const Text('Disable auto scroll test'),
              leading: const Icon(Icons.subscriptions),
              onTap: () {
                Navigator.push(
                    context,
                    MaterialPageRoute(
                      builder: (context) =>
                          HomeScreenSizeWrapper(NoAutoScrollFocusView()),
                    ));
              },
            ),
            ListTile(
              title: const Text('TwoPageNavigation'),
              subtitle: const Text('TwoPageNavigation'),
              leading: const Icon(Icons.subscriptions),
              onTap: () {
                Navigator.push(
                    context,
                    MaterialPageRoute(
                      builder: (context) =>
                          HomeScreenSizeWrapper(TwoPageNavigation()),
                    ));
              },
            ),
          ],
        ),
      ),
    );
  }
}
