import 'package:flutter/material.dart';
import 'package:flutter/services.dart';

class HomePage extends StatelessWidget {
  const HomePage({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: const HomeContent(),
    );
  }
}

class HomeContent extends StatefulWidget {
  const HomeContent({super.key});

  @override
  State<HomeContent> createState() => _HomeContentState();
}

class _HomeContentState extends State<HomeContent> {
  final ScrollController _scrollController = ScrollController();
  @override
  Widget build(BuildContext context) {
    return Container(
      color: Colors.black,
      child: SingleChildScrollView(
        controller: _scrollController,
        child: Column(
          children: [
            MockFocusableTabbar(),
            ImmersiveArea(onFocus: () {
              print('ImmersiveArea focused');
              _scrollController.animateTo(
                0,
                duration: const Duration(milliseconds: 100),
                curve: Curves.easeIn,
              );
            }),
            ImmersiveListArea(
              onFocused: () {
                print('item 1 focused');
                _scrollController.animateTo(
                  80,
                  duration: const Duration(milliseconds: 100),
                  curve: Curves.easeIn,
                );
              },
            ),
            MockItem(onFocus: () {
                print('item 2 focused');
                _scrollController.animateTo(
                    550,
                    duration: const Duration(milliseconds: 100),
                    curve: Curves.easeInQuad,
                );
            },),
            MockItem(),
            MockItem(),
            MockItem(),
            MockItem(),
            MockItem(),
            MockItem(),
            MockItem(),
          ],
        ),
      ),
    );
  }
}

class ImmersiveArea extends StatefulWidget {
  const ImmersiveArea({
    super.key,
    this.onFocus,
  });

  final VoidCallback? onFocus;

  @override
  State<ImmersiveArea> createState() => _ImmersiveAreaState();
}

class _ImmersiveAreaState extends State<ImmersiveArea> {
  final FocusNode _focusNode = FocusNode();
  final PageController _pageController = PageController();

  bool expand = false;

  @override
  void initState() {
    super.initState();
    _focusNode.addListener(_focusChanged);
  }

