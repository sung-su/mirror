import 'dart:async';

import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:tizen_fs/models/immersive_carosel_model.dart';
import 'package:tizen_fs/styles/app_style.dart';
import 'package:tizen_fs/widgets/carousel_content_block.dart';
import 'package:tizen_fs/providers/backdrop_provider.dart';

class ImmersiveCarousel extends StatefulWidget {
  final bool isExpanded;
  final bool isFocused;
  final VoidCallback? onTap;

  const ImmersiveCarousel({
    super.key,
    this.isExpanded = false,
    this.isFocused = false,
    this.onTap
  });

  @override
  State<ImmersiveCarousel> createState() => ImmersiveCarouselState();
}

class ImmersiveCarouselState extends State<ImmersiveCarousel> {
  int _selectedIndex = 0;
  int _prevIndex = -1;
  int _itemCount = 0;
  bool isFocused = false;
  Timer? autoScrollTimer;

  int get selectedIndex => _selectedIndex;

  @override
  void didChangeDependencies() {
    super.didChangeDependencies();
    var model = Provider.of<ImmersiveCarouselModel>(context);
    _itemCount = model.itemCount;
    _selectedIndex = model.selectedIndex;
  }

  @override
  void initState() {
    super.initState();

    WidgetsBinding.instance.addPostFrameCallback((_) {
      Provider.of<BackdropProvider>(context, listen: false).updateBackdrop(getSelectedBackdrop());
    });

    startAutoScroll();
  }

  void startAutoScroll() {
    autoScrollTimer?.cancel();
    autoScrollTimer = Timer.periodic(const Duration(seconds: 10), (timer) {
      if (_itemCount == 0) return;
      final nextIndex = (_selectedIndex + 1) % _itemCount;
      setState(() {
        _prevIndex = _selectedIndex;
        _selectedIndex = nextIndex;
        Provider.of<ImmersiveCarouselModel>(context, listen: false).selectedIndex = _selectedIndex;
      } );

      Provider.of<BackdropProvider>(context, listen: false).updateBackdrop(getSelectedBackdrop());
    });
  }

  void updateBackdrop() {
    Provider.of<BackdropProvider>(context, listen: false).updateBackdrop(getSelectedBackdrop());
  }

  void resetAutoScroll() {
    autoScrollTimer?.cancel();
    startAutoScroll();
  }

  void stopAutoScroll() {
    autoScrollTimer?.cancel();
  }

  void moveCarousel(int delta) {
    if (_itemCount == 0) return;
    final nextIndex = (_selectedIndex + delta + _itemCount) % _itemCount;
    setState(()  {
      _prevIndex = _selectedIndex;
      _selectedIndex = nextIndex;
      Provider.of<ImmersiveCarouselModel>(context, listen: false).selectedIndex = _selectedIndex;
    });
    Provider.of<BackdropProvider>(context, listen: false).updateBackdrop(getSelectedBackdrop());
    resetAutoScroll();
  }

  @override
  void dispose() {
    autoScrollTimer?.cancel();
    super.dispose();
  }

  Widget getSelectedBackdrop() {
    return Container(
        key: ValueKey(_selectedIndex),
        alignment: Alignment.topRight,
        child: SizedBox(
          width: MediaQuery.of(context).size.width,
          height: MediaQuery.of(context).size.height,
          child: CinematicScrim(
            image: Image.asset('assets/images/${Provider.of<ImmersiveCarouselModel>(context, listen: false).getSelectedContent().backdrop}',
            width: 1920,
            height: 1080,
                cacheWidth: (MediaQuery.of(context).size.width * 2).round(),
                // width: MediaQuery.of(context).size.width,
                // height: MediaQuery.of(context).size.height,
                fit: BoxFit.cover),
          ),
        ));
  }

  bool isMovingForward(int prev, int next, int itemCount) {
    final forwardDistance = (next - prev + itemCount) % itemCount;
    final backwardDistance = (prev - next + itemCount) % itemCount;
    return forwardDistance <= backwardDistance;
  }

  @override
  Widget build(BuildContext context) {
    if (_itemCount <= 0) {
      return const SizedBox.shrink();
    }

    final double leftOffset = 58;
    final double rightOffset = 58;
    final double bottomOffset = 40.0;

    return Stack(
        children: [
          Align(
            alignment: Alignment.bottomLeft,
            child: Padding(
              padding: EdgeInsets.only(left: leftOffset, bottom: bottomOffset),
              child: AnimatedSwitcher(
                duration: const Duration(milliseconds: 200),
                transitionBuilder: (Widget child, Animation<double> animation) {
                  bool isForward = isMovingForward(_prevIndex, _selectedIndex, _itemCount);
                  final incoming = child.key == ValueKey(Provider.of<ImmersiveCarouselModel>(context, listen: false).getSelectedContent().title);
                  final inTween = Tween<Offset>(
                    begin: isForward ? const Offset(0.7, 0.0) : const Offset(-0.7, 0.0),
                    end: Offset.zero
                  );
                  final outTween = Tween<Offset>(
                    begin: Offset.zero,
                    end: isForward ? const Offset(-0.7, 0.0) : const Offset(0.7, 0.0)
                  );
                  final curved = CurvedAnimation(
                    parent: incoming ? animation : ReverseAnimation(animation),
                    curve: Curves.easeInOut,
                  );
                  if (incoming) {
                    return SlideTransition(
                    position: inTween.animate(curved),
                    child: FadeTransition(
                      opacity: animation,
                      child: child,
                    ),
                  );
                  } else {
                    return SlideTransition(
                    position: outTween.animate(curved),
                    child: FadeTransition(
                        opacity: Tween<double>(begin: 1.0, end: 0.0).animate(curved),
                        child: child,
                      ),
                  );
                  }
                },
                layoutBuilder: (Widget? currentChild, List<Widget> previousChildren) {
                  return Stack(
                    children: <Widget>[
                      ...previousChildren,
                      if (currentChild != null) currentChild,
                    ]);
                },
                child: KeyedSubtree(
                  key: ValueKey(Provider.of<ImmersiveCarouselModel>(context, listen: false).getSelectedContent().title),
                  child: ContentBlock(
                    item: Provider.of<ImmersiveCarouselModel>(context, listen: false).getSelectedContent(),
                    isFocused: widget.isFocused,
                    isExpanded: widget.isExpanded,
                    onTap: widget.onTap
                  )
                )
              ),
            )
          ),
          Positioned(
            bottom: bottomOffset,
            right: rightOffset,
            child: IgnorePointer(
              child: Row(
              children: List.generate(_itemCount, (index) {
                return Container(
                    margin: const EdgeInsets.symmetric(horizontal: 4),
                    width: 8,
                    height: 8,
                    decoration: BoxDecoration(
                      shape: BoxShape.circle,
                      color: _selectedIndex == index ? Colors.white : Colors.grey,
                    ),
                  );
              }),
                                ),
            )
          ),
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
