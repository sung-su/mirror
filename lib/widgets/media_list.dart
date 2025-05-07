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

enum ColumnCount { one, two, three, four, six }

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

  double _leftPadding = 58;
  bool _hasFocus = false;
  int _itemCount = 0;
  int _selectedIndex = 0;
  double _itemWidth = 196;
  double _itemHeight = 124;

  void calculateItemSize() {
    if (widget.columns == ColumnCount.six) {
      _itemWidth = 124;
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
    calculateItemSize();
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
            begin: Alignment.centerLeft,
            end: Alignment.centerRight,
            colors: [
              Color(0xff1a110f).withAlpha((0.1 * 255).toInt()),
              Color.fromARGB(255, 84, 29, 17),
            ],
            stops: const [0, 0.9],
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
      child: Column(
        children: [
          AnimatedOpacity(
            duration: const Duration(milliseconds: 100),
            opacity: _hasFocus ? 1.0 : 0.0,
            child: Container(
              alignment: Alignment.topLeft,
              padding: EdgeInsets.only(left: _leftPadding, top: 10, bottom: 10),
              child: Text(widget.title,
                  textAlign: TextAlign.left,
                  style: TextStyle(
                    fontSize: 16,
                    color: Colors.grey,
                  )),
            ),
          ),
          SizedBox(
            height: _itemHeight,
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
                      scale: (_hasFocus && index == _selectedIndex) ? 1.1 : 1.0,
                      duration: const Duration(milliseconds: 200),
                      child: Card(
                        margin: EdgeInsets.only(left: 10, right: 10),
                        key: _itemKeys[index],
                        shape: _hasFocus && index == _selectedIndex
                            ? RoundedRectangleBorder(
                                side:
                                    BorderSide(color: Colors.white, width: 2.0),
                                borderRadius: BorderRadius.circular(10),
                              )
                            : null,
                        child: Container(
                          decoration: BoxDecoration(
                            borderRadius: BorderRadius.circular(10),
                            boxShadow: _hasFocus && index == _selectedIndex
                                ? [
                                    BoxShadow(
                                      color: Colors.blue,
                                      spreadRadius: 5,
                                      blurRadius: 7,
                                      offset: const Offset(0, 3),
                                    ),
                                  ]
                                : null,
                            color: Colors.blue,
                          ),
                          width: _itemWidth,
                          child: Stack(
                            fit: StackFit.expand,
                            alignment: Alignment.topCenter,
                            children: [
                              ClipRRect(
                                borderRadius: BorderRadius.circular(10),
                                child: Image.asset(
                                  'assets/mock/images/${Provider.of<ImmersiveListModel>(context).getContent(index).card}',
                                  fit: BoxFit.fill,
                                ),
                              ),
                              if (!_hasFocus || index != _selectedIndex)
                                Container(
                                  decoration: BoxDecoration(
                                    gradient: LinearGradient(
                                      begin: Alignment.topCenter,
                                      end: Alignment.bottomCenter,
                                      colors: [
                                        Colors.black
                                            .withAlpha((255 * 0.5).toInt()),
                                        Colors.black
                                            .withAlpha((255 * 0.8).toInt()),
                                      ],
                                    ),
                                    borderRadius: BorderRadius.circular(10),
                                  ),
                                ),
                              Column(
                                mainAxisAlignment: MainAxisAlignment.start,
                                children: [
                                  if (_hasFocus && index == _selectedIndex)
                                    Padding(
                                      padding: EdgeInsets.all(12),
                                      child: Text(
                                          Provider.of<ImmersiveListModel>(
                                                  context)
                                              .getContent(index)
                                              .title,
                                          style: TextStyle(
                                              color: Colors.white.withAlpha(
                                                  (255 * 0.5).toInt()),
                                              fontSize: 18,
                                              fontWeight: FontWeight.bold)),
                                    ),
                                  if (_hasFocus)
                                    Padding(
                                      padding: EdgeInsets.all(12),
                                      child: Text(
                                          Provider.of<ImmersiveListModel>(
                                                  context)
                                              .getContent(index)
                                              .subtitle,
                                          style: TextStyle(
                                              color: Colors.white.withAlpha(
                                                  (255 * 0.5).toInt()),
                                              fontSize: 18,
                                              fontWeight: FontWeight.bold)),
                                    ),
                                ],
                              ),
                            ],
                          ),
                        ),
                      ),
                    );
                  },
                )),
          ),
        ],
      ),
    );
  }
}
