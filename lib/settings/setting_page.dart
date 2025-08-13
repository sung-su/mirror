import 'package:flutter/material.dart';
import 'package:tizen_fs/models/page_node.dart';
import 'package:tizen_fs/widgets/setting_list_view.dart';

class SettingPage extends StatefulWidget {
  const SettingPage({
    super.key,
    required this.node,
    required this.isEnabled,
    required this.onSelectionChanged,
  });

  final PageNode? node;
  final bool isEnabled;
  final Function(int)? onSelectionChanged;

  @override
  State<SettingPage> createState() => _SettingPageState();
}

class _SettingPageState extends State<SettingPage>
    with AutomaticKeepAliveClientMixin {
  GlobalKey<SettingListViewState> _listKey = GlobalKey<SettingListViewState>();

  @override
  bool get wantKeepAlive => widget.isEnabled;

  @override
  void initState() {
    super.initState();

    if (widget.isEnabled) {
      WidgetsBinding.instance.addPostFrameCallback((_) {
        initFocus();
      });
    }
  }

  void initFocus() {
    _listKey.currentState?.initFocus();
  }

  @override
  void didUpdateWidget(covariant SettingPage oldWidget) {
    super.didUpdateWidget(oldWidget);
    if (widget.isEnabled) {
      initFocus();
    }
  }

  @override
  Widget build(BuildContext context) {
    super.build(context);

    if (widget.node == null) {
      return Container (
        color: Theme.of(context).colorScheme.onTertiary
      );
    }

    if (widget.node!.builder != null) {
      return Container(
        color: widget.isEnabled ? Theme.of(context).colorScheme.surface : Theme.of(context).colorScheme.onTertiary,
        child: widget.node!.builder?.call(context, widget.node!, widget.isEnabled)
      );
    }

    double titleHeight = 100;
    double titleFontSize = 30;

    return Container(
      color:
          widget.isEnabled
              ? Theme.of(context).colorScheme.surface
              : Theme.of(context).colorScheme.onTertiary,
      child: Column(
        children: [
          //title
          Padding(
            padding: widget.isEnabled ? EdgeInsets.fromLTRB(80, 20, 0, 10) : EdgeInsets.fromLTRB(60, 20, 0, 10),
            child: SizedBox(
              height: titleHeight,
              child: Align(
                  alignment: Alignment.bottomLeft,
                  child: Container(
                    child: Text(
                        widget.node?.title ?? '',
                        style: TextStyle(fontSize: titleFontSize),
                    ),
                ),
              ),
            ),
          ),
          //list
          if (!widget.node!.children.isEmpty)
            Expanded(
              child: Align(
                alignment: Alignment.topLeft,
                child: Padding(
                  padding:
                      widget.isEnabled
                          ? const EdgeInsets.symmetric(horizontal: 40)
                          : const EdgeInsets.symmetric(horizontal: 20),
                  child: SettingListView(
                    key: _listKey,
                    node: widget.node!,
                    onSelectionChanged: (selected) {
                      widget.onSelectionChanged?.call(selected);
                    },
                  ),
                ),
              ),
            ),
        ],
      ),
    );
  }
}
