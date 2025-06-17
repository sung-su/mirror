import 'dart:io';
import 'package:cached_network_image/cached_network_image.dart';
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:palette_generator/palette_generator.dart';
import 'package:provider/provider.dart';
import 'package:tizen_fs/models/tile.dart';
import 'package:tizen_fs/providers/backdrop_provider.dart';
import 'package:tizen_fs/styles/app_style.dart';
import 'package:tizen_fs/widgets/media_card.dart';
import 'package:tizen_fs/widgets/selectable_listview.dart';

enum ColumnCount { one, two, three, four, six, nine }

class CategoryList extends StatefulWidget {
  final VoidCallback? onFocused;
  final Function(int)? onItemSelected;
  final List<Tile> tiles;
  final ColumnCount columns;
  final String title;
  final String icon;
  final bool hasTimeStamp;
  final bool isCircle;

  const CategoryList({
    super.key,
    required this.tiles,
    this.onFocused,
    this.onItemSelected,
    this.columns = ColumnCount.four,
    this.title = '',
    this.icon = '',
    this.hasTimeStamp = false,
    this.isCircle = false,
  });

  @override
  State<CategoryList> createState() => _CategoryListState();
}

class _CategoryListState extends State<CategoryList> {
  final ScrollController _scrollController = ScrollController();
  final FocusNode _focusNode = FocusNode();
  final GlobalKey<SelectableListViewState> _listViewKey =
      GlobalKey<SelectableListViewState>();

  bool _hasFocus = false;
  int _selectedIndex = 0;

  late final String _title;
  late final int _itemCount;
  late final double _itemWidth;
  late final double _itemHeight;
  late final double _extendedListHeight;
  late final double _listHeight;

  List<Color?> _extractedColors = [];

  static const double _horizontalPadding = 58;
  static const double _titleFontSize = 14;
  static const double _titleHeight = 38;
  static const double _extendedTitleHeight = 58;

  void calculateItemSize() {
    _itemWidth = switch(widget.columns) {
      ColumnCount.nine => 80,
      ColumnCount.six => 124,
      ColumnCount.three => 268,
      ColumnCount.two => 412,
      ColumnCount.one => 844,
      _ => 196, // default case for four columns
    };
    _itemHeight = (_itemWidth * (widget.isCircle ? 1 : 9 / 16)).roundToDouble();
    _extendedListHeight = _itemHeight * 1.7;
    _listHeight = (_itemHeight * 1.3).roundToDouble();
  }

  bool checkLabelVisible(int order, bool selected) {
    bool title = false;
    bool subTitle = false;
    bool subHeading = false;

    if (!_hasFocus) {
      return false;
    } else {
      if (widget.columns == ColumnCount.three) {
        return true;
      } else if (widget.columns == ColumnCount.six || widget.columns == ColumnCount.nine) {
        title = true;
        subTitle = widget.columns == ColumnCount.six;
      } else {
        //columns == 4
        if (selected) {
          title = true;
          subTitle = true;
        } else {
          title = false;
          subTitle = true;
        }
      }
    }

    if (order == 1) return title;
    if (order == 2) return subTitle;
    return subHeading;
  }

