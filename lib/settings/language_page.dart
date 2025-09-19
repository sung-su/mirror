import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:tizen_fs/models/page_node.dart';
import 'package:tizen_fs/styles/app_style.dart';
import 'package:tizen_fs/utils/language_manager.dart';
import 'package:tizen_fs/widgets/focus_selectable.dart';
import 'package:tizen_fs/widgets/selectable_listview.dart';

class LanguagePage extends StatefulWidget {
  const LanguagePage({
    super.key,
    required this.node,
    required this.isEnabled,
    required this.onSelectionChanged,
  });

  final PageNode? node;
  final bool isEnabled;
  final ValueChanged<int>? onSelectionChanged;

  @override
  State<LanguagePage> createState() => _LanguagePageState();
}

class _LanguagePageState extends State<LanguagePage> {
  final GlobalKey<_LanguageListViewState> _listKey =
      GlobalKey<_LanguageListViewState>();

  @override
  void initState() {
    super.initState();
    if (widget.isEnabled) {
      WidgetsBinding.instance.addPostFrameCallback((_) {
        _listKey.currentState?.initFocus();
      });
    }
  }

  @override
  void didUpdateWidget(covariant LanguagePage oldWidget) {
    super.didUpdateWidget(oldWidget);
    if (widget.isEnabled) {
      _listKey.currentState?.initFocus();
    }
  }

  @override
  Widget build(BuildContext context) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      spacing: 10,
      children: [
        SizedBox(
          width: widget.isEnabled ? 600 : 400,
          child: AnimatedPadding(
            duration: $style.times.med,
            padding: widget.isEnabled
                ? const EdgeInsets.fromLTRB(120, 60, 40, 0)
                : const EdgeInsets.fromLTRB(80, 60, 80, 0),
            child: Align(
              alignment: Alignment.topLeft,
              child: Text(
                widget.node?.title ?? 'Language',
                softWrap: true,
                overflow: TextOverflow.visible,
                maxLines: 2,
                style: const TextStyle(fontSize: 35),
              ),
            ),
          ),
        ),
        Expanded(
          child: Align(
            alignment: Alignment.topLeft,
            child: AnimatedPadding(
              duration: $style.times.med,
              padding: widget.isEnabled
                  ? const EdgeInsets.symmetric(horizontal: 80, vertical: 10)
                  : const EdgeInsets.symmetric(horizontal: 40),
              child: AbsorbPointer(
                absorbing: !widget.isEnabled,
                child: _LanguageListView(
                  key: _listKey,
                  onSelectionChanged: widget.onSelectionChanged,
                ),
              ),
            ),
          ),
        ),
      ],
    );
  }
}

class _LanguageListView extends StatefulWidget {
  const _LanguageListView({
    super.key,
    this.onSelectionChanged,
  });

  final ValueChanged<int>? onSelectionChanged;

  @override
  State<_LanguageListView> createState() => _LanguageListViewState();
}

class _LanguageListViewState extends State<_LanguageListView>
    with FocusSelectable<_LanguageListView> {
  int _focusedIndex = 0;
  int _selectedIndex = 0;

  @override
  void initState() {
    super.initState();
    _syncSelectionFromSystem();
  }

  void _syncSelectionFromSystem() {
    final idx = LanguageManager.currentIndex;
    _selectedIndex = idx >= 0 ? idx : 0;
    _focusedIndex = _selectedIndex;
  }

  void initFocus() {
    _syncSelectionFromSystem();
    focusNode.requestFocus();
    listKey.currentState?.selectTo(_focusedIndex);
  }

  Future<void> _applySelection(int index) async {
    setState(() {
      _selectedIndex = index;
      _focusedIndex = index;
    });
    await LanguageManager.applyLanguage(index);
    widget.onSelectionChanged?.call(index);
  }

  @override
  LogicalKeyboardKey getNextKey() => LogicalKeyboardKey.arrowDown;
  @override
  LogicalKeyboardKey getPrevKey() => LogicalKeyboardKey.arrowUp;

  @override
  KeyEventResult onKeyEvent(FocusNode focusNode, KeyEvent event) {
    if (event is KeyDownEvent || event is KeyRepeatEvent) {
      if (event.logicalKey == LogicalKeyboardKey.enter ||
          event.logicalKey == LogicalKeyboardKey.select) {
        _applySelection(_focusedIndex);
        return KeyEventResult.handled;
      }
    }
    return KeyEventResult.ignored;
  }

  @override
  Widget build(BuildContext context) {
    final languages = LanguageManager.languageList;
    return SelectableListView(
      key: listKey,
      itemCount: languages.length,
      scrollDirection: Axis.vertical,
      scrollOffset: 260,
      padding: const EdgeInsets.symmetric(vertical: 10),
      alignment: 0.5,
      onItemFocused: (index) {
        _focusedIndex = index;
      },
      onItemSelected: (index) {
        _focusedIndex = index;
      },
      itemBuilder: (context, index, selectedIndex, key) {
        final bool focused = Focus.of(context).hasFocus && index == selectedIndex;
        final bool checked = index == _selectedIndex;
        final info = languages[index];
        return AnimatedScale(
          key: key,
          scale: focused ? 1.0 : .9,
          duration: $style.times.med,
          curve: Curves.easeInOut,
          child: GestureDetector(
            onTap: () {
              listKey.currentState?.selectTo(index);
              Focus.of(context).requestFocus();
              _applySelection(index);
            },
            child: SizedBox(
              height: 65,
              child: Container(
                decoration: BoxDecoration(
                  borderRadius: BorderRadius.circular(10),
                  color: focused
                      ? Theme.of(context).colorScheme.tertiary
                      : Colors.transparent,
                ),
                child: Padding(
                  padding: const EdgeInsets.symmetric(horizontal: 20),
                  child: Row(
                    children: [
                      Expanded(
                        child: Column(
                          mainAxisAlignment: MainAxisAlignment.center,
                          crossAxisAlignment: CrossAxisAlignment.start,
                          children: [
                            Text(
                              info.name,
                              style: TextStyle(
                                fontSize: 15,
                                color: focused
                                    ? Theme.of(context).colorScheme.onTertiary
                                    : Theme.of(context).colorScheme.tertiary,
                              ),
                            ),
                            Text(
                              info.locale,
                              style: TextStyle(
                                fontSize: 11,
                                color: focused
                                    ? Theme.of(context)
                                        .colorScheme
                                        .onTertiary
                                        .withOpacity(0.7)
                                    : Theme.of(context)
                                        .colorScheme
                                        .tertiary
                                        .withOpacity(0.5),
                              ),
                            ),
                          ],
                        ),
                      ),
                      Icon(
                        checked
                            ? Icons.radio_button_checked
                            : Icons.radio_button_unchecked,
                        color: focused
                            ? Theme.of(context).colorScheme.onTertiary
                            : Theme.of(context).colorScheme.tertiary,
                      ),
                    ],
                  ),
                ),
              ),
            ),
          ),
        );
      },
    );
  }
}



