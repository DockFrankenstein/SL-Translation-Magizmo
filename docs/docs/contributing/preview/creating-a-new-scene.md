---
sidebar_position: 0
---

# Creating A New Scene

Create a new prefab with the root object having a `Preview Scene` component attached.

:::tip
It's always easier to start with a template by copying an already existing scene.
:::

From there on change the `Path` of the scene. The `/` character will separate it into directories with the last string being the scene name.

Finally, specify for which version of SCPSL you are making your scene for. SLTM will not show your scene on the list if it's current SL version is lower.

## Standard workflow

Most commonly a scene consists of a preview canvas which contains the background image (a screenshot from SL with the text removed) and other preview elements.

When first creating a scene, replace the background with a screenshot that contains text. It makes it easier to layout other elements. Once that's done you can revert the background to normal.

:::tip
Don't worry about being 100% accurate. Most scenes generally ignore the real font size and use what looks the best (even if it means sacrificing accuracy when using the `<size>` tag).
:::

Sometimes it's good to test the text boundaries in SL by making a custom translation with long strings of text. 

## UI

Your scene will most likely feature UI. The standard preview canvas can be found at `Prefab/UI/Preview/Preview Canvas`. It does not require further setup.

## Preview Elements

Preview Elements are objects that can be updated in the scene.

By default, every preview element contains these settings:
- **Linked Elements** - other preview elements that should synchronize it's currently selected item index with this element.

### Preview Entry

Most commonly used element. It displays an entry's value in the scene. When adding, please use the prefab that can be found in `Prefab/UI/Preview/Entry Preview Text`.

Settings:
- **Interactable** - determines if this entry can be clicked on. By disabling this option you will hide the outline and make the object not respond to mouse input.
- **Main Id** - the default mapped field.
- **Other Ids** - other mapped fields that can be switched by using the navigation bar.

### Preview Dynamic Background

A background that can display multiple images. It cannot be changed directly, but it will update itself when being linked in another element.

Settings:
- **Backgrounds** - backgrounds that the element will switch between. Each item represents the selected index.

### Version Element

Piece of text that will show the file's SL version.

## Finalizing

After completing your preview scene go to the root object and on the `Preview Scene` component click both of the populate buttons. This will fill out the list of elements and embedded scenes.

Now go to the `Main` scene and drag your preview scene under `Preview Scenes`. It's position within this object will determine it's priority in the scene selection dropdown.

Finally, go to the `Preview Scene Manager` object and click the `Populate` button to add your scene to the list.