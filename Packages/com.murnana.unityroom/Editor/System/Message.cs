// Copyright (c) Murnana
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.Collections.Generic;
using UnityEditor;

namespace Murnana.UnityRoom.Editor.System
{
    /// <summary>
    /// <see cref="ErrorCode" /> に対応するメッセージ文字列を提供するクラスです。
    /// メッセージは「[unityroom UNR{コード番号}] 説明文」の形式で生成されます。
    /// </summary>
    /// <remarks>
    /// メッセージの翻訳は <c>Editor/Localization/ja.po</c> で管理しています。
    /// 新しいメッセージを追加したときは、<see cref="s_Templates" /> への追加と合わせて
    /// <c>ja.po</c> にも <c>msgid</c>（英語）と <c>msgstr</c>（日本語訳）のペアを追記してください。
    /// </remarks>
    internal static class Message
    {
        /// <summary>
        /// 各エラーコードに対応するメッセージのテンプレートです。
        /// {0} や {1} はプレースホルダーで、<see cref="Format" /> メソッド呼び出し時に実際の値に置き換えられます。
        /// </summary>
        private static readonly Dictionary<ErrorCode, string> s_Templates =
            new()
            {
                {
                    ErrorCode.CompressionFormatInvalid,
                    "Compression Format must be Gzip. " +
                    "Go to Player Settings > Publishing Settings > Compression Format and set it to Gzip."
                },
                {
                    ErrorCode.DevelopmentBuildEnabled,
                    "Development Build must be disabled for unityroom submission."
                },
                {
                    // {0} = 実際の横幅, {1} = 実際の縦幅
                    ErrorCode.ResolutionMismatch,
                    "Canvas resolution is {0}x{1}. The recommended resolution for unityroom is 960x540."
                },
                {
                    ErrorCode.AddressableAssetSettingsNotFound,
                    "AddressableAssetSettings not found. " +
                    "Open Window > Asset Management > Addressables > Groups to check the settings."
                },
                {
                    // {0} = BuildPlayerContent が返したエラー文字列
                    ErrorCode.AddressablesPlayerBuildFailed,
                    "Addressables player build failed: {0}"
                }
            };

        /// <summary>
        /// 指定したエラーコードのメッセージを返します。
        /// プレースホルダーを含むメッセージには <see cref="Format" /> を使ってください。
        /// </summary>
        /// <param name="code">取得したいエラーコード</param>
        /// <returns>「[unityroom UNR{コード}] メッセージ」形式の文字列</returns>
        public static string Get(ErrorCode code)
        {
            return Message.BuildPrefix(code: code) + L10n.Tr(str: s_Templates[key: code]);
        }

        /// <summary>
        /// 指定したエラーコードのメッセージを、引数で書式化して返します。
        /// テンプレート内の {0}, {1} ... が <paramref name="args" /> の値に置き換えられます。
        /// </summary>
        /// <param name="code">取得したいエラーコード</param>
        /// <param name="args">テンプレートのプレースホルダーに埋め込む値</param>
        /// <returns>「[unityroom UNR{コード}] メッセージ」形式の文字列</returns>
        public static string Format(ErrorCode code, params object[] args)
        {
            var body = string.Format(format: L10n.Tr(str: s_Templates[key: code]), args: args);
            return Message.BuildPrefix(code: code) + body;
        }

        /// <summary>
        /// メッセージの先頭に付けるプレフィックス文字列を生成します。
        /// 形式は「[unityroom UNR{コード番号}] 」です。
        /// </summary>
        /// <param name="code">エラーコード</param>
        private static string BuildPrefix(ErrorCode code)
        {
            return $"[unityroom UNR{(int)code}] ";
        }
    }
}
