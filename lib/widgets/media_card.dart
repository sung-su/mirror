import 'dart:io';
import 'package:cached_network_image/cached_network_image.dart';
import 'package:flutter/material.dart';
import 'package:tizen_fs/main.dart';
import 'package:tizen_fs/styles/app_style.dart';

enum MediaCardRatio {
  wide,
  square,
  poster,
}

class MediaCard extends StatelessWidget {
  const MediaCard(
      {super.key,
      this.width = 196,
      required this.imageUrl,
      this.title,
      this.subtitle,
      this.description,
      this.isSelected = false,
      this.duration,
      this.ratio = MediaCardRatio.wide,
      this.shadowColor})
      : height = width *
            (ratio == MediaCardRatio.wide
                ? 9 / 16
                : ratio == MediaCardRatio.square
                    ? 1
                    : 3 / 2);

  const MediaCard.oneCard(
      {Key? key,
      required String imageUrl,
      String? title,
      String? subtitle,
      String? description,
      String? duration,
      bool isSelected = false,
      MediaCardRatio ratio = MediaCardRatio.wide,
      Color? shadowColor})
      : this(
            key: key,
            width: 268,
            imageUrl: imageUrl,
            title: title,
            subtitle: subtitle,
            description: description,
            duration: duration,
            isSelected: isSelected,
            ratio: ratio,
            shadowColor: shadowColor);

  const MediaCard.twoCard(
      {Key? key,
      required String imageUrl,
      String? title,
      String? subtitle,
      String? description,
      String? duration,
      bool isSelected = false,
      MediaCardRatio ratio = MediaCardRatio.wide,
      Color? shadowColor})
      : this(
            key: key,
            width: 268,
            imageUrl: imageUrl,
            title: title,
            subtitle: subtitle,
            description: description,
            duration: duration,
            isSelected: isSelected,
            ratio: ratio,
            shadowColor: shadowColor);

  const MediaCard.threeCard(
      {Key? key,
      required String imageUrl,
      String? title,
      String? subtitle,
      String? description,
      String? duration,
      bool isSelected = false,
      MediaCardRatio ratio = MediaCardRatio.wide,
      Color? shadowColor})
      : this(
            key: key,
            width: 268,
            imageUrl: imageUrl,
            title: title,
            subtitle: subtitle,
            description: description,
            duration: duration,
            isSelected: isSelected,
            ratio: ratio,
            shadowColor: shadowColor);

  const MediaCard.fourCard(
      {Key? key,
      required String imageUrl,
      String? title,
      String? subtitle,
      String? description,
      String? duration,
      bool isSelected = false,
      MediaCardRatio ratio = MediaCardRatio.wide,
      Color? shadowColor})
      : this(
            key: key,
            width: 196,
            imageUrl: imageUrl,
            title: title,
            subtitle: subtitle,
            description: description,
            duration: duration,
            isSelected: isSelected,
            ratio: ratio,
            shadowColor: shadowColor);

  const MediaCard.fiveCard(
      {Key? key,
      required String imageUrl,
      String? title,
      String? subtitle,
      String? description,
      String? duration,
      bool isSelected = false,
      MediaCardRatio ratio = MediaCardRatio.wide,
      Color? shadowColor})
      : this(
            key: key,
            width: 124,
            imageUrl: imageUrl,
            title: title,
            subtitle: subtitle,
            description: description,
            duration: duration,
            isSelected: isSelected,
            ratio: ratio,
            shadowColor: shadowColor);

  const MediaCard.nineCard(
      {Key? key,
      required String imageUrl,
      String? title,
      String? subtitle,
      String? description,
      String? duration,
      bool isSelected = false,
      MediaCardRatio ratio = MediaCardRatio.wide,
      Color? shadowColor})
      : this(
            key: key,
            width: 80,
            imageUrl: imageUrl,
            title: title,
            subtitle: subtitle,
            description: description,
            duration: duration,
            isSelected: isSelected,
            ratio: ratio,
            shadowColor: shadowColor);

  const MediaCard.circle(
      {Key? key,
      required String imageUrl,
      String? title,
      String? subtitle,
      String? description,
      String? duration,
      bool isSelected = false,
      Color? shadowColor})
      : this(
            key: key,
            width: 80,
            imageUrl: imageUrl,
            title: title,
            subtitle: subtitle,
            description: description,
            duration: duration,
            isSelected: isSelected,
            ratio: MediaCardRatio.square,
            shadowColor: shadowColor);

  static const int animationDuration = 100;
  final double width;
  final double height;
  final String imageUrl;
  final String? title;
  final String? subtitle;
  final String? description;
  final String? duration;
  final bool isSelected;
  final MediaCardRatio ratio;
  final Color? shadowColor;

