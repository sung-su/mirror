import 'dart:async';
import 'dart:ui';
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:tizen_fs/styles/app_style.dart';
import 'package:video_player/video_player.dart';

class VideoBackdrop extends StatefulWidget {
  const VideoBackdrop({
    super.key,
    required this.imageUrl,
    required this.videoUrl,
    this.onStatusChanged,
    }
  );

  final void Function(int)? onStatusChanged;
  final String imageUrl;
  final String videoUrl;

  @override
  State<VideoBackdrop> createState() => VideoBackdropState();
}

class VideoBackdropState extends State<VideoBackdrop> {
  late VideoPlayerController _videoController;

  Timer? _inactivityTimer;
  bool _isIdle = true;
  bool _isEffectOn = false;

  // State - 0: initial background image, 1: video start, 2: expand video, 3: backdrop image, 4: blur effect
  int _statusIndex = 0;
  int get status => _statusIndex;
  set status(int value) {
    setState(() {
      _statusIndex = value;

      if(_statusIndex == 3) {
        _isIdle = false;
        _isEffectOn = false;
        _inactivityTimer?.cancel();
      } else if (_statusIndex == 4) {
        _isIdle = false;
        _isEffectOn = true;
        _inactivityTimer?.cancel();
      } else {
        _isIdle = true;
        _isEffectOn = false;
      }
    });
  }

  Duration _getDuration() {
    return (_statusIndex == 0) ? const Duration(seconds: 0) : const Duration(seconds: 5);
  }
  void _startInactivityTimer() {
    _inactivityTimer?.cancel();
    _inactivityTimer = Timer(_getDuration(), _advanceState);
  }

  void _callStatusChanged() {
    widget.onStatusChanged?.call(status);
  }

  void _advanceState() {
    if (status == 0) {
      debugPrint('state0 : image');
      _callStatusChanged();
      status = 1;
      _startInactivityTimer();
    }
    else if (status == 1) {
      debugPrint('state1 : video start');
      _callStatusChanged();
      _videoController.initialize().then((_) {
        status = 2;
        _videoController.play();
        _startInactivityTimer(); 
      });
    }
    else if (status == 2) {
      debugPrint('state2 : expand video');
      _callStatusChanged();
      _inactivityTimer?.cancel();
    }
    else {
      _inactivityTimer?.cancel();
      _callStatusChanged();
    }    
  }

  bool _isNetworkUrl(String input) {
    final RegExp urlRegex = RegExp(r'^(https?:)?\/\/[^\s]+$');
    return urlRegex.hasMatch(input);
  }

  @override
  void initState() {
    super.initState();

    _videoController = _isNetworkUrl(widget.videoUrl)
    ? VideoPlayerController.networkUrl(Uri.parse(widget.videoUrl))
    : VideoPlayerController.asset(widget.videoUrl)
      ..setLooping(false);

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
    return Stack(
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
            : (_isEffectOn) ? 
            ImageFiltered(
              imageFilter: ImageFilter.blur(
                sigmaX: 5,
                sigmaY: 5,
              ),
              child: Image.network(
                widget.imageUrl,
                fit: BoxFit.cover
              ),
            )
            : Image.network(
              widget.imageUrl,
              fit: BoxFit.cover
              )
            ),
        Container(
          decoration: BoxDecoration(
            gradient: LinearGradient(
              begin: Alignment.bottomLeft,
              end: Alignment.topRight,
              colors: (status == 2 ) ? [Colors.transparent, Colors.transparent]
              :[Colors.black.withAlphaF(0.9), Colors.transparent],
            ),
          ),
        )
      ],
    );
  }
}