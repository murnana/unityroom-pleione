// Copyright (c) Murnana
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using UnityEditor;
using UnityEngine;

namespace Murnana.AudioManagement.Editor.Settings.ProjectSettings
{
    /// <summary>
    /// AudioMixer パラメーター定数ジェネレーターの設定を管理するクラスです。
    /// <see cref="ScriptableSingleton{T}" /> を利用して ProjectSettings フォルダーに自動保存されます。
    /// </summary>
    [FilePath(
        relativePath: FileRelativePath,
        location: FilePathAttribute.Location.ProjectFolder
    )]
    internal sealed class AudioMixerParameterGeneratorSettings
        : ScriptableSingleton<AudioMixerParameterGeneratorSettings>
    {
        /// <summary>
        /// 生成先ディレクトリのデフォルトパスです。
        /// </summary>
        private const string DefaultOutputDirectory = "Assets/Murnana AudioManagement/Scripts/Runtime/Generated";

        /// <summary>
        /// 生成コードの名前空間のデフォルト値です。
        /// </summary>
        private const string DefaultRootNamespace = "Murnana.AudioManagement";

        /// <summary>
        /// ファイルパス
        /// </summary>
        private const string FileRelativePath
            = "ProjectSettings/com.murnana.audiomanagement/AudioMixerParameterGeneratorSettings.asset";

        [SerializeField] private string m_OutputDirectory = DefaultOutputDirectory;

        [SerializeField] private string m_RootNamespace = DefaultRootNamespace;

        /// <summary>
        /// 生成先ディレクトリのパスを取得または設定します。
        /// </summary>
        public string OutputDirectory
        {
            get { return m_OutputDirectory; }
            set
            {
                m_OutputDirectory = value;
                Save(saveAsText: true);
            }
        }

        /// <summary>
        /// 生成コードのルート名前空間を取得または設定します。
        /// </summary>
        public string RootNamespace
        {
            get { return m_RootNamespace; }
            set
            {
                m_RootNamespace = value;
                Save(saveAsText: true);
            }
        }
    }
}
