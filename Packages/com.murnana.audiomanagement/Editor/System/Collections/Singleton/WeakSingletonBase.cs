// Copyright (c) Murnana
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System;

namespace Murnana.AudioManagement.Editor.System.Collections.Singleton
{
    /// <summary>
    /// WeakReference を用いたシングルトンの基底クラスです。
    /// 派生クラスは <see cref="GetInstanceImpl{T}" /> を呼び出すことで、
    /// GC によって回収可能なシングルトンインスタンスを取得できます。
    /// </summary>
    public abstract class WeakSingletonBase
    {
        /// <summary>
        /// 型 <typeparamref name="T" /> のシングルトンインスタンスを取得します。
        /// インスタンスが存在しない、または GC に回収済みの場合は <paramref name="create" /> で新規作成します。
        /// </summary>
        /// <typeparam name="T">取得するシングルトンの型。</typeparam>
        /// <param name="create">インスタンスが存在しない場合に呼び出されるファクトリ関数。</param>
        /// <returns>シングルトンインスタンス。</returns>
        protected static T GetInstanceImpl<T>(Func<T> create)
            where T : WeakSingletonBase
        {
            var store = WeakSingletonStore.GetInstance();
            return store.GetInstance(create: create);
        }

    #region Private

    #region Private readonly member fields

        /// <summary>
        /// このインスタンスが所属する <see cref="WeakSingletonStore" /> への参照です。
        /// インスタンスが生存している間、Store が GC に回収されないよう保持します。
        /// </summary>
        // ReSharper disable once UnusedMember.Local
        private readonly WeakSingletonStore m_Store = WeakSingletonStore.GetInstance();

    #endregion

    #endregion
    }
}
