import 'dart:async';

import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:provider/provider.dart';
import 'package:tizen_fs/widgets/backdrop_scaffold.dart';
import 'package:tizen_fs/widgets/immersive_list.dart';
import 'package:tizen_fs/widgets/immersive_carousel.dart';

class ImmersiveCarouselPocPage extends StatelessWidget {
  const ImmersiveCarouselPocPage({super.key});

  @override
  Widget build(BuildContext context) {
    return BackdropScaffold(child: const HomeContent());
  }
}

class HomeContent extends StatefulWidget {
  const HomeContent({super.key});

  @override
  State<HomeContent> createState() => _HomeContentState();
}

class _HomeContentState extends State<HomeContent> {
  final ScrollController _scrollController = ScrollController();
  final ImmersiveListModel _immersiveListModel = ImmersiveListModel.fromMock();
  final ImmersiveCarouselModel _immersiveCarouselModel = ImmersiveCarouselModel.fromMock();
  final ImmersiveAreaController _immersiveAreaController =
      ImmersiveAreaController();


  @override
  Widget build(BuildContext context) {
    return MultiProvider(
      providers: [
        ChangeNotifierProvider(
          create: (context) => _immersiveListModel,
        ),
        ChangeNotifierProvider(
          create: (context) => _immersiveCarouselModel,
        ),
      ],
      child: SingleChildScrollView(
        controller: _scrollController,
        child: Column(
          children: [
            MockFocusableTabbar(onFocused: () {
              print('Header focused');
              _scrollController.animateTo(
                0,
                duration: const Duration(milliseconds: 100),
                curve: Curves.easeIn,
              );
              _immersiveAreaController
                  .setState(ImmersiveAreaController.headerFocused);
            }),
            ImmersiveArea(_immersiveAreaController, onFocused: () {
              print('ImmersiveArea focused');
              _scrollController.animateTo(
                0,
                duration: const Duration(milliseconds: 100),
                curve: Curves.easeIn,
              );
              _immersiveAreaController
                  .setState(ImmersiveAreaController.carouselFocused);
            }),
            ImmersiveListArea(
              onFocused: () {
                print('item 1 focused');
                _scrollController.animateTo(
                  80,
                  duration: const Duration(milliseconds: 100),
                  curve: Curves.easeIn,
                );
                _immersiveAreaController
                    .setState(ImmersiveAreaController.immersiveListFocused);
              },
            ),
            MockItem(
              onFocus: () {
                print('item 2 focused');
                _scrollController.animateTo(
                  550,
                  duration: const Duration(milliseconds: 100),
                  curve: Curves.easeInQuad,
                );
              },
            ),
            MockItem(),
            MockItem(),
            MockItem(),
            MockItem(),
            MockItem(),
            MockItem(),
            MockItem(),
          ],
        ),
      )
    );
  }
}

class ImmersiveAreaController {
  static const int headerFocused = 0;
  static const int carouselFocused = 1;
  static const int immersiveListFocused = 2;

  final List<void Function(int)> _listeners = [];

  void addListener(void Function(int) listener) {
    _listeners.add(listener);
  }

  void removeListener(void Function(int) listener) {
    _listeners.remove(listener);
  }

  void setState(int value) {
    for (final listener in _listeners) {
      listener.call(value);
    }
  }
}

class ImmersiveArea extends StatefulWidget {
  final VoidCallback? onFocused;
  final ImmersiveAreaController? controller;

  const ImmersiveArea(
    this.controller, {
    super.key,
    this.onFocused,
  });

  @override
  State<ImmersiveArea> createState() => _ImmersiveAreaState();
}

class _ImmersiveAreaState extends State<ImmersiveArea> {
  final _carouselKey = GlobalKey<ImmersiveCarouselState>();
  final FocusNode _focusNode = FocusNode();
  final PageController _pageController = PageController();

  bool expand = false;

  @override
  void initState() {
    super.initState();
    _focusNode.addListener(_focusChanged);
    widget.controller?.addListener((state) {
      if (state == ImmersiveAreaController.headerFocused) {
        // focused on header
        setState(() {
          expand = false;
        });
        _pageController.animateToPage(0,
            duration: const Duration(milliseconds: 100), curve: Curves.ease);
      } else if (state == ImmersiveAreaController.carouselFocused) {
        // focused on carousel area
        setState(() {
          expand = true;
        });
        _pageController.animateToPage(0,
            duration: const Duration(milliseconds: 100), curve: Curves.ease);
      } else if (state == ImmersiveAreaController.immersiveListFocused) {
        // focused on immersive list area
        setState(() {
          expand = true;
        });
        _pageController.animateToPage(1,
            duration: const Duration(milliseconds: 100), curve: Curves.ease);
      }
    });
  }

  @override
  void dispose() {
    _repeatingTimer?.cancel();
    _focusNode.removeListener(_focusChanged);
    _focusNode.dispose();
    super.dispose();
  }

