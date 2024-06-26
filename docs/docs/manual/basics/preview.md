---
sidebar_position: 1
---

# Preview Panel

The preview panel is located in the middle of the application. It's used for displaying an approximate preview of how your translation will look like in the game.

## Accuracy

<u>**SLTM does not use decompiled assets from SCPSL!**</u>

Instead it uses screenshots and rough reconstructions to simulate the look of the game. The result tends to be inaccurate when:
- using the `<size>` tag
- reaching the bounds of a text box

:::tip
It's recommended to test translations in the game first before releasing.
:::

## Scene Navigation

The preview panel separates different parts of the game into scenes. These scenes can be navigated by using the scene selection dropdown in the toolbar.

![scene navigation](/img/preview-scenenav.png)

:::info Note
Scenes will be automatically changed when selecting an item in the hierarchy. This won't happen if item is in the currently loaded scene or if it doesn't exist in any of them.
:::

## Scenes

Not every entry has it's scene. They are created for items that would benefit from having a preview or when they are easy to make. They also need to be possible to reconstruct (e.g. SCP-079 uses a lot of post processing effects on it's UI, so there is no way to preview it in SLTM).

## Elements

You can recognize editable elements by an outline around them. These elements can be clicked on to quickly select them in the hierarchy.

![elements](/img/preview-entries.png)

### Multi-entry elements

An element can display multiple entries. You can quickly switch between them by using arrows in the bottom left corner.

![multi-entry elements](/img/preview-multi-entry.png)