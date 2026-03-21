// Copyright (c) Murnana
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Murnana.AudioManagement.Editor.Settings.ProjectSettings;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

namespace Murnana.AudioManagement.Editor.CodeGen
{
    /// <summary>
    /// AudioMixer の公開パラメーター名から C# 定数クラスを自動生成するクラスです。
    /// <see cref="global::UnityEditor.SerializedObject" /> を使って AudioMixer の内部プロパティ
    /// <c>m_ExposedParameters</c> を読み取ります。
    /// メニューまたは <see cref="AssetPostprocessor" /> から呼び出されます。
    /// </summary>
    internal static class AudioMixerParameterGenerator
    {
        /// <summary>
        /// C# の予約語一覧です。パラメーター名が予約語と一致する場合にエスケープするために使用します。
        /// </summary>
        private static readonly HashSet<string> CSharpKeywords = new()
        {
            "abstract", "as", "base", "bool", "break", "byte", "case", "catch",
            "char", "checked", "class", "const", "continue", "decimal", "default",
            "delegate", "do", "double", "else", "enum", "event", "explicit",
            "extern", "false", "finally", "fixed", "float", "for", "foreach",
            "goto", "if", "implicit", "in", "int", "interface", "internal", "is",
            "lock", "long", "namespace", "new", "null", "object", "operator",
            "out", "override", "params", "private", "protected", "public",
            "readonly", "ref", "return", "sbyte", "sealed", "short", "sizeof",
            "stackalloc", "static", "string", "struct", "switch", "this", "throw",
            "true", "try", "typeof", "uint", "ulong", "unchecked", "unsafe",
            "ushort", "using", "virtual", "void", "volatile", "while"
        };

        /// <summary>
        /// 指定した AudioMixer から定数クラスを生成します。
        /// <see cref="SerializedObject" /> 経由で <c>m_ExposedParameters</c> 配列を読み取り、
        /// 各パラメーターの <c>name</c> フィールドから定数を生成します。
        /// </summary>
        /// <param name="mixer">対象の AudioMixer アセット。</param>
        public static void Generate(AudioMixer mixer)
        {
            using var serializedMixer = new SerializedObject(obj: mixer);
            var exposedParameters =
                serializedMixer.FindProperty(propertyPath: "m_ExposedParameters");

            if((exposedParameters == null) || (exposedParameters.arraySize == 0))
            {
                Debug.Log(
                    message: string.Format(
                        format: L10n.Tr(str: "AudioMixer '{0}' has no exposed parameters. Skipping generation."),
                        arg0: mixer.name
                    )
                );
                return;
            }

            // パラメーター名を収集する
            var parameterNames = new List<string>(capacity: exposedParameters.arraySize);
            for(var i = 0 ; i < exposedParameters.arraySize ; i++)
            {
                var element      = exposedParameters.GetArrayElementAtIndex(index: i);
                var nameProperty = element.FindPropertyRelative(relativePropertyPath: "name");
                if((nameProperty != null) && !string.IsNullOrEmpty(value: nameProperty.stringValue))
                {
                    parameterNames.Add(item: nameProperty.stringValue);
                }
            }

            if(parameterNames.Count == 0)
            {
                Debug.Log(
                    message: string.Format(
                        format: L10n.Tr(str: "AudioMixer '{0}' has no exposed parameters. Skipping generation."),
                        arg0: mixer.name
                    )
                );
                return;
            }

            var mixerName = SanitizeIdentifier(name: mixer.name);
            var sourceCode = BuildSourceCode(
                originalMixerName: mixer.name,
                sanitizedMixerName: mixerName,
                parameterNames: parameterNames
            );
            var outputPath = GetOutputPath(sanitizedMixerName: mixerName);

            // 出力先ディレクトリが存在しない場合は作成する
            var directory = Path.GetDirectoryName(path: outputPath);
            if(!string.IsNullOrEmpty(value: directory) && !Directory.Exists(path: directory))
            {
                Directory.CreateDirectory(path: directory);
            }

            // 内容が同一なら書き込みをスキップして不要な再コンパイルを回避する
            if(File.Exists(path: outputPath))
            {
                var existingContent = File.ReadAllText(path: outputPath);
                if(existingContent == sourceCode)
                {
                    return;
                }
            }

            File.WriteAllText(
                path: outputPath,
                contents: sourceCode,
                encoding: Encoding.UTF8
            );
            Debug.Log(
                message: string.Format(
                    format: L10n.Tr(str: "Generated parameter constants for '{0}' at '{1}'."),
                    arg0: mixer.name,
                    arg1: outputPath
                )
            );
        }

