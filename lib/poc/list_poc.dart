
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';

class ListPocPage extends StatefulWidget {
  const ListPocPage({super.key});

  @override
  State<ListPocPage> createState() => _ListPocPageState();
}

class _ListPocPageState extends State<ListPocPage> {
  bool _isHorizontal = false;

  @override
  Widget build(BuildContext context) {
    return _isHorizontal ? buildDrawerScaffold(context) : buildAppbarScaffold(context);
  }

  Widget buildDrawerScaffold(BuildContext context) {
    return Scaffold(
      body: SafeArea(
        bottom: false,
        top: false,
        child: Row(
          children: [
            GestureDetector(
              onTap: () {
                setState(() {
                  _isHorizontal = !_isHorizontal;  
                });
              },
              child: SizedBox(
                width: 100,
                height: 1080,
                child: Column(
                  mainAxisAlignment: MainAxisAlignment.center,
                  crossAxisAlignment: CrossAxisAlignment.center,
                  children: [
                    Text("HList",
                      textAlign: TextAlign.center,
                      style: TextStyle(
                        fontSize: 32,
                      )
                    ),
                    Text("Tap \nto change",
                      textAlign: TextAlign.center,
                      style: TextStyle(
                        fontSize: 16,
                      )
                    )
                  ]
                ),  
              ),
            ),
            Expanded(child: FocusScrollView(isHorizontal: _isHorizontal))
          ]
        ),
      )
    );
  }

  Widget buildAppbarScaffold(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        automaticallyImplyLeading: false,
        backgroundColor: Colors.transparent,
        surfaceTintColor: Colors.transparent,
        toolbarHeight: 150,
        title: Center(
          child: GestureDetector(
            onTap: () {
              setState(() {
                _isHorizontal = !_isHorizontal;  
              });
            },
            child: Column(
              children: [
                Text("VList",
                  style: TextStyle(
                      fontSize: 32,
                  )
                ),
                Text("Tap title to change the direction of the listview",
                  style: TextStyle(
                    fontSize: 16,
                  )
                )
              ]
            ),
          )
        ),
      ),
      body: FocusScrollView(isHorizontal: _isHorizontal)
    );
  }
}
class FocusScrollView extends StatefulWidget {
  FocusScrollView({super.key, required this.isHorizontal});

  bool isHorizontal = false;

  @override
  State<FocusScrollView> createState() => _FocusScrollViewState();
}

class _FocusScrollViewState extends State<FocusScrollView> {
  final ScrollController _scrollController = ScrollController();
  late List<GlobalKey<_ItemContainerState>> _itemKeys = List.generate(30, (_) => GlobalKey<_ItemContainerState>());
  int _focusedIndex = 0;

  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addPostFrameCallback((_) {
      _itemKeys[_focusedIndex].currentState?.changeFocus(true);
    });
  }

  KeyEventResult _handleKey(FocusNode node, KeyEvent event) {
    var nextKey = widget.isHorizontal ? LogicalKeyboardKey.arrowRight : LogicalKeyboardKey.arrowDown;
    var prevKey = widget.isHorizontal ? LogicalKeyboardKey.arrowLeft : LogicalKeyboardKey.arrowUp;

    if (event is KeyDownEvent || event is KeyRepeatEvent) {
      int newIndex = _focusedIndex;
      if (event.logicalKey == nextKey) {
        newIndex++;
      } else if (event.logicalKey == prevKey) {
        newIndex--;
      }

      if(newIndex < 0 || newIndex >= _itemKeys.length) return KeyEventResult.handled;
      move(event is KeyRepeatEvent, newIndex);
      return KeyEventResult.handled;
    }
    return KeyEventResult.ignored;
  }

  void move(bool fast, int index) async
  {
    int? current = await _updateFocusAndScroll(fast ? 1 : 100, index);

    if (current != null) {
      _itemKeys[_focusedIndex].currentState?.changeFocus(false);
      _itemKeys[current].currentState?.changeFocus(true);
      _focusedIndex = current;
    }
  }

  Future<int?> _updateFocusAndScroll(int durationMilliseconds, int index) async {
    final context = _itemKeys[index].currentContext;

    if (context != null) {  
      Scrollable.ensureVisible(
        context,
        alignment: 0.75,
        duration: Duration(milliseconds: durationMilliseconds),
        curve: Curves.easeInOut,
      );
      return index;
    }
    else {
      return null;
    }
  }

  @override
  void dispose() {
    _scrollController.dispose();
    super.dispose();
  }
  @override
  Widget build(BuildContext context) {
    return Focus(
      autofocus: true,
      focusNode: FocusNode(),
      onKeyEvent: _handleKey,
      child: ListView.separated(
        controller: _scrollController,
        scrollDirection: widget.isHorizontal ? Axis.horizontal : Axis.vertical,
        itemCount: _itemKeys.length,
        padding: widget.isHorizontal ? EdgeInsets.only(left: 50) : EdgeInsets.only(top: 50),
        itemBuilder: (context, index) {
          return GestureDetector(
            onTap: () {
              move(false, index);
            },
            child: ItemContainer(
              index: index,
              key: _itemKeys[index],
              isFocused: index == _focusedIndex,
              isHorizontal: widget.isHorizontal,
            ),
          );
        },
        separatorBuilder: (BuildContext context, int index) => widget.isHorizontal ? const SizedBox(width: 20) : const SizedBox(height: 10)
      )
    );
  }
}

class ItemContainer extends StatefulWidget {
  final int index;
  final bool isFocused;
  bool isHorizontal;

  ItemContainer({
    super.key,
    required this.index,
    required this.isFocused,
    required this.isHorizontal
  });

  @override
  State<ItemContainer> createState() => _ItemContainerState();
}

class _ItemContainerState extends State<ItemContainer> {
  bool isFocused = false;

  @override
  Widget build(BuildContext context) {
    return AnimatedScale(
      duration: Duration(milliseconds: 50),
      scale: isFocused ? 1.1 : 1.0,
      child: Container(
        margin: const EdgeInsets.symmetric(vertical: 4),
        // color: isFocused ? Colors.blue : Colors.grey,
        alignment: Alignment.center,
        child: SizedBox(
          width: widget.isHorizontal ? 200 : 500,
          height: widget.isHorizontal ? 300 : 80,
          child: Container(
            decoration: BoxDecoration(
              borderRadius: BorderRadius.circular(10),
              color: isFocused ? Colors.blue : Colors.grey,
            ),
            child: Center(
              child: Text(
                'Item ${widget.index}',
                style: const TextStyle(fontSize: 16, color: Colors.white),
              ),
            ),
          ),
        ),
      ),
    );
  }

  void changeFocus(bool isFocus)
  {
    setState(() {
      isFocused = isFocus;
    });
  }
}