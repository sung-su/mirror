import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:tizen_fs/screens/home_screen.dart';
import 'package:tizen_fs/poc/poc_gallery.dart';

class ScreenPaths {
  static const String home = '/';
  static const String poc = '/poc';
}

final RouteObserver<ModalRoute<void>> routeObserver =
    RouteObserver<ModalRoute<void>>();

class AppRouter {
  static final router = GoRouter(
    routes: [
      GoRoute(
        path: ScreenPaths.home,
        builder: (context, state) => const HomeScreen(),
      ),
      GoRoute(
        path: ScreenPaths.poc,
        builder: (context, state) => const PocGalleryPage(title: 'Poc gallery'),
      ),
    ],
    observers: [routeObserver],
  );
}
