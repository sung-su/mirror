import 'dart:convert';
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:provider/provider.dart';
import 'package:tizen_fs/providers/backdrop_provider.dart';
import 'package:tizen_fs/widgets/backdrop_scaffold.dart';
import 'package:tizen_fs/widgets/immersive_list.dart';

class MediaContent {
  final String backdrop;
  final String card;
  final String title;
  final String subtitle;
  final String description;

  MediaContent({
    required this.backdrop,
    required this.card,
    required this.title,
    required this.subtitle,
    required this.description,
  });

  factory MediaContent.fromJson(Map<String, dynamic> json) {
    return MediaContent(
      backdrop: json['backdrop'] as String,
      card: json['card'] as String,
      title: json['title'] as String,
      subtitle: json['metadata'] as String,
      description: (json['description'] as String).length > 100
          ? '${json['description'].substring(0, 100)}...'
          : json['description'] as String,
    );
  }

  static Future<List<MediaContent>> loadFromJson() async {
    final jsonString =
        await rootBundle.loadString('assets/mock/mock_content.json');
    final List<dynamic> jsonList = jsonDecode(jsonString);
    final List<MediaContent> contents =
        jsonList.map((json) => MediaContent.fromJson(json)).toList();

    return contents;
  }

  static List<MediaContent> generateMockContent() {
    return List.generate(
      10,
      (index) => MediaContent(
        backdrop: 'backdrop${(index % 3) + 1}.png',
        card: 'ContentPath $index',
        title: 'Title $index',
        subtitle: 'Subtitle $index',
        description: '',
      ),
    );
  }
}

enum ColumnCount { one, two, three, four, six, nine }

class MediaList extends StatefulWidget {
  final String title;
  final ColumnCount columns;
  final VoidCallback? onFocused;
 
  const MediaList(
      {super.key,
      this.onFocused,
      this.title = 'Title',
      this.columns = ColumnCount.four});

  @override
  State<MediaList> createState() => _MediaListState();
}

class _MediaListState extends State<MediaList> {
  final ScrollController _scrollController = ScrollController();
  final FocusNode _focusNode = FocusNode();
  late List<GlobalKey> _itemKeys;
  List<ImmersiveContent> contents = [];

  bool _hasFocus = false;
  int _itemCount = 0;
  int _selectedIndex = 0;
  double _peekPadding = 58;
  double _itemWidth = 196;
  double _itemHeight = 110;
  Color _extractColor = Colors.white;
  bool _isCircleShape = false;
  double _titleFontSize = 18;
  double _subTitleFontSize = 16;

  void calculateItemSize() {
    if (widget.columns == ColumnCount.nine) {
      _itemWidth = 80;
      _itemHeight = 80;
      _isCircleShape = true;
    } else if (widget.columns == ColumnCount.six) {
      _itemWidth = 124;
      _itemHeight = 124;
      _isCircleShape = true;
    } else if (widget.columns == ColumnCount.three) {
      _itemWidth = 268;
      _itemHeight = 150;
    } else if (widget.columns == ColumnCount.two) {
      _itemWidth = 412;
      _itemHeight = 230;
    } else if (widget.columns == ColumnCount.one) {
      _itemWidth = 844;
      _itemHeight = 470;
    } else {
      _itemWidth = 196;
      _itemHeight = 110;
    }
    _itemWidth *= 0.95;
    _itemHeight *= 0.95;
  }

  @override
  void initState() {
    super.initState();
    calculateItemSize();
    _focusNode.addListener(_onFocusChanged);

    if (Provider.of<ImmersiveListModel>(context, listen: false).itemCount == 0) {
      Provider.of<ImmersiveListModel>(context, listen: false).addListener(_handleModelUpdate);
    } else {
      contents = Provider.of<ImmersiveListModel>(context, listen: false).contents;
      _itemCount = contents.length;
      _itemKeys = List.generate(_itemCount, (index) => GlobalKey());
    }
    _selectedIndex = 0;
  }

