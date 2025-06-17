import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:tizen_fs/models/movie.dart';
import 'package:tizen_fs/widgets/focus_selectable.dart';
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

class _MovieListState extends State<MovieList> with FocusSelectable<MovieList> {
  late String _title;

  bool _hasFocus = false;
  int _itemCount = 0;
  final double _titleFontSize = 14;
  final double _listExtenedHeight = 160;
  final double _listHeight = 130;

  @override
  void initState() {
    super.initState();
    focusNode.addListener(_onFocusChanged);
    _itemCount = widget.similars.length;
    _title = widget.title;
  }

  @override
  void dispose() {
    focusNode.removeListener(_onFocusChanged);
    super.dispose();
  }

  void _onFocusChanged() async {
    setState(() {
      _hasFocus = focusNode.hasFocus;
    });

    if (_hasFocus) {
      widget.onFocused?.call();
    }
  }

  @override
  Widget build(BuildContext context) {
    return Focus(
      focusNode: focusNode,
      child: Column(
        children: [
          //list title
          Padding(
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
          //list
          SizedBox(
              height: _hasFocus ? _listExtenedHeight : _listHeight,
              child: AnimatedOpacity(
                opacity: _hasFocus ? 1.0 : 0.6,
                duration: const Duration(milliseconds: 100),
                child: SelectableListView(
                    key: listKey,
                    padding: EdgeInsets.symmetric(horizontal: 58),
                    itemCount: _itemCount,
                    itemBuilder: (context, index, selectedIndex, key) {
                      return Container(
                          clipBehavior: Clip.none,
                          margin: EdgeInsets.symmetric(horizontal: 10),
                          child: MediaCard.fourCard(
                            key: key,
                            imageUrl: widget.similars[index].posterUrl,
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
