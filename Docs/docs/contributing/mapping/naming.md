---
sidebar_position: 1
---

# Naming Guide

Mapped Fields ids are supposed to never change. This is why it's important to identify them properly as soon as possible to insure there will be no problems with save files.

:::caution
This is an early version of this guide, take everything with a grain of salt.
:::

## Syntax

SLTM's mapped field names follow these syntax rules:
- everything is typed lowercase
- no spaces are allowed
- words are divided by an underscore (`_`)
- only english letters are allowed
- special characters (except for the underscore) are forbidden

So in practice, these are the only allowed characters:
`abcdefghijklmoprstuvwxyz0123456789_`

## Naming

### Important notes

SLTM's mapped field names cannot be changed later in development, unless the following:
* target entry's content changes to one that is less generic (an example of this could be the word "visible" being changed to "badge visible" in an update to SCP: Secret Laboratory).

### Path pattern

Ids follow a "path pattern" which make it easier to categorize them in bulk. It's sort of similar to how a file gets saved (e.g. a video file is saved as `Videos/file.mp4`). The best way to explain this is via an example:

| ❌ Don't | ✅ Do |
| :-- | :-- |
| closed_door | door_closed |
| patreon_badge | badge_patreon |
| red_candy | candy_red |

We do this because it's easier to search for ids of a similar type.

This is how the results for searching "badge" would look like without using the pattern:
```
- hidden_badge
- sl_artist_badge
- sl_partner_badge
- sl_animator_badge
- sl_developer_badge
...
```

And this is how it looks like with the pattern:
```
- badge_hidden
- badge_sl_artist
- badge_sl_partner
- badge_sl_animator
- badge_sl_developer
...
```

## Handling duplicates

Commonly, there might be a situation where there will be multiple fields that correspond to the same word, sometimes even of the same context. If this happens, follow these steps:
1. If duplicate entry is used in an old file (such as "Facility.txt" that contains multiple unused fields), keep the same name and mark "don't add to list" the entry in the old file to prevent importing legacy translations.
2. If both entries are identical and are used in the same file, use the same name for both.
3. If boh entries are identical and are used in multiple files with generic entries, use the same name for both.
4. If both entries are identical and are used in multiple files, where at least one is made for a specific part of the game, start a discussion with the rest of the community on if to split it, but most of the time it should be kept the same.
5. If both entries aren't identical, ask yourself what reason there could have been to split them and start a discussion with the rest of the community on if to split it.