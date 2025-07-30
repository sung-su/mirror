import 'package:flutter/material.dart';

class ColorTablePage extends StatefulWidget {
  const ColorTablePage({super.key});

  @override
  State<ColorTablePage> createState() => _ColorTableState();
}

class _ColorTableState extends State<ColorTablePage> {
late Map<String, Color Function(BuildContext)> colorMap;

  @override
  void initState() {
    super.initState();

    colorMap = {
      "Primary" : (context) => Theme.of(context).colorScheme.primary,
      "onPrimary" : (context) => Theme.of(context).colorScheme.onPrimary,
      // "onPrimaryFixedVariant" : (context) => Theme.of(context).colorScheme.onPrimaryFixedVariant,
      "primaryContainer" : (context) => Theme.of(context).colorScheme.primaryContainer,
      "onPrimaryContainer" : (context) => Theme.of(context).colorScheme.onPrimaryContainer,
      
      "secondary" : (context) => Theme.of(context).colorScheme.secondary,
      "onSecondary" : (context) => Theme.of(context).colorScheme.onSecondary,
      // "onSecondaryFixedVariant" : (context) => Theme.of(context).colorScheme.onSecondaryFixedVariant,
      "secondaryContainer" : (context) => Theme.of(context).colorScheme.secondaryContainer,
      "onSecondaryContainer" : (context) => Theme.of(context).colorScheme.onSecondaryContainer,

      "tertiary" : (context) => Theme.of(context).colorScheme.tertiary,
      "onTertiary" : (context) => Theme.of(context).colorScheme.onTertiary,
      "tertiaryContainer" : (context) => Theme.of(context).colorScheme.tertiaryContainer,
      "onTertiaryContainer" : (context) => Theme.of(context).colorScheme.onTertiaryContainer,
      
      // deprecated
      // "background" : (context) => Theme.of(context).colorScheme.background,
      "surface" : (context) => Theme.of(context).colorScheme.surface,
      "onSurface" : (context) => Theme.of(context).colorScheme.onSurface,
      "surfaceContainerHighest" : (context) => Theme.of(context).colorScheme.surfaceContainerHighest,
      "onSurfaceVariant" : (context) => Theme.of(context).colorScheme.onSurfaceVariant,
      
      "error" : (context) => Theme.of(context).colorScheme.error,
      "onError" : (context) => Theme.of(context).colorScheme.onError,
      "errorContainer" : (context) => Theme.of(context).colorScheme.errorContainer,
      "onErrorContainer" : (context) => Theme.of(context).colorScheme.onErrorContainer,

      "outline" : (context) => Theme.of(context).colorScheme.outline,
      
      // deprecated
      // "onBackground" : (context) => Theme.of(context).colorScheme.onBackground,
    };
  }

  Widget build(BuildContext context) {
    return Scaffold(
      body: Column(
        children: [
          Row(
            children: [
              Container(
                child: Text("Color Table for TizenFS")
              ),
              const Spacer(),
            ],
          ),
          Expanded(
            child: Container(
              color: Theme.of(context).colorScheme.surface,
              child: Padding(
                padding: const EdgeInsets.all(5),
                child: Container(
                  child: ListView(
                    children: colorMap.entries.map((entry) {
                      var color = entry.value(context);
                      return SizedBox(
                        height: 20,
                        child: Container(
                          child: Row(
                            children: [
                              SizedBox(
                                width: 200,
                                child: Text(entry.key,
                                  style: const TextStyle(
                                    fontSize: 10,
                                  )
                                ),
                              ),
                              Container(
                                color: color,
                                width: 10,
                                height: 10,
                              )
                            ],
                          ),
                        ),
                      );
                    }).toList(),
                  ),
                ),
              ),
            ),
          ),
        ],
      ),
    );
  }
}