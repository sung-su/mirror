import 'dart:async';

import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:tizen_fs/styles/app_style.dart';
import 'package:video_player/video_player.dart';

class VideoPlayerPage extends StatefulWidget {
  const VideoPlayerPage({
    super.key,
    required this.title,
    required this.videoUrl,
    }
  );

  final String title;
  final String videoUrl;


  @override
  State<VideoPlayerPage> createState() => _VideoPlayerPageState();
}

class _VideoPlayerPageState extends State<VideoPlayerPage> with SingleTickerProviderStateMixin, WidgetsBindingObserver  {
  late VideoPlayerController _controller;
  final FocusNode _focusNode = FocusNode();
  late AnimationController _iconAnimationController;
  late Animation<double> _iconScaleAnimation;
  late Animation<double> _iconFadeAnimation;

  Timer? _overlayTimer;
  String? _seekDirection; // 'forward' or 'backward'
  bool _showControls = false;
  Duration _currentPosition = Duration.zero;
  bool _isSeeking = false;
  bool _isBuffering = false;

  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addObserver(this);

    _controller = _isNetworkUrl(widget.videoUrl)
    ? VideoPlayerController.networkUrl(Uri.parse(widget.videoUrl))
    : VideoPlayerController.asset(widget.videoUrl)
    ..initialize().then((_) {
        setState(() {
          _showControls = true;
        });
        _controller.play();
      });

    _iconAnimationController = AnimationController(
      duration: Duration(milliseconds: 100),
      vsync: this,
    );
    _iconScaleAnimation = Tween<double>(begin: 1.0, end: 1.2).animate(
      CurvedAnimation(parent: _iconAnimationController, curve: Curves.easeOut)
    );
    _iconFadeAnimation = Tween<double>(begin: 0.0, end: 1.0).animate(
      CurvedAnimation(parent: _iconAnimationController, curve: Curves.easeIn)
    );

    _controller.addListener(() => setState(() {}));