  @override
  Widget build(BuildContext context) {
    return Column(
      spacing: 1,
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        AnimatedScale(
            scale: isSelected ? 1.1 : 1,
            duration: Duration(milliseconds: animationDuration),
            child: Stack(children: [
              isSelected
                  ? BlinkBorder(
                      width: width,
                      height: height,
                      ratio: ratio,
                      shadowColor: shadowColor ?? Colors.white,
                      child: ratio == MediaCardRatio.square
                          ? ClipOval(child: _buildTileImage(imageUrl))
                          : ClipRRect(
                              borderRadius: BorderRadius.circular(8),
                              child: _buildTileImage(imageUrl),
                            ))
                  : SizedBox(
                      width: width,
                      height: height,
                      child: ratio == MediaCardRatio.square
                          ? ClipOval(child: _buildTileImage(imageUrl))
                          : ClipRRect(
                              borderRadius: BorderRadius.circular(10),
                              child: _buildTileImage(imageUrl),
                            ),
                    ),
              if (duration != null) 
                Positioned(
                  bottom: 8,
                  right: 8,
                  child: Container(
                    decoration: BoxDecoration(
                      borderRadius: BorderRadius.circular(3),
                      color: Colors.black.withAlphaF(0.8),
                    ),
                    padding: EdgeInsets.symmetric(horizontal: 4, vertical: 1),
                    child: Text(duration!, style: TextStyle(fontSize: 9, fontWeight: FontWeight.bold)))),
            ])),
        if (title != null || subtitle != null || description != null)
          SizedBox(height: 4),
        if (title != null)
          SizedBox(width:width, child:Text(title!, style:TextStyle(fontSize: 12, color: $style.colors.onPrimary.withAlphaF(0.9 * (isSelected ? 1 : 0.9))), maxLines: 1, overflow: TextOverflow.ellipsis)),
        if (subtitle != null)
          SizedBox(width:width, child:Text(subtitle!, style:TextStyle(fontSize: 11, color: $style.colors.onPrimary.withAlphaF(0.7 * (isSelected ? 1 : 0.9))), maxLines: 1, overflow: TextOverflow.ellipsis)),
        if (description != null)
          SizedBox(width: width, child:Text(description!, style:TextStyle(fontSize: 11, color: $style.colors.onPrimary.withAlphaF(0.7 * (isSelected ? 1 : 0.9))), maxLines: 1, overflow: TextOverflow.ellipsis)),
      ],
    );
  }

  Widget _buildTileImage(String iconUrl) {
    if (iconUrl.startsWith('http')) {
      return CachedNetworkImage(
        imageUrl: iconUrl,
        errorWidget: (context, url, error) => const Icon(Icons.error),
        fit: BoxFit.cover,
      );
    } else if (iconUrl.startsWith('/')) {
      return Image.file(
        File(iconUrl),
        errorBuilder: (context, error, stackTrace) =>
            const Icon(Icons.broken_image),
        fit: BoxFit.cover,
      );
    } else if (iconUrl.startsWith('assets')) {
      return Image.asset(
        iconUrl,
        fit: BoxFit.cover,
      );
    } else {
      return const Center(
          child: Icon(Icons.image_not_supported, color: Colors.grey));
    }
  }
}

class BlinkBorder extends StatefulWidget {
  const BlinkBorder(
      {super.key,
      required this.child,
      required this.width,
      required this.height,
      required this.ratio,
      required this.shadowColor});

  final double width;
  final double height;
  final MediaCardRatio ratio;
  final Widget child;
  final Color shadowColor;

  @override
  State<BlinkBorder> createState() => _BlinkBorderState();
}

class _BlinkBorderState extends State<BlinkBorder>
    with SingleTickerProviderStateMixin {
  static const int blinkDuration = 800;
  late AnimationController controller;
  late Animation<double> animation;
  bool _disposed = false;

  @override
  void initState() {
    super.initState();
    controller = AnimationController(
      duration: const Duration(milliseconds: blinkDuration),
      vsync: this,
    );

    animation = Tween<double>(begin: 0.7, end: 0)
        .animate(CurvedAnimation(parent: controller, curve: Curves.linear))
      ..addListener(() {
        setState(() {});
      });

    Future.delayed(Duration(milliseconds: 500)).whenComplete(() {
      if (!_disposed) {
        controller.repeat(reverse: true);
      }
    });
  }

  @override
  void dispose() {
    _disposed = true;
    controller.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Container(
        width: widget.width,
        height: widget.height,
        decoration: BoxDecoration(
            border: Border.all(
                color: Colors.white.withAlphaF(animation.value), width: 2),
            borderRadius: widget.ratio != MediaCardRatio.square
                ? BorderRadius.circular(10)
                : null,
            shape: widget.ratio == MediaCardRatio.square
                ? BoxShape.circle
                : BoxShape.rectangle,
            boxShadow: [
              BoxShadow(
                color: widget.shadowColor.withAlphaF(0.7),
                spreadRadius: 1,
                blurRadius: 20,
                blurStyle: BlurStyle.normal,
                offset: Offset(0, 0),
              )
            ]),
        child: widget.child);
  }
}
