import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:tizen_fs/providers/backdrop_provider.dart';
import 'package:tizen_fs/styles/app_style.dart';

class BackdropScaffold extends StatelessWidget {
  final Widget child;

  const BackdropScaffold({super.key, required this.child});

  @override
  Widget build(BuildContext context) {
    return ChangeNotifierProvider(
      create: (context) => BackdropProvider(),
      child: Builder(
        builder: (context) {
          var backdrop = Provider.of<BackdropProvider>(context).backdrop;
          return Scaffold(
            body: Stack(
              children: [
                // Background
                SizedBox.expand(
                  child: DecoratedBox(
                    decoration: BoxDecoration(
                      // color: const Color.fromARGB(255, 18, 18, 18)
                      color: Theme.of(context).colorScheme.surface,
                    ),
                  ),
                ),
                // Backdrop
                RepaintBoundary(
                  child: AnimatedSwitcher(
                    duration: $style.times.fast,
                    switchInCurve: Curves.easeOutCubic,
                    switchOutCurve: Curves.easeOutCubic,
                    transitionBuilder:
                        (child, animation) => ScaleTransition(
                          scale: Tween<double>(
                            begin: 0.95,
                            end: 1.1,
                          ).animate(animation),
                          child: FadeTransition(
                            opacity: animation,
                            child: child,
                          ),
                        ),
                    child:
                        (backdrop != null)
                            ? AnimatedScale(
                              duration: $style.times.fast,
                              curve: Curves.easeOutCubic,
                              scale:
                                  Provider.of<BackdropProvider>(
                                        context,
                                      ).isZoomIn
                                      ? 1.1
                                      : 1.0,
                              child: backdrop,
                            )
                            : SizedBox.shrink(),
                  ),
                ),
                // Main content
                child,
              ],
            ),
          );
        },
      ),
    );
  }
}
