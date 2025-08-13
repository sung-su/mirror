import 'package:flutter/material.dart';

class NoScrollFocusTraversalPolicy extends WidgetOrderTraversalPolicy {
  final bool Function() isAnimating;

  NoScrollFocusTraversalPolicy(this.isAnimating);

  @override
  bool inDirection(FocusNode currentNode, TraversalDirection direction) {
    if(isAnimating()) { // to prevent focus movement while an widget's animation is running
      return true;
    }
    final next = _findNext(currentNode, direction);
    if (next != null) {
      next.requestFocus(); // to prevent auto scroll
      return true;
    }
    return false;
  }

  FocusNode? _findNext(FocusNode currentNode, TraversalDirection direction) {
    final context = currentNode.context;
    if (context == null) return null;

    final scope = FocusScope.of(context);
    final filtered = _filterFocusNodesDepth2(scope);

    final index = filtered.indexOf(currentNode);
    if (index == -1) return null;

    switch (direction) {
      case TraversalDirection.down:
      case TraversalDirection.right:
        return index < filtered.length - 1 ? filtered[index + 1] : null;
      case TraversalDirection.up:
      case TraversalDirection.left:
        return index > 0 ? filtered[index - 1] : null;
      default:
        return null;
    }
  }

  List<FocusNode> _filterFocusNodesDepth2(FocusScopeNode scope) {
    final all = scope.traversalDescendants.toList();

    return all.where((node) {
      final parent = node.parent;
      final grandParent = parent?.parent;

      return parent == scope || grandParent == scope;
    }).toList();
  }
}