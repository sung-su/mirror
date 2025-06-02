import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:tizen_fs/models/movie.dart';
import 'package:tizen_fs/widgets/media_card.dart';
import 'package:tizen_fs/widgets/selectable_listview.dart';

class CastList extends StatefulWidget {
  final VoidCallback? onFocused;
  final List<Cast> casts;
  final String title;

  const CastList({
    super.key,
    required this.casts,
    this.onFocused,
    this.title = '',
  });

  @override
  State<CastList> createState() => _CastListState();
}

class _CastListState extends State<CastList> {
  // final ScrollController _scrollController = ScrollController();
  final FocusNode _focusNode = FocusNode();
  final GlobalKey<SelectableListViewState> _listViewKey =
      GlobalKey<SelectableListViewState>();

  // late List<GlobalKey> _itemKeys;
  late String _title;

  bool _hasFocus = false;
  int _itemCount = 10;
  int _selectedIndex = 0;
  double _titleFontSize = 14; // * 1.7
  double _listExtenedHeight = 170;
  double _listHeight = 130;

  @override
  void initState() {
    super.initState();
    _focusNode.addListener(_onFocusChanged);
    // _itemCount = widget.casts.length;
    _itemCount = 10;
    _selectedIndex = 0;
    _title = widget.title;
  }

  @override
  void dispose() {
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
    }
  }

  KeyEventResult _onKeyEvent(FocusNode focusNode, KeyEvent event) {
    if (event is KeyDownEvent || event is KeyRepeatEvent) {
      if (event.logicalKey == LogicalKeyboardKey.arrowLeft &&
          _selectedIndex > 0) {
        _prev(event is KeyRepeatEvent);
        return KeyEventResult.handled;
      } else if (event.logicalKey == LogicalKeyboardKey.arrowRight &&
          _selectedIndex < _itemCount - 1) {
        _next(event is KeyRepeatEvent);
        return KeyEventResult.handled;
      } else if (event.logicalKey == LogicalKeyboardKey.enter ||
          event.logicalKey == LogicalKeyboardKey.select) {
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
  }

  Future<void> _prev(bool fast) async {
    if (_selectedIndex <= 0) {
      return;
    }
    var moved = await _listViewKey.currentState?.previous(fast: fast);
    _selectedIndex = moved ?? _selectedIndex;
  }

  @override
  Widget build(BuildContext context) {
    return Focus(
      focusNode: _focusNode,
      onKeyEvent: _onKeyEvent,
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          //list title
          Padding(
            padding: const EdgeInsets.fromLTRB(58, 10, 70, 8),
            child: SizedBox(
              height: _hasFocus ? 40 : 20,
              child: AnimatedScale(
                  scale: _hasFocus ? 1.7 : 1.0,
                  duration: const Duration(milliseconds: 100),
                  alignment: Alignment.centerLeft,
                  child: Text(
                    _title,
                    textAlign: TextAlign.left,
                    style: TextStyle(
                      fontSize: _titleFontSize,
                      color: _hasFocus
                          ? Colors.white
                              .withAlpha((255 * 0.7).toInt())
                          : Colors.grey,
                    ))),
            ),
          ),
          //list
          SizedBox(
              height: _hasFocus ? _listExtenedHeight : _listHeight,
              child: AnimatedOpacity(
                opacity: _hasFocus ? 1.0 : 0.3,
                duration: const Duration(milliseconds: 100),
                child: SelectableListView(
                    key: _listViewKey,
                    padding: EdgeInsets.only(left: 58),
                    itemCount: _itemCount,
                    itemBuilder: (context, index, selectedIndex, key) {
                      return Container(
                          clipBehavior: Clip.none,
                          margin: EdgeInsets.symmetric(horizontal: 10),
                          child: MediaCard.circleLarge(
                            key: key,
                            imageUrl: widget.casts[index].profilePath.isNotEmpty
                                ? 'https://media.themoviedb.org/t/p/w500${widget.casts[index].profilePath}'
                                : '',
                            isSelected: Focus.of(context).hasFocus &&
                                index == selectedIndex,
                            title: _hasFocus ? widget.casts[index].name : null,
                            subtitle:
                                _hasFocus ? widget.casts[index].character : null,
                          ));
                    }),
              )),
        ],
      ),
    );
  }
}
