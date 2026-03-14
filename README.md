# unityroom-pleione

> A Unity WebGL Template for unityroom

![Unity](https://img.shields.io/badge/Unity-6000.3.10f1-black?logo=unity)
![Platform](https://img.shields.io/badge/Platform-WebGL-blue)
![URP](https://img.shields.io/badge/URP-17.3.0-orange)
![License](https://img.shields.io/badge/License-MIT-green)
![Status](https://img.shields.io/badge/Status-WIP-yellow)

[unityroom](https://unityroom.com/)（日本の WebGL ゲームホスティングサービス）へゲームを投稿するための Unity テンプレートです。

---

## Pleione（プレイオネ）について

**Pleione（プレイオネ）** はギリシャ神話に登場するオーケアノスの娘で、アトラスの妻です。
「航行する者」を意味するその名は、海への旅立ちを連想させます。

このテンプレートも、unityroom へゲームを送り出す——そんな**旅立ちの起点**として名付けました。

> **Status: WIP（作成中）** — このテンプレートは現在開発中です。予告なく仕様が変わることがあります。

---

## 機能

### ビルド前自動チェック

WebGL ビルドを実行すると、unityroom への投稿に必要な設定が自動で確認されます。
問題があればビルドが中断され、何を修正すればよいかメッセージで案内します。

| チェック項目 | 条件 | ビルドへの影響 |
|---|---|---|
| 圧縮形式 | Gzip であること | 違反するとビルド失敗（エラー） |
| Development Build | OFF であること | 違反するとビルド失敗（エラー） |
| キャンバス解像度 | 960×540 であること | 違反しても続行（警告のみ） |

### エラーメッセージの日本語対応

エラーや警告のメッセージは日本語で Unity Console に表示されます。
英語が苦手でも、次に何をすればよいか分かりやすくなっています。

---

## 動作環境

| ソフトウェア | バージョン |
|---|---|
| Unity | 6000.3.10f1 (LTS) |
| Universal Render Pipeline | 17.3.0 |
| Input System | 1.18.0 |
| Addressables | 2.9.1 |

---

## セットアップ

1. このリポジトリを Clone またはダウンロードします。
2. Unity Hub で **「プロジェクトを追加（Add project from disk）」** からフォルダを選択します。
3. Unity 6000.3.10f1 が未インストールの場合は、Unity Hub から先にインストールしてください。
4. Unity Editor が起動し、パッケージの解決が完了するまで待ちます。

---

## サードパーティアセット

サンプルには以下のサードパーティアセットが含まれています。
詳細は [`THIRD PARTY NOTICES.md`](./THIRD%20PARTY%20NOTICES.md) を参照してください。

| アセット | 著作者 | ライセンス |
|---|---|---|
| キャラクタースプライト | ぴぽや | [ぴぽや利用規約](https://pipoya.net/sozai/terms-of-use/) |
| 8-bit BGM | 魔王魂（森田交一） | [CC BY 4.0](https://creativecommons.org/licenses/by/4.0/) |
| DotGothic16 フォント | Fontworks Inc. | [SIL OFL 1.1](https://openfontlicense.org/open-font-license-official-text/) |
| SmartAddressor | CyberAgent Game & Entertainment | [MIT](https://github.com/CyberAgentGameEntertainment/SmartAddresser/blob/main/LICENSE.md) |

---

## ライセンス

Copyright 2026 Murnana
MIT License — 詳細は [`LICENSE`](./LICENSE) を参照してください。
