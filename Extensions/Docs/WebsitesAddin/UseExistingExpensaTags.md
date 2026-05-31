# Use Existing Expensa Tags

The Websites add-in now uses Expensa's existing generic tagging model.

## Source tables

```text
Tag
TagAssignment
```

The tag assignment filter is:

```sql
[EntityType] = 'Website'
```

## Tree behavior

Websites are grouped under existing Expensa tags.

Untagged websites appear under:

```text
Uncategorized
```

No `WebsiteTag` table is created. That would duplicate the existing tagging system and make future reuse worse, which is exactly how software grows extra elbows.
