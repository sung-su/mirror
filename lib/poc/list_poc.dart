
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';

class ListPocPage extends StatelessWidget {
  const ListPocPage({super.key});

  @override
  Widget build(BuildContext context) {
    return FocusScrollView();
  }
}
class FocusScrollView extends StatefulWidget {
  const FocusScrollView({super.key});

  @override
  State<FocusScrollView> createState() => _FocusScrollViewState();
}

class _FocusScrollViewState extends State<FocusScrollView> {
  final ScrollController _scrollController = ScrollController();
  late List<GlobalKey<_ItemContainerState>> _itemKeys = List.generate(10, (_) => GlobalKey<_ItemContainerState>());
  int _focusedIndex = 0;

  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addPostFrameCallback((_) {
      _itemKeys[_focusedIndex].currentState?.changeFocus(true);
    });
  }

  KeyEventResult _handleKey(FocusNode node, KeyEvent event) {
    if (event is KeyDownEvent || event is KeyRepeatEvent) {
      int newIndex = _focusedIndex;
      if (event.logicalKey == LogicalKeyboardKey.arrowRight) {
        newIndex++;
      } else if (event.logicalKey == LogicalKeyboardKey.arrowLeft) {
        newIndex--;
      }

      if(newIndex < 0 || newIndex >= _itemKeys.length) return KeyEventResult.handled;
      move(event is KeyRepeatEvent, newIndex);
      return KeyEventResult.handled;
    }
    return KeyEventResult.ignored;
  }

  void move (bool fast, int index) async
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
        alignment: 0.0,
        duration: Duration(milliseconds: durationMilliseconds),
        curve: Curves.easeInOut,
      );
      return index;

      // animateto
      // final RenderBox box = context.findRenderObject() as RenderBox;
      // final Offset position = box.localToGlobal(Offset.zero);
      // await _scrollController.animateTo(
      //   position.dx + _scrollController.offset - 10,
      //   duration: Duration(milliseconds: 1), 
      //   curve: Curves.easeInOut
      // );
    }
    else {
      return null;
    }
  }

  @override
  Widget build(BuildContext context) {
    return Focus(
      autofocus: true,
      focusNode: FocusNode(),
      onKeyEvent: _handleKey,
      child: ListView.builder(
        controller: _scrollController,
        scrollDirection: Axis.horizontal,
        itemCount: 10,
        itemBuilder: (context, index) {
          return ItemContainer(
            index: index,
            key: _itemKeys[index],
            isFocused: index == _focusedIndex
          );
        },
      ),
    );
  }
}

class ItemContainer extends StatefulWidget {
  final int index;
  final bool isFocused;

  const ItemContainer({
    super.key,
    required this.index,
    required this.isFocused,
  });

  @override
  State<ItemContainer> createState() => _ItemContainerState();
}

class _ItemContainerState extends State<ItemContainer> {
  bool isFocused = false;

  @override
  Widget build(BuildContext context) {
    return SizedBox(
      height: 100,
      width: 300,
      child: Container(
        height: 100,
        margin: const EdgeInsets.symmetric(vertical: 4),
        color: isFocused ? Colors.blue : Colors.grey,
        alignment: Alignment.center,
        child: Text(
          'Item ${widget.index}',
          style: const TextStyle(fontSize: 24, color: Colors.white),
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