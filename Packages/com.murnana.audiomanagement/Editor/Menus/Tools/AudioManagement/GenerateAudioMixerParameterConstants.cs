// Copyright (c) Murnana
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using Murnana.AudioManagement.Editor.CodeGen;
using Murnana.AudioManagement.Editor.System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

namespace Murnana.AudioManagement.Editor.Menus.Tools.AudioManagement
{
    /// <summary>
    /// Unity エディターのメニュー「Tools/Audio Management」から、
    /// プロジェクト内のすべての AudioMixer アセットに対してパラメーター定数クラスを一括生成するコマンドです。
    /// </summary>
    internal sealed class GenerateAudioMixerParameterConstants
    {
        /// <summary>
        /// メニュー名
        /// </summary>
        private const string MenuItemName = "Tools/Audio Management/Generate AudioMixer Parameter Constants";

        /// <summary>
        /// メニュー項目が選択されたときに実行されるエントリーポイントです。
        /// プロジェクト内のすべての AudioMixer を検索し、それぞれの定数クラスを生成します。
        /// </summary>
        [MenuItem(itemName: MenuItemName)]
        private static void Execute()
        {
            // 未保存のアセット変更を保存し、最新の状態でコード生成を行う
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.UnloadUnusedAssetsImmediate(includeMonoReferencesAsRoots: true);

            using(new AssetDatabase.AssetEditingScope())
            {
                var cache  = EditorAssetCache<AudioMixer>.GetInstance();
                var mixers = cache.GetAssets();

                if(mixers.Count == 0)
                {
                    Debug.Log(message: L10n.Tr(str: "No AudioMixer assets found in the project."));
                    return;
                }

                foreach(var mixer in mixers)
                {
                    AudioMixerParameterGenerator.Generate(mixer: mixer);
                }
            }

            AssetDatabase.Refresh();
            Debug.Log(message: L10n.Tr(str: "Generated parameter constants for all AudioMixer assets."));
        }
    }
}
