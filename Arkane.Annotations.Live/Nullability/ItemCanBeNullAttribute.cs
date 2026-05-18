// Licensed under the MIT License. See LICENSE.md at the repository root:
// https://github.com/arkane-systems/Arkane.Annotations.Live/blob/master/LICENSE.md

#nullable disable

using System;
using System.Diagnostics;
#pragma warning disable 1591
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable IntroduceOptionalParameters.Global
// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable ConvertToPrimaryConstructor
// ReSharper disable RedundantTypeDeclarationBody
// ReSharper disable ArrangeNamespaceBody
// ReSharper disable InconsistentNaming

namespace Arkane.Annotations.Live
{
  /// <summary>
  /// Can be applied to symbols of types derived from <c>IEnumerable</c> as well as to symbols of <c>Task</c>
  /// and <c>Lazy</c> classes to indicate that the value of a collection item, of the <c>Task.Result</c> property,
  /// or of the <c>Lazy.Value</c> property can be null.
  /// </summary>
  /// <example><code>
  /// public void Foo([ItemCanBeNull]List&lt;string&gt; books)
  /// {
  ///   foreach (var book in books)
  ///   {
  ///     // Warning: Possible 'System.NullReferenceException'
  ///     Console.WriteLine(book.ToUpper());
  ///   }
  /// }
  /// </code></example>
  [AttributeUsage(
    AttributeTargets.Method
    | AttributeTargets.Parameter
    | AttributeTargets.Property
    | AttributeTargets.Delegate
    | AttributeTargets.Field)]
  public sealed class ItemCanBeNullAttribute : Attribute { }
}
