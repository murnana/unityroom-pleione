// Copyright (c) Murnana
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System;
using Murnana.UnityRoom.Editor.System;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;

namespace Murnana.UnityRoom.Editor.CI
{
    /// <summary>
    /// Jenkins などの CI 環境から Unity をバッチモードで起動したときに
    /// 呼び出すビルド処理をまとめたクラスです。
    /// </summary>
    /// <remarks>
    /// このクラスのメソッドは Unity の -executeMethod オプションで直接指定できます。
    /// 例: -executeMethod Murnana.UnityRoom.Editor.CI.CIBuildScript.BuildAddressables
    /// -executeMethod で呼び出せるのは「引数なし・戻り値 void・public static」のメソッドだけです。
    /// </remarks>
    internal static class CIBuildScript
    {
        /// <summary>
        /// Addressables のコンテンツをビルドします。
        /// </summary>
        /// <remarks>
        /// このプロジェクトは m_BuildAddressablesWithPlayerBuild が 0（無効）に設定されているため、
        /// Player ビルド時に Addressables が自動ビルドされません。
        /// CI ではこのメソッドを先に実行してから WebGL ビルドを行う必要があります。
        /// <see cref="AddressableAssetSettings.BuildPlayerContent()" /> は
        /// Addressables Groups ウィンドウの「Build」ボタンと同じ処理を実行します。
        /// 内部では AddressableAssetSettings に設定されたアクティブなビルドスクリプト
        /// （通常は BuildScriptPackedMode）を使ってコンテンツをパッケージ化します。
        /// </remarks>
        public static void BuildAddressables()
        {
            // AddressableAssetSettings が見つからない場合はプロジェクト設定が壊れている可能性がある
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if(settings == null)
            {
                // Unity のバッチモードでは例外がそのままログに出力され、終了コード 1 になる
                throw new InvalidOperationException(
                    message: Message.Get(code: ErrorCode.AddressableAssetSettingsNotFound)
                );
            }

            AddressableAssetSettings.BuildPlayerContent(result: out var result);

            // BuildPlayerContent は例外を投げず result.Error に文字列を設定して結果を返す。
            // エラーが空でない場合は例外に変換して Unity を終了コード 1 で終了させる。
            // これをしないと Jenkins がビルド成功と誤判定する。
            if(!string.IsNullOrEmpty(value: result.Error))
            {
                throw new Exception(
                    message: Message.Format(code: ErrorCode.AddressablesPlayerBuildFailed, result.Error)
                );
            }
        }
    }
}
