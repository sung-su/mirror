
import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:tizen_fs/locator.dart';
import 'package:tizen_fs/models/app_data_model.dart';
import 'package:tizen_fs/providers/wifi_provider.dart';
import 'package:tizen_fs/router.dart';
import 'package:tizen_fs/styles/app_style.dart';
import 'package:provider/provider.dart';
import 'package:tizen_fs/providers/device_info_provider.dart';

void main() {
  WidgetsFlutterBinding.ensureInitialized();
  setupAppModel();
  runApp(
    MultiProvider (
      providers: [
        ChangeNotifierProvider<AppDataModel>(
          create: (context) => getIt<AppDataModel>(),
        ),
        ChangeNotifierProvider(
          create: (context) => DeviceInfoProvider()..loadDeviceInfo(),
        ),
        ChangeNotifierProvider<WifiProvider>(
          create: (context) => WifiProvider(),
        ),
      ],
      child: const App(),
    ),
  );
}

class App extends StatelessWidget {
  const App({super.key});

  // This widget is the root of your application.
  @override
  Widget build(BuildContext context) {
    return MaterialApp.router(
      title: 'Tizen First Screen',
      themeMode: ThemeMode.dark,
      theme: $style.colors.toLightThemeData(),

      darkTheme: $style.colors.toDarkThemeData(),
      routerConfig: AppRouter.router,
      // scrollBehavior: MouseDraggableScrollBehavior()
    );
  }
}

// class MouseDraggableScrollBehavior extends ScrollBehavior {
//   @override
//   // Add mouse drag on desktop for easier responsive testing
//   Set<PointerDeviceKind> get dragDevices {
//     final devices = Set<PointerDeviceKind>.from(super.dragDevices);
//     devices.add(PointerDeviceKind.mouse);
//     devices.add(PointerDeviceKind.touch);
//     return devices;
//   }
// }
