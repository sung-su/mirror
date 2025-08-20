
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:provider/provider.dart';
import 'package:tizen_fs/models/app_info.dart';
import 'package:tizen_fs/providers/backdrop_provider.dart';
import 'package:tizen_fs/styles/app_style.dart';
import 'package:tizen_fs/utils/noscroll_focustraversal_policy.dart';
import 'package:tizen_fs/apps/app_list.dart';
import 'package:tizen_fs/widgets/immersive_carousel.dart';
import 'package:tizen_fs/models/immersive_carosel_model.dart';
import 'package:tizen_fs/poc/webview.dart';
import 'package:tizen_fs/widgets/home_screen_size_wrapper.dart';

class HomePage extends StatefulWidget {
  const HomePage({super.key, required this.scrollController});

  final ScrollController scrollController;
  @override
  State<HomePage> createState() => _HomePageState();
}

class _HomePageState extends State<HomePage> {
  final ImmersiveCarouselModel _immersiveCarouselModel = ImmersiveCarouselModel.fromMock();
  final GlobalKey<ImmersiveAreaState> _carouselKey = GlobalKey<ImmersiveAreaState>();
  final GlobalKey<AppListState> _applistKey = GlobalKey<AppListState>();
  final GlobalKey<FooterState> _footerKey = GlobalKey<FooterState>();

  List<AppInfo> appInfos = [];
  bool _isAnimating = false;
  bool _isScrolling = false;

  @override
  void initState() {
    super.initState();
  }

  @override
  Widget build(BuildContext context) {
    return MultiProvider(
      providers: [
        ChangeNotifierProvider( 
          create: (context) => _immersiveCarouselModel,
        ),
      ],
      child: FocusTraversalGroup(
        policy: NoScrollFocusTraversalPolicy(() => _isAnimating || _isScrolling),
        child: Column(
          children: [
            ImmersiveArea(
              key: _carouselKey,
              onFocusChanged: (hasFocus) async {
                if(hasFocus) {
                  _isAnimating = true;
                  _isScrolling = true;
                  await widget.scrollController.animateTo(
                    0,
                    duration: $style.times.med,
                    curve: Curves.easeOutSine
                  );
                  _isScrolling = false;
                  _footerKey.currentState?.show();
                }
              },
              onExpanded: () {
                _isAnimating = false;
              }
            ),
            AppList(
              key: _applistKey,
              scrollController: widget.scrollController,
              onFocusChanged: (hasFocus) async {
                if (hasFocus) {
                  _carouselKey.currentState?.stopAutoScroll();
                  WidgetsBinding.instance.addPostFrameCallback((_) async {
                    var context = _applistKey.currentContext;
                    if (context != null) {
                      _isScrolling = true;
                      await Scrollable.ensureVisible(
                        context,
                        alignment: 0,
                        duration: $style.times.med,
                        curve: Curves.easeInOut
                      );
                      _isScrolling = false;
                      _footerKey.currentState?.hide();
                    }
                  });
                } else {
                  _carouselKey.currentState?.restartAutoScroll();
                }
              },
              onScrollup: () async {
                await widget.scrollController.animateTo(
                  0,
                  duration: $style.times.med,
                  curve: Curves.easeInOut
                );
                _carouselKey.currentState?.initFocus();
              },
            ),
            Footer(key: _footerKey)
          ],
        ),
      ),
    );
  }
}

class Footer extends StatefulWidget {
  const Footer({super.key});

  @override
  State<Footer> createState() => FooterState();
}

class FooterState extends State<Footer> {

  bool _isVisible = true;

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
        child: Container(
          child: Icon(
            Icons.keyboard_arrow_down,
            size: 30,
          ),
        ),
      ),
    );
  }
}

class ImmersiveArea extends StatefulWidget {
  const ImmersiveArea({super.key, this.onFocused, this.onFocusChanged, this.onExpanded});

  final VoidCallback? onFocused;
  final Function(bool)? onFocusChanged;
  final VoidCallback? onExpanded;

  @override
  State<ImmersiveArea> createState() => ImmersiveAreaState();
}

class ImmersiveAreaState extends State<ImmersiveArea> with SingleTickerProviderStateMixin{
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
      CurvedAnimation(
        parent: _animationController,
        curve: Curves.easeInOut,
      ),
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
        if(hasFocus) {
          setState(() {          
            _isExpanded = true;
            _isfocused = true;
          });
          _animationController.forward();
          restartAutoScroll();
        }
        else {
          if ((_focusNode.parent != null) && (!_focusNode.parent!.hasFocus)) {
            setState(() {          
              _isExpanded = false;
              _isfocused = false;
            });
            _animationController.reverse();
          } else {
            setState(() {
              _isExpanded = true;     
              _isfocused = false;
            });
            _animationController.forward();
            stopAutoScroll();
          }
        }
        widget.onFocusChanged?.call(hasFocus);
        if(_animationController.value == 1) {
          widget.onExpanded?.call();
        }
      },
      child: AnimatedBuilder(
        animation: _heightAnimation,
        builder: (context, child) {
          return SizedBox(
            height: _heightAnimation.value,
            child: ImmersiveCarousel(
              key: _carouselKey,
              isExpanded: _isExpanded,
              isFocused: _isfocused,
              onTap: () {
                if (!_focusNode.hasFocus) {
                  _focusNode.requestFocus();
                }
              }
            ),
          );
        }
      ),
    );
  }

  KeyEventResult _onKeyEvent(FocusNode node, KeyEvent event) {
    if (event is KeyDownEvent) {
      if (event.logicalKey == LogicalKeyboardKey.arrowRight) {
          _carouselKey.currentState?.moveCarousel(1);

        return KeyEventResult.handled;
      } else if (event.logicalKey == LogicalKeyboardKey.arrowLeft) {
          _carouselKey.currentState?.moveCarousel(-1);

        return KeyEventResult.handled;
      } else if (event.logicalKey == LogicalKeyboardKey.enter || event.logicalKey == LogicalKeyboardKey.select) {
        // widget.onItemSelected?.call(_carouselKey.currentState?.selectedIndex ?? 0);
        Navigator.push(
          context,
          MaterialPageRoute(builder: (_) => HomeScreenSizeWrapper(WebViewExample())),
        );
        return KeyEventResult.handled;
      }
      return KeyEventResult.ignored;
    } 
    return KeyEventResult.ignored;
  }
}