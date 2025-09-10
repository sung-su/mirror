import 'dart:io';
import 'package:flutter/material.dart';
import 'package:tizen_fs/models/app_data.dart';
import 'package:tizen_fs/styles/app_style.dart';

class AppTile extends StatelessWidget {
  AppTile({super.key, required this.app});

  final AppData app;
  late final Widget foreground;
  late final Widget background;

  late Widget _icon;
  final int _iconWidth = 80;
  Color _backgroundColor = Colors.transparent;

  Widget? _getForeground() {
    return null;
  }

  Widget? _getBackground() {
    return null;
  }

  Widget _loadIcon() {
    final iconUrl = app.icon;

    if (iconUrl.startsWith('/')) {
      final icon = Image.file(
        File(iconUrl),
        cacheWidth: _iconWidth,
        errorBuilder:
            (context, error, stackTrace) => const Icon(Icons.broken_image),
        fit: BoxFit.fitHeight,
      );
      // _backgroundColor2 = getDominantColor(icon.image);
      return icon;
    } else if (iconUrl.startsWith('assets')) {
      final icon = Image.asset(
        iconUrl,
        cacheWidth: _iconWidth,
        fit: BoxFit.fitHeight,
      );
      // _backgroundColor2 = getDominantColor(icon.image);
      return icon;
    } else {
      return const Center(
        child: Icon(Icons.image_not_supported, color: Colors.grey),
      );
    }
  }

  Color getAppColor() {
    if (app.appType.contains('dotnet')) {
      return $style.colors.dotnetApp;
    } else if (app.appType.contains('cpp')) {
      return $style.colors.cApp;
    }
    return $style.colors.defaulApp;
  }

  @override
  Widget build(BuildContext context) {
    _icon = _loadIcon();
    return Stack(
      children: [
        Container(color: getAppColor()),
        Center(child: SizedBox(width: 50, child: _icon)),
      ],
    );
  }
}
