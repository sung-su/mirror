import 'package:flutter/material.dart';
import 'package:tizen_fs/styles/app_style.dart';

class TopMenuAvatarItem extends StatelessWidget {
  const TopMenuAvatarItem(
      {super.key, this.imageUrl, this.isSelected = false, this.text});
  final bool isSelected;
  final String? imageUrl;
  final String? text;

  @override
  Widget build(BuildContext context) {
    return Container(
        width: 30,
        height: 30,
        decoration: BoxDecoration(
          shape: BoxShape.circle,
          boxShadow:[
            BoxShadow(
              color: isSelected ? $style.colors.onSurface : Colors.transparent,
              spreadRadius: 2,
            )
          ]
        ),
        child: GestureDetector(
            onTap: () {
            },
            child: CircleAvatar(
              backgroundColor: Colors.brown.shade800,
              backgroundImage:
                  imageUrl != null ? NetworkImage(imageUrl!) : null,
              child: (imageUrl == null && text != null) ? Text(text!) : null,
            )));
  }
}