import 'package:flutter/material.dart';
import 'package:tizen_fs/styles/app_style.dart';


class TopMenuButtonItem extends StatelessWidget {
  const TopMenuButtonItem({super.key,
      required this.text,
      required this.isSelected,
      required this.isFocused,
      this.onPressed});
  final bool isSelected;
  final bool isFocused;
  final String text;
  final void Function()? onPressed;

  @override
  Widget build(BuildContext context) {
    return GestureDetector(
      onTap: () => onPressed?.call(),
      child: Container(
        decoration: BoxDecoration(
          color: isSelected
              ? (isFocused
                  ? Colors.white.withAlphaF(0.9)
                  : Colors.white.withAlphaF(0.15))
              : Colors.transparent,
          borderRadius: isSelected ? BorderRadius.circular(30) : BorderRadius.zero,
        ),
        child: Padding(
          padding: const EdgeInsets.symmetric(horizontal: 17, vertical: 10),
          child: Text(
            text,
            style: TextStyle(
              fontSize: 16,
              color: isFocused
                ? (isSelected
                    ? $style.colors.surface
                    : $style.colors.onSurface)
                : $style.colors.onSurface.withAlphaF(0.8),
            ),
          ),
        ),
      ),
    );
  }
}