  void _handleModelUpdate() {
    setState(() {
      contents = Provider.of<ImmersiveListModel>(context, listen: false).contents;
      _itemCount = contents.length;
      _itemKeys = List.generate(_itemCount, (index) => GlobalKey());
    });
    Provider.of<ImmersiveListModel>(context, listen: false).removeListener(_handleModelUpdate);
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
    _scrollToSelected(100, true);

    if (_hasFocus) {
      widget.onFocused?.call();
      Provider.of<BackdropProvider>(context, listen: false)
          .updateBackdrop(getSelectedBackdrop());
    } else {
      Provider.of<BackdropProvider>(context, listen: false)
          .updateBackdrop(null);
    }
  }

  KeyEventResult _onKeyEvent(FocusNode focusNode, KeyEvent event) {
    if (event is KeyDownEvent || event is KeyRepeatEvent) {
      if (event.logicalKey == LogicalKeyboardKey.arrowLeft &&
          _selectedIndex > 0) {
        setState(() {
          _selectedIndex = (_selectedIndex - 1).clamp(0, _itemCount - 1);
        });
        _scrollToSelected(event is KeyRepeatEvent ? 1 : 100,
            event is KeyRepeatEvent ? false : true);
        return KeyEventResult.handled;
      } else if (event.logicalKey == LogicalKeyboardKey.arrowRight &&
          _selectedIndex < _itemCount - 1) {
        setState(() {
          _selectedIndex = (_selectedIndex + 1).clamp(0, _itemCount - 1);
        });
        _scrollToSelected(event is KeyRepeatEvent ? 1 : 100,
            event is KeyRepeatEvent ? false : true);
        return KeyEventResult.handled;
      }
    }
    return KeyEventResult.ignored;
  }

  Future<void> _scrollToSelected(
      int durationMilliseconds, bool backdrop) async {
    if (_itemKeys[_selectedIndex].currentContext != null) {
      int current = _selectedIndex;
      final RenderBox box = _itemKeys[_selectedIndex]
          .currentContext!
          .findRenderObject() as RenderBox;
      final Offset position = box.localToGlobal(Offset.zero);

      await _scrollController.animateTo(
        position.dx + _scrollController.offset - _peekPadding,
        duration: Duration(milliseconds: backdrop ? durationMilliseconds : 1),
        curve: Curves.easeInOut,
      );

      if (backdrop) {
        await Future.delayed(Duration(milliseconds: 300));
        if (current == _selectedIndex && _hasFocus) {
          Provider.of<BackdropProvider>(context, listen: false)
              .updateBackdrop(getSelectedBackdrop());
        }
      }
    } else {
      print("Item $_selectedIndex is not in the widget tree");
    }
  }

