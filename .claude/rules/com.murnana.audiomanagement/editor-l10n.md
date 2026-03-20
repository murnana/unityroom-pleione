---
paths:
  - "Packages/com.murnana.audiomanagement/Editor/**/*.cs"
---

# AudioManagement Editor Localization

## Rule

All **user-facing strings** (GUI labels, error logs, warning messages, etc.) in `Packages/com.murnana.audiomanagement/Editor/` must be wrapped with Unity's `L10n.Tr()` to support Japanese localization.

## Requirements

1. Wrap all user-facing strings with `L10n.Tr()`
2. Strings passed to `L10n.Tr()` (`msgid`) must be written in **English**
3. When adding a new string, add a corresponding `msgid` / `msgstr` pair to `Packages/com.murnana.audiomanagement/Editor/Localization/ja.po`
4. `msgstr` must contain the **Japanese translation**
5. For parameterized strings (`{0}`, `{1}`, etc.), apply `L10n.Tr()` first, then pass the result to `string.Format`
6. Preserve placeholders (`{0}`, `{1}`, ...) as-is in `msgstr`

## ja.po Entry Format

```po
msgid "English source string"
msgstr "Japanese translation"
```

## Code Examples

```csharp
// Simple string
Debug.LogError(L10n.Tr("Failed to load audio clip."));

// Parameterized string — apply L10n.Tr() before formatting
string message = string.Format(L10n.Tr("Audio clip '{0}' not found in group '{1}'."), clipName, groupName);
Debug.LogWarning(message);

// GUI label
EditorGUILayout.LabelField(L10n.Tr("Volume"));
```

Corresponding `ja.po` entries:

```po
msgid "Failed to load audio clip."
msgstr "オーディオクリップの読み込みに失敗しました。"

msgid "Audio clip '{0}' not found in group '{1}'."
msgstr "オーディオクリップ '{0}' がグループ '{1}' に見つかりません。"

msgid "Volume"
msgstr "音量"
```

## Out of Scope

- Code comments (`//`, `///`) are not covered by this rule (see `code-comments.md`)
- Internal debug logs or strings not displayed to the user are not covered
