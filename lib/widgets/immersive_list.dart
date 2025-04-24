import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:provider/provider.dart';
import 'package:tizen_fs/services/backdrop_provider.dart';

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

class ImmersiveListModel extends ChangeNotifier {
  final List<ImmersiveContent> contents;
  int _selectedIndex = 0;

  ImmersiveListModel(this.contents);
  ImmersiveListModel.fromMock() : this(ImmersiveContent.generateMockContent());

  int get selectedIndex => _selectedIndex;
  set selectedIndex(int index) {
    _selectedIndex = index;
    notifyListeners();
  }

  int get itemCount => contents.length;
  ImmersiveContent getContent(int index) {
    return contents[index];
  }

  ImmersiveContent getSelectedContent() {
    return contents[_selectedIndex];
  }
}

class ImmersiveContentArea extends StatelessWidget {
  const ImmersiveContentArea({
    super.key,
  });
  static const double leftPadding = 58;
  @override
  Widget build(BuildContext context) {
    return Column(crossAxisAlignment: CrossAxisAlignment.start, children: [
      Center(
          child: Padding(
              padding: EdgeInsets.only(top: 10),
              child:
                  Icon(Icons.keyboard_arrow_up, size: 35, color: Colors.grey))),
      Padding(
          padding: EdgeInsets.only(left: leftPadding, top: 10, bottom: 10),
          child: const Text(
            'Top picks for you',
            textAlign: TextAlign.left,
            style: TextStyle(fontSize: 24, color: Colors.grey),
          )),
      Expanded(child: SizedBox.shrink()),
      Padding(
        padding: EdgeInsets.only(left: leftPadding),
        child: SizedBox(
            width: 480,
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              mainAxisAlignment: MainAxisAlignment.start,
              children: [
                AnimatedSwitcher(
                  duration: const Duration(milliseconds: 300),
                  transitionBuilder:
                      (Widget child, Animation<double> animation) {
                    return FadeTransition(
                      opacity: animation,
                      child: child,
                    );
                  },
                  child: Text(
                    Provider.of<ImmersiveListModel>(context)
                        .getSelectedContent()
                        .title,
                    key: ValueKey(
                        Provider.of<ImmersiveListModel>(context).selectedIndex),
                    style: TextStyle(
                      fontSize: 40,
                      color: Colors.white,
                    ),
                  ),
                ),
                SizedBox(height: 8),
                AnimatedSwitcher(
                  duration: const Duration(milliseconds: 300),
                  transitionBuilder:
                      (Widget child, Animation<double> animation) {
                    return FadeTransition(
                      opacity: animation,
                      child: child,
                    );
                  },
                  child: Text(
                    Provider.of<ImmersiveListModel>(context)
                        .getSelectedContent()
                        .subtitle,
                    key: ValueKey(
                        Provider.of<ImmersiveListModel>(context).selectedIndex),
                    style: TextStyle(fontSize: 18, color: Colors.grey),
                  ),
                ),
                SizedBox(height: 5),
                AnimatedSwitcher(
                  duration: const Duration(milliseconds: 300),
                  transitionBuilder:
                      (Widget child, Animation<double> animation) {
                    return FadeTransition(
                      opacity: animation,
                      child: child,
                    );
                  },
                  child: Text(
                    Provider.of<ImmersiveListModel>(context)
                        .getSelectedContent()
                        .description,
                    key: ValueKey(
                        Provider.of<ImmersiveListModel>(context).selectedIndex),
                    style: TextStyle(fontSize: 16, color: Colors.grey),
                    softWrap: true,
                  ),
                ),
              ],
            )),
      )
    ]);
  }
}

class ImmersiveListArea extends StatefulWidget {
  final VoidCallback? onFocused;

  const ImmersiveListArea({super.key, this.onFocused});

  @override
  State<ImmersiveListArea> createState() => _ImmersiveListAreaState();
}

class _ImmersiveListAreaState extends State<ImmersiveListArea> {
  final ScrollController _scrollController = ScrollController();
  final FocusNode _focusNode = FocusNode();
  late List<GlobalKey> _itemKeys;

  bool _hasFocus = false;
  static const double _leftPadding = 58;
  int _itemCount = 0;
  int _selectedIndex = 0;

  @override
  void didChangeDependencies() {
    super.didChangeDependencies();
    var model = Provider.of<ImmersiveListModel>(context);
    _itemCount = model.itemCount;
    _selectedIndex = model.selectedIndex;
    _itemKeys = List.generate(_itemCount, (index) => GlobalKey());
  }

  @override
  void initState() {
    super.initState();
    _focusNode.addListener(_onFocusChanged);
  }