  Widget getSelectedBackdrop() {
    return Container(
      key: ValueKey(_selectedIndex),
      child: Container(
        decoration: BoxDecoration(
          gradient: LinearGradient(
            begin: Alignment.center,
            end: Alignment.centerRight,
            colors: [
              Colors.black.withAlpha((0.1 * 255).toInt()),
              _extractColor.withAlpha((0.2 * 255).toInt()),
            ],
            stops: const [0, 1],
          ),
        ),
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    return Focus(
      focusNode: _focusNode,
      onKeyEvent: _onKeyEvent,
      child: Column(mainAxisSize: MainAxisSize.min, children: [
        //list title
        SizedBox(
          height: _hasFocus ? 70 : 35,
          child: AnimatedScale(
              scale: _hasFocus ? 1.7 : 1.0,
              duration: const Duration(milliseconds: 100),
              alignment: Alignment.topLeft,
              child: Container(
                alignment: Alignment.topLeft,
                padding: EdgeInsets.only(
                  left: _hasFocus ? 35 : 70,
                  top: 10,
                ),
                child: Text(widget.title,
                    textAlign: TextAlign.left,
                    style: TextStyle(
                      fontSize: _titleFontSize,
                      color: _hasFocus
                          ? Colors.white.withAlpha((255 * 0.7).toInt())
                          : Colors.grey,
                    )),
              )),
        ),
        //list
        SizedBox(
          height: _hasFocus ? _itemHeight * 1.7 : _itemHeight * 1.3,
          child: ScrollConfiguration(
            behavior:
                ScrollBehavior().copyWith(scrollbars: false, overscroll: false),
            child: AnimatedOpacity(
              opacity: _hasFocus ? 1.0 : 0.5,
              duration: const Duration(milliseconds: 100),
              child: ListView.builder(
                //peek space
                padding:
                    EdgeInsets.only(left: _peekPadding, right: _peekPadding),
                clipBehavior: Clip.none,
                controller: _scrollController,
                scrollDirection: Axis.horizontal,
                itemCount: _itemCount,
                itemBuilder: (context, index) {
                  return Container(
                    //between items, image-label space
                    margin: EdgeInsets.all(5),
                    child: Column(
                      children: [
                        //scale image area
                        AnimatedScale(
                            scale: (_hasFocus && index == _selectedIndex)
                                ? _isCircleShape
                                    ? 1.15
                                    : 1.1
                                : 1.0,
                            duration: const Duration(milliseconds: 100),
                            //card with border
                            child: Card(
                              color: Colors.transparent,
                              shadowColor: Colors.transparent,
                              key: _itemKeys[index],
                              shape: (_hasFocus && index == _selectedIndex)
                                  ? (_isCircleShape
                                      ? CircleBorder(
                                          side: BorderSide(
                                              color: Colors.white.withAlpha(
                                                  (255 * 0.7).toInt()),
                                              width: 2.0),
                                        )
                                      : RoundedRectangleBorder(
                                          side: BorderSide(
                                              color: Colors.white.withAlpha(
                                                  (255 * 0.7).toInt()),
                                              width: 2.0),
                                          borderRadius:
                                              BorderRadius.circular(10),
                                        ))
                                  : null,
                              //glow shadow layer
                              child: Container(
                                decoration: BoxDecoration(
                                  borderRadius: _isCircleShape
                                      ? BorderRadius.circular(50)
                                      : BorderRadius.circular(10),
                                  boxShadow: (_hasFocus &&
                                          index == _selectedIndex)
                                      ? [
                                          BoxShadow(
                                            color: _extractColor
                                                .withAlpha((255 * 0.7).toInt()),
                                            spreadRadius: 1,
                                            blurRadius: 20,
                                            blurStyle: BlurStyle.normal,
                                            offset: Offset(0, 1),
                                          ),
                                        ]
                                      : null,
                                ),
                                width: _itemWidth,
                                height: _itemHeight,
                                //image layer
                                child: _isCircleShape
                                    ? ClipOval(
                                        child: Image.asset(
                                          'assets/mock/images/${contents[index].card}',
                                          fit: BoxFit.fill,
                                        ),
                                      )
                                    : ClipRRect(
                                        borderRadius: BorderRadius.circular(10),
                                        child: Image.asset(
                                          'assets/mock/images/${contents[index].card}',
                                          fit: BoxFit.fill,
                                        ),
                                      ),
                              ),
                            )),
                        //labels
                        SizedBox(
                          width: _itemWidth,
                          child: Column(
                            children: [
                              //title
                              if (_hasFocus && index == _selectedIndex ||
                                  _hasFocus && _isCircleShape)
                                Container(
                                  padding: EdgeInsets.only(top: 5),
                                  alignment: _isCircleShape
                                      ? Alignment.center
                                      : Alignment.topLeft,
                                  child: Text(contents[index].title,
                                      overflow: TextOverflow.ellipsis,
                                      maxLines: 1,
                                      style: TextStyle(
                                        color: Colors.white
                                            .withAlpha((255 * 0.7).toInt()),
                                        fontSize: _subTitleFontSize,
                                      )),
                                ),
                              //subtitle
                              if (_hasFocus && !_isCircleShape)
                                Container(
                                  alignment: Alignment.topLeft,
                                  padding: EdgeInsets.only(
                                      top: index == _selectedIndex ? 0 : 5),
                                  child: Text(widget.contents[index].subtitle,
                                      overflow: TextOverflow.ellipsis,
                                      maxLines: 1,
                                      style: TextStyle(
                                        color: Colors.white
                                            .withAlpha((255 * 0.5).toInt()),
                                        fontSize: _subTitleFontSize,
                                      )),
                                ),
                            ],
                          ),
                        ),
                      ],
                    ),
                  );
                },
              ),
            ),
          ),
        ),
      ]),
    );
  }
}
