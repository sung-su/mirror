import 'dart:async';
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:tizen_fs/widgets/carousel_content_block.dart';
import 'package:tizen_fs/providers/backdrop_provider.dart';
import 'package:provider/provider.dart';

class ImmersiveCarouselContent {
  final String overline;
  final String title;
  final String description;
  final String image;
  final String buttonText;

  ImmersiveCarouselContent({
    required this.overline,
    required this.title,
    required this.description,
    required this.image,
    required this.buttonText,
  });

  static List<ImmersiveCarouselContent> generateMockContent() {
    return List.generate(
      5,
      (index) => ImmersiveCarouselContent(
        overline: 'Overline $index',
        title: 'Movie Title $index',
        description: 'Description of the movie goes here. This is a sample description for item $index.',
        image: 'assets/mock/images/backdrop${(index % 3) + 1}.png',
        buttonText: 'Watch Now',
      ),
    );
  }
}

class CinematicScrim extends StatelessWidget {
  const CinematicScrim({super.key, required this.image});

  final Widget image;

  @override
  Widget build(BuildContext context) {
    return Stack(
      children: [
        image,
        Container(
          decoration: BoxDecoration(
            gradient: RadialGradient(
              center: Alignment(0.8, -0.8),
              radius: 2,
              colors: [
                Color(0xff1a110f).withAlpha((0.1 * 255).toInt()),
                Color(0xff1a110f),
              ],
              stops: const [0, 0.7],
            ),
          ),
        ),
      ],
    );
  }
}

class ImmersiveCarousel extends StatefulWidget {
  final List<ImmersiveCarouselContent> items;
  final bool isExpanded;

  const ImmersiveCarousel({
    super.key,
    required this.items,
    this.isExpanded = false,
  });

  @override
  State<ImmersiveCarousel> createState() => ImmersiveCarouselState();
}

class ImmersiveCarouselState extends State<ImmersiveCarousel> with TickerProviderStateMixin {
  int currentIndex = 0;
  bool isFocused = false;
  Timer? autoScrollTimer;

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
      if (widget.items.isNotEmpty) {
        final next = (currentIndex + 1) % widget.items.length;
        setState(() => currentIndex = next );
      }

      Provider.of<BackdropProvider>(context, listen: false).updateBackdrop(getSelectedBackdrop());
    });
  }

  void resetAutoScroll() {
    autoScrollTimer?.cancel();
    startAutoScroll();
  }

  void moveCarousel(int delta) {
    if (widget.items.isEmpty) {
      return;
    }
    final nextIndex = (currentIndex + delta + widget.items.length) % widget.items.length;
    setState(() => currentIndex = nextIndex);
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
        key: ValueKey(currentIndex),
        alignment: Alignment.topRight,
        child: SizedBox(
          width: MediaQuery.of(context).size.width,
          height: MediaQuery.of(context).size.height,
          child: CinematicScrim(
            image: Image.asset(widget.items[currentIndex].image,
                width: MediaQuery.of(context).size.width, height: MediaQuery.of(context).size.height, fit: BoxFit.fill),
          ),
        ));
  }

  @override
  Widget build(BuildContext context) {
    if (widget.items.isEmpty) {
      return const SizedBox.shrink();
    }

    final double leftOffset = 58;
    final double rightOffset = 58;
    final double bottomOffset = 24.0;

    return Stack(
        children: [
          // Content Block
          Align(
            alignment: Alignment.bottomLeft,
            child: Padding(
            padding: EdgeInsets.only(left: leftOffset, bottom: bottomOffset),
              child: AnimatedSwitcher(
                  duration: const Duration(milliseconds: 200),
                  transitionBuilder: (Widget child, Animation<double> animation) {
                    final inOffset = const Offset(0.5, 0.0);
                    final outOffset = const Offset(1.0, 0.0);
                    return SlideTransition(
                      position: Tween<Offset>(
                        begin: child.key == ValueKey(widget.items[currentIndex].title)
                        ? inOffset : outOffset,
                        end: Offset.zero,
                      ).animate(animation),
                      child: FadeTransition(opacity: animation, child: child),
                    );
                  },
                  layoutBuilder: (Widget? currentChild, List<Widget> previousChildren) {
                    return Stack(
                      children: <Widget>[
                        ...previousChildren,
                        if (currentChild != null) currentChild,
                      ]);
                  },
                  child: KeyedSubtree(
                    key: ValueKey(widget.items[currentIndex].title),
                    child: ContentBlock(
                      item: widget.items[currentIndex],
                      isFocused: widget.isExpanded,
                    ),
                  )
                ),
          )
          ),
              // Pagination
              Positioned(
                bottom: bottomOffset,
                right: rightOffset,
                child: IgnorePointer(
                  child: Row(
                  children: List.generate(widget.items.length, (index) {
                    return Container(
                        margin: const EdgeInsets.symmetric(horizontal: 4),
                        width: 8,
                        height: 8,
                        decoration: BoxDecoration(
                          shape: BoxShape.circle,
                          color: currentIndex == index ? Colors.white : Colors.grey,
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

