import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:tizen_fs/models/movie.dart';

class CastList extends StatefulWidget {
  final void Function(BuildContext)? onFocused;

  const CastList({
    super.key,
    required this.movie,
    this.onFocused,
  });

  final Movie movie;

  @override
  State<CastList> createState() => _CastListState();
}

class _CastListState extends State<CastList> {
  final FocusNode _focusNode = FocusNode();
  final _itemCount = 10;
  bool _hasFocus = false;
  int _selectedIndex = 0;

  @override
  void initState() {
    super.initState();
    _focusNode.addListener(_onFocusChanged);
  }

  @override
  void dispose() {
    _focusNode.removeListener(_onFocusChanged);
    _focusNode.dispose();
    super.dispose();
  }

  void _onFocusChanged() {
    setState(() {
      _hasFocus = _focusNode.hasFocus;
    });

    if (_hasFocus) {
      widget.onFocused?.call(context);
    }
  }

  void requestFocus()
  {
    _focusNode.requestFocus();
  }

  KeyEventResult _onKeyEvent(FocusNode focusNode, KeyEvent event) {
    if (event is KeyDownEvent || event is KeyRepeatEvent) {
      if (event.logicalKey == LogicalKeyboardKey.arrowLeft &&
          _selectedIndex > 0) {

        return KeyEventResult.handled;
      } else if (event.logicalKey == LogicalKeyboardKey.arrowRight &&
          _selectedIndex < _itemCount - 1) {

        return KeyEventResult.handled;
      }
    }
    return KeyEventResult.ignored;
  }

  @override
  Widget build(BuildContext context) {
    return Focus(
      focusNode: _focusNode,
      onKeyEvent: _onKeyEvent,
      child: Container(
          padding: const EdgeInsets.only(left: 58, right: 58),
          //TODO: to be removed
          color : _hasFocus ? Colors.red : Colors.transparent, 
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              const Text("Cast & Crew", style: TextStyle(fontSize: 20)),
              SingleChildScrollView(
                clipBehavior: Clip.none,
                scrollDirection: Axis.horizontal,
                child: Row(
                  spacing: 10,
                  children:
                    List.generate(widget.movie.cast.length,
                        (index) {
                          return Padding(
                            padding: const EdgeInsets.symmetric(horizontal: 10),
                            child: Column(
                              children: [
                                CircleAvatar(
                                  radius: 60,
                                  backgroundImage: NetworkImage(
                                      widget.movie.cast.isNotEmpty
                                          ? 'https://media.themoviedb.org/t/p/w500${widget.movie.cast[index].profilePath}'
                                          : ''),
                                ),
                                const SizedBox(height: 5),
                                Text(widget.movie.cast[index].name,
                                    style: TextStyle(fontSize: 12)),
                                const SizedBox(height: 5),
                                Text(widget.movie.cast[index].character,
                                    style: TextStyle(fontSize: 10)),
                              ],
                            ),
                          );
                        },
                    ),
                  ),
              )],
          )),
    );
  }
}
