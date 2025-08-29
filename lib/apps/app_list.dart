import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:tizen_fs/apps/app_popup.dart';
import 'package:tizen_fs/models/app_data.dart';
import 'package:tizen_fs/models/app_data_model.dart';
import 'package:tizen_fs/native/app_manager.dart';
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

  List<AppData> AppDatas = [];

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
  }

  @override
  Widget build(BuildContext context) {
    AppDatas = context.watch<AppDataModel>().appInfos;

    _itemCount = AppDatas.length;
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
              itemCount: _isFocused ? AppDatas.length : AppDatas.length < 5 ? AppDatas.length : 5,
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
                ApplicationManager.launch(AppDatas[selected].appId);
              },
              onItemLongPressed: (selected) {
                _showFullScreenPopup(context, AppDatas[selected]);
              },
              itemBuilder: (context, index, selectedIndex, key) {
                return Center(
                  child: GestureDetector(
                    onDoubleTap: () => ApplicationManager.launch(AppDatas[index].appId),
                    onLongPress: () => _showFullScreenPopup(context, AppDatas[index]),
                    child: MediaCard(
                      key: key,
                      width: _itemWidth,
                      imageUrl: '',
                      content: AppTile(app: AppDatas[index], index: index),
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

  void _removeApp(AppData app) {
    ApplicationManager.uninstallPackage(app.packageName);
  }

  void _showFullScreenPopup (BuildContext context, AppData app) {
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
