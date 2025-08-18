import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:tizen_fs/models/app_info.dart';
import 'package:tizen_fs/widgets/app_tile.dart';

class AppPopup extends StatefulWidget {
  const AppPopup({ super.key, required this.app});

  final AppInfo app;
  @override
  State<AppPopup> createState() => _AppPopupState();
}

class _AppPopupState extends State<AppPopup> {

  int _selected = 0;


  KeyEventResult _onKeyEvent(FocusNode node, KeyEvent event) {
    if (event is KeyDownEvent) {
      if (event.logicalKey == LogicalKeyboardKey.arrowDown) {
        setState(() {
          _selected = (_selected + 1).clamp(0, 1);
        });
        return KeyEventResult.handled;
      } 
      else if (event.logicalKey == LogicalKeyboardKey.arrowUp) {
        setState(() {
          _selected = (_selected - 1).clamp(0, 1);
        });
        return KeyEventResult.handled;
      } 
      else if (event.logicalKey == LogicalKeyboardKey.enter) {
        return KeyEventResult.handled;
      }
    }
    return KeyEventResult.ignored;
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: Focus(
        autofocus: true,
        onKeyEvent: _onKeyEvent,
        child: Container(
          child: Row(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Padding(
                padding: const EdgeInsets.fromLTRB(0, 100, 0, 0),
                child: Container(
                  width: 500,
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.center,
                    spacing: 10,
                    children: [
                      SizedBox(
                        width: 152 * 1.3,
                        height: 86 * 1.3,
                        child: AppTile(app: widget.app)
                      ),
                    ]
                  ),
                ),
              ),
              Expanded(
                child: Padding(
                  padding: const EdgeInsets.fromLTRB(0, 100, 100, 0),
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.end,
                    spacing: 20,
                    children: [
                      PopupButton(text: 'Move', isSelected: _selected == 0,),
                      PopupButton(text: 'Uninstall', isSelected: _selected == 1,),
                    ]
                  ),
                ),
              )
            ],
          ),
        )
      )
    );
  }
}

class PopupButton extends StatelessWidget{
  const PopupButton({super.key, this.isSelected = false, required this.text});

  final bool isSelected;
  final String text;

  @override
  Widget build(BuildContext context) {
    return SizedBox(
      width: 250,
      height: 50,
      child: AnimatedScale(
        scale: isSelected ? 1.1 : 1,
        duration: const Duration(milliseconds: 80),
        child: Container(
          decoration: BoxDecoration(
            borderRadius: BorderRadius.circular(30),
            color: isSelected ? Theme.of(context).colorScheme.primary : Theme.of(context).colorScheme.surfaceContainerHighest,
          ),
          child: Padding(
            padding: const EdgeInsets.symmetric(horizontal: 30, vertical: 5),
            child: Row(
              mainAxisAlignment: MainAxisAlignment.start,
              spacing: 15,
              children: [
                Column(
                  mainAxisAlignment: MainAxisAlignment.center,
                  crossAxisAlignment: CrossAxisAlignment.start,
                  spacing: 3,
                  children: [
                    Text(
                      text,
                      style: TextStyle(
                        fontSize: 13,
                        color: isSelected ? Theme.of(context).colorScheme.onPrimary : Theme.of(context).colorScheme.primary,
                      )
                    ),
                  ],
                ),
              ],
            ),
          )
        ),
      ),
    );
  }
}