import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:tizen_fs/services/backdrop_provider.dart';
import 'package:provider/provider.dart';

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

class ImmersiveList extends StatefulWidget {
  const ImmersiveList(this.contents, {super.key, this.onFocused});
  final VoidCallback? onFocused;
  final List<ImmersiveContent> contents;

  @override
  State<ImmersiveList> createState() => _ImmersiveListState();
}

class _ImmersiveListState extends State<ImmersiveList> {
  static const int _itemCount = 20;
  final GlobalKey _targetKey = GlobalKey();
  final FocusNode _focusNode = FocusNode();
  final ScrollController _scrollController = ScrollController();
  final List<GlobalKey> _itemKeys =
      List.generate(_itemCount, (index) => GlobalKey());

  bool hasFocus = false;
  int selectedIndex = 0;
  double leftPadding = 58;

  @override
  void initState() {
    super.initState();
    _focusNode.addListener(_onFocusChanged);
    hasFocus = _focusNode.hasFocus;
  }

  @override
  void dispose() {
    _focusNode.removeListener(_onFocusChanged);
    _focusNode.dispose();
    super.dispose();
  }

  void _onFocusChanged() async {
    
    setState(() {
      hasFocus = _focusNode.hasFocus;
    });

    if (hasFocus) {
      print('get focus and update backdrop');
      widget.onFocused?.call();
      Provider.of<BackdropProvider>(context, listen: false)
          .updateBackdrop(getSelectedBackdrop());
    } else {
      print('Focus lost and update backdrop to empty widget');
      Provider.of<BackdropProvider>(context, listen: false).updateBackdrop(Container(color: Color(0xff1a110f)));
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
        Provider.of<BackdropProvider>(context, listen: false)
            .updateBackdrop(getSelectedBackdrop());
      }
    } else {
      print("Item $selectedIndex is not in the widget tree");
    }
  }

  Widget getSelectedBackdrop() {
    return Container(
        key: ValueKey(selectedIndex),
        alignment: Alignment.topRight,
        child: SizedBox(
          width: 758,
          height: 426,
          child: CinematicScrim(
            image: Image.asset(widget.contents[selectedIndex].backdrop,
                width: 758, height: 426, fit: BoxFit.fill),
          ),
        ));
  }

  @override
  Widget build(BuildContext context) {
    if (hasFocus) {}
    return Focus(
      key: _targetKey,
      focusNode: _focusNode,
      onKeyEvent: _onKeyEvent,
      child: AnimatedContainer(
          height: hasFocus ? 620 : 230,
          duration: const Duration(milliseconds: 150),
          child:
              Column(crossAxisAlignment: CrossAxisAlignment.start, children: [
            hasFocus
                ? Center(
                    child: Padding(
                        padding: EdgeInsets.only(top: 10),
                        child: Icon(Icons.keyboard_arrow_up,
                            size: 35, color: Colors.grey)))
                : SizedBox.shrink(),
            Padding(
              padding: EdgeInsets.only(left: leftPadding, top: 10, bottom: 10),
              child: AnimatedDefaultTextStyle(
                style: TextStyle(
                  fontSize: hasFocus ? 30 : 20,
                  color: Colors.grey,
                ),
                duration: const Duration(milliseconds: 150),
                child: const Text('Top picks for you',
                    textAlign: TextAlign.left),
              ),
            ),
            hasFocus ? Expanded(child: SizedBox.shrink()) : SizedBox.shrink(),
            hasFocus
                ? Padding(
                    padding: EdgeInsets.only(left: leftPadding),
                    child: SizedBox(
                        width: 600,
                        child: Column(
                          crossAxisAlignment: CrossAxisAlignment.start,
                          mainAxisAlignment: MainAxisAlignment.start,
                          children: [
                            AnimatedSwitcher(
                              duration: const Duration(milliseconds: 300),
                              transitionBuilder: (Widget child,
                                  Animation<double> animation) {
                                return FadeTransition(
                                  opacity: animation,
                                  child: child,
                                );
                              },
                              child: Text(
                                widget.contents[selectedIndex].title,
                                key: ValueKey(selectedIndex),
                                style: TextStyle(
                                  fontSize: 50,
                                  fontWeight: FontWeight.bold,
                                  color: Colors.white,
                                ),
                              ),
                            ),
                            SizedBox(height: 10),
                            AnimatedSwitcher(
                              duration: const Duration(milliseconds: 300),
                              transitionBuilder: (Widget child,
                                  Animation<double> animation) {
                                return FadeTransition(
                                  opacity: animation,
                                  child: child,
                                );
                              },
                              child: Text(
                                widget.contents[selectedIndex].subtitle,
                                key: ValueKey(selectedIndex),
                                style:
                                    TextStyle(fontSize: 25, color: Colors.grey),
                              ),
                            ),
                            SizedBox(height: 5),
                            AnimatedSwitcher(
                              duration: const Duration(milliseconds: 300),
                              transitionBuilder: (Widget child,
                                  Animation<double> animation) {
                                return FadeTransition(
                                  opacity: animation,
                                  child: child,
                                );
                              },
                              child: Text(
                                widget.contents[selectedIndex].description,
                                key: ValueKey(selectedIndex),
                                style:
                                    TextStyle(fontSize: 20, color: Colors.grey),
                                softWrap: true,
                              ),
                            ),
                          ],
                        )),
                  )
                : SizedBox.shrink(),
            hasFocus ? SizedBox(height: 20) : SizedBox.shrink(),
            SizedBox(
              height: 150,
              child: ScrollConfiguration(
                  behavior: ScrollBehavior().copyWith(scrollbars: false),
                  child: ListView.builder(
                    padding:
                        EdgeInsets.only(left: leftPadding, right: leftPadding),
                    clipBehavior: Clip.none,
                    controller: _scrollController,
                    scrollDirection: Axis.horizontal,
                    itemCount: _itemCount,
                    itemBuilder: (context, index) {
                      return AnimatedScale(
                          scale:
                              (hasFocus && index == selectedIndex) ? 1.1 : 1.0,
                          duration: const Duration(milliseconds: 200),
                          child: Card(
                            margin: EdgeInsets.only(left: 10, right: 10),
                            key: _itemKeys[index],
                            shape: hasFocus && index == selectedIndex
                                ? RoundedRectangleBorder(
                                    side: BorderSide(
                                        color: Colors.white, width: 2.0),
                                    borderRadius: BorderRadius.circular(10),
                                  )
                                : null,
                            child: Container(
                              decoration: BoxDecoration(
                                borderRadius: BorderRadius.circular(10),
                                color: Colors.blue,
                              ),
                              width: 250,
                              //height: 200,
                              child: Center(child: Text(widget.contents[index].title)),
                            ),
                          ));
                    },
                  )),
            ),
            SizedBox(height: 30),
          ])),
    );
  }
}
