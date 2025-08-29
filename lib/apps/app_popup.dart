import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:tizen_fs/apps/app_popup_remove.dart';
import 'package:tizen_fs/models/app_data.dart';
import 'package:tizen_fs/styles/app_style.dart';
import 'package:tizen_fs/widgets/app_tile.dart';

class AppPopup extends StatefulWidget {
  const AppPopup({ super.key, required this.app, required this.onRemovePressed});

  final AppData app;
  final VoidCallback? onRemovePressed;
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
        _showAppRemovePopup();
        return KeyEventResult.handled;
      }
    }
    return KeyEventResult.ignored;
  }

  void _showAppRemovePopup() {
    showGeneralDialog(
      context: context,
      barrierDismissible: true,
      barrierLabel: "Close",
      barrierColor: Colors.transparent,
      transitionDuration: $style.times.pageTransition,
      pageBuilder: (BuildContext buildContext, Animation animation, Animation secondaryAnimation) {
        return AppPopupRemove(app: widget.app,
          onConfirm: (){
            widget.onRemovePressed?.call();
            Navigator.of(context).pop();
          }
        );
      },
      transitionBuilder: (context, animation, secondaryAnimation, child) {
        return FadeTransition(
            opacity: animation,
            child: child,
        );
      },
    );
  }

  void _select(int index) {
    setState(() {
      _selected = index;
    });
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
                      Text(widget.app.name)
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
                      PopupButton(
                        text: 'Move',
                        isSelected: _selected == 0,
                        onPressed: (){
                          _select(0);
                        }
                      ),
                      PopupButton(
                        text: 'Uninstall',
                        isSelected: _selected == 1,
                        onPressed: () {
                          _select(1);
                          _showAppRemovePopup();
                        },
                      ),
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
  const PopupButton({super.key, this.isSelected = false, required this.text, required this.onPressed, this.width = 250, this.height= 50});

  final bool isSelected;
  final String text;
  final VoidCallback? onPressed;
  final double width;
  final double height;

  @override
  Widget build(BuildContext context) {
    return GestureDetector(
      onTap: onPressed,
      child: SizedBox(
        width: width,
        height: height,
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
      ),
    );
  }
}