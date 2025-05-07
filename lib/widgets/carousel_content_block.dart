import 'package:flutter/material.dart';
import 'package:tizen_fs/widgets/immersive_carousel.dart';

class ContentBlock extends StatelessWidget {
  final ImmersiveCarouselContent item;
  final bool isFocused;

  const ContentBlock({
    super.key,
    required this.item,
    required this.isFocused,
  });

  @override
  Widget build(BuildContext context) {
    return SizedBox(
      width: 484,
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        mainAxisSize: MainAxisSize.min,
        children: [
          SizedBox(
            height: 20,
            child: Text(item.overline, style: const TextStyle(color: Colors.white70))
          ),
          const SizedBox(height: 4),
          Text (item.title, style: const TextStyle(color: Colors.white, fontSize: 32, fontWeight: FontWeight.bold)),
          const SizedBox(height: 20),
          SizedBox(
            height: 40,
            child: Text(item.description, maxLines: 2, overflow: TextOverflow.ellipsis, style: const TextStyle(color: Colors.white70)),
          ),
          AnimatedSize(
            duration: const Duration(milliseconds: 100),
            child: Column(
              mainAxisSize: MainAxisSize.min,
              children: [
                SizedBox(height: isFocused ? 28 : 0),
                AnimatedOpacity(
                  opacity: isFocused ? 1.0 : 0.0,
                  duration: const Duration(milliseconds: 100),
                  child: SizedBox(
                    height: isFocused ? 40 : 0,
                    child:
                      TextButton(
                        onPressed: isFocused ? () {} : null,
                        focusNode: isFocused ? null : AlwaysDisabledFocusNode(),
                        style: ElevatedButton.styleFrom(
                          backgroundColor: Colors.white,
                          padding: const EdgeInsets.symmetric(horizontal: 24),
                          shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(20)),
                        ),
                        child: Text(item.buttonText, style: const TextStyle(color: Colors.black, fontSize: 16)),
                      )
                    ),
                  ),
              ],
            )
          ),
        ]
      )
    );
  }
}

class AlwaysDisabledFocusNode extends FocusNode {
  @override
  bool get hasFocus => false;
}