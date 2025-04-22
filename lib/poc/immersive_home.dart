import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:tizen_fs/services/backdrop_provider.dart';
import 'package:tizen_fs/widgets/immersive_list.dart';


class ImmersiveHomeSizeWrapper extends StatelessWidget {
  const ImmersiveHomeSizeWrapper({super.key});

  @override
  Widget build(BuildContext context) {
    var screenWidth = MediaQuery.of(context).size.width;
    return (screenWidth != 960) ? FractionallySizedBox(
      widthFactor: 960 / screenWidth,
      heightFactor: 960 / screenWidth,
      child: Transform.scale(
          scale: screenWidth / 960,
          child: const ImmersiveHome(),
        ),
    ) : const ImmersiveHome();
  }
}

class ImmersiveHome extends StatelessWidget {
  const ImmersiveHome({super.key});

  @override
  Widget build(BuildContext context) {
    return ChangeNotifierProvider(
      create: (context) => BackdropProvider(),
      child: Builder(builder: (context) {
        var backdrop = Provider.of<BackdropProvider>(context).backdrop;
        return Scaffold(
            body: Stack(children: [
          // Background
          SizedBox.expand(
              child: DecoratedBox(
                  decoration: BoxDecoration(color: const Color(0xff1a110f)))),
          // Backdrop
          AnimatedSwitcher(
            duration: const Duration(milliseconds: 200),
            transitionBuilder: (child, animation) => ScaleTransition(
                scale: Tween<double>(begin: 0.95, end: 1.1).animate(animation),
                child: FadeTransition(opacity: animation, child: child)),
            child: (backdrop != null) ? backdrop : SizedBox.shrink(),
          ),

          // Main content
          MainContent()
        ]));
      }),
    );
  }
}

class MainContent extends StatelessWidget {
  MainContent({super.key});
  final ScrollController _scrollController = ScrollController();

  @override
  Widget build(BuildContext context) {
    return SingleChildScrollView(
      controller: _scrollController,
      scrollDirection: Axis.vertical,
      child: Column(
        mainAxisAlignment: MainAxisAlignment.start,
        crossAxisAlignment: CrossAxisAlignment.start,
        children: <Widget>[
          MockFocusableTabbar(),
          MockCarousel(() {
            print('Carousel focused');
             _scrollController.animateTo(
               0,
               duration: const Duration(milliseconds: 100),
               curve: Curves.easeIn,
             );
          }),

          ImmersiveList(
            ImmersiveContent.generateMockContent(),
            onFocused: () {
              print('ImmersiveList focused');
              _scrollController.animateTo(
                280,
                duration: const Duration(milliseconds: 100),
                curve: Curves.easeIn,
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
          MockItem(),
          MockItem(),
          MockItem(),
          MockItem(),
          SizedBox(
            height: 300,
          ),
        ],
      ),
    );
  }
}

class MockItem extends StatelessWidget {
  const MockItem({
    super.key,
  });

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.fromLTRB(55, 10, 55, 10),
      child: Focus(child: Builder(builder: (buildContext) {
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

class MockCarousel extends StatefulWidget {
  const MockCarousel(
    this.onFocused, {
    super.key,
  });

  final VoidCallback onFocused;

  @override
  State<MockCarousel> createState() => _MockCarouselState();
}

class _MockCarouselState extends State<MockCarousel> {
  final FocusNode focusNode = FocusNode();

  @override
  void initState() {
    super.initState();
    focusNode.addListener(_focusChanged);
  }

  void _focusChanged() {
    if (focusNode.hasFocus) {
      widget.onFocused.call();
    }
  }

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.only(left: 58, right: 58),
      child: Focus(
          focusNode: focusNode,
          child: Builder(builder: (buildContext) {
            return AnimatedContainer(
                duration: const Duration(milliseconds: 100),
                height: Focus.of(buildContext).hasFocus ? 344 : 200,
                decoration: BoxDecoration(
                  color: Focus.of(buildContext).hasFocus
                      ? Colors.blue.withAlpha(100)
                      : Colors.white.withAlpha(100),
                ),
                child: Center(child: const Text('CarouselView')));
          })),
    );
  }
}