        /// <summary>
        /// 生成する C# ソースコードを組み立てます。
        /// </summary>
        /// <param name="originalMixerName">元の AudioMixer 名（コメント用）。</param>
        /// <param name="sanitizedMixerName">サニタイズ済みの Mixer 名（クラス名用）。</param>
        /// <param name="parameterNames">公開パラメーター名のリスト。</param>
        /// <returns>生成された C# ソースコード文字列。</returns>
        private static string BuildSourceCode(
            string       originalMixerName,
            string       sanitizedMixerName,
            List<string> parameterNames
        )
        {
            var settings      = AudioMixerParameterGeneratorSettings.instance;
            var rootNamespace = settings.RootNamespace;

            // EditorSettings に応じた改行文字を決定します
            var newLine = GetNewLine(lineEndingsMode: EditorSettings.lineEndingsForNewScripts);
            var builder = new StringBuilder();

            // auto-generated ヘッダー
            builder.Append(value: "// <auto-generated>");
            builder.Append(value: newLine);
            builder.Append(
                value: "// このファイルはツールによって自動生成されました。手動で編集しないでください。"
            );
            builder.Append(value: newLine);
            builder.Append(
                value: string.Format(format: "// Generated from: {0}", arg0: originalMixerName)
            );
            builder.Append(value: newLine);
            builder.Append(value: "// </auto-generated>");
            builder.Append(value: newLine);
            builder.Append(value: newLine);

            // 名前空間の開始
            builder.Append(
                value: string.Format(format: "namespace {0}", arg0: rootNamespace)
            );
            builder.Append(value: newLine);
            builder.Append(value: "{");
            builder.Append(value: newLine);

            // クラスの XML ドキュメント
            builder.Append(value: "    /// <summary>");
            builder.Append(value: newLine);
            builder.Append(
                value: string.Format(
                    format: "    /// {0} の公開パラメーター名を定義する定数クラスです。",
                    arg0: originalMixerName
                )
            );
            builder.Append(value: newLine);
            builder.Append(
                value: "    /// <see cref=\"global::UnityEngine.Audio.AudioMixer.SetFloat\"/> や"
            );
            builder.Append(value: newLine);
            builder.Append(
                value: "    /// <see cref=\"global::UnityEngine.Audio.AudioMixer.GetFloat\"/> に渡す文字列として使用します。"
            );
            builder.Append(value: newLine);
            builder.Append(value: "    /// </summary>");
            builder.Append(value: newLine);

            // クラス宣言
            builder.Append(
                value: string.Format(
                    format: "    public static class {0}Parameters",
                    arg0: sanitizedMixerName
                )
            );
            builder.Append(value: newLine);
            builder.Append(value: "    {");
            builder.Append(value: newLine);

            // サニタイズ後の名前の重複を検出するための辞書
            var identifierCounts = new Dictionary<string, int>();

            foreach(var parameterName in parameterNames)
            {
                var identifier = SanitizeIdentifier(name: parameterName);

                // 重複チェック
                if(!identifierCounts.TryAdd(identifier, 0))
                {
                    identifierCounts[key: identifier]++;
                    identifier = string.Format(
                        format: "{0}_{1}",
                        arg0: identifier,
                        arg1: identifierCounts[key: identifier]
                    );
                }

                builder.Append(
                    value: string.Format(
                        format: "        public const string {0} = \"{1}\";",
                        arg0: identifier,
                        arg1: parameterName
                    )
                );
                builder.Append(value: newLine);
            }

            builder.Append(value: "    }");
            builder.Append(value: newLine);
            builder.Append(value: "}");
            builder.Append(value: newLine);

            return builder.ToString();
        }

        /// <summary>
        /// パラメーター名を有効な C# 識別子に変換します。
        /// 空白や特殊文字はアンダースコアに置換し、先頭が数字の場合は <c>_</c> を付加します。
        /// C# の予約語に一致する場合は <c>@</c> を先頭に付けます。
        /// </summary>
        /// <param name="name">元のパラメーター名。</param>
        /// <returns>有効な C# 識別子。</returns>
        private static string SanitizeIdentifier(string name)
        {
            if(string.IsNullOrEmpty(value: name))
            {
                return"_";
            }

            var builder = new StringBuilder(capacity: name.Length);

            foreach(var c in name)
            {
                if(char.IsLetterOrDigit(c: c) || (c == '_'))
                {
                    builder.Append(value: c);
                }
                else
                {
                    builder.Append(value: '_');
                }
            }

            var result = builder.ToString();

            // 空文字になった場合のフォールバック
            if(result.Length == 0)
            {
                return"_";
            }

            // 先頭が数字の場合はアンダースコアを付加する
            if(char.IsDigit(c: result[index: 0]))
            {
                result = "_" + result;
            }

            // C# 予約語と一致する場合は @ を付ける
            if(CSharpKeywords.Contains(item: result))
            {
                result = "@" + result;
            }

            return result;
        }

        /// <summary>
        /// <see cref="LineEndingsMode" /> に対応する改行文字列を返します。
        /// </summary>
        /// <param name="lineEndingsMode">エディター設定の改行モード。</param>
        /// <returns>改行文字列。</returns>
        private static string GetNewLine(LineEndingsMode lineEndingsMode)
        {
            switch(lineEndingsMode)
            {
                case LineEndingsMode.Unix:
                    return"\n";
                case LineEndingsMode.Windows:
                    return"\r\n";
                case LineEndingsMode.OSNative:
                default:
                    return Environment.NewLine;
            }
        }

        /// <summary>
        /// 生成先のファイルパスを返します。
        /// </summary>
        /// <param name="sanitizedMixerName">サニタイズ済みの Mixer 名。</param>
        /// <returns>出力ファイルの相対パス。</returns>
        private static string GetOutputPath(string sanitizedMixerName)
        {
            var settings =
                AudioMixerParameterGeneratorSettings.instance;
            var outputDirectory = settings.OutputDirectory;

            return Path.Combine(
                path1: outputDirectory,
                path2: string.Format(format: "{0}Parameters.cs", arg0: sanitizedMixerName)
            );
        }
    }
}
