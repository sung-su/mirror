import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:tizen_fs/providers/backdrop_provider.dart';
import 'package:tizen_fs/styles/app_style.dart';
import 'package:tizen_fs/widgets/focus_selectable.dart';
import 'package:tizen_fs/widgets/selectable_listview.dart';

class MockAppsPage extends StatefulWidget {
  const MockAppsPage({super.key});

  @override
  State<MockAppsPage> createState() => _MockAppsPageState();
}

class _MockAppsPageState extends State<MockAppsPage> {
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
        MockListH(),
        MockListH(),
      ],
    );
  }
}

class MockListH extends StatefulWidget {
  const MockListH({super.key});

  @override
  State<MockListH> createState() => _MockListHState();
}

class _MockListHState extends State<MockListH> with FocusSelectable<MockListH> {
  final double itemSize = 300;

  @override
  Widget build(BuildContext context) {
    return Focus(
      focusNode: focusNode,
      child: SizedBox(
        height: itemSize * 0.6,
        child: SelectableListView(
            key: listKey,
            padding: const EdgeInsets.symmetric(horizontal: 58, vertical: 5),
            itemCount: 5,
            itemBuilder: (context, index, selectedIndex, key) {
              return AnimatedScale(
                key: key,
                scale:
                    Focus.of(context).hasFocus && index == selectedIndex ? 1.1 : 1.0,
                duration: const Duration(milliseconds: 200),
                child: Card(
                  margin: EdgeInsets.all(10),
                  shape: Focus.of(context).hasFocus && index == selectedIndex
                      ? RoundedRectangleBorder(
                          side: BorderSide(color: Colors.grey.withAlphaF(0.5), width: 2.0),
                          borderRadius: BorderRadius.circular(10),
                        )
                      : null,
                  child: Container(
                    decoration: BoxDecoration(
                      borderRadius: BorderRadius.circular(10),
                      color: Colors.grey[900],
                    ),
                    width: itemSize,
                  ),
                ),
              );
            }),
      ),
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
