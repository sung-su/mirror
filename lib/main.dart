import 'package:flutter/gestures.dart';
import 'package:flutter/material.dart';
import 'router.dart';

void main() {
  runApp(const TizenFS());

  // for testing purposes
  bool showPoc = true;
  if (showPoc) {
    AppRouter.router.go(ScreenPaths.poc);
  }
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

class TizenFS extends StatelessWidget {
  const TizenFS({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp.router(
      title: 'Tizen First Screen',
      theme: ThemeData.dark(useMaterial3: true),
      scrollBehavior: MouseDraggableScrollBehavior(),
      routerConfig: AppRouter.router,
    );
  }
}
