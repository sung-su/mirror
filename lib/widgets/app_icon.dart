import 'package:flutter/material.dart';

class AppLabel extends StatelessWidget {
  AppLabel({super.key, required this.foreground, required this.background});

  final Widget foreground;
  final Widget background;

  @override
  Widget build(BuildContext context) {
    return Stack(
      children: [
        background,
        foreground
      ],
    );
  }
}