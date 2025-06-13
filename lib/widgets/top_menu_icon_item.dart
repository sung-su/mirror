import 'package:flutter/material.dart';
import 'package:tizen_fs/styles/app_style.dart';

class TopMenuIconItem extends StatelessWidget {
  const TopMenuIconItem(
      {super.key,
      required this.icon,
      required this.isSelected,
      required this.hasFocus});
  final bool isSelected;
  final IconData icon;
  final bool hasFocus;

  @override
  Widget build(BuildContext context) {
    return Container(
      height: 30,
      width: 30,
      decoration: BoxDecoration(
        shape: BoxShape.circle,
        boxShadow: [
          BoxShadow(
            color: isSelected ? $style.colors.onSurface : Colors.transparent,
            spreadRadius: 1,
          )
        ]
      ),
      child: IconButton(
        padding: EdgeInsets.all(0.0),
        icon: Icon(
          icon,
          size: 17,
          color: isSelected ? $style.colors.surface : $style.colors.onSurface,
        ),
        onPressed: () {
          debugPrint('IconButton pressed');
        },
        style: IconButton.styleFrom(
            backgroundColor: isSelected
                ? (hasFocus
                    ? $style.colors.onPrimaryContainer.withAlphaF(0.8)
                    : $style.colors.onPrimaryContainer.withAlphaF(0.3))
                : $style.colors.onPrimaryContainer.withAlphaF(0.3)),
      ),
    );
  }
}
