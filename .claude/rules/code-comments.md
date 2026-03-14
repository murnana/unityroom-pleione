# Code Comment Style

> **開発者向け日本語ドキュメント**: `Packages/com.murnana.unityroom/Documentation~/developer-guide/code-comments.md`
>
> このファイル（`.claude/rules/code-comments.md`）と上記ドキュメントは同じルールを記述しています。
> いずれかを変更した場合は、もう一方も必ず同じ内容に更新してください。

## Language
- All code comments must be written in **Japanese**
- XML doc comments (`///`) must also be in Japanese
- Error/warning messages embedded in code (strings) may remain in English

## Audience
Write comments assuming the reader is a **beginner Unity developer**. Explain:
- *Why* the code exists, not just what it does
- What happens if a condition is true/false
- What Unity concepts (e.g. `IPreprocessBuildWithReport`, `BuildOptions`) mean in plain terms

## Format
- `/// <summary>` — describe what the class or method does in one or two sentences
- `/// <remarks>` — add extra context (when it is called, caveats, related concepts)
- Inline `//` comments — explain non-obvious logic or flag important conditions
- Do **not** add comments that merely restate the code (e.g. `// increment i` above `i++`)

## Error Messages

Error and warning messages are managed via `ErrorCode` and `Message`.

- Do **not** write message strings inline in code — always use `Message.Get(code)` or `Message.Format(code, args...)`
- To add a new message:
  1. Add a value to `ErrorCode` with a unique integer (next in sequence)
  2. Add the English template string to `Message.s_Templates`
  3. Add the corresponding `msgid` / `msgstr` entry to `Editor/Localization/ja.po`
- Message source strings (`msgid`) must be in **English** — they are the keys used by `L10n.Tr()`
- Japanese translations go in `msgstr` inside `ja.po`
- Parameterized messages use `{0}`, `{1}`, ... placeholders; preserve them as-is in `msgstr`

## Example

```csharp
/// <summary>
/// WebGL ビルドを開始する前に、unityroom への投稿に必要な設定が正しいかチェックするクラスです。
/// 問題が見つかった場合はビルドを中断してエラーを表示します。
/// </summary>
internal sealed class PreprocessBuildWithReport : IPreprocessBuildWithReport {
    /// <inheritdoc />
    /// <remarks>
    /// 複数の <see cref="IPreprocessBuildWithReport"/> が登録されている場合に実行順を決める番号です。
    /// 小さい数字ほど先に実行されます。
    /// </remarks>
    public int callbackOrder => 0;
}
```
