import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:tizen_fs/models/movie.dart';
import 'package:tizen_fs/widgets/media_card.dart';
import 'package:tizen_fs/widgets/selectable_listview.dart';

class MovieList extends StatefulWidget {
  final VoidCallback? onFocused;
  final List<Similar> similars;
  final String title;

  const MovieList({
    super.key,
    required this.similars,
    this.onFocused,
    this.title = '',
  });

  @override
  State<MovieList> createState() => _MovieListState();
}

class _MovieListState extends State<MovieList> {
  final FocusNode _focusNode = FocusNode();
  final GlobalKey<SelectableListViewState> _listViewKey =
      GlobalKey<SelectableListViewState>();

  late String _title;

  bool _hasFocus = false;
  int _itemCount = 0;
  int _selectedIndex = 0;
  double _titleFontSize = 14;
  double _listExtenedHeight = 160;
  double _listHeight = 130;

  @override
  void initState() {
    super.initState();
    _focusNode.addListener(_onFocusChanged);
    _itemCount = widget.similars.length;
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
        children: [
          //list title
          Container(
            child: Padding(
              padding: const EdgeInsets.fromLTRB(70, 0, 70, 8),
              child: SizedBox(
                height: _hasFocus ? 40 : 20,
                child: AnimatedScale(
                    scale: _hasFocus ? 1.7 : 1.0,
                    duration: const Duration(milliseconds: 100),
                    alignment: Alignment.topLeft,
                    child: Row(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      spacing: 5,
                      children: [
                        if (_title.isNotEmpty)
                          Text(_title,
                              textAlign: TextAlign.left,
                              style: TextStyle(
                                fontSize: _titleFontSize,
                                color: _hasFocus
                                    ? Colors.white
                                        .withAlpha((255 * 0.7).toInt())
                                    : Colors.grey,
                              )),
                      ],
                    )),
              ),
            ),
          ),
          //list
          SizedBox(
              height: _hasFocus ? _listExtenedHeight : _listHeight,
              child: AnimatedOpacity(
                opacity: _hasFocus ? 1.0 : 0.6,
                duration: const Duration(milliseconds: 100),
                child: SelectableListView(
                    key: _listViewKey,
                    padding: EdgeInsets.only(left: 58),
                    itemCount: _itemCount,
                    itemBuilder: (context, index, selectedIndex, key) {
                      return Container(
                          clipBehavior: Clip.none,
                          margin: EdgeInsets.symmetric(horizontal: 10),
                          child: MediaCard.fourCard(
                            key: key,
                            imageUrl: widget.similars[index].posterPath.isNotEmpty
                                ? 'https://image.tmdb.org/t/p/w500${widget.similars[index].posterPath}'
                                : '',
                            isSelected: Focus.of(context).hasFocus &&
                                index == selectedIndex,
                            title: _hasFocus && index == selectedIndex
                                ? widget.similars[index].title
                                : null,
                            subtitle: _hasFocus
                                ? widget.similars[index].releaseYear
                                : null,
                          ));
                    }),
              )),
        ],
      ),
    );
  }
}
