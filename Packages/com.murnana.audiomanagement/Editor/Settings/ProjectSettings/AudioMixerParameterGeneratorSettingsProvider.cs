// Copyright (c) Murnana
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Murnana.AudioManagement.Editor.Settings.ProjectSettings
{
    /// <summary>
    /// Project Settings ウィンドウに「Audio Management」セクションを登録する <see cref="SettingsProvider" /> です。
    /// </summary>
    internal static class AudioMixerParameterGeneratorSettingsProvider
    {
        /// <summary>
        /// Project Settings ウィンドウに「Audio Management」セクションを登録します。
        /// </summary>
        [SettingsProvider]
        private static SettingsProvider CreateSettingsProvider()
        {
            var provider = new SettingsProvider(
                path: "Project/Audio Management",
                scopes: SettingsScope.Project,
                keywords: new HashSet<string>(collection: new[] { "audio", "mixer", "parameter", "generator" })
            )
            {
                label           = L10n.Tr(str: "Audio Management"),
                activateHandler = OnActivate
            };

            return provider;
        }

        /// <summary>
        /// 設定画面を UIElements で構築します。
        /// </summary>
        private static void OnActivate(string searchContext, VisualElement rootElement)
        {
            var settings = AudioMixerParameterGeneratorSettings.instance;

            var header = new Label(text: L10n.Tr(str: "AudioMixer Parameter Generator"))
            {
                style =
                {
                    fontSize                = 18,
                    unityFontStyleAndWeight = FontStyle.Bold,
                    marginTop               = 4,
                    marginBottom            = 4
                }
            };
            rootElement.Add(child: header);

            // 出力先ディレクトリ
            var outputDirectoryField = new TextField(label: L10n.Tr(str: "Output Directory"))
            {
                value = settings.OutputDirectory
            };
            outputDirectoryField.RegisterValueChangedCallback(
                callback: evt => { settings.OutputDirectory = evt.newValue; }
            );
            rootElement.Add(child: outputDirectoryField);

            // ルート名前空間
            var rootNamespaceField = new TextField(label: L10n.Tr(str: "Root Namespace"))
            {
                value = settings.RootNamespace
            };
            rootNamespaceField.RegisterValueChangedCallback(
                callback: evt => { settings.RootNamespace = evt.newValue; }
            );
            rootElement.Add(child: rootNamespaceField);
        }
    }
}