  @override
  void initState() {
    super.initState();
    calculateItemSize();
    _focusNode.addListener(_onFocusChanged);
    _itemCount = widget.tiles.length;
    _selectedIndex = 0;
    _title = widget.title;
    _extractedColors = List.generate(widget.tiles.length, (i) => null);
    _extractColor(0);
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
      if (event.logicalKey == LogicalKeyboardKey.arrowLeft) {
        _prev(event is KeyRepeatEvent);
        return KeyEventResult.handled;
      } else if (event.logicalKey == LogicalKeyboardKey.arrowRight) {
        _next(event is KeyRepeatEvent);
        return KeyEventResult.handled;
      } else if (event.logicalKey == LogicalKeyboardKey.enter ||
          event.logicalKey == LogicalKeyboardKey.select) {
        widget.onItemSelected?.call(_selectedIndex);
        return KeyEventResult.handled;
      }
    }
    return KeyEventResult.ignored;
  }

  Future<void> _next(bool fast) async {
    if (_selectedIndex >= _itemCount - 1) {
      return;
    }
    var moved = await _listViewKey.currentState?.next(fast: fast);
    _selectedIndex = moved ?? _selectedIndex;
    final current = _selectedIndex;
    _extractColor(current);

    await Future.delayed(const Duration(milliseconds: 300));
    if (current == _selectedIndex) {
      if (mounted) {
        Provider.of<BackdropProvider>(context, listen: false)
            .updateBackdrop(getSelectedBackdrop());
      }
    }
  }

  Future<void> _prev(bool fast) async {
    if (_selectedIndex <= 0) {
      return;
    }
    var moved = await _listViewKey.currentState?.previous(fast: fast);
    _selectedIndex = moved ?? _selectedIndex;
    final current = _selectedIndex;
    _extractColor(current);

    await Future.delayed(const Duration(milliseconds: 300));
    if (current == _selectedIndex) {
      if (mounted) {
        Provider.of<BackdropProvider>(context, listen: false)
            .updateBackdrop(getSelectedBackdrop());
      }
    }
  }

  int _isExtracting = 0;
  void _extractColor(int index) async {
    if (_isExtracting < 1 && _extractedColors[index] == null) {
      _isExtracting++;
      final generator = await PaletteGenerator.fromImageProvider(
        _getImageProvider(widget.tiles[index].iconUrl!),
        maximumColorCount: 1,
      );
      _isExtracting--;
      setState(() {
        _extractedColors[index] = generator.dominantColor?.color;
      });
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
              (_extractedColors[_selectedIndex] ?? Colors.white).withAlpha((0.2 * 255).toInt()),
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
        children: [
          //title
          _buildTitle(),
          //list
          SizedBox(
            height: _hasFocus ? _extendedListHeight : _listHeight,
            child: SelectableListView(
              key: _listViewKey,
              itemCount: _itemCount,
              padding: EdgeInsets.symmetric(horizontal: _horizontalPadding),
              itemBuilder: (context, index, selectedIndex, key) {
                return Container(
                  margin: EdgeInsets.all(10),
                  child: MediaCard(
                    key: key,
                    width: _itemWidth,
                    imageUrl: widget.tiles[index].iconUrl!,
                    isSelected: _hasFocus && index == selectedIndex,
                    ratio: widget.isCircle
                        ? MediaCardRatio.square
                        : MediaCardRatio.wide,
                    shadowColor: (_extractedColors[index] ?? Colors.white).withAlphaF(0.7),
                    title: checkLabelVisible(1, index == selectedIndex)
                        ? widget.tiles[index].title
                        : null,
                    subtitle: checkLabelVisible(2, index == selectedIndex)
                        ? getSubtitle(index)
                        : null,
                    description: checkLabelVisible(3, index == selectedIndex)
                        ? widget.tiles[index].details['price'] ?? 'subHeading'
                        : null,
                    duration: widget.hasTimeStamp
                        ? widget.tiles[index].details['duration']
                        : null,
                  ),
                );
              },
            ),
          ),
        ],
      ),
    );
  }

  String getSubtitle(int index) {
    return widget.tiles[index].details['app_name_list']?[0] ?? 'subTitle';
  }

  Widget _buildTitle() {
    return Container(
      height: _hasFocus ? _extendedTitleHeight : _titleHeight,
      padding: const EdgeInsets.fromLTRB(_horizontalPadding + 12, 10, 0, 8),
      child: AnimatedScale(
          scale: _hasFocus ? 1.7 : 1.0,
          duration: const Duration(milliseconds: 100),
          alignment: Alignment.centerLeft,
          child: Row(
            crossAxisAlignment: CrossAxisAlignment.center,
            spacing: 5,
            children: [
              if (widget.icon.isNotEmpty)
                _buildTileImage(widget.icon),
              if (_title.isNotEmpty)
                Text(_title,
                    textAlign: TextAlign.left,
                    style: TextStyle(
                      fontSize: _titleFontSize,
                      color: _hasFocus
                          ? Colors.white.withAlpha((255 * 0.7).toInt())
                          : Colors.grey,
                    )),
            ],
          )),
    );
  }

  ImageProvider _getImageProvider(String iconUrl) {
    if (iconUrl.startsWith('http')) {
      return CachedNetworkImageProvider(iconUrl);
    } else if (iconUrl.startsWith('/')) {
      return FileImage(File(iconUrl));
    } else if (iconUrl.startsWith('assets')) {
      return AssetImage(iconUrl);
    } else {
      return const AssetImage('assets/images/placeholder.png');
    }
  }

  Widget _buildTileImage(String iconUrl) {
    if (iconUrl.startsWith('http')) {
      return CachedNetworkImage(
        width: 25,
        height: 17,
        imageUrl: iconUrl,
        placeholder: (context, url) => const CircularProgressIndicator(),
        errorWidget: (context, url, error) => const Icon(Icons.error),
        fit: BoxFit.fill,
      );
    } else if (iconUrl.startsWith('/')) {
      return Image.file(
        width: 25,
        height: 17,
        File(iconUrl),
        errorBuilder: (context, error, stackTrace) =>
            const Icon(Icons.broken_image),
        fit: BoxFit.fill,
      );
    } else if (iconUrl.startsWith('assets')) {
      return Image.asset(
        width: 25,
        height: 17,
        iconUrl,
        fit: BoxFit.fill,
      );
    } else {
      return const Center(
          child: Icon(Icons.image_not_supported, color: Colors.grey));
    }
  }
}
