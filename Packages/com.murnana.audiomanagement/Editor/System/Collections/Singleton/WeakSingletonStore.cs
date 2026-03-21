// Copyright (c) Murnana
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System;
using System.Collections.Generic;
using UnityEditor;

namespace Murnana.AudioManagement.Editor.System.Collections.Singleton
{
    /// <summary>
    /// <see cref="WeakSingletonBase" /> 派生クラスのインスタンスを WeakReference で管理するストアです。
    /// 参照が不要になった場合は GC によって自動的に回収されます。
    /// エディター終了時に <see cref="IDisposable" /> / <see cref="IAsyncDisposable" /> を実装した
    /// インスタンスのクリーンアップも行います。
    /// </summary>
    internal sealed class WeakSingletonStore
    {
        /// <summary>
        /// ストア自体のシングルトンインスタンスを取得します。
        /// WeakReference で保持されているため、参照がなくなれば GC に回収されます。
        /// </summary>
        internal static WeakSingletonStore GetInstance()
        {
            if((s_Instance != null) && s_Instance.TryGetTarget(target: out var target))
            {
                return target;
            }

            var newTarget = new WeakSingletonStore();
            if(s_Instance == null)
            {
                s_Instance = new(target: newTarget);
            }
            else
            {
                s_Instance.SetTarget(target: newTarget);
            }

            return newTarget;
        }

        /// <summary>
        /// 指定した型 <typeparamref name="TInstance" /> のシングルトンインスタンスを取得します。
        /// 既にインスタンスが存在し GC に回収されていなければそれを返し、
        /// そうでなければ <paramref name="create" /> で新しいインスタンスを作成して登録します。
        /// </summary>
        /// <typeparam name="TInstance">取得するシングルトンの型。</typeparam>
        /// <param name="create">インスタンスが存在しない場合に呼び出されるファクトリ関数。</param>
        /// <returns>シングルトンインスタンス。</returns>
        internal TInstance GetInstance<TInstance>(Func<TInstance> create) where TInstance : WeakSingletonBase
        {
            lock(m_Lock)
            {
                var type = typeof(TInstance);

                // 既に登録済みの型かどうかを確認する
                if(m_ReferenceTable.TryGetValue(key: type, value: out var reference))
                {
                    // WeakReference のターゲットがまだ生存していればそのまま返す
                    if(reference.TryGetTarget(target: out var target)
                       && target is TInstance instance)
                    {
                        return instance;
                    }

                    // GC に回収されていた場合は新しいインスタンスで置き換える
                    var newTarget = create();
                    reference.SetTarget(target: newTarget);
                    m_ReferenceTable[key: type] = reference;
                    return newTarget;
                }

                // 未登録の型なので新規作成して登録する
                var newInstance  = create();
                var newReference = new WeakReference<WeakSingletonBase>(target: newInstance);
                if(!m_ReferenceTable.TryAdd(key: type, value: newReference))
                {
                    m_ReferenceTable[key: type] = newReference;
                }

                return newInstance;
            }
        }

    #region Private

    #region Private static fields

        /// <summary>
        /// ストア自身の WeakReference。参照元がなくなれば GC に回収される。
        /// </summary>
        private static WeakReference<WeakSingletonStore>? s_Instance;

    #endregion

    #region Private readonly member fields

        /// <summary>
        /// スレッドセーフな操作を保証するためのロックオブジェクト。
        /// </summary>
        private readonly object m_Lock = new();

        /// <summary>
        /// 型をキーとしてシングルトンインスタンスの WeakReference を保持するテーブル。
        /// </summary>
        private readonly Dictionary<Type, WeakReference<WeakSingletonBase>> m_ReferenceTable = new();

    #endregion

    #region Private construcator

        /// <summary>
        /// コンストラクタ。エディター終了時のクリーンアップコールバックを登録します。
        /// </summary>
        private WeakSingletonStore()
        {
            EditorApplication.quitting += EditorApplicationOnQuitting;
        }

    #endregion

    #region Private member methods

        /// <summary>
        /// 登録済みのインスタンスのうち、<typeparamref name="TInstance" /> 型のものに対して
        /// 指定したアクションを実行します。
        /// </summary>
        private void ExecuteInstance<TInstance>(Action<TInstance> action) where TInstance : class
        {
            foreach(var reference in m_ReferenceTable.Values)
            {
                if(!reference.TryGetTarget(target: out var target))
                {
                    continue;
                }

                if(target is not TInstance instance)
                {
                    continue;
                }

                action(obj: instance);
            }
        }

        /// <summary>
        /// エディター終了時に呼び出されるコールバックです。
        /// 登録済みインスタンスの Dispose を呼び出し、テーブルをクリアします。
        /// </summary>
        private void EditorApplicationOnQuitting()
        {
            // コールバックの二重呼び出しを防ぐため、まず登録を解除する
            EditorApplication.quitting -= EditorApplicationOnQuitting;

            lock(m_Lock)
            {
                // IAsyncDisposable → IDisposable の順にクリーンアップする
                ExecuteInstance<IAsyncDisposable>(action: disposable => disposable.DisposeAsync());
                ExecuteInstance<IDisposable>(action: disposable => disposable.Dispose());
                m_ReferenceTable.Clear();
            }
        }

    #endregion

    #endregion
    }
}
