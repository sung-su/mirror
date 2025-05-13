import 'dart:io';
import 'package:cached_network_image/cached_network_image.dart';
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:provider/provider.dart';
import 'package:tizen_fs/models/category.dart';
import 'package:tizen_fs/providers/backdrop_provider.dart';

enum ColumnCount { one, two, three, four, six, nine }

class CategoryList extends StatefulWidget {
  final ColumnCount columns;
  final VoidCallback? onFocused;
  final Category category;

  const CategoryList({
    super.key,
    required this.category,
    this.onFocused,
    this.columns = ColumnCount.four,
  });

  @override
  State<CategoryList> createState() => _CategoryListState();
}

class _CategoryListState extends State<CategoryList> {
  final ScrollController _scrollController = ScrollController();
  final FocusNode _focusNode = FocusNode();
  late List<GlobalKey> _itemKeys;

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
    _itemCount = widget.category.tileCount;
    _selectedIndex = 0;
    _itemKeys = List.generate(_itemCount, (index) => GlobalKey());
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
        duration: Duration(milliseconds: durationMilliseconds),
        curve: Curves.easeInOut,
      );

      if (!backdrop) {
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
        mainAxisSize: MainAxisSize.min,
        children: [
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
                  child: Text(
                      widget.category.name == 'Launcher'
                          ? 'Your apps'
                          : widget.category.name,
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
              behavior: ScrollBehavior()
                  .copyWith(scrollbars: false, overscroll: false),
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
                                child: Container(
                                  //glow shadow layer
                                  decoration: BoxDecoration(
                                    borderRadius: _isCircleShape
                                        ? BorderRadius.circular(50)
                                        : BorderRadius.circular(10),
                                    boxShadow: (_hasFocus &&
                                            index == _selectedIndex)
                                        ? [
                                            BoxShadow(
                                              color: _extractColor.withAlpha(
                                                  (255 * 0.7).toInt()),
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
                                          child: _buildTileImage(widget.category
                                              .getTile(index)
                                              .iconUrl!),
                                        )
                                      : ClipRRect(
                                          borderRadius:
                                              BorderRadius.circular(10),
                                          child: _buildTileImage(widget.category
                                              .getTile(index)
                                              .iconUrl!),
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
                                    child: Text(
                                        widget.category.getTile(index).title,
                                        overflow: TextOverflow.ellipsis,
                                        maxLines: 1,
                                        style: TextStyle(
                                          color: Colors.white
                                              .withAlpha((255 * 0.7).toInt()),
                                          fontSize: _subTitleFontSize,
                                        )),
                                  ),
                                //subtitle
                                if (_hasFocus &&
                                    !_isCircleShape &&
                                    widget.category
                                        .getTile(index)
                                        .details['app_name_list'][0]
                                        .isNotEmpty)
                                  Container(
                                    alignment: Alignment.topLeft,
                                    padding: EdgeInsets.only(
                                        top: index == _selectedIndex ? 0 : 5),
                                    child: Text(
                                        widget.category
                                            .getTile(index)
                                            .details['app_name_list'][0],
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
        ],
      ),
    );
  }

  Widget _buildTileImage(String iconUrl) {
    if (iconUrl.startsWith('http')) {
      return CachedNetworkImage(
        imageUrl: iconUrl,
        placeholder: (context, url) => const CircularProgressIndicator(),
        errorWidget: (context, url, error) => const Icon(Icons.error),
        fit: BoxFit.fill,
      );
    } else if (iconUrl.startsWith('/')) {
      return Image.file(
        File(iconUrl),
        errorBuilder: (context, error, stackTrace) =>
            const Icon(Icons.broken_image),
        fit: BoxFit.fill,
      );
    } else {
      return const Center(
          child: Icon(Icons.image_not_supported, color: Colors.grey));
    }
  }
}
