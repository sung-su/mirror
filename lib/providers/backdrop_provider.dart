import 'package:flutter/material.dart';

class BackdropProvider extends ChangeNotifier {
  Widget? _backdrop;
  Widget? get backdrop => _backdrop;

  void updateBackdrop(Widget? backdrop) {
    _backdrop = backdrop;
    notifyListeners();
  }

  bool _isZoomin = false;

  bool get isZoomIn => _isZoomin;
  set isZoomIn(bool value) {
    _isZoomin = value;
    notifyListeners();
  }
}
