import 'package:flutter/gestures.dart';
import 'package:flutter/material.dart';
import 'package:tizen_fs/poc/immersive_home.dart';
import 'package:tizen_fs/poc/home_list.dart';

void main() {
  runApp(const MyApp());
}

class MouseDraggableScrollBehavior extends ScrollBehavior {
  @override
  // Add mouse drag on desktop for easier responsive testing
  Set<PointerDeviceKind> get dragDevices {
    final devices = Set<PointerDeviceKind>.from(super.dragDevices);
    devices.add(PointerDeviceKind.mouse);
    return devices;
  }
}

class MyApp extends StatelessWidget {
  const MyApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'Demo',
      theme: ThemeData(
        colorScheme: ColorScheme.fromSeed(seedColor: Colors.deepPurple),
        useMaterial3: true,
      ),
      home: ScrollConfiguration(
          behavior: MouseDraggableScrollBehavior(),
          child: const PocGalleryPage(title: 'Poc gallery')),
    );
  }
}

class PocGalleryPage extends StatelessWidget {
  const PocGalleryPage({super.key, required this.title});

  final String title;
  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        backgroundColor: Theme.of(context).colorScheme.inversePrimary,
        title: Text(title),
      ),
      body: Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: <Widget>[
            ListTile(
              title: const Text('Poc 1'),
              subtitle: const Text('Description of poc 1'),
              leading: const Icon(Icons.podcasts),
              onTap: () {
                Navigator.push(
                    context,
                    MaterialPageRoute(
                      builder: (context) => const ImmersiveHomeSizeWrapper(),
                    ));
              },
            ),
            ListTile(
              title: const Text('Poc 2'),
              subtitle: const Text('Home List POC'),
              leading: const Icon(Icons.accessibility),
              onTap: () {
                Navigator.push(
                    context,
                    MaterialPageRoute(
                      builder: (context) => const HomePage(),
                    ));
              },
            ),
          ],
        ),
      ),
    );
  }
}
