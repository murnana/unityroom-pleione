// Copyright (c) Murnana
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Murnana.UnityRoom.Editor.System
{
    /// <inheritdoc />
    /// <summary>
    /// WebGL ビルドを開始する前に、unityroom への投稿に必要な設定が正しいかチェックするクラスです。
    /// 問題が見つかった場合はビルドを中断してエラーを表示します。
    /// </summary>
    internal sealed class PreprocessBuildWithReport : IPreprocessBuildWithReport
    {
        /// <inheritdoc />
        /// <remarks>
        /// 複数の <see cref="IPreprocessBuildWithReport" /> が登録されている場合に実行順を決める番号です。
        /// 小さい数字ほど先に実行されます。
        /// </remarks>
        public int callbackOrder
        {
            get { return 0; }
        }

        /// <inheritdoc />
        /// <remarks>
        /// Unity がビルドを開始するときに自動で呼び出されます。
        /// WebGL 以外のプラットフォームではチェックをスキップします。
        /// </remarks>
        public void OnPreprocessBuild(BuildReport report)
        {
            // WebGL 以外のビルド（Windows・Android など）はチェック不要なので何もしない
            if(report.summary.platform != BuildTarget.WebGL)
            {
                return;
            }

            ValidateCompressionFormat();
            ValidateDevelopmentBuild(report: report);
            ValidateResolution();
        }

        /// <summary>
        /// 圧縮形式が Gzip になっているかチェックします。
        /// unityroom のサーバーは Gzip を処理できるため、Gzip 以外だとゲームが正しく動作しません。
        /// </summary>
        private static void ValidateCompressionFormat()
        {
            if(PlayerSettings.WebGL.compressionFormat != WebGLCompressionFormat.Gzip)
            {
                // Gzip 以外（Brotli や Disabled）が設定されていたらビルドを中断する
                throw new BuildFailedException(
                    message: Message.Get(code: ErrorCode.CompressionFormatInvalid)
                );
            }
        }

        /// <summary>
        /// Development Build（開発用ビルド）が OFF になっているかチェックします。
        /// Development Build はファイルサイズが大幅に増えるため、投稿には使用しません。
        /// </summary>
        private static void ValidateDevelopmentBuild(BuildReport report)
        {
            // BuildOptions.Development フラグが立っていれば Development Build が有効
            if((report.summary.options & BuildOptions.Development) != 0)
            {
                throw new BuildFailedException(
                    message: Message.Get(code: ErrorCode.DevelopmentBuildEnabled)
                );
            }
        }

        /// <summary>
        /// キャンバスの解像度が unityroom の推奨サイズ（960×540）かどうか確認します。
        /// 異なっていてもビルドは続行しますが、警告を表示します。
        /// </summary>
        private static void ValidateResolution()
        {
            var width  = PlayerSettings.defaultWebScreenWidth;
            var height = PlayerSettings.defaultWebScreenHeight;
            if((width != 960) || (height != 540))
            {
                // 推奨解像度と異なる場合は警告のみ（ビルドは止めない）
                Debug.LogWarning(
                    message: Message.Format(
                        code: ErrorCode.ResolutionMismatch,
                        width,
                        height
                    )
                );
            }
        }
    }
}
