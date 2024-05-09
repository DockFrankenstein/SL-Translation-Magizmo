---
sidebar_position: 1
---

# CSV

The "Comma Separated Values" format allows for editing inside spreadsheet applications such as [Microsoft Excel](https://www.microsoft.com/en-us/microsoft-365/excel) or [LibreOffice Calc](https://www.libreoffice.org/discover/calc/). This workflow is very popular for translating games.

## Exporting
By going to `File/Export/CSV` you can open the "Export to CSV" prompt.

### Path
Path of where you want to save your csv file.

### Create Categories
`default: true`

When enabled, the output `.csv` file will separate entries into labeled categories. It's recommended to keep this option enabled for easier readability.

### Columns Order
`default: id, display name, original translation, value, dynamic values`

Order of different columns. You can click and drag individual elements to rearrange them. 

## Editing

You can open the `.csv` file in your preferred spreadsheet application.

### Columns

Here's a detailed explanation of each column:

| Name | Description |
| :-- | :-- |
| Id | The id used by sltm to recognize which row corresponds to which entry. |
| Display Name | Name of the entry that's typically displayed in the app. |
| Original Translation | Original value of the entry from the translation you're currently comparing. |
| Value | Value of the entry. |
| Dynamic Values | List of dynamic values available for the entry. These tags will be replaced by SCPSL with a different value. |

## Importing

You can open the import prompt by going to `File/Import/CSV`. This panel features a couple of options and a preview on the right.

### Preview
The preview panel located on the right side will display the file that's currently selected in the `Path` field.

### Path
Path of the `.csv` file you're trying to load.

:::warning
In the current version of SLTM, changing the path can cause a lag spike, due to the preview updating. Please be patient.
:::

### Id Column

Column that contains entry ids. Please make sure to select the right column for the import to work. 

### Value column

Column that contains entry values. Please make sure to select the right column for the import to work.