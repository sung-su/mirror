import 'package:flutter/material.dart';
import 'package:tizen_fs/router.dart';
import 'package:tizen_fs/styles/app_style.dart';

void main() {
  runApp(const App());
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
    );
  }
}
