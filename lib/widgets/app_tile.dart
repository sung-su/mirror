import 'package:flutter/material.dart';
import 'package:tizen_fs/models/app_data.dart';
import 'package:tizen_fs/styles/app_style.dart';

class AppTile extends StatelessWidget {
  AppTile({super.key, required this.app});

  final AppData app;
  late final Widget foreground;
  late final Widget background;

  Widget? _getForeground() {
    return null;
  }

  Widget? _getBackground() {
    return null;
  }

  @override
  Widget build(BuildContext context) {
    return Stack(
      children: [
        _getBackground() ?? Container(
          decoration: BoxDecoration(
            borderRadius: BorderRadius.circular(8),
            gradient: $style.gradients.getGradient(app.name.length % 5)
          ),
        ),
        Center(
          child: _getForeground() ?? Text(
            app.name,
          ),
        )
      ],
    );
  }
}