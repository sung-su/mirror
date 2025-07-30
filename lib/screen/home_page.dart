import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:tizen_fs/providers/backdrop_provider.dart';

class HomePage extends StatefulWidget {
  const HomePage({super.key});

  @override
  State<HomePage> createState() => _HomePageState();
}

class _HomePageState extends State<HomePage> {
  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addPostFrameCallback((_) {
      if (mounted) {
        Provider.of<BackdropProvider>(context, listen: false).updateBackdrop(_getBackdrop());
      }
    });
  }

  Widget _getBackdrop() {
    return CinematicScrim(image: Image.asset(
      'assets/images/gradient-bg-static-alt-a-D0K-Mjox.webp',
      width: 1920,
      height: 1080,
      fit: BoxFit.cover
    )
    );
  }

  @override
  Widget build(BuildContext context) {
    return Column(
      children: [
        SizedBox(
          height: 300,
          child: Opacity(
            opacity: 0.7,
            child: Container(
              color: Theme.of(context).colorScheme.surfaceContainer
            ),
          ),
        )
      ],
    );
  }
}

class CinematicScrim extends StatelessWidget {
  const CinematicScrim({super.key, required this.image});

  final Widget image;

  @override
  Widget build(BuildContext context) {
    var surfaceColor = Theme.of(context).colorScheme.surface;
    return Stack(
      children: [
        Padding(
          padding: const EdgeInsets.all(2.0),
          child: image,
        ),
        Container(
          decoration: BoxDecoration(
            gradient: LinearGradient(
              begin: Alignment(0, 0),
              end: Alignment(0, 1),
              // radius: 2,
              colors: [
                surfaceColor.withAlpha(0),
                surfaceColor
              ],
              stops: const [0, 0.8],
            ),
          ),
        ),
      ],
    );
  }
}
