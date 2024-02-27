---
sidebar_position: 0
---

# Getting Started

SLTM uses a map of SL's files to create a list of it's entries and for exporting to it's format. Every [Translation Version](/contributing/lingo#translation-version) contains it's own list of Mapping Containers that each define their own [Mapped Fields](/contributing/basics/terminology#mapped-field).

## Types of containers

### Multi Entry

This container contains multiple entries, most of the time separated by new lines (sometimes a single line can have multiple entries separated by a special character such as `~` or `:`). This is the most commonly used container type in SCPSL.

Usually entries are identified by using their line number, but sometimes the first entry in a line is used instead.

### Array Entry

This container contains a single entry that can contain multiple items separated by new lines.

### Manifest

This container contains entries related to the manifest file. SL's manifest file is saved using json.