import 'package:flutter/material.dart';
import 'package:gradient_borders/box_borders/gradient_box_border.dart';
import 'package:tizen_fs/main.dart';
import 'package:tizen_fs/styles/app_style.dart';

class ReviewCard extends StatelessWidget {
  const ReviewCard(
      {super.key,
      this.width = 390,
      this.title,
      this.description,
      this.isSelected = false,
      this.duration,
      this.shadowColor,
      this.content})
      : height = 110;

  static const int animationDuration = 100;
  final double width;
  final double height;
  final String? title;
  final String? description;
  final String? duration;
  final bool isSelected;
  final Color? shadowColor;
  final Widget? content;

  @override
  Widget build(BuildContext context) {
    return Column(
      spacing: 1,
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        AnimatedScale(
          scale: isSelected ? 1.1 : 1,
          duration: Duration(milliseconds: animationDuration),
          child: _buildBorder(
            ClipRRect(
              child: _buildTileContent()
          )),
        )
      ],
    );
  }

  Widget _buildBorder(Widget content) {
    return Container(
      width: width,
      height: height,
      padding : EdgeInsets.all(1),
      decoration: BoxDecoration(
        border: const GradientBoxBorder(
          gradient: LinearGradient(colors: [Colors.blue, Colors.pink]),
          width: 1,
        ),
        borderRadius: BorderRadius.circular(16),
        color: Colors.black.withOpacity(0.5)
      ),
      child: content
    );
  }

  Widget _buildTileContent() {
    if (content != null) {
      return content!;
    } else {
      return Container(
        child: Padding(
          padding: const EdgeInsets.only(left: 20, right: 20, top: 20, bottom: 20),
          child: Column(
            spacing: 10,
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Row(
                spacing: 10,
                children: [
                  Icon(
                    Icons.people_outline,
                    size: 17,
                  ),
                  Text(this.title!, style: const TextStyle(fontSize: 15)),
                  Spacer(),
                  MaterialIconButton(
                    icon: Icons.arrow_forward,
                    isSelected: this.isSelected,
                    width: 20,
                    height: 20,
                  )
                ],
              ),
              Text(
                this.description!,
                maxLines: 2,
                overflow: TextOverflow.ellipsis,
                style: const TextStyle(fontSize: 12)
              ),
            ],
          ),
        ),
      );
    }
  }
}

class MaterialIconButton extends StatelessWidget {
  const MaterialIconButton(
      {super.key,
      required this.icon,
      required this.isSelected,
      required this.height,
      required this.width});

  final bool isSelected;
  final IconData icon;
  final double height;
  final double width;

  @override
  Widget build(BuildContext context) {
    debugPrint('_TabButtonState.build()');
    return Container(
      height: this.height,
      width: this.width,
      child: IconButton(
        padding: EdgeInsets.all(0.0),
        icon: Icon(
          icon,
          size: 15,
          color: isSelected ? $style.colors.surface : $style.colors.onSurface,
        ),
        onPressed: () {
          debugPrint('IconButton pressed');
        },
        style: IconButton.styleFrom(
            backgroundColor: isSelected
                ? $style.colors.onPrimaryContainer.withAlphaF(0.8)
                : $style.colors.onPrimaryContainer.withAlphaF(0.3)),
      ),
    );
  }
}