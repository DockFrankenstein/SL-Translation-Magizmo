---
sidebar_position: 1
---

# Gathering Backgrounds

Backgrounds used in scenes are screenshots from SCPSL with their text elements removed.

## Removing text from SCPSL

There isn't an ideal way of going about this, so here's a couple of tips:

### 1. Use an empty translation

:::warning
SCPSL will use the default english translation when it encounters a field that's [null or white space](https://learn.microsoft.com/en-us/dotnet/api/system.string.isnullorwhitespace?view=net-8.0).
:::

To create an empty translation open up SLTM, go to `Tools/Set All` and fill content with this...
```
<color=#00000000></color>
```
...and click apply.

Then, replace the empty translations in the main menu and settings navigation tabs (so that you will be able to navigate the game).

![](/img/empty-translation-tabs.png)

Finally, you can export your translation to SL and set it in game by going to `Settings/Interface/Interface Language`.

:::info
Make sure to give your translation a name in the manifest.
:::

:::warning This doesn't always work
Sometimes SL uses translation entries inside of other entries or adds it's own characters (such as in human f1 menus). In that case look at [step #2](#2-abuse-text-mesh-pro).
:::

### 2. Abuse Text Mesh Pro

Sometimes SL may add it's own characters that you can't remove. In the case these characters are preceded by an entry and are contained inside of the same text object, you can follow this simple trick.

Edit the preceding entry and set it's value to...
```
<color=#00000000>
```

Text Mesh Pro will now hide the rest of the text for us, since it's gonna remain invisible. This worked for creating SCP F1 menu preview scenes.

### 3. Do something else

If none of the above ways work, you will have to just figure this out on your own, most likely by editing screenshots with other programs.

You can make your life a bit easier by either using an empty translation or replacing entry contents with a short text like `.`.

## Background rules

1. You can reconstruct backgrounds inside of the app, however <u>**DO NOT USE DECOMPILED ASSETS**</u>. You can use them externally for modifying existing screenshots, but you cannot import them into Unity.
2. Do not feature nicknames of people that do not want to be associated with the app. Ask for permission or use fake accounts.
3. Don't feature elements in your backgrounds that can be edited using the magizmo.