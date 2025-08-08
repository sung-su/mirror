import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:tizen_fs/models/app_list.dart';
import 'package:tizen_fs/styles/app_style.dart';
import 'package:tizen_fs/widgets/media_card.dart';
import 'package:tizen_fs/widgets/selectable_gridview.dart';

class AppList extends StatefulWidget {
  const AppList({super.key, this.onFocused, this.scrollController});

  final VoidCallback? onFocused;
  final ScrollController? scrollController;

  @override
  State<AppList> createState() => _AppListState();
}

class _AppListState extends State<AppList> {
  final GlobalKey<SelectableGridViewState> _gridKey = GlobalKey<SelectableGridViewState>();

  final double _itemWidth = 152;
  final double _itemRatio = 16/9;
  final double _width = 960;

  final double _minimumHeight = 150;
  final double _vPadding = 30;
  final double _hPadding = 58;

  bool _isFocused = false;
  int _itemCount = 0;
  double get itemHeight => _itemWidth / _itemRatio + 30;
  int get columnCount => (_width < 152) ? 1: (_width - 116) ~/ 162;
  int get rowCount => (_itemCount % columnCount) > 0 ? (_itemCount ~/ columnCount) + 1 : _itemCount ~/ columnCount;

  List<AppInfo> appinfos = [];

  @override
  void initState() {
    super.initState();
  }

  @override
  void dispose() {
    super.dispose();
  }

  @override
  void didChangeDependencies() {
    super.didChangeDependencies();
    appinfos = Provider.of<AppInfoModel>(context).appInfos;
  }


  @override
  Widget build(BuildContext context) {
    return Column(
      children: [
        SizedBox(
          height: (itemHeight * rowCount + _vPadding) < _minimumHeight ? _minimumHeight : (itemHeight * rowCount + _vPadding),
          child: SelectableGridView(
            key: _gridKey,
            padding: EdgeInsets.symmetric(horizontal: _hPadding, vertical: _vPadding),
            itemCount: appinfos.length,
            itemRatio: _itemRatio,
            onFocused: () {
              setState(() {
                _isFocused = true;
              });
              widget.onFocused?.call();
            },
            onUnfocused: (){
              setState(() {;
                _isFocused = false;
              });
            },
            itemBuilder: (context, index, selectedIndex, key) {
              return Center(
                child: MediaCard(
                  key: key,
                  width: _itemWidth,
                  imageUrl: '',
                  content: Container(
                    decoration: BoxDecoration(
                      gradient: $style.gradients.generateLinearGradient(index % 5)
                    ),
                    child: Center(
                      child: Text(appinfos[index].name),
                    )
                  ),
                  isSelected: index == selectedIndex,
                  onRequestSelect: () {
                    _gridKey.currentState?.selectTo(index);
                  },
                ),
              );
              },
          ),
        ),
        SizedBox(
          height: 30,
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
