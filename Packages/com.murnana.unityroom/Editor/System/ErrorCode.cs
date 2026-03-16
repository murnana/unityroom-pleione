// Copyright (c) Murnana
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Murnana.UnityRoom.Editor.System
{
    /// <summary>
    /// unityroom ビルドチェックで発生するエラー・警告のコードを定義します。
    /// コードを使うことで、ログやエラーメッセージを一覧で管理しやすくなります。
    /// </summary>
    internal enum ErrorCode
    {
        /// <summary>
        /// 圧縮形式が Gzip 以外に設定されている。
        /// unityroom のサーバーは Gzip のみ対応しているため、他の形式ではゲームが動作しません。
        /// </summary>
        CompressionFormatInvalid = 1001,

        /// <summary>
        /// Development Build（開発用ビルド）が有効になっている。
        /// ファイルサイズが大幅に増えるため、unityroom への投稿には使用しません。
        /// </summary>
        DevelopmentBuildEnabled = 1002,

        /// <summary>
        /// キャンバスの解像度が unityroom 推奨の 960×540 と異なる。
        /// エラーではなく警告扱いで、ビルドは続行されます。
        /// </summary>
        ResolutionMismatch = 1003,

        /// <summary>
        /// CI ビルド時に AddressableAssetSettings が見つからなかった。
        /// プロジェクトに Addressables の設定ファイルが存在しないか、パスが間違っています。
        /// </summary>
        AddressableAssetSettingsNotFound = 1004,

        /// <summary>
        /// CI ビルド時に Addressables のコンテンツビルドが失敗した。
        /// {0} には AddressableAssetSettings.BuildPlayerContent が返したエラー文字列が入ります。
        /// </summary>
        AddressablesPlayerBuildFailed = 1005
    }
}
