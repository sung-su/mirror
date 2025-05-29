import 'dart:async';
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:provider/provider.dart';
import 'package:tizen_fs/models/category.dart';
import 'package:tizen_fs/models/movie.dart';
import 'package:tizen_fs/screens/detail_page.dart';
import 'package:tizen_fs/widgets/category_list.dart';
import 'package:tizen_fs/widgets/immersive_list.dart';
import 'package:tizen_fs/widgets/immersive_carousel.dart';
import 'footer.dart';

class HomePage extends StatefulWidget {
  const HomePage(
      {super.key, required this.scrollController, required this.categories});

  final ScrollController scrollController;
  final List<Category> categories;

  @override
  State<HomePage> createState() => _HomePageState();
}

class _HomePageState extends State<HomePage> {
  late ScrollController _scrollController;
  final ImmersiveListModel _immersiveListModel = ImmersiveListModel.fromMock();
  final ImmersiveCarouselModel _immersiveCarouselModel =
      ImmersiveCarouselModel.fromMock();
  final ImmersiveAreaController _immersiveAreaController =
      ImmersiveAreaController();
  late List<Movie> movies;

  @override
  void initState() {
    super.initState();

    if (_immersiveListModel.itemCount == 0) {
      _immersiveListModel.addListener(_handleModelUpdate);
    }
    _scrollController = widget.scrollController;
  }

  void _handleModelUpdate() {
    _immersiveListModel.removeListener(_handleModelUpdate);
    setState(() {});
  }

  @override
  Widget build(BuildContext context) {
    final movies = context.watch<MovieViewModel>().movies;

    return MultiProvider(
        providers: [
          ChangeNotifierProvider(
            create: (context) => _immersiveListModel,
          ),
          ChangeNotifierProvider(
            create: (context) => _immersiveCarouselModel,
          )
        ],
        child: Column(
          children: [
            ImmersiveArea(
              _immersiveAreaController,
              onFocused: () {
                print('ImmersiveArea focused');
                _scrollController.animateTo(
                  0,
                  duration: const Duration(milliseconds: 100),
                  curve: Curves.easeIn,
                );
                _immersiveAreaController
                    .setState(ImmersiveAreaController.carouselFocused);
              },
              onUnFocused: () {
                print('Header focused');
                _scrollController.animateTo(
                  0,
                  duration: const Duration(milliseconds: 100),
                  curve: Curves.easeIn,
                );
                _immersiveAreaController
                    .setState(ImmersiveAreaController.headerFocused);
              },
            ),
            ImmersiveListArea(
              movies: movies,
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
              onExecute: (index) {
                print('item $index executed');
                Navigator.push(
                    context,
                    MaterialPageRoute(
                      builder: (context) => DetailPage(movie: movies[index%4], categories: widget.categories),
                    ));
              },
            ),
            SizedBox(height: 20),
            ...List.generate(
              widget.categories.length,
              (index) => CategoryList(
                title: widget.categories[index].name == 'Launcher'
                    ? 'Your apps'
                    : widget.categories[index].name,
                tiles: widget.categories[index].tiles,
                columns: widget.categories[index].name == 'Launcher'
                    ? ColumnCount.nine
                    : ColumnCount.four,
                onFocused: () {
                  print(
                      'item $index focused - Category: ${widget.categories[index].name}');
                  _scrollController.animateTo(
                    index == 0
                        ? 570
                        : index == 1
                            ? 570 + 140
                            : 570 + 140 + ((index - 1) * 168),
                    duration: const Duration(milliseconds: 100),
                    curve: Curves.easeInQuad,
                  );
                },
              ),
            ),
            CategoryList(
                tiles: widget.categories[3].tiles,
                columns: ColumnCount.three,
                title: 'Recomended videos',
                icon: 'assets/mock/images/icons8-youtube-144.png',
                timeStamp: true,
                onFocused: () {
                  debugPrint('Recomended videos focused');
                  _scrollController.animateTo(
                    570 + 140 + ((widget.categories.length - 1) * 168),
                    duration: const Duration(milliseconds: 100),
                    curve: Curves.easeInQuad,
                  );
                }),
            CategoryList(
                tiles: widget.categories[4].tiles,
                columns: ColumnCount.three,
                title: 'Recently uploaded',
                icon: 'assets/mock/images/icons8-youtube-144.png',
                timeStamp: true,
                onFocused: () {
                  debugPrint('Recently uploaded focused');
                  _scrollController.animateTo(
                    570 + 140 + ((widget.categories.length - 1) * 168) + 233,
                    duration: const Duration(milliseconds: 100),
                    curve: Curves.easeInQuad,
                  );
                }),
            Footer(
              scrollController: _scrollController,
            )
          ],
        ));
  }
}

class ImmersiveAreaController {
  static const int headerFocused = 0;
  static const int carouselFocused = 1;
  static const int immersiveListFocused = 2;
  static const int mediaListFocused = 3;

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
  final VoidCallback? onUnFocused;
  final ImmersiveAreaController? controller;

  const ImmersiveArea(
    this.controller, {
    super.key,
    this.onFocused,
    this.onUnFocused,
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
      } else if (state == ImmersiveAreaController.mediaListFocused) {
        // focused on list area
        setState(() {
          expand = false;
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
    } else {
      widget.onUnFocused?.call();
    }
  }

  Timer? _initialDelayTimer;
  Timer? _repeatingTimer;
  void _startRepeating(VoidCallback action) {
    action();
    _initialDelayTimer?.cancel();
    _initialDelayTimer = Timer(const Duration(milliseconds: 300), () {
      _repeatingTimer =
          Timer.periodic(const Duration(milliseconds: 100), (timer) {
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
          return KeyEventResult.ignored;
        } else if (event is KeyUpEvent) {
          _stopRepeating();
          return KeyEventResult.handled;
        }
        return KeyEventResult.ignored;
      },
      focusNode: _focusNode,
      child: Builder(builder: (context) {
        return AnimatedContainer(
          duration: const Duration(milliseconds: 100),
          height: expand ? 370 : 210,
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
