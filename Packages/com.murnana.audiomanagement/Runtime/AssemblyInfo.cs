// Copyright (c) Murnana
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using Unity.IL2CPP.CompilerServices;

// IL2CPP ビルド時のランタイムチェックを無効化し、パフォーマンスを向上させます。
// これらのチェックはデバッグには有用ですが、リリースビルドでは不要なオーバーヘッドとなります。

// null チェックを無効化します。
// NullReferenceException がスローされなくなるため、null 参照はクラッシュとして現れる場合があります。
[assembly: Il2CppSetOption(option: Option.NullChecks, value: false)]

// 配列の境界チェックを無効化します。
// IndexOutOfRangeException がスローされなくなるため、境界外アクセスは未定義動作となります。
[assembly: Il2CppSetOption(option: Option.ArrayBoundsChecks, value: false)]

// ゼロ除算チェックを無効化します。
// DivideByZeroException がスローされなくなります。デフォルトでも無効なため影響は小さいです。
[assembly: Il2CppSetOption(option: Option.DivideByZeroChecks, value: false)]