  @override
  void dispose() {
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
      widget.onFocus?.call();
    }
  }

  KeyEventResult _onKeyEvent(FocusNode focusNode, KeyEvent event) {
    if (event is KeyDownEvent) {
      if (event.logicalKey == LogicalKeyboardKey.arrowUp) {
        setState(() {
          expand = false;
        });
        return KeyEventResult.ignored;
      } else if (event.logicalKey == LogicalKeyboardKey.arrowDown) {
        setState(() {
          expand = true;
        });
        _pageController.animateToPage(1,
            duration: const Duration(milliseconds: 100), curve: Curves.ease);
        Scrollable.ensureVisible(context,
            alignment: 0, duration: const Duration(milliseconds: 100));
        return KeyEventResult.ignored;
      }
    }
    return KeyEventResult.ignored;
  }

  @override
  Widget build(BuildContext context) {
    return Focus(
      focusNode: _focusNode,
      onKeyEvent: _onKeyEvent,
      child: Builder(builder: (context) {
        return AnimatedContainer(
          duration: const Duration(milliseconds: 100),
          height: expand ? 344 : 210,
          child: PageView(
            controller: _pageController,
            scrollDirection: Axis.vertical,
            children: [
              MockHorizontalCarousel(expand),
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
  const MockFocusableTabbar({
    super.key,
  });

  @override
  Widget build(BuildContext context) {
    return Focus(
        autofocus: true,
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

class ImmersiveContentArea extends StatelessWidget {
  const ImmersiveContentArea({
    super.key,
  });
  static const double leftPadding = 58;
  @override
  Widget build(BuildContext context) {
    return Column(crossAxisAlignment: CrossAxisAlignment.start, children: [
      Center(
          child: Padding(
              padding: EdgeInsets.only(top: 10),
              child:
                  Icon(Icons.keyboard_arrow_up, size: 35, color: Colors.grey))),
      Padding(
          padding: EdgeInsets.only(left: leftPadding, top: 10, bottom: 10),
          child: const Text(
            'Top picks for you',
            textAlign: TextAlign.left,
            style: TextStyle(fontSize: 24, color: Colors.grey),
          )),
      Expanded(child: SizedBox.shrink()),
      Padding(
        padding: EdgeInsets.only(left: leftPadding),
        child: SizedBox(
            width: 480,
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              mainAxisAlignment: MainAxisAlignment.start,
              children: [
                AnimatedSwitcher(
                  duration: const Duration(milliseconds: 300),
                  transitionBuilder:
                      (Widget child, Animation<double> animation) {
                    return FadeTransition(
                      opacity: animation,
                      child: child,
                    );
                  },
                  child: Text(
                    "Title !!!!!!!",
                    key: ValueKey(1),
                    style: TextStyle(
                      fontSize: 40,
                      color: Colors.white,
                    ),
                  ),
                ),
                SizedBox(height: 8),
                AnimatedSwitcher(
                  duration: const Duration(milliseconds: 300),
                  transitionBuilder:
                      (Widget child, Animation<double> animation) {
                    return FadeTransition(
                      opacity: animation,
                      child: child,
                    );
                  },
                  child: Text(
                    "Subtitle!!!!",
                    key: ValueKey(1),
                    style: TextStyle(fontSize: 18, color: Colors.grey),
                  ),
                ),
                SizedBox(height: 5),
                AnimatedSwitcher(
                  duration: const Duration(milliseconds: 300),
                  transitionBuilder:
                      (Widget child, Animation<double> animation) {
                    return FadeTransition(
                      opacity: animation,
                      child: child,
                    );
                  },
                  child: Text(
                    "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.",
                    key: ValueKey(1),
                    style: TextStyle(fontSize: 16, color: Colors.grey),
                    softWrap: true,
                  ),
                ),
              ],
            )),
      )
    ]);
  }
}

class ImmersiveListArea extends StatefulWidget {
  ImmersiveListArea({
    super.key,
    this.onFocused,
  });
  final VoidCallback? onFocused;
  final List<ImmersiveContent> contents =
      ImmersiveContent.generateMockContent();

  @override
  State<ImmersiveListArea> createState() => _ImmersiveListAreaState();
}

class _ImmersiveListAreaState extends State<ImmersiveListArea> {
  final ScrollController _scrollController = ScrollController();
  final FocusNode _focusNode = FocusNode();
  late List<GlobalKey> _itemKeys;

  bool hasFocus = false;
  int selectedIndex = 0;
  double leftPadding = 58;
  int _itemCount = 0;

  @override
  void initState() {
    super.initState();
    _itemCount = widget.contents.length;
    _itemKeys = List.generate(_itemCount, (index) => GlobalKey());
    _focusNode.addListener(_onFocusChanged);
  }

  @override
  void dispose() {
    // TODO: implement dispose
    super.dispose();
    _scrollController.dispose();
    _focusNode.dispose();
  }

  void _onFocusChanged() async {
    setState(() {
      hasFocus = _focusNode.hasFocus;
    });

    if (hasFocus) {
      print('get focus and update backdrop');
      widget.onFocused?.call();
    } else {
      print('Focus lost and update backdrop to empty widget');
    }
  }

  KeyEventResult _onKeyEvent(FocusNode focusNode, KeyEvent event) {
    if (event is KeyDownEvent || event is KeyRepeatEvent) {
      if (event.logicalKey == LogicalKeyboardKey.arrowLeft &&
          selectedIndex > 0) {
        setState(() {
          selectedIndex = (selectedIndex - 1).clamp(0, _itemCount - 1);
        });
        _scrollToSelected(event is KeyRepeatEvent ? 1 : 100);
        return KeyEventResult.handled;
      } else if (event.logicalKey == LogicalKeyboardKey.arrowRight &&
          selectedIndex < _itemCount - 1) {
        setState(() {
          selectedIndex = (selectedIndex + 1).clamp(0, _itemCount - 1);
        });
        _scrollToSelected(event is KeyRepeatEvent ? 1 : 100);
        return KeyEventResult.handled;
      }
    }
    return KeyEventResult.ignored;
  }

  Future<void> _scrollToSelected(int durationMilliseconds) async {
    if (_itemKeys[selectedIndex].currentContext != null) {
      int current = selectedIndex;
      final RenderBox box = _itemKeys[selectedIndex]
          .currentContext!
          .findRenderObject() as RenderBox;
      final Offset position = box.localToGlobal(Offset.zero);
      await _scrollController.animateTo(
        position.dx + _scrollController.offset - leftPadding,
        duration: Duration(milliseconds: durationMilliseconds),
        curve: Curves.easeInOut,
      );
      await Future.delayed(Duration(milliseconds: 300));
      if (current == selectedIndex) {
        // Provider.of<BackdropProvider>(context, listen: false)
        //     .updateBackdrop(getSelectedBackdrop());
      }
    } else {
      print("Item $selectedIndex is not in the widget tree");
    }
  }

  @override
  Widget build(BuildContext context) {
    return Focus(
        focusNode: _focusNode,
        onKeyEvent: _onKeyEvent,
      child: Column(
        children: [
          AnimatedOpacity(
            duration: const Duration(milliseconds: 100),
            opacity: hasFocus ? 0.0 : 1.0,
            child: Container(
              alignment: Alignment.topLeft,
              padding: EdgeInsets.only(left: leftPadding, top: 10, bottom: 10),
              child: const Text('Top picks for you',
                  textAlign: TextAlign.left,
                  style: TextStyle(
                    fontSize: 16,
                    color: Colors.grey,
                  )),
            ),
          ),
          SizedBox(
            height: 115,
            child: ScrollConfiguration(
                behavior: ScrollBehavior().copyWith(scrollbars: false),
                child: ListView.builder(
                  padding: EdgeInsets.only(left: leftPadding, right: leftPadding),
                  clipBehavior: Clip.none,
                  controller: _scrollController,
                  scrollDirection: Axis.horizontal,
                  itemCount: _itemCount,
                  itemBuilder: (context, index) {
                    return AnimatedScale(
                        scale: (hasFocus && index == selectedIndex) ? 1.1 : 1.0,
                        duration: const Duration(milliseconds: 200),
                        child: Card(
                          margin: EdgeInsets.only(left: 10, right: 10),
                          key: _itemKeys[index],
                          shape: hasFocus && index == selectedIndex
                              ? RoundedRectangleBorder(
                                  side:
                                      BorderSide(color: Colors.white, width: 2.0),
                                  borderRadius: BorderRadius.circular(10),
                                )
                              : null,
                          child: Container(
                            decoration: BoxDecoration(
                              borderRadius: BorderRadius.circular(10),
                              color: Colors.blue,
                            ),
                            width: 190,
                            child:
                                Center(child: Text(widget.contents[index].title)),
                          ),
                        ));
                  },
                )),
          ),
        ],
      ),
    );
  }
}

class ImmersiveContent {
  final String title;
  final String description;
  final String subtitle;
  final String backdrop;
  final String contentPath;

  ImmersiveContent({
    required this.title,
    required this.description,
    required this.subtitle,
    required this.backdrop,
    required this.contentPath,
  });

  factory ImmersiveContent.fromJson(Map<String, dynamic> json) {
    return ImmersiveContent(
      title: json['title'] as String,
      description: json['description'] as String,
      subtitle: json['subtitle'] as String,
      backdrop: json['backdrop'] as String,
      contentPath: json['contentPath'] as String,
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'title': title,
      'description': description,
      'subtitle': subtitle,
      'backdrop': backdrop,
      'contentPath': contentPath,
    };
  }

  @override
  String toString() {
    return 'MediaContent(title: $title, description: $description, subtitle: $subtitle, backdrop: $backdrop, contentPath: $contentPath)';
  }

  static List<ImmersiveContent> generateMockContent() {
    return List.generate(
      20,
      (index) => ImmersiveContent(
        title: 'Power Sisters $index',
        description:
            'A dynmaic duo of superhero siblings join forces to save their city from a sinister vailain, redefining sisterhood in action. $index',
        subtitle: 'Subtitle $index',
        backdrop: 'assets/mock/images/backdrop${(index % 3) + 1}.png',
        contentPath: 'ContentPath $index',
      ),
    );
  }
}
