
// import 'package:flutter/material.dart';
// import 'package:flutter/services.dart';
// import 'package:tizen_fs/poc/list_poc.dart';
// import 'package:tizen_fs/settings/settings.dart';
// import 'package:tizen_fs/styles/app_style.dart';

// class ImmersiveCarouselPoC extends StatefulWidget {
//   @override
//   State<ImmersiveCarouselPoC> createState() => _ImmersiveCarouselPoCState();
// }

// class _ImmersiveCarouselPoCState extends State<ImmersiveCarouselPoC> {
//   final FocusNode _focusNode = FocusNode();
//   final PageController _pageController = PageController(initialPage: 3);
//   ScrollController _scrollController = ScrollController(initialScrollOffset: 300);

//   bool _expand = false;
//   int _itemCount = 3;
//   int _globalIndex = 0;
//   int _current = 1;

//   var colors = <Color>[Colors.red, Colors.green, Colors.blue];
//   late List<GlobalKey> _itemKeys = [];
//   GlobalKey _listviewKey = GlobalKey();
//   List<Color> _visibleItems = [];

//   @override
//   void initState() {
//     super.initState();
//     _updateVisibleItems();

//     _scrollController.addListener(() {
//       debugPrint("scroller offset : ${_scrollController.offset}");
//       debugPrint("scroller direction : ${_scrollController.position.userScrollDirection}");
//     });
//   }

//   @override
//   void dispose() {
//     _focusNode.dispose();
//     super.dispose();
//   }

//   void _onFocusChanged(bool hasFocus) {
//     if (hasFocus != _expand) {
//       setState(() {
//         _expand = hasFocus;
//       });
//     }
//   }
//   void _updateVisibleItems() {
//     _visibleItems = List.generate(_itemCount, (i) {
//       final index = _globalIndex - 1 + i;
//       return colors[index % colors.length];
//     });

//     debugPrint("####################### itemsKey=${_itemKeys.length}");
//     _itemKeys = List.generate(_itemCount, (_) => GlobalKey());

//     setState(() {
//       // _scrollController.initialScrollOffset = 300;
//       _current = 1;
//     });

//     WidgetsBinding.instance.addPostFrameCallback((_) async {
//       _scrollController.jumpTo(300);
//       debugPrint("#######################update _scroller offset: ${_scrollController.offset}"); 
//     });

    
//     // debugPrint("####################### _scroller offset: ${_scrollController.offset}");
//     // WidgetsBinding.instance.addPostFrameCallback((_) async {
//     //   var context = _itemKeys[1].currentContext;
//     //   if(context!= null) {
//     //     await Scrollable.ensureVisible(
//     //       context,
//     //       alignment: 0,
//     //       duration: Duration.zero,
//     //       curve: Curves.easeInOut,
//     //     );
//     //   }
//     // });
//   }


//   KeyEventResult _onKeyEvent(FocusNode focusNode, KeyEvent event) {
//     if (event is KeyDownEvent || event is KeyRepeatEvent) {
//       if (event.logicalKey == LogicalKeyboardKey.arrowRight) {
//         _next(fast: event is KeyRepeatEvent);
//         return KeyEventResult.handled;
//       } else if (event.logicalKey == LogicalKeyboardKey.arrowLeft) {
//         _prev(fast: event is KeyRepeatEvent);
//         return KeyEventResult.handled;
//       }
//     }
//     return KeyEventResult.ignored;
//   }

//   void _next({bool fast = false}) async {
//     int? current = await _scrollToSelected(fast ? 1 : 100, _current + 1);
//     if(current != null) {
//       _globalIndex++;
//       _updateVisibleItems();
//     }
//   }

//   void _prev({bool fast = false}) async {
//     int? current = await _scrollToSelected(fast ? 1 : 100, _current - 1);
//     if(current != null) {
//       _globalIndex--;
//       _updateVisibleItems();
//     }
//   }

//   Future<int?> _scrollToSelected(int duration, int index) async {
//     debugPrint("############################## _scrollToSelected start, duration: $duration, : $index");
//     final context = _itemKeys[index].currentContext;
//    debugPrint("############################## _scrollToSelected start, context: $context");
//     if (context != null) {
//       debugPrint("#######################before _scroller offset: ${_scrollController.offset}"); 
//       await Scrollable.ensureVisible(
//         context,
//         alignment: 0,
//         duration: Duration(milliseconds: duration),
//         curve: Curves.easeInOut,
//       );
//       debugPrint("#######################after _scroller offset: ${_scrollController.offset}"); 
//       debugPrint("############################## _scrollToSelected end");
//       // _scrollController.animateTo(offset, duration: duration, curve: curve)
//       return index;
//     }
//     else {
//       debugPrint("############################## _scrollToSelected end null");
//       return null;
//     }
//   }

//   @override
//   Widget build(BuildContext context) {
//     debugPrint("############################## build");
//     return Focus(
//       focusNode: _focusNode,
//       onFocusChange: _onFocusChanged,
//       onKeyEvent: _onKeyEvent,
//       child: Container(
//         color: Colors.grey,
//         height: _expand ? 330 : 230,
//         child: Stack(
//           children: [
//             Positioned(
//               left: 0,
//               bottom: 30,
//               width: 300,
//               height: _expand ? 100 : 50,
//               child: Container(
//                 child: ListView.builder(
//                   key: _listviewKey,
//                   itemCount: _visibleItems.length,
//                   scrollDirection: Axis.horizontal,
//                   controller: _scrollController,
//                   itemBuilder: (context, index) {
//                     return Container(
//                       width: 300,
//                       key: _itemKeys[index],
//                       color: _visibleItems[index],
//                       child: Center(child: Text((_globalIndex % 3).toString())),
//                     );
//                     }
//                 ),
//               ),
//             ),
//             Positioned(
//               right: 50,
//               bottom: 30,
//               child: SmoothPageIndicator(    
//                 controller: _pageController,  // PageController    
//                 count:  3,    
//                 effect:  ColorTransitionEffect(
//                   spacing:  8.0,    
//                   radius:  4.0,    
//                   dotWidth:  10.0,    
//                   dotHeight:  10.0,    
//                   dotColor:  Colors.grey,    
//                   activeDotColor:  Colors.indigo    
//                 ),  // your preferred effect    
//                 onDotClicked: (index){    
//                 },
//               ),
//             )    
//           ]
//         ),
//       ),
//     );
//   }
// }

// class ImmersiveContent extends StatelessWidget {
//   @override
//   Widget build(BuildContext context) {
//     // TODO: implement build
//     throw UnimplementedError();
//   }
// }
