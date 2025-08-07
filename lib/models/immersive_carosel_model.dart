import 'dart:async';
import 'dart:convert';

import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:provider/provider.dart';
import 'package:tizen_fs/widgets/carousel_content_block.dart';
import 'package:tizen_fs/providers/backdrop_provider.dart';

class ImmersiveCarouselContent {
  final String overline;
  final String title;
  final String description;
  final String backdrop;
  final String buttonText;

  ImmersiveCarouselContent({
    required this.overline,
    required this.title,
    required this.description,
    required this.backdrop,
    required this.buttonText,
  });

  factory ImmersiveCarouselContent.fromJson(Map<String, dynamic> json) {
    return ImmersiveCarouselContent(
      overline: json['overline'] as String,
      title: json['title'] as String,
      description: (json['description'] as String).length > 100
          ? '${json['description'].substring(0, 100)}...'
          : json['description'] as String,
      backdrop: json['backdrop'] as String,
      buttonText: json['buttonText'] as String,
    );
  }

  static Future<List<ImmersiveCarouselContent>> loadFromJson() async {
    final jsonString =
        await rootBundle.loadString('assets/mock/mock_carousel_content.json');
    final List<dynamic> jsonList = jsonDecode(jsonString);
    final List<ImmersiveCarouselContent> contents =
        jsonList.map((json) => ImmersiveCarouselContent.fromJson(json)).toList();

    return contents;
  }

  static List<ImmersiveCarouselContent> generateMockContent() {
    return List.generate(
      5,
      (index) => ImmersiveCarouselContent(
        overline: 'Overline $index',
        title: 'Movie Title $index',
        description: 'Description of the movie goes here.\n This is a sample description for item $index.',
        backdrop: 'assets/mock/images/backdrop${(index % 3) + 1}.png',
        buttonText: 'Watch Now',
      ),
    );
  }
}

class ImmersiveCarouselModel extends ChangeNotifier {
  late List<ImmersiveCarouselContent> contents;
  bool _isLoading = false;
  int _selectedIndex = 0;

  ImmersiveCarouselModel(this.contents);

  ImmersiveCarouselModel.fromMock() {
    _isLoading = true;
    ImmersiveCarouselContent.loadFromJson().then((value) {
      _isLoading = false;
      contents = value;
      notifyListeners();
    });
  }

  int get selectedIndex => _selectedIndex;
  set selectedIndex(int index) {
    _selectedIndex = index;
    notifyListeners();
  }

  int get itemCount => _isLoading ? 0 : contents.length;
  ImmersiveCarouselContent getContent(int index) {
    if (_isLoading) {
      return ImmersiveCarouselContent(
        overline: 'Loading...',
        title: 'Loading...',
        description: 'Loading...',
        backdrop: 'Loading...',
        buttonText: 'Loading...',
      );
    }
    return contents[index];
  }

  ImmersiveCarouselContent getSelectedContent() {
    if (_isLoading) {
      return ImmersiveCarouselContent(
        overline: 'Loading...',
        title: 'Loading...',
        description: 'Loading...',
        backdrop: 'gradient-bg-static-alt-a-D0K-Mjox.webp',
        buttonText: 'Loading...',
      );
    }
    return contents[_selectedIndex];
  }
}
