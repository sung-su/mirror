import 'dart:math';
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:tizen_fs/styles/app_style.dart';
import 'package:tizen_fs/widgets/media_card.dart';

class AdaptiveGridPage extends StatefulWidget {
  @override
  State<AdaptiveGridPage> createState() => _AdaptiveGridPageState();
}

class _AdaptiveGridPageState extends State<AdaptiveGridPage> {
  final double _padding = 58;
  final FocusNode _focusNode = FocusNode();
  final ScrollController _listScroller = ScrollController();
  final ScrollController _scrollController = ScrollController();
  late List<GlobalKey> _itemKeys;


  double _width = 960;
  double _itemWidth = 152;
  int get _itemCount => 52;
  int get columnCount => (_width < 152) ? 1: (_width - 116) ~/ 162;
  int get rowCount => (_itemCount % columnCount) > 0 ? (_itemCount ~/ columnCount) + 1 : _itemCount ~/ columnCount;

  List<double> _widths = [100, 300, 480, 700, 960];
  int _selectedIndex = 0;
  int _lastSelected = -1;

  @override
  void initState() {
    super.initState();

    _itemKeys = List.generate(
      _itemCount,
      (index) => GlobalKey(),
    );

    WidgetsBinding.instance.addPostFrameCallback((_) {
      _focusNode.requestFocus();
    });
  }

  @override
  void didUpdateWidget(covariant AdaptiveGridPage oldWidget) {
    super.didUpdateWidget(oldWidget);
    if(_lastSelected != -1) {
      setState(() {
        _selectedIndex = _lastSelected;
      });
    }
  }

  @override
  void dispose() {
    _focusNode.dispose();
    super.dispose();
  }

  void _selectTo(int index) async {
    if (index >= 0 && index < _itemCount) {
      int current = await _scrollToSelected(index);
      setState(() {
        if (_selectedIndex != current)
          _selectedIndex = current;  
      });
    }  
  }


  Future<int> _scrollToSelected(var index) async {
    final context = _itemKeys[index].currentContext;
    if (context != null) {
      Scrollable.ensureVisible(
        context,
        alignment: 1,
        duration: $style.times.fast,
        curve: Curves.easeInOut
      );
      return index;
    } else {
      return _selectedIndex;
    }
  }

  KeyEventResult _handleKey(FocusNode node, KeyEvent event) {
    if(event is KeyDownEvent || event is KeyRepeatEvent) {
      int col = _selectedIndex % columnCount;

      if (event.logicalKey == LogicalKeyboardKey.arrowUp) {
        if (_selectedIndex - columnCount >= 0) {
          _selectTo(_selectedIndex - columnCount);
          return KeyEventResult.handled;
        }
        else {
        }
      }
      else if (event.logicalKey == LogicalKeyboardKey.arrowDown) {
        if (_selectedIndex + columnCount < _itemCount) {
          _selectTo(_selectedIndex + columnCount);
          return KeyEventResult.handled;
        } 
      }
      else if (event.logicalKey == LogicalKeyboardKey.arrowRight) {
        if (col < columnCount - 1 && _selectedIndex + 1 < _itemCount) {
          _selectTo(_selectedIndex + 1);
        }
        return KeyEventResult.handled;
      }
      else if (event.logicalKey == LogicalKeyboardKey.arrowLeft) {
        if (col > 0) {
          _selectTo(_selectedIndex - 1);
        }
        return KeyEventResult.handled;
      }
    }

    return KeyEventResult.ignored;
  }


  void _onFocusChanged(bool hasFocus) {
    if(hasFocus) {
      if(_lastSelected != -1) {
        setState(() {  
          _selectedIndex = _lastSelected;
        });
      }
    } 
    else {
      _lastSelected = _selectedIndex;
      setState(() {  
        _selectedIndex = -1;
      });
    }
  }

  @override
  Widget build(BuildContext context) {
    var screenSize = MediaQuery.of(context).size;
    return Scaffold(
      appBar: AppBar(
        automaticallyImplyLeading: false,
        title: Text('Adaptive Grid')
      ),
      body: Column(
        spacing: 10,
        children: [
          Align(
            alignment: Alignment.topLeft,
            child: Padding(
              padding: const EdgeInsets.symmetric(horizontal: 50),
              child: Text("screen size width=${screenSize.width}, height=${screenSize.height}"),
            )
          ),
          SizedBox(
            height: 50,
            child: Container(
              color: Colors.transparent,
              child: Padding(
                padding: const EdgeInsets.symmetric(horizontal: 50),
                child: ListView.builder(
                  controller: _listScroller,
                  scrollDirection: Axis.horizontal,
                  itemCount: _widths.length,
                  itemBuilder: (context, index) {
                    return ElevatedButton(onPressed: () {
                      setState(() {
                        _width = _widths[index];
                      });
                    }, child: Text(_widths[index].toString()));
                  }
                ),
              ),
            ),
          ),
          Expanded(
            child: SizedBox(
              width: _width,
              child: Focus(
                focusNode: _focusNode,
                onKeyEvent: _handleKey,
                onFocusChange: _onFocusChanged,
                child: GridView.builder(
                  controller: _scrollController,
                  padding: EdgeInsets.symmetric(horizontal: _padding, vertical: 30),
                  physics: const ClampingScrollPhysics(),
                  gridDelegate: SliverGridDelegateWithFixedCrossAxisCount(
                    crossAxisCount: columnCount,
                    crossAxisSpacing: 10,
                    mainAxisSpacing: 10,
                    childAspectRatio: 16/9,
                  ),
                  itemBuilder: (context, index) {
                    return SizedBox(
                      key: _itemKeys[index],
                      width: (_width < _itemWidth) ? _width : _itemWidth,
                      child: Center(
                        child: MediaCard(
                          width: (_width < _itemWidth) ? _width : _itemWidth,
                          imageUrl: '',
                          content: Container(
                            decoration: BoxDecoration(
                              gradient: $style.gradients.getGradient(index % 5)
                            ),
                          ),
                          isSelected: index == _selectedIndex,
                          onRequestSelect: () {
                            _selectTo(index);
                          },
                        ),
                      ),
                    );
                  },
                  itemCount: _itemCount,
                ),
              ),
            ),
          ),
        ],
      ),
    );
  }
}