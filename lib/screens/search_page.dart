import 'package:flutter/material.dart';
import 'package:tizen_fs/widgets/fssearchbar.dart';

class SearchPage extends StatefulWidget {
  const SearchPage({super.key, required this.scrollController});

  final ScrollController scrollController;

  @override
  State<SearchPage> createState() => _SearchPageState();
}


class _SearchPageState extends State<SearchPage> {

  @override
  void initState() {
    super.initState();
  }

  @override
  Widget build(BuildContext context) {
    return Center(
        child: Column(
      children: [
        Padding(
          padding: const EdgeInsets.only(top: 40, bottom: 20, left: 58, right: 58),
          child: Column(
            children: [
              FSSearchBar()
            ],
          ),
        ),
      ],
    ));
  }
}

// class FindSuggestionContent {
//   final String overline;
//   final String title;
//   final String description;
//   final String backdrop;
//   final String buttonText;

//   FindSuggestionContent({
//     required this.overline,
//     required this.title,
//     required this.description,
//     required this.backdrop,
//     required this.buttonText,
//   });

//   factory FindSuggestionContent.fromJson(Map<String, dynamic> json) {
//     return FindSuggestionContent(
//       overline: json['overline'] as String,
//       title: json['title'] as String,
//       description: (json['description'] as String).length > 100
//           ? '${json['description'].substring(0, 100)}...'
//           : json['description'] as String,
//       backdrop: json['backdrop'] as String,
//       buttonText: json['buttonText'] as String,
//     );
//   }

//   static Future<List<FindSuggestionContent>> loadFromJson() async {
//     final jsonString =
//         await rootBundle.loadString('assets/mock/mock_carousel_content.json');
//     final List<dynamic> jsonList = jsonDecode(jsonString);
//     final List<FindSuggestionContent> contents =
//         jsonList.map((json) => FindSuggestionContent.fromJson(json)).toList();

//     return contents;
//   }

//   static List<FindSuggestionContent> generateMockContent() {
//     return List.generate(
//       5,
//       (index) => FindSuggestionContent(
//         overline: 'Overline $index',
//         title: 'Movie Title $index',
//         description: 'Description of the movie goes here.\n This is a sample description for item $index.',
//         backdrop: 'assets/mock/images/backdrop${(index % 3) + 1}.png',
//         buttonText: 'Watch Now',
//       ),
//     );
//   }
// }

// class FindSuggestionModel extends ChangeNotifier {
//   late List<FindSuggestionContent> contents;
//   bool _isLoading = false;
//   int _selectedIndex = 0;

//   FindSuggestionModel(this.contents);

//   FindSuggestionModel.fromMock() {
//     _isLoading = true;
//     FindSuggestionContent.loadFromJson().then((value) {
//       _isLoading = false;
//       contents = value;
//       notifyListeners();
//     });
//   }

//   int get selectedIndex => _selectedIndex;
//   set selectedIndex(int index) {
//     _selectedIndex = index;
//     notifyListeners();
//   }

// int get itemCount => _isLoading ? 0 : contents.length;
//   FindSuggestionContent getContent(int index) {
//     if (_isLoading) {
//       return FindSuggestionContent(
//         overline: 'Loading...',
//         title: 'Loading...',
//         description: 'Loading...',
//         backdrop: 'Loading...',
//         buttonText: 'Loading...',
//       );
//     }
//     return contents[index];
//   }

//   FindSuggestionContent getSelectedContent() {
//     if (_isLoading) {
//       return FindSuggestionContent(
//         overline: 'Loading...',
//         title: 'Loading...',
//         description: 'Loading...',
//         backdrop: 'backdrop1.png',
//         buttonText: 'Loading...',
//       );
//     }
//     return contents[_selectedIndex];
//   }
// }