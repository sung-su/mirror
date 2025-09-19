import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:tizen_fs/models/page_node.dart';
import 'package:tizen_fs/styles/app_style.dart';
import 'package:tizen_fs/utils/language_manager.dart';

class LanguageInputPage extends StatefulWidget {
  const LanguageInputPage({
    super.key,
    required this.node,
    required this.isEnabled,
    required this.onSelectionChanged,
  });

  final PageNode? node;
  final bool isEnabled;
  final ValueChanged<int>? onSelectionChanged;

  @override
  State<LanguageInputPage> createState() => _LanguageInputPageState();
}

class _LanguageInputPageState extends State<LanguageInputPage> {
  late List<_LanguageInputEntry> _entries;

  @override
  void initState() {
    super.initState();
    _entries = _buildEntries();
  }

  @override
  void didUpdateWidget(covariant LanguageInputPage oldWidget) {
    super.didUpdateWidget(oldWidget);
    setState(() {
      _entries = _buildEntries();
    });
  }

  List<_LanguageInputEntry> _buildEntries() {
    final displayName = LanguageManager.currentName;
    return <_LanguageInputEntry>[
      _LanguageInputEntry.item(
        title: 'Display language',
        subtitle: displayName,
        childIndex: 0,
      ),
      const _LanguageInputEntry.header(title: 'Keyboard'),
      const _LanguageInputEntry.item(
        title: 'Keyboard',
        childIndex: 1,
      ),
      const _LanguageInputEntry.header(title: 'Input assistance'),
      const _LanguageInputEntry.item(
        title: 'Autofill service',
        childIndex: 2,
      ),
      const _LanguageInputEntry.header(title: 'Speech'),
      const _LanguageInputEntry.item(
        title: 'Voice control',
        childIndex: 3,
      ),
      const _LanguageInputEntry.item(
        title: 'Text-to-speech',
        childIndex: 4,
      ),
      const _LanguageInputEntry.item(
        title: 'Speech-to-text',
        childIndex: 5,
      ),
    ];
  }

  void _handleSelect(_LanguageInputEntry entry) {
    if (entry.childIndex == null) {
      return;
    }
    widget.onSelectionChanged?.call(entry.childIndex!);
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
                widget.node?.title ?? 'Language & Input',
                softWrap: true,
                overflow: TextOverflow.visible,
                maxLines: 2,
                style: const TextStyle(fontSize: 35),
              ),
            ),
          ),
        ),
        Expanded(
          child: AnimatedOpacity(
            duration: $style.times.med,
            opacity: widget.isEnabled ? 1.0 : 0.6,
            child: ListView.builder(
              padding: widget.isEnabled
                  ? const EdgeInsets.symmetric(horizontal: 80, vertical: 10)
                  : const EdgeInsets.symmetric(horizontal: 40, vertical: 10),
              itemCount: _entries.length,
              itemBuilder: (context, index) {
                final entry = _entries[index];
                if (entry.isHeader) {
                  return Padding(
                    padding: const EdgeInsets.only(top: 18, bottom: 6),
                    child: Text(
                      entry.title,
                      style: TextStyle(
                        fontSize: 13,
                        color: Theme.of(context)
                            .colorScheme
                            .tertiary
                            .withOpacity(0.8),
                      ),
                    ),
                  );
                }

                return _LanguageInputTile(
                  title: entry.title,
                  subtitle: entry.subtitle,
                  enabled: widget.isEnabled,
                  autofocus: entry.childIndex == LanguageManager.currentIndex,
                  onSelect: () => _handleSelect(entry),
                );
              },
            ),
          ),
        ),
      ],
    );
  }
}

class _LanguageInputTile extends StatefulWidget {
  const _LanguageInputTile({
    required this.title,
    this.subtitle,
    required this.enabled,
    this.autofocus = false,
    required this.onSelect,
  });

  final String title;
  final String? subtitle;
  final bool enabled;
  final bool autofocus;
  final VoidCallback onSelect;

  @override
  State<_LanguageInputTile> createState() => _LanguageInputTileState();
}

class _LanguageInputTileState extends State<_LanguageInputTile> {
  late final FocusNode _focusNode;

  @override
  void initState() {
    super.initState();
    _focusNode = FocusNode();
  }

  @override
  void dispose() {
    _focusNode.dispose();
    super.dispose();
  }

  KeyEventResult _handleKey(FocusNode node, KeyEvent event) {
    if (!widget.enabled) {
      return KeyEventResult.ignored;
    }
    if (event is KeyDownEvent || event is KeyRepeatEvent) {
      if (event.logicalKey == LogicalKeyboardKey.enter ||
          event.logicalKey == LogicalKeyboardKey.select) {
        widget.onSelect();
        return KeyEventResult.handled;
      }
    }
    return KeyEventResult.ignored;
  }

  @override
  Widget build(BuildContext context) {
    return Focus(
      focusNode: _focusNode,
      autofocus: widget.autofocus,
      canRequestFocus: widget.enabled,
      onKeyEvent: _handleKey,
      child: Builder(
        builder: (context) {
          final hasFocus = Focus.of(context).hasFocus && widget.enabled;
          return GestureDetector(
            onTap: widget.enabled ? widget.onSelect : null,
            behavior: HitTestBehavior.opaque,
            child: AnimatedContainer(
              duration: $style.times.med,
              curve: Curves.easeInOut,
              margin: const EdgeInsets.symmetric(vertical: 6),
              padding: const EdgeInsets.symmetric(horizontal: 20, vertical: 14),
              decoration: BoxDecoration(
                borderRadius: BorderRadius.circular(10),
                color: hasFocus
                    ? Theme.of(context).colorScheme.tertiary
                    : Colors.transparent,
              ),
              child: Row(
                children: [
                  Expanded(
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      mainAxisSize: MainAxisSize.min,
                      children: [
                        Text(
                          widget.title,
                          style: TextStyle(
                            fontSize: 15,
                            color: hasFocus
                                ? Theme.of(context).colorScheme.onTertiary
                                : Theme.of(context).colorScheme.tertiary,
                          ),
                        ),
                        if (widget.subtitle != null && widget.subtitle!.isNotEmpty)
                          Padding(
                            padding: const EdgeInsets.only(top: 4),
                            child: Text(
                              widget.subtitle!,
                              style: TextStyle(
                                fontSize: 12,
                                color: hasFocus
                                    ? Theme.of(context)
                                        .colorScheme
                                        .onTertiary
                                        .withOpacity(0.75)
                                    : Theme.of(context)
                                        .colorScheme
                                        .tertiary
                                        .withOpacity(0.6),
                              ),
                            ),
                          ),
                      ],
                    ),
                  ),
                  Icon(
                    Icons.chevron_right,
                    color: hasFocus
                        ? Theme.of(context).colorScheme.onTertiary
                        : Theme.of(context).colorScheme.tertiary,
                  ),
                ],
              ),
            ),
          );
        },
      ),
    );
  }
}

class _LanguageInputEntry {
  const _LanguageInputEntry._({
    required this.title,
    this.subtitle,
    this.childIndex,
    required this.isHeader,
  });

  const _LanguageInputEntry.header({required String title})
      : this._(title: title, isHeader: true);

  const _LanguageInputEntry.item({
    required String title,
    String? subtitle,
    required int childIndex,
  }) : this._(
          title: title,
          subtitle: subtitle,
          childIndex: childIndex,
          isHeader: false,
        );

  final String title;
  final String? subtitle;
  final int? childIndex;
  final bool isHeader;
}
