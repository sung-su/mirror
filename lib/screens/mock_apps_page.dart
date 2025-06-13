import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:provider/provider.dart';
import 'package:tizen_fs/providers/backdrop_provider.dart';
import 'package:tizen_fs/styles/app_style.dart';

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
        Provider.of<BackdropProvider>(context, listen: false)
            .updateBackdrop(null);
      }
    });
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

class _MockListHState extends State<MockListH> {
  final ScrollController _controller = ScrollController();
  int selected = 0;
  final double itemSize = 300;

  @override
  void initState() {
    super.initState();
  }

  void _moveTo(int index) {
    _controller.animateTo(index * itemSize, duration: const Duration(milliseconds: 150), curve: Curves.easeIn);
  }

  @override
  Widget build(BuildContext context) {
    return Focus(
      onKeyEvent: (_, keyEvent) {
        if (keyEvent is KeyDownEvent || keyEvent is KeyRepeatEvent) {
          if (keyEvent.logicalKey == LogicalKeyboardKey.arrowLeft &&
              selected > 0) {
            setState(() {
              selected--;
            });
            _moveTo(selected);
            return KeyEventResult.handled;
          } else if (keyEvent.logicalKey == LogicalKeyboardKey.arrowRight &&
              selected < 9) {
            setState(() {
              selected++;
            });
            _moveTo(selected);
            return KeyEventResult.handled;
          }
        }
        return KeyEventResult.ignored;
      },
      child: SizedBox(
        height: itemSize * 0.6,
        child: ListView.builder(
            controller: _controller,
            padding: const EdgeInsets.symmetric(horizontal: 58, vertical: 5),
            scrollDirection: Axis.horizontal,
            itemCount: 5,
            itemExtent: itemSize,
            itemBuilder: (context, index) {
              return AnimatedScale(
                scale:
                    Focus.of(context).hasFocus && index == selected ? 1.1 : 1.0,
                duration: const Duration(milliseconds: 200),
                child: Card(
                  margin: EdgeInsets.all(10),
                  shape: Focus.of(context).hasFocus && index == selected
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