# コメントの書き方ガイド

このドキュメントは、`com.murnana.unityroom` パッケージのコードを書くときのコメントルールをまとめたものです。

---

## コメントの言語

- コード内のコメント（`//`、`///`）はすべて**日本語**で書いてください
- XML ドキュメントコメント（`/// <summary>` など）も日本語です
- コードに埋め込む文字列（エラーメッセージ・警告メッセージ）は英語のままにします

---

## 読み手を意識する

コメントは**Unity 初心者の開発者**が読むことを想定して書いてください。

- コードが**なぜ**存在するのかを説明する（何をするかだけでは不十分）
- 条件が true / false のときに何が起きるかを明示する
- `IPreprocessBuildWithReport` や `BuildOptions` といった Unity 固有の概念を平易な言葉で補足する

---

## コメントの種類と書き方

### `/// <summary>`

クラスやメソッドが**何をするものか**を 1〜2 文で説明します。

```csharp
/// <summary>
/// WebGL ビルドを開始する前に、unityroom への投稿に必要な設定が正しいかチェックするクラスです。
/// 問題が見つかった場合はビルドを中断してエラーを表示します。
/// </summary>
internal sealed class PreprocessBuildWithReport : IPreprocessBuildWithReport { }
```

### `/// <remarks>`

呼び出されるタイミング・注意点・関連する概念など、**補足情報**を書きます。

```csharp
/// <inheritdoc />
/// <remarks>
/// 複数の <see cref="IPreprocessBuildWithReport"/> が登録されている場合に実行順を決める番号です。
/// 小さい数字ほど先に実行されます。
/// </remarks>
public int callbackOrder => 0;
```

### インラインコメント `//`

一目では意味がわかりにくいロジックや、重要な条件を補足します。
コードをそのまま言い換えるだけのコメントは書きません。

```csharp
// Gzip 以外（Brotli や Disabled）が設定されていたらビルドを中断する
if (PlayerSettings.WebGL.compressionFormat != WebGLCompressionFormat.Gzip) { ... }

// NG 例：コードを言い換えているだけで価値がない
// i をインクリメントする
i++;
```

---

## エラーメッセージのルール

エラー・警告メッセージは `ErrorCode` と `Message` で一元管理します。
コードにメッセージ文字列を直書きしてはいけません。

### メッセージの使い方

```csharp
// プレースホルダーなし
throw new BuildFailedException(message: Message.Get(ErrorCode.CompressionFormatInvalid));

// プレースホルダーあり（{0}, {1} に値を埋め込む）
Debug.LogWarning(message: Message.Format(ErrorCode.ResolutionMismatch, width, height));
```

### 新しいメッセージを追加する手順

1. **`ErrorCode.cs`** に新しい値を追加する（番号は連番）
2. **`Message.cs`** の `s_Templates` に英語のテンプレート文字列を追加する
3. **`Editor/Localization/ja.po`** に `msgid`（英語）と `msgstr`（日本語訳）のペアを追加する

```po
msgid "Your new English message here."
msgstr "ここに日本語訳を書きます。"
```

パラメーターが必要な場合は `{0}`, `{1}`, ... のプレースホルダーを使い、`msgstr` にもそのまま残してください。
