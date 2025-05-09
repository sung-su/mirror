import 'dart:async';
import 'dart:convert';

import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:provider/provider.dart';
import 'package:tizen_fs/widgets/carousel_content_block.dart';
import 'package:tizen_fs/providers/backdrop_provider.dart';

class ImmersiveCarouselContent {
  final String overline;
  final String title;
  final String description;
  final String backdrop;
  final String buttonText;

  ImmersiveCarouselContent({
    required this.overline,
    required this.title,
    required this.description,
    required this.backdrop,
    required this.buttonText,
  });

  factory ImmersiveCarouselContent.fromJson(Map<String, dynamic> json) {
    return ImmersiveCarouselContent(
      overline: json['overline'] as String,
      title: json['title'] as String,
      description: (json['description'] as String).length > 100
          ? '${json['description'].substring(0, 100)}...'
          : json['description'] as String,
      backdrop: json['backdrop'] as String,
      buttonText: json['buttonText'] as String,
    );
  }

  static Future<List<ImmersiveCarouselContent>> loadFromJson() async {
    final jsonString =
        await rootBundle.loadString('assets/mock/mock_carousel_content.json');
    final List<dynamic> jsonList = jsonDecode(jsonString);
    final List<ImmersiveCarouselContent> contents =
        jsonList.map((json) => ImmersiveCarouselContent.fromJson(json)).toList();

    return contents;
  }

  static List<ImmersiveCarouselContent> generateMockContent() {
    return List.generate(
      5,
      (index) => ImmersiveCarouselContent(
        overline: 'Overline $index',
        title: 'Movie Title $index',
        description: 'Description of the movie goes here.\n This is a sample description for item $index.',
        backdrop: 'assets/mock/images/backdrop${(index % 3) + 1}.png',
        buttonText: 'Watch Now',
      ),
    );
  }
}


class ImmersiveCarouselModel extends ChangeNotifier {
  late List<ImmersiveCarouselContent> contents;
  bool _isLoading = false;
  int _selectedIndex = 0;

  ImmersiveCarouselModel(this.contents);

  ImmersiveCarouselModel.fromMock() {
    _isLoading = true;
    ImmersiveCarouselContent.loadFromJson().then((value) {
      _isLoading = false;
      contents = value;
      notifyListeners();
    });
  }

  int get selectedIndex => _selectedIndex;
  set selectedIndex(int index) {
    _selectedIndex = index;
    notifyListeners();
  }

  int get itemCount => _isLoading ? 0 : contents.length;
  ImmersiveCarouselContent getContent(int index) {
    if (_isLoading) {
      return ImmersiveCarouselContent(
        overline: 'Loading...',
        title: 'Loading...',
        description: 'Loading...',
        backdrop: 'Loading...',
        buttonText: 'Loading...',
      );
    }
    return contents[index];
  }

  ImmersiveCarouselContent getSelectedContent() {
    if (_isLoading) {
      return ImmersiveCarouselContent(
        overline: 'Loading...',
        title: 'Loading...',
        description: 'Loading...',
        backdrop: 'backdrop1.png',
        buttonText: 'Loading...',
      );
    }
    return contents[_selectedIndex];
  }
}

class ImmersiveCarousel extends StatefulWidget {
  final bool isExpanded;

  const ImmersiveCarousel({
    super.key,
    this.isExpanded = false,
  });

  @override
  State<ImmersiveCarousel> createState() => ImmersiveCarouselState();
}

class ImmersiveCarouselState extends State<ImmersiveCarousel> {
  int _selectedIndex = 0;
  int _itemCount = 0;
  bool isFocused = false;
  Timer? autoScrollTimer;

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
        _selectedIndex = nextIndex;
        Provider.of<ImmersiveCarouselModel>(context, listen: false).selectedIndex = _selectedIndex;
      } );

      Provider.of<BackdropProvider>(context, listen: false).updateBackdrop(getSelectedBackdrop());
    });
  }

  void resetAutoScroll() {
    autoScrollTimer?.cancel();
    startAutoScroll();
  }

  void moveCarousel(int delta) {
    if (_itemCount == 0) return;
    final nextIndex = (_selectedIndex + delta + _itemCount) % _itemCount;
    setState(()  {
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
            image: Image.asset('assets/mock/images/${Provider.of<ImmersiveCarouselModel>(context, listen: false).getSelectedContent().backdrop}',
                width: MediaQuery.of(context).size.width, height: MediaQuery.of(context).size.height, fit: BoxFit.fill),
          ),
        ));
  }

  @override
  Widget build(BuildContext context) {
    if (_itemCount <= 0) {
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
                    final inOffset = const Offset(0.7, 0.0);
                    final outOffset = const Offset(1.0, 0.0);
                    return SlideTransition(
                      position: Tween<Offset>(
                        begin: child.key == ValueKey(Provider.of<ImmersiveCarouselModel>(context, listen: false).getSelectedContent().title)
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
                    key: ValueKey(Provider.of<ImmersiveCarouselModel>(context, listen: false).getSelectedContent().title),
                    child: ContentBlock(
                      item: Provider.of<ImmersiveCarouselModel>(context, listen: false).getSelectedContent(),
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
