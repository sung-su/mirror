import 'dart:async';
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:tizen_fs/styles/app_style.dart';
import 'package:video_player/video_player.dart';

class VideoBackdrop extends StatefulWidget {
  const VideoBackdrop({
    super.key,
    required this.scrollController,
    required this.imageUrl,
    required this.videoUrl,
    }
  );

  final ScrollController scrollController;
  final String imageUrl;
  final String videoUrl;

  @override
  State<VideoBackdrop> createState() => _VideoBackdropState();
}

class _VideoBackdropState extends State<VideoBackdrop> {
  late VideoPlayerController _videoController;

  Timer? _inactivityTimer;
  bool _isIdle = true;

  // Status - 0: background image, 1: video start, 2: scroll down content
  int _statusIndex = 0;

  Duration _getDuration() {
    return (_statusIndex == 0) ? const Duration(seconds: 3) : const Duration(seconds: 5);
  }

  void _startInactivityTimer() {
    _inactivityTimer?.cancel();
    _inactivityTimer = Timer(_getDuration(), _advanceState);
  }

  void _advanceState() {
    if (_statusIndex == 0) {
      debugPrint('state0 : image');
      setState(() {
        _statusIndex++;
      });
      if(_isIdle)
        _startInactivityTimer();
    } else if (_statusIndex == 1) {
      debugPrint('state1 : video start');
      _videoController.initialize().then((_) {
        setState(() {
          _statusIndex++;
          _videoController.play();
        });
        if(_isIdle)
          _startInactivityTimer();
      });
    } else if (_statusIndex == 2) {
      debugPrint('state2 : scroll down content');
      widget.scrollController.animateTo(0, duration: const Duration(milliseconds: 500), curve: Curves.ease);
      _inactivityTimer?.cancel();
    } else {
      _inactivityTimer?.cancel();
    }
  }

  void _handleUserInteraction() {
    // TODO : _isIdle = false; _statusIndex = 0;
  }

  @override
  void initState() {
    super.initState();

    debugPrint('initialize video');
    _videoController = VideoPlayerController.asset(widget.videoUrl)
      ..setLooping(true);

    widget.scrollController.addListener(_handleUserInteraction);

    _startInactivityTimer();
  }

  @override
  void dispose() {
    _inactivityTimer?.cancel();
    _videoController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Focus(
      onKeyEvent: (node, event) {
        if (event is KeyDownEvent) {
          if (event.logicalKey == LogicalKeyboardKey.arrowUp) {
            debugPrint('key evnet: arrowUP');
            widget.scrollController.animateTo(0, duration: const Duration(milliseconds: 500), curve: Curves.ease);
            return KeyEventResult.handled;
          }
          else if (event.logicalKey == LogicalKeyboardKey.arrowDown) {
            debugPrint('key evnet: arrowdown');
            widget.scrollController.animateTo(250, duration: const Duration(milliseconds: 500), curve: Curves.ease);
            return KeyEventResult.handled;
          }
        }
        return KeyEventResult.ignored;
      },
      child: Stack(
        children: [
          SizedBox.expand(
              child: DecoratedBox(
                  decoration: BoxDecoration(
                      color: const Color.fromARGB(255, 18, 18, 18)))),
          SizedBox.expand(
              child: (_isIdle && _videoController.value.isInitialized) ?
              AspectRatio(aspectRatio: _videoController.value.aspectRatio,
                  child: VideoPlayer(_videoController),
                )
              : Image.network(widget.imageUrl,
                  fit: BoxFit.cover)),
          Container(
            decoration: BoxDecoration(
              gradient: LinearGradient(
                begin: Alignment.bottomLeft,
                end: Alignment.topRight,
                colors: (_statusIndex == 2 ) ? [Colors.transparent, Colors.transparent]
                :[Colors.black.withAlphaF(0.9), Colors.transparent],
              ),
            ),
          )
        ],
      ),
    );
  }
}