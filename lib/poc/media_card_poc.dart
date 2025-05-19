import 'package:flutter/material.dart';
import 'package:tizen_fs/widgets/backdrop_scaffold.dart';
import 'package:tizen_fs/widgets/media_card.dart';

class MediaCardPocPage extends StatelessWidget {
  const MediaCardPocPage({super.key});

  @override
  Widget build(BuildContext context) {
    return BackdropScaffold(
      child: MediaCardPoc(),
    );
  }
}

class MediaCardPoc extends StatefulWidget {
  const MediaCardPoc({super.key});

  @override
  State<MediaCardPoc> createState() => _MediaCardPocState();
}

class _MediaCardPocState extends State<MediaCardPoc> {
  bool _isSelected = false;

  @override
  Widget build(BuildContext context) {
    return SingleChildScrollView(
      child: Padding(
        padding: const EdgeInsets.only(left: 58, top: 58),
        child: Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            spacing: _isSelected ? 50 : 15,
            children: [
              Row(
                children: [
                  Text('IsSelected'),
                  Switch(
                      onChanged: (value) {
                        setState(() {
                          _isSelected = !_isSelected;
                        });
                      },
                      value: _isSelected),
                ],
              ),
              Center(
                  child: MediaCard.fourCard(
                    imageUrl: 'assets/mock/images/1_card.jpg',
                    isSelected: _isSelected,
                    title: "Title area.. abc abc abcd abcde abcdef",
                    subtitle: "Subtitle area",
                    description: "1.3M views • 2 days ago",)),
              Center(
                  child: MediaCard.fiveCard(
                      imageUrl: 'assets/mock/images/2_card.jpg',
                      isSelected: _isSelected,
                      ratio: MediaCardRatio.square)),
              Center(
                  child: MediaCard.circle(
                      imageUrl: 'assets/mock/images/2_card.jpg',
                      isSelected: _isSelected)),
              Center(
                  child: MediaCard.threeCard(
                    imageUrl: 'assets/mock/images/3_card.jpg',
                    isSelected: _isSelected,
                    shadowColor: Colors.red[400],
                    duration: "20:10",
                    title: "Title..", 
                    subtitle: _isSelected ? "Subtitle.." : null,
              )),
              Center(
                  child: MediaCard.fiveCard(
                imageUrl: 'assets/mock/images/4_card.jpg',
                isSelected: _isSelected,
                ratio: MediaCardRatio.poster,
              )),
              SizedBox(
                height: 200,
              )
            ]),
      ),
    );
  }
}