  void _focusChanged() {
    if (_focusNode.hasFocus) {
      setState(() {
        expand = true;
      });
      _pageController.animateToPage(0,
          duration: const Duration(milliseconds: 100), curve: Curves.ease);
      widget.onFocused?.call();
    }
  }

  Timer? _initialDelayTimer;
  Timer? _repeatingTimer;
  void _startRepeating(VoidCallback action) {
    action();
    _initialDelayTimer?.cancel();
    _initialDelayTimer = Timer(const Duration(milliseconds: 300), () {
      _repeatingTimer = Timer.periodic(const Duration(milliseconds: 100), (timer) {
        action();
      });
    });
  }

  void _stopRepeating() {
    _initialDelayTimer?.cancel();
    _repeatingTimer?.cancel();
    _initialDelayTimer = null;
    _repeatingTimer = null;
  }

  @override
  Widget build(BuildContext context) {
    return Focus(
      onKeyEvent: (node, event) {
        if (event is KeyDownEvent) {
          if (event.logicalKey == LogicalKeyboardKey.arrowRight) {
            _startRepeating(() {
              _carouselKey.currentState?.moveCarousel(1);
            });
            return KeyEventResult.handled;
          } else if (event.logicalKey == LogicalKeyboardKey.arrowLeft) {
            _startRepeating(() {
              _carouselKey.currentState?.moveCarousel(-1);
            });
            return KeyEventResult.handled;
          }
        }
        else if (event is KeyUpEvent) {
          _stopRepeating();
          return KeyEventResult.handled;
        }
        return KeyEventResult.ignored;
      },
      focusNode: _focusNode,
      child: Builder(builder: (context) {
        return AnimatedContainer(
          duration: const Duration(milliseconds: 100),
          height: expand ? 354 : 210,
          child: PageView(
            physics: const NeverScrollableScrollPhysics(),
            controller: _pageController,
            scrollDirection: Axis.vertical,
            children: [
              ImmersiveCarousel(
                key: _carouselKey,
                isExpanded: expand,
              ),
              ImmersiveContentArea(),
            ],
          ),
        );
      }),
    );
  }
}

class MockItem extends StatefulWidget {
  const MockItem({
    super.key,
    this.onFocus,
  });

  final VoidCallback? onFocus;

  @override
  State<MockItem> createState() => _MockItemState();
}

class _MockItemState extends State<MockItem> {
  final FocusNode _focusNode = FocusNode();

  @override
  void initState() {
    super.initState();
    _focusNode.addListener(_focusChanged);
  }

  void _focusChanged() {
    if (_focusNode.hasFocus) {
      widget.onFocus?.call();
    }
  }

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.fromLTRB(55, 10, 55, 10),
      child: Focus(
          focusNode: _focusNode,
          child: Builder(builder: (buildContext) {
            // create size animation for the container
            return AnimatedContainer(
                duration: const Duration(milliseconds: 100),
                height: Focus.of(buildContext).hasFocus ? 150 : 100,
                decoration: BoxDecoration(
                  color: Focus.of(buildContext).hasFocus
                      ? Colors.purple
                      : Colors.yellow,
                  border: Border.all(color: Colors.black),
                  borderRadius: BorderRadius.circular(10),
                ));
          })),
    );
  }
}

class MockFocusableTabbar extends StatelessWidget {
  final VoidCallback? onFocused;
  const MockFocusableTabbar({super.key, this.onFocused});

  @override
  Widget build(BuildContext context) {
    return Focus(
        autofocus: true,
        onFocusChange: (onFocused != null)
            ? (hasFocus) {
                if (hasFocus) {
                  onFocused?.call();
                }
              }
            : null,
        child: Builder(builder: (buildContext) {
          return AnimatedContainer(
              duration: const Duration(milliseconds: 100),
              height: 80,
              decoration: BoxDecoration(
                color: Focus.of(buildContext).hasFocus
                    ? Colors.blue
                    : Colors.white.withAlpha(30),
              ),
              child: Center(
                  child: const Text('Header',
                      style: TextStyle(fontSize: 30, color: Colors.white))));
        }));
  }
}

class MockHorizontalCarousel extends StatelessWidget {
  const MockHorizontalCarousel(this.expand, {super.key});

  final bool expand;

  @override
  Widget build(BuildContext context) {
    return AnimatedContainer(
        duration: const Duration(milliseconds: 100),
        height: expand ? 300 : 200,
        decoration: BoxDecoration(
          color: expand ? Colors.blue : Colors.white.withAlpha(30),
        ),
        child: PageView(
          scrollDirection: Axis.horizontal,
          children: [
            Card(
              color: Colors.lightBlue,
              child: const Center(
                child: Text('Page 1'),
              ),
            ),
            Card(
              color: Colors.yellow,
              child: const Center(
                child: Text('Page 2'),
              ),
            ),
          ],
        ));
  }
}
