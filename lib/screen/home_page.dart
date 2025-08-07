
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:provider/provider.dart';
import 'package:tizen_fs/models/app_list.dart';
import 'package:tizen_fs/styles/app_style.dart';
import 'package:tizen_fs/utils/noscroll_focustraversal_policy.dart';
import 'package:tizen_fs/widgets/app_list.dart';
import 'package:tizen_fs/widgets/immersive_carousel.dart';
import 'package:tizen_fs/models/immersive_carosel_model.dart';

class HomePage extends StatefulWidget {
  const HomePage({super.key, required this.scrollController});

  final ScrollController scrollController;
  @override
  State<HomePage> createState() => _HomePageState();
}

class _HomePageState extends State<HomePage> {
  final ImmersiveCarouselModel _immersiveCarouselModel =
      ImmersiveCarouselModel.fromMock();
  final AppInfoModel _appInfoModel =
      AppInfoModel.fromMock();      

  final List<GlobalKey> _keys= List.generate(3, (index) => GlobalKey());

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
        ChangeNotifierProvider(
          create: (context) => _appInfoModel,
        )
      ],
      child: FocusTraversalGroup(
        policy: NoScrollFocusTraversalPolicy(),
        child: Column(
          children: [
            ImmersiveArea(
              key: _keys[0],
              onFocusChanged: (hasFocus) {
                if(hasFocus) {
                  widget.scrollController.animateTo(
                    0,
                    duration: $style.times.fast,
                    curve: Curves.easeInOut
                  );
                }
              },
            ),
            AppList(
              key: _keys[1],
              scrollController: widget.scrollController,
              onFocused: () {
                // TODO: issue, scroll stops when it starts while previous scrolling works.
                // widget.scrollController.animateTo(
                //   430,
                //   duration: $style.times.fast,
                //   curve: Curves.easeInOut
                // );
                var itemContext = _keys[1].currentContext;
                if(itemContext != null) {
                  Scrollable.ensureVisible(
                    itemContext,
                    alignment: 0,
                    duration: $style.times.fast,
                    curve: Curves.easeInOut
                  );
                }
              }
            ),
          ],
        ),
      ),
    );
  }
}

class ImmersiveArea extends StatefulWidget {
  const ImmersiveArea({super.key, this.onFocused, this.onFocusChanged});

  final VoidCallback? onFocused;
  final Function(bool)? onFocusChanged;

  @override
  State<ImmersiveArea> createState() => _ImmersiveAreaState();
}

class _ImmersiveAreaState extends State<ImmersiveArea> {
  final _carouselKey = GlobalKey<ImmersiveCarouselState>();
  final _focusNode = FocusNode();

  bool _isExpanded = false;
  bool _isfocused = false;

  @override
  void initState() {
    super.initState();
  }

  @override
  void dispose() {
    _focusNode.dispose();
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
        }
        else {
          if ((_focusNode.parent != null) && (!_focusNode.parent!.hasFocus)) {
            setState(() {          
              _isExpanded = false;
              _isfocused = false;
            });
          } else {
            setState(() {
              _isExpanded = true;     
              _isfocused = false;
            });
          }
            
          debugPrint("@@@@@ unfocused : _focusNode.parent?.hasFocus=${_focusNode.parent?.hasFocus}");
          debugPrint("@@@@@ unfocused : _focusNode.parent?.parent?.hasFocus=${_focusNode.parent?.parent?.hasFocus}");
        }
        widget.onFocusChanged?.call(hasFocus);
      },
      child: Builder(
        builder: (context) {
          return AnimatedContainer(
            duration: $style.times.fast,
            curve: Curves.easeInOut,
            height: _isExpanded ? 350 : 280,
            child: ImmersiveCarousel(
                key: _carouselKey,
                isExpanded: _isExpanded,
                isFocused: _isfocused,
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
        return KeyEventResult.handled;
      }
      return KeyEventResult.ignored;
    } 
    return KeyEventResult.ignored;
  }
}