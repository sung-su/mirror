import 'dart:async';
import 'package:flutter/material.dart';
import 'package:flutter/rendering.dart';
import 'package:flutter/services.dart';
import 'package:provider/provider.dart';
import 'package:tizen_fs/models/app_data_model.dart';
import 'package:tizen_fs/providers/backdrop_provider.dart';
import 'package:tizen_fs/styles/app_style.dart';
import 'package:tizen_fs/utils/noscroll_focustraversal_policy.dart';
import 'package:tizen_fs/apps/app_list.dart';
import 'package:tizen_fs/widgets/immersive_carousel.dart';
import 'package:tizen_fs/models/immersive_carosel_model.dart';

class HomePage extends StatefulWidget {
  const HomePage({
    super.key,
    required this.scrollController,
    required this.register,
    required this.unregister,
  });

  final ScrollController scrollController;
  final void Function(int, Function(ScrollDirection, bool)) register;
  final void Function(int) unregister;

  @override
  State<HomePage> createState() => _HomePageState();
}

class _HomePageState extends State<HomePage> {
  final ImmersiveCarouselModel _immersiveCarouselModel =
      ImmersiveCarouselModel.fromMock();
  final GlobalKey<ImmersiveAreaState> _carouselKey =
      GlobalKey<ImmersiveAreaState>();
  final GlobalKey<AppListState> _applistKey = GlobalKey<AppListState>();
  final GlobalKey<FooterState> _footerKey = GlobalKey<FooterState>();

  bool _isAnimating = false;
  bool _isScrolling = false;

  @override
  void initState() {
    super.initState();
    widget.register(0, _scrollEnd);
  }

  @override
  void dispose() {
    widget.unregister(0);
    super.dispose();
  }

