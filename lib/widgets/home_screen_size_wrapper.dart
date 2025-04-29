import 'package:flutter/material.dart';

class HomeScreenSizeWrapper extends StatelessWidget {
  const HomeScreenSizeWrapper(this.child, {super.key});
  final Widget child;

  @override
  Widget build(BuildContext context) {
    var screenWidth = MediaQuery.of(context).size.width;
    return (screenWidth != 960)
        ? FractionallySizedBox(
            widthFactor: 960 / screenWidth,
            heightFactor: 960 / screenWidth,
            child: Transform.scale(
              scale: screenWidth / 960,
              child: child,
            ),
          )
        : child;
  }
}
