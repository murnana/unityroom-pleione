// Copyright (c) Murnana
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using Murnana.AudioManagement.Editor.System.Collections.Singleton;
using UnityEditor;
using Object = UnityEngine.Object;

namespace Murnana.AudioManagement.Editor.System
{
    /// <summary>
    /// 指定した型 <typeparamref name="TAsset" /> のアセットをプロジェクト全体から検索し、
    /// 結果をキャッシュして返すユーティリティクラスです。
    /// <see cref="WeakSingletonBase" /> を継承しているため、参照がなくなれば GC に回収されます。
    /// </summary>
    /// <typeparam name="TAsset">キャッシュ対象のアセット型（例: AudioMixer）。</typeparam>
    public sealed class EditorAssetCache<TAsset> : WeakSingletonBase,
                                                   IDisposable
        where TAsset : Object
    {
        /// <summary>
        /// キャッシュのシングルトンインスタンスを取得します。
        /// </summary>
        public static EditorAssetCache<TAsset> GetInstance()
        {
            return GetInstanceImpl(
                create: () => new EditorAssetCache<TAsset>()
            );
        }

        /// <summary>
        /// プロジェクト内の <typeparamref name="TAsset" /> 型アセットを全件取得します。
        /// 初回呼び出し時に <see cref="AssetDatabase" /> を検索し、結果をキャッシュします。
        /// </summary>
        /// <returns>見つかったアセットの読み取り専用リスト。</returns>
        public IReadOnlyList<TAsset> GetAssets()
        {
            if(m_Assets == null)
            {
                using(new AssetDatabase.AssetEditingScope())
                {
                    var type = typeof(TAsset);
                    m_Assets = AssetDatabase.FindAssetGUIDs(filter: $"t:{type.Name}")
                                            .Select(selector: AssetDatabase.LoadAssetByGUID<TAsset>)
                                            .Where(predicate: asset => asset != null)
                                            .ToArray();
                }
            }

            return m_Assets;
        }

    #region Private

    #region Private member fields

        private TAsset[]? m_Assets;

    #endregion

    #endregion

        /// <inheritdoc />
        public void Dispose()
        {
            m_Assets = null;
        }
    }
}
