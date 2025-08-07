// import 'package:flutter/material.dart';
// import 'package:flutter/services.dart';

// class TwoPageNavigation extends StatefulWidget {
//   const TwoPageNavigation({super.key});

//   @override
//   State<TwoPageNavigation> createState() => _TwoPageNavigationState();
// }

// class _TwoPageNavigationState extends State<TwoPageNavigation> {



//   @override
//   Widget build(BuildContext context) {
//     return Container(
//       color: Colors.amber
//     );
//   }
// }

// class PageModel {


//   static List<Page> generateMockPageModel() {
//   }
// }

// class Page {
//   Page({
//     required this.title,
//     required this.children,
//   });

//   final String title;
//   final List<Page> children;
// }

// class TopPage extends StatelessWidget with Page {

//   @override
//   set _title(String __title) {
//     // TODO: implement _title
//     super._title = __title;
//   }


//   @override
//   Widget build(BuildContext context) {
//     return Center(
//       child: Text(title)
//     );
//   }
// }