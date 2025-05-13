import 'dart:async';
import 'package:flutter/material.dart';

class FSSearchBar extends StatefulWidget {
  const FSSearchBar({super.key});

  @override
  State<FSSearchBar> createState() => _FSSearchBarState();
}

class _FSSearchBarState extends State<FSSearchBar> with SingleTickerProviderStateMixin {
  late FocusNode _focusNode;
  late AnimationController _blinkController;
  late Animation<double> _blinkAnimation;
  Timer? _blinkDelayTimer;
  bool _startBlink = false;

  @override
  void initState() {
    super.initState();
    _focusNode = FocusNode();
    _blinkController = AnimationController(
      vsync: this,
      duration: const Duration(milliseconds: 1000),
    )..addListener(() {
      setState(() {});
    });

    _blinkAnimation = Tween<double>(begin: 1.0, end: 0.3).animate(
      CurvedAnimation(
        parent: _blinkController,
        curve: Curves.easeInOut,
      ),
    );
  }

  @override
  void dispose() {
    _blinkController.dispose();
    _blinkDelayTimer?.cancel();
    _focusNode.dispose();
    super.dispose();
  }

  void _handleFocusChange() {
    if (_focusNode.hasFocus) {
      _blinkDelayTimer?.cancel();
      _blinkDelayTimer = Timer(const Duration(milliseconds: 1000), () {
        _startBlink = true;
        _blinkController.repeat(reverse: true);
      });
    } else {
      _startBlink = false;
      _blinkDelayTimer?.cancel();
      _blinkController.reset();
    }
    setState(() {});
  }

  @override
  Widget build(BuildContext context) {
    final bool isFocused = _focusNode.hasFocus;
    final double scale = isFocused ? 1.0 : 0.98;
    final Color borderColor = isFocused ? Colors.white.withOpacity(_startBlink? _blinkAnimation.value : 1.0) : Colors.transparent;
    final Color iconAndTextColor = isFocused ? Colors.white : Colors.grey;
    
    return Focus(
      focusNode: _focusNode,
      onFocusChange: (_) => _handleFocusChange(),
      child: GestureDetector(
        onTap: () => _focusNode.requestFocus(),
        child: AnimatedScale(
          scale: scale,
          duration: const Duration(milliseconds: 200),
          curve: Curves.easeInOut,
          child: Container(
            height: 60,
            padding: const EdgeInsets.symmetric(horizontal: 20),
            decoration: BoxDecoration(
              borderRadius: BorderRadius.circular(32),
              border: Border.all(color: borderColor, width: 2),
              color: Colors.black.withOpacity(0.3),
              ),
            child: Row(
              children: [
                Icon(Icons.search, color: iconAndTextColor),
                const SizedBox(width: 10),
                Text(
                  'Search films, cast and more',
                  style: TextStyle(
                    color: iconAndTextColor,
                    fontSize: 18,
                  ),
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }
}