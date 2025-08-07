import 'package:flutter/material.dart';
import 'package:tizen_fs/widgets/selectable_gridview.dart';

class AppList extends StatefulWidget {
  const AppList({super.key, this.onFocused, this.scrollController});

  final VoidCallback? onFocused;
  final ScrollController? scrollController;

  @override
  State<AppList> createState() => _AppListState();
}

class _AppListState extends State<AppList> {
final FocusNode _focusNode = FocusNode();
final _gridKey = GlobalKey();

bool _isFocused = false;

  @override
  void dispose() {
    _focusNode.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Column(
      children: [
        SelectableGridView(
          key: _gridKey,
          padding: EdgeInsets.symmetric(horizontal: 58, vertical: 30),
          itemCount: 5,
          ScrollController: widget.scrollController,
          onFocused: () {
            setState(() {
              _isFocused = true;
            });
            widget.onFocused?.call();
          },
          onUnfocused: (){
            setState(() {
              _isFocused = false;
            });
          },
        ),
        SizedBox(
          height: 30,
          width: 200,
          child: Container(
            child: _isFocused ? null: Icon(
              Icons.keyboard_arrow_down,
              size: 30,
            ),
          ),
        )
      ],
    );
  }
}
