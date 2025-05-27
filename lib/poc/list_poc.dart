
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
  int _prevIndex = -1;

  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addPostFrameCallback((_) {
      _itemKeys[_focusedIndex].currentState?.changeFocus(true);
    });
  }

  KeyEventResult _handleKey(FocusNode node, KeyEvent event) {
    if (event is KeyDownEvent) {
      int newIndex = _focusedIndex;
      if (event.logicalKey == LogicalKeyboardKey.arrowDown) {
        newIndex++;
      } else if (event.logicalKey == LogicalKeyboardKey.arrowUp) {
        newIndex--;
      }

      if(newIndex < 0 || newIndex >= _itemKeys.length)
      newIndex = (newIndex >= _itemKeys.length) ? _itemKeys.length - 1
             : (newIndex < 0 ? 0 : _itemKeys.length - 1);
             
      _prevIndex = _focusedIndex;       
      _focusedIndex = newIndex;
      _updateFocusAndScroll(); 

      return KeyEventResult.handled;
    }
    return KeyEventResult.ignored;
  }

  void _updateFocusAndScroll() {
    final context = _itemKeys[_focusedIndex].currentContext;
    if (context != null) {
      Scrollable.ensureVisible(
        context,
        alignment: 0.0,
        duration: const Duration(milliseconds: 100),
        curve: Curves.easeInOut,
      );
    }

    _itemKeys[_prevIndex].currentState?.changeFocus(false);
    _itemKeys[_focusedIndex].currentState?.changeFocus(true);
    
  }

  @override
  Widget build(BuildContext context) {
    return Focus(
      autofocus: true,
      focusNode: FocusNode(),
      onKeyEvent: _handleKey,
      child: ListView.builder(
        controller: _scrollController,
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
    return Container(
      height: 100,
      margin: const EdgeInsets.symmetric(vertical: 4),
      color: isFocused ? Colors.blue : Colors.grey,
      alignment: Alignment.center,
      child: Text(
        'Item ${widget.index}',
        style: const TextStyle(fontSize: 24, color: Colors.white),
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