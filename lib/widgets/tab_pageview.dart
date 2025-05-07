import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:tizen_fs/widgets/immersive_list.dart';
import 'tab_bar.dart';

class TvPageView extends StatefulWidget {
  const TvPageView({
    super.key,
    required this.scrollController,
  });

  final ScrollController scrollController;

  @override
  State<TvPageView> createState() => _TvPageViewState();
}

class _TvPageViewState extends State<TvPageView> {
  late PageController _pageViewController;
  int _pageCount = 0;
  int _currentPageIndex = 0;

  @override
  void initState() {
    super.initState();

    _pageCount = context.read<PageModel>().pageCount;
    _currentPageIndex = context.read<PageModel>().currentIndex;

    debugPrint('initState _currentPageIndex=$_currentPageIndex');
    _pageViewController = PageController(
      initialPage: _currentPageIndex,
    );
  }

  @override
  void dispose() {
    super.dispose();
    _pageViewController.dispose();
  }

  @override
  void didUpdateWidget(covariant TvPageView oldWidget) {
    debugPrint('_TvPageViewState.didUpdateWidget()');
    super.didUpdateWidget(oldWidget);

    if (_currentPageIndex != context.read<PageModel>().currentIndex) {
      _movePage(context.read<PageModel>().currentIndex);
    }
  }

  @override
  Widget build(BuildContext context) {
    debugPrint('_TvPageViewState.build() pageIndex=${_pageCount}, currentIndex=$_currentPageIndex');

    //TODO : page builder
    int index = 0;
    return PageView(
      controller: _pageViewController,
      physics: const NeverScrollableScrollPhysics(),
      children: [
        AnimatedPage(
          isVisible: _currentPageIndex == index++,
          child: HomeContent(scrollController: widget.scrollController)
        ),
        AnimatedPage(
          isVisible: _currentPageIndex == index++,
          child: ColoredPage(scrollController: widget.scrollController)
        ),
        AnimatedPage(
          isVisible: _currentPageIndex == index++,
          child: EmptyPage()
        ),
      ]
    );
  }

  void _movePage(int currentPageIndex) {
    debugPrint('_TvPageViewState._movePage: $currentPageIndex');
    if (_currentPageIndex != currentPageIndex) {
      _pageViewController.animateToPage(
        currentPageIndex,
        duration: const Duration(milliseconds: 300),
        curve: Curves.easeInOut,
      );
      _currentPageIndex = currentPageIndex;
    }
  }
}

class AnimatedPage extends StatelessWidget {
  const AnimatedPage({super.key, required this.child, required this.isVisible});

  final Widget child;
  final bool isVisible;

  @override
  Widget build(BuildContext context) {
    return AnimatedOpacity(
      opacity: isVisible ? 1.0 : 0.0,
      duration: const Duration(milliseconds: 300),
      child: this.child
    );
  }
}


//----------------------------------------------------------------------- sample pages
class ColoredPage extends StatefulWidget {
  const ColoredPage({super.key, required this.scrollController});

  final ScrollController scrollController;

  @override
  State<ColoredPage> createState() => _ColoredPageState();
}

class _ColoredPageState extends State<ColoredPage> {

  @override
  Widget build(BuildContext context) {
    return Container(
      child: SingleChildScrollView (
        controller: widget.scrollController,
        child: Column(
          children: [
            MockItem(),
            MockItem(),
            MockItem(),
            MockItem(),
            MockItem(),
            MockItem(),
            MockItem(),
            MockItem(),
            MockItem(),
            MockItem(),
            MockItem(),
            MockItem(),
            MockItem(),
          ]
        )
      )
    );
  }
}

class EmptyPage extends StatelessWidget {
  const EmptyPage({super.key});

  @override
  Widget build(BuildContext context) {
    return Container(
        child: Text(
          'Empty Page',
          style: const TextStyle(color: Colors.white, fontSize: 24)
        )
      );
  }
}

class HomeContent extends StatefulWidget {
  const HomeContent({super.key, required this.scrollController});

  final ScrollController scrollController;

  @override
  State<HomeContent> createState() => _HomeContentState();
}

class _HomeContentState extends State<HomeContent> {
  late ScrollController _scrollController;
  final ImmersiveListModel _immersiveListModel = ImmersiveListModel.fromMock();
  final ImmersiveAreaController _immersiveAreaController =
      ImmersiveAreaController();

  @override
  void initState() {
    super.initState();
    _scrollController = widget.scrollController;
  }
  
  @override
  Widget build(BuildContext context) {
    return ChangeNotifierProvider(
      create: (context) => _immersiveListModel,
      child: SingleChildScrollView(
        controller: _scrollController,
        child: Column(
          children: [
            ImmersiveArea(_immersiveAreaController, onFocused: () {
              print('ImmersiveArea focused');
              _scrollController.animateTo(
                0,
                duration: const Duration(milliseconds: 100),
                curve: Curves.easeIn,
              );
              _immersiveAreaController
                  .setState(ImmersiveAreaController.carouselFocused);
              },
            ),
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
      ),
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

  @override
  Widget build(BuildContext context) {
    return Focus(
      focusNode: _focusNode,
      child: Builder(builder: (context) {
        return AnimatedContainer(
          duration: const Duration(milliseconds: 100),
          height: expand ? 344 : 210,
          child: PageView(
            physics: const NeverScrollableScrollPhysics(),
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