  void _scrollEnd(ScrollDirection direction, bool scrollEnd) {
    final context = _applistKey.currentContext;
    if (context != null) {
      final screenHeight = MediaQuery.of(context).size.height;
      final applistBox = context.findRenderObject() as RenderBox;
      final dy = applistBox.localToGlobal(Offset.zero).dy;
      final threshold = screenHeight / 2;

      if (scrollEnd &&
          Provider.of<AppDataModel>(context, listen: false).appInfos.length >
              0) {
        if (direction == ScrollDirection.reverse) {
          if (!_footerKey.currentState!.isVisible) return;
          WidgetsBinding.instance.addPostFrameCallback((_) async {
            _isScrolling = true;
            await widget.scrollController.animateTo(
              360,
              duration: $style.times.med,
              curve: Curves.easeOutCubic,
            );
            _isScrolling = false;
            _footerKey.currentState?.hide();
            _applistKey.currentState?.setFocus();
          });
        } else {
          WidgetsBinding.instance.addPostFrameCallback((_) async {
            _isScrolling = true;
            await widget.scrollController.animateTo(
              0,
              duration: $style.times.med,
              curve: Curves.easeOutCubic,
            );
            _isScrolling = false;
            _footerKey.currentState?.show();
            _carouselKey.currentState?.initFocus();
          });
        }
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    return MultiProvider(
      providers: [
        ChangeNotifierProvider(create: (context) => _immersiveCarouselModel),
      ],
      child: FocusTraversalGroup(
        policy: NoScrollFocusTraversalPolicy(
          () => _isAnimating || _isScrolling,
        ),
        child: Column(
          children: [
            ImmersiveArea(
              key: _carouselKey,
              onFocusChanged: (hasFocus) async {
                if (hasFocus) {
                  _isAnimating = true;
                  _isScrolling = true;
                  await widget.scrollController.animateTo(
                    0,
                    duration: $style.times.med,
                    curve: Curves.easeOutCubic,
                  );
                  _isScrolling = false;
                  _footerKey.currentState?.show();
                }
              },
              onExpanded: () {
                _isAnimating = false;
              },
            ),
            if (Provider.of<AppDataModel>(
                  context,
                  listen: false,
                ).appInfos.length >
                0)
              AppList(
                key: _applistKey,
                scrollController: widget.scrollController,
                onFocusChanged: (hasFocus) async {
                  if (hasFocus) {
                    _carouselKey.currentState?.stopAutoScroll();
                    var context = _applistKey.currentContext;
                    if (context != null) {
                      _isScrolling = true;
                      await widget.scrollController.animateTo(
                        // 430,
                        360,
                        duration: $style.times.med,
                        curve: Curves.easeInOut,
                      );
                      _isScrolling = false;
                      _footerKey.currentState?.hide();
                    }
                  } else {
                    _carouselKey.currentState?.restartAutoScroll();
                  }
                },
                onScrollup: () async {
                  await widget.scrollController.animateTo(
                    0,
                    duration: $style.times.med,
                    curve: Curves.easeInOut,
                  );
                  _carouselKey.currentState?.initFocus();
                },
              ),
            if (Provider.of<AppDataModel>(
                  context,
                  listen: false,
                ).appInfos.length >
                0)
              Footer(
                key: _footerKey,
                onTap: () {
                  _applistKey.currentState?.setFocus();
                },
              ),
          ],
        ),
      ),
    );
  }
}

class Footer extends StatefulWidget {
  const Footer({super.key, this.onTap});

  final VoidCallback? onTap;

  @override
  State<Footer> createState() => FooterState();
}

class FooterState extends State<Footer> {
  bool _isVisible = true;
  bool get isVisible => _isVisible;

  void show() {
    setState(() {
      _isVisible = true;
    });
  }

  void hide() {
    setState(() {
      _isVisible = false;
    });
  }

  @override
  Widget build(BuildContext context) {
    return SizedBox(
      height: _isVisible ? 500 : 0,
      child: Align(
        alignment: Alignment.topCenter,
        child: GestureDetector(
          onTap: widget.onTap,
          child: Container(child: Icon(Icons.keyboard_arrow_down, size: 30)),
        ),
      ),
    );
  }
}

class ImmersiveArea extends StatefulWidget {
  const ImmersiveArea({
    super.key,
    this.onFocused,
    this.onFocusChanged,
    this.onExpanded,
  });

  final VoidCallback? onFocused;
  final Function(bool)? onFocusChanged;
  final VoidCallback? onExpanded;

  @override
  State<ImmersiveArea> createState() => ImmersiveAreaState();
}

class ImmersiveAreaState extends State<ImmersiveArea>
    with SingleTickerProviderStateMixin {
  final _carouselKey = GlobalKey<ImmersiveCarouselState>();
  late final AnimationController _animationController;
  late final Animation<double> _heightAnimation;
  late final FocusNode _focusNode;

  bool _isExpanded = false;
  bool _isfocused = false;

  @override
  void initState() {
    super.initState();
    _focusNode = FocusNode();
    _animationController = AnimationController(
      vsync: this,
      duration: $style.times.fast,
    );

    _heightAnimation = Tween<double>(begin: 280, end: 350).animate(
      CurvedAnimation(parent: _animationController, curve: Curves.easeInOut),
    );

    _animationController.addStatusListener((status) {
      if (status == AnimationStatus.completed ||
          status == AnimationStatus.dismissed) {
        widget.onExpanded?.call();
      }
    });
  }

  void stopAutoScroll() {
    Provider.of<BackdropProvider>(context, listen: false).updateBackdrop(null);
    _carouselKey.currentState?.stopAutoScroll();
  }

  void restartAutoScroll() {
    _carouselKey.currentState?.resetAutoScroll();
    _carouselKey.currentState?.updateBackdrop();
  }

  void initFocus() {
    _focusNode.requestFocus();
  }

  @override
  void dispose() {
    _focusNode.dispose();
    _animationController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Focus(
      focusNode: _focusNode,
      onKeyEvent: _onKeyEvent,
      onFocusChange: (hasFocus) {
        if (hasFocus) {
          setState(() {
            // _isExpanded = true;
            _isfocused = true;
          });
          _animationController.forward();
          restartAutoScroll();
        } else {
          if ((_focusNode.parent != null) && (!_focusNode.parent!.hasFocus)) {
            setState(() {
              // _isExpanded = false;
              _isfocused = false;
            });
            _animationController.reverse();
          } else {
            setState(() {
              // _isExpanded = true;
              _isfocused = false;
            });
            _animationController.forward();
            stopAutoScroll();
          }
        }
        widget.onFocusChanged?.call(hasFocus);
        if (_animationController.value == 1) {
          widget.onExpanded?.call();
        }
      },
      child: AnimatedBuilder(
        animation: _heightAnimation,
        builder: (context, child) {
          return SizedBox(
            // height: _heightAnimation.value,
            height: 280,
            child: ImmersiveCarousel(
              key: _carouselKey,
              // isExpanded: _isExpanded,
              // isFocused: _isfocused,
              isExpanded: false,
              isFocused: false,
              onTap: () {
                if (!_focusNode.hasFocus) {
                  _focusNode.requestFocus();
                }
              },
            ),
          );
        },
      ),
    );
  }

  void _stopRepeating() {
    _initialDelayTimer?.cancel();
    _repeatingTimer?.cancel();
    _initialDelayTimer = null;
    _repeatingTimer = null;
  }

  Timer? _initialDelayTimer;
  Timer? _repeatingTimer;
  void _startRepeating(VoidCallback action) {
    action();
    _initialDelayTimer?.cancel();
    _initialDelayTimer = Timer(const Duration(milliseconds: 300), () {
      _repeatingTimer = Timer.periodic(const Duration(milliseconds: 100), (
        timer,
      ) {
        action();
      });
    });
  }

  KeyEventResult _onKeyEvent(FocusNode node, KeyEvent event) {
    if (event is KeyDownEvent || event is KeyRepeatEvent) {
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
      } else if (event.logicalKey == LogicalKeyboardKey.enter ||
          event.logicalKey == LogicalKeyboardKey.select) {
        // widget.onItemSelected?.call(_carouselKey.currentState?.selectedIndex ?? 0);
        // Navigator.push(
        //   context,
        //   MaterialPageRoute(builder: (_) => HomeScreenSizeWrapper(WebViewExample())),
        // );
        return KeyEventResult.handled;
      }
      return KeyEventResult.ignored;
    } else if (event is KeyUpEvent) {
      _stopRepeating();
      return KeyEventResult.handled;
    }
    return KeyEventResult.ignored;
  }
}