    WidgetsBinding.instance.addPostFrameCallback((_) {
      _focusNode.requestFocus();
    });
  }

  @override
  void didChangeAppLifecycleState(AppLifecycleState state) async {
    if (state == AppLifecycleState.paused) {
      if (_controller.value.isPlaying) {
        _controller.pause();
      }
    } else if (state == AppLifecycleState.resumed) {
      if (!_controller.value.isInitialized) {
        await _controller.initialize();
      }
      _controller.play();
      await _controller.seekTo(_currentPosition);
      setState(() {
        _showControls = true;
      });
    }
  }

  @override
  void dispose() {
    WidgetsBinding.instance.removeObserver(this);
    _overlayTimer?.cancel();
    _controller.dispose();
    _focusNode.dispose();
    _iconAnimationController.dispose();
    super.dispose();
  }

  void _togglePlayPause() {
    if (_controller.value.isPlaying) {
      _controller.pause();
      setState (() {
        _showControls = false;
        _seekDirection = 'pause';
      });
    } else {
      _controller.play();
      setState (() {
        _showControls = true;
        _seekDirection = 'play';
      });
    }

    _overlayTimer?.cancel();
    _iconAnimationController.forward(from:0.0);
    _overlayTimer = Timer(Duration(seconds: 1), () {
      setState(() {
        _seekDirection = null;
      });
    });
  }

  String _formatDuration(Duration duration) {
    String twoDigits(int n) => n.toString().padLeft(2, '0');
    final minutes = twoDigits(duration.inMinutes.remainder(60));
    final seconds = twoDigits(duration.inSeconds.remainder(60));
    return "$minutes:$seconds";
  }

  bool _isSelectKey(KeyEvent event) {
    return event.logicalKey == LogicalKeyboardKey.select ||
        event.logicalKey == LogicalKeyboardKey.enter ||
        event.logicalKey == LogicalKeyboardKey.gameButtonA;
  }

  void _forward(bool forward) {
    if (_controller.value.isPlaying)
      _controller.pause();

    _isSeeking = true;
    _currentPosition = _currentPosition + Duration(seconds: forward ? 10 : -10);

    setState(() {
      _seekDirection = forward ? 'forward' : 'backward';
    });
    _overlayTimer?.cancel();
    _iconAnimationController.forward(from:0.0);
    _overlayTimer = Timer(Duration(seconds: 1), () {
      setState(() {
        _seekDirection = null;
      });
    });
  }

  void _seek() async {
    setState(() {
      _isBuffering = true;
    });

    await _controller.seekTo(_currentPosition);

    if (!_controller.value.isPlaying)
      _controller.play();

    setState(() {
      _isBuffering = false;
    });

    _isSeeking = false;
  }

  KeyEventResult _onKeyEvent(FocusNode focusNode, KeyEvent event) {
    if (!_isBuffering) {
      if (event is KeyDownEvent || event is KeyRepeatEvent) {
        if (event.logicalKey == LogicalKeyboardKey.arrowRight) {
          _forward(true);
          return KeyEventResult.handled;
        } else if (event.logicalKey == LogicalKeyboardKey.arrowLeft) {
          _forward(false);
          return KeyEventResult.handled;
        } else if (_isSelectKey(event)) {
          if (!_isSeeking)
            _togglePlayPause();
          return KeyEventResult.handled;
        }
      } else if (event is KeyUpEvent)
      {
        if (event.logicalKey == LogicalKeyboardKey.arrowRight
        || event.logicalKey == LogicalKeyboardKey.arrowLeft) {
          _seek();
          return KeyEventResult.handled;
        }
      }
    }

    return KeyEventResult.ignored;
  }

  IconData _getIconData()
  {
    return switch (_seekDirection) {
      'forward' => Icons.forward_10,
      'backward' => Icons.replay_10,
      'pause' => Icons.pause,
      _ => Icons.play_arrow,
    };
  }

  bool _isNetworkUrl(String input) {
    final RegExp urlRegex = RegExp(r'^(https?:)?\/\/[^\s]+$');
    return urlRegex.hasMatch(input);
  }

  @override
  Widget build(BuildContext context) {
    final position = _isSeeking ? _currentPosition : _controller.value.position;

    return Focus(
      focusNode: _focusNode,
      onKeyEvent: _onKeyEvent,
      autofocus: true,
      child: Scaffold(
        backgroundColor: Colors.black,
        body: _controller.value.isInitialized
            ? Stack(
                children: [
                  Center(
                    child: AspectRatio(
                      aspectRatio: _controller.value.aspectRatio,
                      child: VideoPlayer(_controller),
                    ),
                  ),
                  if(_isBuffering)
                    Center(
                      child: CircularProgressIndicator(),
                    ),
                  if (!_showControls)
                    Center(
                      child: Container(
                        decoration: BoxDecoration(
                          gradient: LinearGradient(
                            begin: Alignment.topCenter,
                            end: Alignment.bottomCenter,
                            colors: [
                              Colors.transparent,
                              Colors.black.withAlphaF(0.9)
                            ]
                          )
                        ),
                      ),
                    ),
                  if (_seekDirection != null)
                    Center(
                      child: FadeTransition(
                        opacity: _iconFadeAnimation,
                        child: ScaleTransition(
                          scale: _iconScaleAnimation,
                          child: SizedBox(
                            width: 70,
                            height: 70,
                            child: IconButton(
                              onPressed: (){},
                              icon: Icon(
                                _getIconData(),
                                size: 40,
                                color: Colors.white70,
                              ),
                              style: IconButton.styleFrom(
                                backgroundColor: Colors.black.withAlphaF(0.5),
                              )
                            ),
                          ),
                        ),
                      ),
                    ),
                  Positioned(
                    bottom: 50,
                    left: 50,
                    right: 50,
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Text(
                          widget.title,
                          style: TextStyle(color: Colors.white, fontSize: 30),
                        ),
                        SizedBox(height:40),
                        SliderTheme(
                          data: SliderTheme.of(context).copyWith(
                            thumbShape: _showControls ? ConcentricCircleThumb() : SliderComponentNoThumb(), // 👈 적용
                            trackHeight: _showControls ? 4 : 2,
                            activeTrackColor: Colors.white,
                            inactiveTrackColor: Colors.white30,
                            overlayColor: Colors.transparent,
                            padding: EdgeInsets.all(0)
                          ),
                          child: Slider(
                            value : position.inSeconds.clamp(0, _controller.value.duration.inSeconds).toDouble(),
                            max: _controller.value.duration.inSeconds.toDouble(),
                            onChanged: null,
                            activeColor: Colors.white,
                            inactiveColor: Colors.white24,
                          ),
                        ),
                        Row(
                          mainAxisAlignment: MainAxisAlignment.spaceBetween,
                          children: [
                            Text(
                              _formatDuration(position),
                              style: TextStyle(color: Colors.white),
                            ),
                            Text(
                              _formatDuration(_controller.value.duration),
                              style: TextStyle(color: Colors.white),
                            ),
                          ],
                        )
                      ],
                    ),
                  ),
                ],
              )
            : Center(child: CircularProgressIndicator()),
      ),
    );
  }
}

class ConcentricCircleThumb extends SliderComponentShape {
  @override
  Size getPreferredSize(bool isEnabled, bool isDiscrete) => Size(40, 40);

  @override
  void paint(
    PaintingContext context,
    Offset center, {
    required Animation<double> activationAnimation,
    required Animation<double> enableAnimation,
    required bool isDiscrete,
    required TextPainter labelPainter,
    required RenderBox parentBox,
    required SliderThemeData sliderTheme,
    required TextDirection textDirection,
    required double value,
    required double textScaleFactor,
    required Size sizeWithOverflow,
  }) {
    final canvas = context.canvas;

    final outerPaint = Paint()
      ..color = Colors.white.withOpacity(0.3)
      ..style = PaintingStyle.fill;
    canvas.drawCircle(center, 15, outerPaint);

    final middlePaint = Paint()
      ..color = Colors.white
      ..style = PaintingStyle.fill;
    canvas.drawCircle(center, 8, middlePaint);

    final innerPaint = Paint()
      ..color = Colors.grey
      ..style = PaintingStyle.fill;
    canvas.drawCircle(center, 4, innerPaint);
  }
}

class SliderComponentNoThumb extends SliderComponentShape {
  @override
  Size getPreferredSize(bool isEnabled, bool isDiscrete) => Size.zero;

  @override
  void paint(
    PaintingContext context,
    Offset center, {
    required Animation<double> activationAnimation,
    required Animation<double> enableAnimation,
    required bool isDiscrete,
    required TextPainter labelPainter,
    required RenderBox parentBox,
    required SliderThemeData sliderTheme,
    required TextDirection textDirection,
    required double value,
    required double textScaleFactor,
    required Size sizeWithOverflow,
  }) {
  }
}