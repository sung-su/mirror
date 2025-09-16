// import 'package:flutter/material.dart';
// import 'package:reorderable_grid_view/reorderable_grid_view.dart';

// class GridTestPage2 extends StatefulWidget {
//   const GridTestPage2({Key? key}) : super(key: key);

//   @override
//   _GridTestPage2State createState() => _GridTestPage2State();
// }
// class _GridTestPage2State extends State<GridTestPage2> {
//   final data = [1, 2, 3, 4, 5];

//   @override
//   Widget build(BuildContext context) {
//     Widget buildItem(String text) {
//       // return Card(
//       //   key: ValueKey(text),
//       //   child: Text(text),
//       // );

//       return Stack(
//         key: ValueKey(text),
//         children: [Container(
//           color: Colors.amber,
//           child: SizedBox(
//             width: 100,
//             height: 100,
//             child: Container(
//               color: Colors.red,
//               child: Center(child: Text(text)))),
//         )]
//       );
//     }

//     return Scaffold(
//       body: Padding(
//         padding: const EdgeInsets.fromLTRB(55, 10, 55, 10),
//         child: Center(
//           // use ReorderableGridView.count() when version >= 2.0.0
//           // else use ReorderableGridView()
//           child: ReorderableGridView.count(
//             crossAxisSpacing: 10,
//             mainAxisSpacing: 10,
//             crossAxisCount: 5,
//             childAspectRatio: 1.5,
//             children: this.data.map((e) => buildItem("$e")).toList(),
//             onReorder: (oldIndex, newIndex) {
//               setState(() {
//                 final element = data.removeAt(oldIndex);
//                 data.insert(newIndex, element);
//               });
//             },
//             footer: [
//               Card(
//                 child: Center(
//                   child: Icon(Icons.add),
//                 ),
//               ),
//             ],
//           ),
//         ),
//       ),
//     );
//   }
// }
