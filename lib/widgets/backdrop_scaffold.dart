import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:tizen_fs/services/backdrop_provider.dart';

class BackdropScaffold extends StatelessWidget {
  final Widget child;

  const BackdropScaffold({
    super.key,
    required this.child,
  });

  @override
  Widget build(BuildContext context) {
    return ChangeNotifierProvider(
      create: (context) => BackdropProvider(),
      child: Builder(builder: (context) {
        var backdrop = Provider.of<BackdropProvider>(context).backdrop;
        return Scaffold(
            body: Stack(children: [
          // Background
          SizedBox.expand(
              child: DecoratedBox(
                  decoration: BoxDecoration(color: const Color(0xff1a110f)))),
          // Backdrop
          AnimatedSwitcher(
            duration: const Duration(milliseconds: 200),
            transitionBuilder: (child, animation) => ScaleTransition(
                scale: Tween<double>(begin: 0.95, end: 1.1).animate(animation),
                child: FadeTransition(opacity: animation, child: child)),
            child: (backdrop != null) ? backdrop : SizedBox.shrink(),
          ),

          // Main content
          child,
        ]));
      }),
    );
  }

}