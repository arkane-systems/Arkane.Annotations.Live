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
  /// Indicates that the method is a pure LINQ method with postponed enumeration (like <c>Enumerable.Select</c> or
  /// <c>Enumerable.Where</c>). This annotation allows inference of the <c>[InstantHandle]</c> annotation
  /// for parameters of a delegate type by analyzing LINQ method chains.
  /// </summary>
  [AttributeUsage(AttributeTargets.Method)]
  public sealed class LinqTunnelAttribute : Attribute { }

  /// <summary>
  /// Indicates that an <c>IEnumerable</c> passed as a parameter is not enumerated.
  /// Use this annotation to suppress the 'Possible multiple enumeration of IEnumerable' inspection.
  /// </summary>
  /// <example><code>
  /// static void ThrowIfNull&lt;T&gt;([NoEnumeration] T v, string n) where T : class
  /// {
  ///   // custom check for null but no enumeration
  /// }
  ///
  /// void Foo(IEnumerable&lt;string&gt; values)
  /// {
  ///   ThrowIfNull(values, nameof(values));
  ///   var x = values.ToList(); // No warnings about multiple enumeration
  /// }
  /// </code></example>
  [AttributeUsage(AttributeTargets.Parameter)]
  public sealed class NoEnumerationAttribute : Attribute { }
}
