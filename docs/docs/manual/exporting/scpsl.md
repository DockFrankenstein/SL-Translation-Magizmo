---
sidebar_position: 0
---

# SCPSL

SCPSL refers to the format used by the game for loading translations. 

## Exporting

By going to `File/Export/SCPSL` you can open the "Export to SCPSL" prompt, where you have a couple of things to setup, after which you can press the "Export" button at the bottom.

### Path

Path of the folder to which the translation will be exported. Most of the time you want to export directly to the Translations folder. You can find it in steam by right clicking on SCP: Secret Laboratory and going to `Manager/Browser Local Files`.

![File Explorer screenshot of the Translations folder](/img/translations-folder-location.png)

:::warning
**Please create a new folder when exporting!** SCPSL's format consists of multiple files that cannot be in the Translations path's root!
:::

### Blank Entry
`default: -`

Text that will be used for unused entries. Not everything in the SCPSL's files is used by the game and SLTM removes the unnecessary entries. Most of the time there is no reason to change this field.

## Importing

SLTM also supports importing of SCPSL translations. By going over to `File/Import/SCPSL` you can select the folder containing the SCPSL translation that you want to use.

:::warning
**This will override every entry in your currently opened file!** Please make sure to save a copy or use a blank translation.
:::