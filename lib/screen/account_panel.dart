import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:tizen_fs/router.dart';
import 'package:tizen_fs/styles/app_style.dart';
import 'package:tizen_fs/widgets/top_menu_avatar_item.dart';

class AccountPanel extends StatefulWidget {
  const AccountPanel({super.key});

  @override
  State<AccountPanel> createState() => _AccountPanelState();
}

class _AccountPanelState extends State<AccountPanel> {
  bool _iconSelected = true;
  bool _buttonSelected = false;

  @override
  Widget build(BuildContext context) {
    debugPrint('AccountPanel build');
    return Material(
      type: MaterialType.transparency,
      child: Container(
        color: Colors.black.withAlphaF(0.7),
        child: FocusScope(
          autofocus: true,
          onKeyEvent: (node, event) {
            if (event is KeyDownEvent && event.logicalKey == LogicalKeyboardKey.arrowRight) {
              debugPrint('[onKeyEvent] LogicalKeyboardKey.escape');
              Navigator.of(context).pop();
              return KeyEventResult.handled;
            } else if (event is KeyDownEvent &&
                event.logicalKey == LogicalKeyboardKey.enter) {
              debugPrint('[onKeyEvent] LogicalKeyboardKey.enter');
              AppRouter.router.push(ScreenPaths.poc);
              return KeyEventResult.handled;
            } else if (event is KeyDownEvent && event.logicalKey == LogicalKeyboardKey.arrowDown) {
              debugPrint('[onKeyEvent] LogicalKeyboardKey.arrowDown');
              if(_iconSelected) {
                setState(() {
                  _iconSelected = false;
                  _buttonSelected = true;
                });
              }
              return KeyEventResult.handled;
            } else if (event is KeyDownEvent && event.logicalKey == LogicalKeyboardKey.arrowUp) {
              debugPrint('[onKeyEvent] LogicalKeyboardKey.arrowUp');
              if(_buttonSelected) {
                setState(() {
                  _iconSelected = true;
                  _buttonSelected = false;
                });
              }
              return KeyEventResult.handled;
            }
            return KeyEventResult.ignored;
          },
          child: Column(
            mainAxisAlignment: MainAxisAlignment.start,
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Padding(
                padding: const EdgeInsets.fromLTRB(59, 35, 0, 0),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    TopMenuAvatarItem(
                      imageUrl: null,
                      text: 'QA',
                      isSelected: _iconSelected,
                    ),
                    SizedBox(
                      height: 15,
                    ),
                    Text('QA', style: TextStyle(fontSize: 15)),
                    SizedBox(
                      height: 10,
                    )
                  ],
                ),
              ),
              Padding(
                padding: const EdgeInsets.fromLTRB(52, 0, 0, 0),
                child: TextButton (
                  onPressed: () { print('Add an account'); },
                  style: TextButton.styleFrom(
                    backgroundColor: _buttonSelected ? Colors.white : Colors.transparent,
                    foregroundColor: _buttonSelected ? Colors.black : Colors.white,
                    animationDuration: const Duration(milliseconds: 100),
                  ),
                  child: const Text(
                    'Add an account',
                    style: TextStyle(
                      fontSize: 12,
                    ),
                  ),
                ),
              )
            ]
          )
        )
      ),
    );
  }
}