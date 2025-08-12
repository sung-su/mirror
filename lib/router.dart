import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:tizen_fs/poc/poc_gallery.dart';
import 'package:tizen_fs/screen/main_screen.dart';
import 'package:tizen_fs/settings/settings.dart';

class ScreenPaths {
  static const String main = '/';
  static const String settings = '/settings';
  static const String poc = '/poc';
}

final RouteObserver<ModalRoute<void>> routeObserver =
    RouteObserver<ModalRoute<void>>();

class AppRouter {
  static final router = GoRouter(
    routes: [
      GoRoute(
        path: ScreenPaths.main,
        builder: (context, state) => const MainScreen(),
      ),
      GoRoute(
        path: ScreenPaths.poc,
        builder: (context, state) => const PocGalleryPage(title: 'Poc gallery'),
      ),
      GoRoute(
        path: ScreenPaths.settings,
        builder: (context, state) => Settings(),
      ),
    ],
    observers: [routeObserver],
  );
}
