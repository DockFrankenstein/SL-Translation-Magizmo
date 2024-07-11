---
sidebar_position: 3
---

# Text Styling

Most of the time, SL uses [Text Mesh Pro](https://docs.unity3d.com/Packages/com.unity.textmeshpro@3.2/manual/index.html) for rendering text. This fact allows us to change how text looks in our translations.

## Text Mesh Pro Rich Text

Text can be stylized by using rich text tags that are built in to [Text Mesh Pro](https://docs.unity3d.com/Packages/com.unity.textmeshpro@3.2/manual/index.html). For more information on how to use them, please visit [this page](https://docs.unity3d.com/Packages/com.unity.textmeshpro@3.2/manual/RichText.html).

:::caution Rich Text doesn't work everywhere
Rich Text isn't enabled on every text element in SL so sometimes stylizing won't work.
:::

## Magizmo Rich Text

Magizmo also includes it's own proprietary set of rich text tags that get converted to something SL can read when exporting.

### ```<chargradient>```/```<cg>```

Character gradient will create a per-character gradient using color tags.

```<cg=red,blue>Hello</cg>``` will be converted into `<color=#FF0000FF>H</color><color=#CC0033FF>e</color>...` which will then look like this...

![Image of the result](/img/rt-cg.png)