  @override
  void dispose() {
    _scrollController.dispose();
    _focusNode.removeListener(_onFocusChanged);
    _focusNode.dispose();
    super.dispose();
  }

  void _onFocusChanged() async {
    setState(() {
      _hasFocus = _focusNode.hasFocus;
    });

    if (_hasFocus) {
      widget.onFocused?.call();
      Provider.of<BackdropProvider>(context, listen: false).updateBackdrop(getSelectedBackdrop());
    } else {
      Provider.of<BackdropProvider>(context, listen: false).updateBackdrop(null);
    }
  }

  KeyEventResult _onKeyEvent(FocusNode focusNode, KeyEvent event) {
    if (event is KeyDownEvent || event is KeyRepeatEvent) {
      if (event.logicalKey == LogicalKeyboardKey.arrowLeft &&
          _selectedIndex > 0) {
        _selectedIndex = (_selectedIndex - 1).clamp(0, _itemCount - 1);
        Provider.of<ImmersiveListModel>(context, listen: false).selectedIndex =
            _selectedIndex;
        _scrollToSelected(event is KeyRepeatEvent ? 1 : 100);
        return KeyEventResult.handled;
      } else if (event.logicalKey == LogicalKeyboardKey.arrowRight &&
          _selectedIndex < _itemCount - 1) {
        _selectedIndex = (_selectedIndex + 1).clamp(0, _itemCount - 1);
        Provider.of<ImmersiveListModel>(context, listen: false).selectedIndex =
            _selectedIndex;
        _scrollToSelected(event is KeyRepeatEvent ? 1 : 100);
        return KeyEventResult.handled;
      }
    }
    return KeyEventResult.ignored;
  }

  Future<void> _scrollToSelected(int durationMilliseconds) async {
    if (_itemKeys[_selectedIndex].currentContext != null) {
      int current = _selectedIndex;
      final RenderBox box = _itemKeys[_selectedIndex]
          .currentContext!
          .findRenderObject() as RenderBox;
      final Offset position = box.localToGlobal(Offset.zero);
      await _scrollController.animateTo(
        position.dx + _scrollController.offset - _leftPadding,
        duration: Duration(milliseconds: durationMilliseconds),
        curve: Curves.easeInOut,
      );
      await Future.delayed(Duration(milliseconds: 300));
      if (current == _selectedIndex && _hasFocus) {
        Provider.of<BackdropProvider>(context, listen: false).updateBackdrop(getSelectedBackdrop());
      }
    } else {
      print("Item $_selectedIndex is not in the widget tree");
    }
  }

  Widget getSelectedBackdrop() {
    return Container(
        key: ValueKey(_selectedIndex),
        alignment: Alignment.topRight,
        child: SizedBox(
          width: 758,
          height: 426,
          child: CinematicScrim(
            image: Image.asset(
                Provider.of<ImmersiveListModel>(context, listen: false).getSelectedContent().backdrop,
                width: 758,
                height: 426,
                fit: BoxFit.fill),
          ),
        ));
  }

  @override
  Widget build(BuildContext context) {
    return Focus(
      focusNode: _focusNode,
      onKeyEvent: _onKeyEvent,
      child: Column(
        children: [
          AnimatedOpacity(
            duration: const Duration(milliseconds: 100),
            opacity: _hasFocus ? 0.0 : 1.0,
            child: Container(
              alignment: Alignment.topLeft,
              padding: EdgeInsets.only(left: _leftPadding, top: 10, bottom: 10),
              child: const Text('Top picks for you',
                  textAlign: TextAlign.left,
                  style: TextStyle(
                    fontSize: 16,
                    color: Colors.grey,
                  )),
            ),
          ),
          SizedBox(
            height: 115,
            child: ScrollConfiguration(
                behavior: ScrollBehavior()
                    .copyWith(scrollbars: false, overscroll: false),
                child: ListView.builder(
                  padding:
                      EdgeInsets.only(left: _leftPadding, right: _leftPadding),
                  clipBehavior: Clip.none,
                  controller: _scrollController,
                  scrollDirection: Axis.horizontal,
                  itemCount: _itemCount,
                  itemBuilder: (context, index) {
                    return AnimatedScale(
                        scale:
                            (_hasFocus && index == _selectedIndex) ? 1.1 : 1.0,
                        duration: const Duration(milliseconds: 200),
                        child: Card(
                          margin: EdgeInsets.only(left: 10, right: 10),
                          key: _itemKeys[index],
                          shape: _hasFocus && index == _selectedIndex
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
                            width: 190,
                            child: Center(
                                child: Text(
                                    Provider.of<ImmersiveListModel>(context)
                                        .getContent(index)
                                        .title)),
                          ),
                        ));
                  },
                )),
          ),
        ],
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
