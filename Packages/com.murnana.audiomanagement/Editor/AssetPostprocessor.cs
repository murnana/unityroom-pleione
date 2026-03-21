// Copyright (c) Murnana
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using Murnana.AudioManagement.Editor.CodeGen;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

namespace Murnana.AudioManagement.Editor
{
    /// <summary>
    /// AudioMixer アセットの変更を検知し、パラメーター定数クラスを自動的に再生成するクラスです。
    /// <see cref="UnityEditor.AssetPostprocessor.OnPostprocessAllAssets" /> を利用して、
    /// <c>.mixer</c> ファイルのインポートや変更を監視します。
    /// </summary>
    internal sealed class AssetPostprocessor : UnityEditor.AssetPostprocessor
    {
        /// <summary>
        /// アセットがインポート・削除・移動されたときに呼び出されるコールバックです。
        /// <c>.mixer</c> ファイルが含まれている場合、対応する定数クラスを再生成します。
        /// </summary>
        private static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths
        )
        {
            // インポートまたは変更された mixer ファイルを処理する
            foreach(var assetPath in importedAssets)
            {
                if(!assetPath.EndsWith(value: ".mixer"))
                {
                    continue;
                }

                var mixer = AssetDatabase.LoadAssetAtPath<AudioMixer>(assetPath: assetPath);
                if(mixer != null)
                {
                    AudioMixerParameterGenerator.Generate(mixer: mixer);
                }
            }

            // 移動された mixer ファイルも再生成対象とする
            foreach(var assetPath in movedAssets)
            {
                if(!assetPath.EndsWith(value: ".mixer"))
                {
                    continue;
                }

                var mixer = AssetDatabase.LoadAssetAtPath<AudioMixer>(assetPath: assetPath);
                if(mixer != null)
                {
                    AudioMixerParameterGenerator.Generate(mixer: mixer);
                }
            }

            // 削除された mixer については警告ログのみ出力する
            // 生成済みファイルの自動削除は意図しないデータ損失を防ぐため行わない
            foreach(var assetPath in deletedAssets)
            {
                if(!assetPath.EndsWith(value: ".mixer"))
                {
                    continue;
                }

                Debug.LogWarning(
                    message: string.Format(
                        format: L10n.Tr(
                            str: "AudioMixer '{0}' was deleted. "
                                 + "Please manually remove the corresponding generated parameter constants file if it is no longer needed."
                        ),
                        arg0: assetPath
                    )
                );
            }
        }
    }
}
