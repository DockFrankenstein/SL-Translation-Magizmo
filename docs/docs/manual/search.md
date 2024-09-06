---
sidebar_position: 2
---

# Search

At the top of the hierarchy panel there is a search bar that can be used for finding entries.

## Normal Search

By default, the search bar looks through the names of entries and tries to show the ones that fit your input the best.

## Advanced Search

By using certain **keywords** the hierarchy can search through a lot more than just entry names. The format is as follows:

```
keyword: value
```

:::tip
You can use multiple keywords at a time. E.g. `079 content:aux id:notif`.
:::

These keywords include...

### Name (`name`)

This is what is used by default when no keyword is specified. With this keyword, the hierarchy will search through the names of it's items.

### Id (`id`, `i`)

With this keyword, the hierarchy will search through the entries id's that are used by the program behind the scenes. You can toggle id display in `View/Show Ids Instead Of Names`.

### Content (`content`, `c`)

With this keyword, the hierarchy will search through the contents of the entries.

## [Regex](https://en.wikipedia.org/wiki/Regular_expression)

You can use Regular Expression by adding a `*` between the keyword and the colon symbol. E.g. `name*:[0-9]`.

:::tip
You can use the same keywords with and without regex at the same time. E.g. `id:scp id*:[0-9]`.
:::