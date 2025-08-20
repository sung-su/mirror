import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:tizen_fs/apps/app_popup.dart';
import 'package:tizen_fs/models/app_info.dart';
import 'package:tizen_fs/models/app_list.dart';
import 'package:tizen_fs/styles/app_style.dart';
import 'package:tizen_fs/widgets/app_tile.dart';
import 'package:tizen_fs/widgets/media_card.dart';
import 'package:tizen_fs/widgets/selectable_gridview.dart';

class AppList extends StatefulWidget {
  const AppList({super.key, this.onFocusChanged, this.onScrollup ,this.scrollController});

  final Function(bool)? onFocusChanged;
  final VoidCallback? onScrollup;
  final ScrollController? scrollController;

  @override
  State<AppList> createState() => AppListState();
}

class AppListState extends State<AppList> {
  final GlobalKey<SelectableGridViewState> _gridKey = GlobalKey<SelectableGridViewState>();

  final double _itemWidth = 150;
  final double _itemRatio = 16/9;
  final double _width = 960;

  final double _minimumHeight = 130;
  final double _vPadding = 10;
  final double _hPadding = 58;

  bool _isFocused = false;
  bool _isPopupOpened = false;
  int _itemCount = 0;
  double get itemHeight => _itemWidth / _itemRatio + 30;
  int get columnCount => (_width < 152) ? 1: (_width - 116) ~/ 162;
  int get rowCount => (_itemCount % columnCount) > 0 ? (_itemCount ~/ columnCount) + 1 : _itemCount ~/ columnCount;

  double _scrollOffset = 0;

  List<AppInfo> appinfos = [];

  @override
  void initState() {
    super.initState();
    _itemCount = appinfos.length;
  }

  @override
  void dispose() {
    super.dispose();
  }

  @override
  void didChangeDependencies() {
    super.didChangeDependencies();
    appinfos = Provider.of<AppInfoModel>(context).appInfos;
    _itemCount = appinfos.length;
  }

  @override
  Widget build(BuildContext context) {
    double height = (itemHeight + 30) * rowCount;
    height = height < MediaQuery.of(context).size.height ? MediaQuery.of(context).size.height : height;
    return SizedBox(
      height: _isFocused ? height : _minimumHeight,
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          //Header
          SizedBox(
            height: _isFocused ? 100 : 25,
            child: Padding(
              padding: EdgeInsets.symmetric(horizontal: 60),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  if (_isFocused)
                  Container(
                    height: 40, 
                    child: Padding(
                      padding: const EdgeInsets.fromLTRB(0, 10, 0 ,0),
                      child: Align(
                        alignment: Alignment.topCenter,
                        child: Container(
                          child: GestureDetector(
                            onTap: widget.onScrollup,
                            child: Icon(
                              Icons.keyboard_arrow_up,
                              size: 30,
                            ),
                          ),
                        ),
                      ),
                    )
                  ),
                  Text(
                    'Your Apps',
                    style: TextStyle(
                      fontSize: _isFocused ? 30 : 15
                    ),
                    ),
                ],
              ),
            ),
          ),
          //GridView
          Expanded(
            child: SelectableGridView(
              key: _gridKey,
              scrollController: widget.scrollController!,
              padding: EdgeInsets.symmetric(horizontal: _hPadding, vertical: _isFocused ? _vPadding : _vPadding),
              itemCount: _isFocused ? appinfos.length : appinfos.length < 5 ? appinfos.length : 5,
              itemRatio: _itemRatio,
              onFocused: () {
                setState(() {
                  _isFocused = true;
                });
                widget.onFocusChanged?.call(true);
              },
              onUnfocused: (){
                if(!_isPopupOpened)
                {
                  setState(() {
                    _isFocused = false;
                  });
                  widget.onFocusChanged?.call(false);
                }
              },
              onItemSelected: (selected) {
                _showFullScreenPopup(context, appinfos[selected]);
              },
              itemBuilder: (context, index, selectedIndex, key) {
                return Center(
                  child: GestureDetector(
                    onDoubleTap: () => _showFullScreenPopup(context, appinfos[index]),
                    child: MediaCard(
                      key: key,
                      width: _itemWidth,
                      imageUrl: '',
                      content: AppTile(app: appinfos[index]),
                      isSelected: index == selectedIndex,
                      onRequestSelect: () {
                        _gridKey.currentState?.setFocus();
                        WidgetsBinding.instance.addPostFrameCallback((_) {
                          _gridKey.currentState?.selectTo(index);
                          _gridKey.currentState?.scrollToSelected(index);
                        });
                      },
                    ),
                  ),
                );
              },
            ),
          ),
        ],
      ),
    );
  }

  void _removeApp(AppInfo app) {
    Provider.of<AppInfoModel>(context, listen: false).removeApp(app);
  }

  void _showFullScreenPopup (BuildContext context, AppInfo app) {
    _scrollOffset = widget.scrollController?.offset ?? 0;
    setState(() {
      _isPopupOpened = true;
    });
    showGeneralDialog(
      context: context,
      useRootNavigator: false,
      barrierDismissible: true,
      barrierLabel: '',
      transitionDuration: const Duration(milliseconds: 80),
      pageBuilder: (context, animation, secondaryAnimation) {
        return AppPopup(
          app: app,
          onRemovePressed: () {
            _removeApp(app);
          },
        );
      },
      transitionBuilder: (context, animation, secondaryAnimation, child) {
        return FadeTransition(
          opacity: animation,
          child: child,
        );
      },
    ).then((_){
      setState(() {
        _isPopupOpened = false;
      });
      WidgetsBinding.instance.addPostFrameCallback((_) {
        widget.scrollController?.jumpTo(_scrollOffset);
      });
    });
  }
}
