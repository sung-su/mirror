import 'dart:convert';
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:palette_generator/palette_generator.dart';
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

enum ColumnCount { one, two, three, four, six }

class MediaList extends StatefulWidget {
  final String title;
  final ColumnCount columns;
  final VoidCallback? onFocused;
  final List<ImmersiveContent> contents;
  const MediaList(
      {super.key,
      required this.contents,
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

  bool _hasFocus = false;
  int _itemCount = 0;
  int _selectedIndex = 0;
  double _peek = 58;
  double _itemWidth = 196;
  double _itemHeight = 124;
  Color _extractColor = Colors.white;
  bool _circle = false;

  void calculateItemSize() {
    if (widget.columns == ColumnCount.six) {
      _itemWidth = 124;
      _circle = true;
    } else if (widget.columns == ColumnCount.three) {
      _itemWidth = 268;
    } else if (widget.columns == ColumnCount.two) {
      _itemWidth = 412;
    } else if (widget.columns == ColumnCount.one) {
      _itemWidth = 844;
    } else {
      _itemWidth = 196;
    }
  }

  @override
  void initState() {
    super.initState();
    calculateItemSize();
    _focusNode.addListener(_onFocusChanged);
    _itemCount = widget.contents.length;
    _itemKeys = List.generate(_itemCount, (index) => GlobalKey());
    _selectedIndex = 0;
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
    _scrollToSelected(100);

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
        _scrollToSelected(event is KeyRepeatEvent ? 1 : 100);
        return KeyEventResult.handled;
      } else if (event.logicalKey == LogicalKeyboardKey.arrowRight &&
          _selectedIndex < _itemCount - 1) {
        setState(() {
          _selectedIndex = (_selectedIndex + 1).clamp(0, _itemCount - 1);
        });
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
        position.dx + _scrollController.offset - _peek,
        duration: Duration(milliseconds: durationMilliseconds),
        curve: Curves.easeInOut,
      );

      var imagePath =
          'assets/mock/images/${widget.contents[_selectedIndex].card}';
      var paletteGenerator = await PaletteGenerator.fromImageProvider(
        AssetImage(imagePath),
        size: Size(100, 100),
        maximumColorCount: 1,
      );
      _extractColor = paletteGenerator.dominantColor?.color ?? Colors.white;

      await Future.delayed(Duration(milliseconds: 300));
      if (current == _selectedIndex && _hasFocus) {
        Provider.of<BackdropProvider>(context, listen: false)
            .updateBackdrop(getSelectedBackdrop());
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
        SizedBox(
          height: _hasFocus ? 80 : 50,
          child: AnimatedScale(
              scale: _hasFocus ? 2.0 : 1.0,
              duration: const Duration(milliseconds: 100),
              alignment: Alignment.topLeft,
              child: Container(
                alignment: Alignment.topLeft,
                padding: EdgeInsets.only(
                  left: _hasFocus ? 30 : 64,
                  top: _hasFocus ? 5 : 10,
                ),
                child: Text(widget.title,
                    textAlign: TextAlign.left,
                    style: TextStyle(
                      fontSize: 18,
                      color: _hasFocus
                          ? Colors.white.withAlpha((255 * 0.8).toInt())
                          : Colors.grey,
                    )),
              )),
        ),
        SizedBox(
          height: _itemHeight + 64,
          child: ScrollConfiguration(
            behavior:
                ScrollBehavior().copyWith(scrollbars: false, overscroll: false),
            child: AnimatedOpacity(
              opacity: _hasFocus ? 1.0 : 0.5,
              duration: const Duration(milliseconds: 100),
              child: ListView.builder(
                padding: EdgeInsets.only(left: _peek, right: _peek),
                clipBehavior: Clip.none,
                controller: _scrollController,
                scrollDirection: Axis.horizontal,
                itemCount: _itemCount,
                itemBuilder: (context, index) {
                  return Container(
                    margin: EdgeInsets.only(left: 10, right: 10, bottom: 10),
                    child: Column(
                      children: [
                        AnimatedScale(
                            scale: (_hasFocus && index == _selectedIndex)
                                ? 1.15
                                : 1.0,
                            duration: const Duration(milliseconds: 100),
                            child: Card(
                              margin: EdgeInsets.only(left: 10, right: 10),
                              key: _itemKeys[index],
                              shape: (_hasFocus && index == _selectedIndex)
                                  ? (_circle
                                      //TODO : blinking border
                                      ? CircleBorder(
                                          side: BorderSide(
                                              color: Colors.white.withAlpha(
                                                  (255 * 0.8).toInt()),
                                              width: 2.0),
                                        )
                                      : RoundedRectangleBorder(
                                          side: BorderSide(
                                              color: Colors.white.withAlpha(
                                                  (255 * 0.8).toInt()),
                                              width: 2.0),
                                          borderRadius:
                                              BorderRadius.circular(10),
                                        ))
                                  : null,
                              child: Container(
                                decoration: BoxDecoration(
                                  borderRadius: _circle
                                      ? BorderRadius.circular(50)
                                      : BorderRadius.circular(10),
                                  boxShadow: (_hasFocus &&
                                          index == _selectedIndex)
                                      ? [
                                          BoxShadow(
                                            color: _extractColor
                                                .withAlpha((255 * 0.8).toInt()),
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
                                child: _circle
                                    ? ClipOval(
                                        child: Image.asset(
                                          'assets/mock/images/${widget.contents[index].card}',
                                          fit: BoxFit.fill,
                                        ),
                                      )
                                    : ClipRRect(
                                        borderRadius: BorderRadius.circular(10),
                                        child: Image.asset(
                                          'assets/mock/images/${widget.contents[index].card}',
                                          fit: BoxFit.fill,
                                        ),
                                      ),
                              ),
                            )),
                        SizedBox(
                          width: _itemWidth,
                          height: _itemHeight,
                          child: Column(
                            children: [
                              if (_hasFocus && index == _selectedIndex ||
                                  _circle)
                                Container(
                                  padding: EdgeInsets.only(top: 10),
                                  alignment: _circle
                                      ? Alignment.center
                                      : Alignment.topLeft,
                                  child: Text(widget.contents[index].title,
                                      style: TextStyle(
                                        color: Colors.white
                                            .withAlpha((255 * 0.8).toInt()),
                                        fontSize: 16,
                                      )),
                                ),
                              if (_hasFocus && !_circle)
                                Container(
                                  alignment: Alignment.topLeft,
                                  child: Text(widget.contents[index].subtitle,
                                      overflow: TextOverflow.ellipsis,
                                      maxLines: 1,
                                      style: TextStyle(
                                        color: Colors.white
                                            .withAlpha((255 * 0.5).toInt()),
                                        fontSize: 16,
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
