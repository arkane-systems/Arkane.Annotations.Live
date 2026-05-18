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
  /// Indicates that the value of the marked element could be <c>null</c> sometimes,
  /// so checking for <c>null</c> is required before its usage.
  /// </summary>
  /// <example><code>
  /// [CanBeNull] object Test() { ... }
  /// 
  /// void UseTest()
  /// {
  ///   var p = Test();
  ///   var s = p.ToString(); // Warning: Possible 'System.NullReferenceException'
  /// }
  /// </code></example>
  [AttributeUsage(
    AttributeTargets.Method
    | AttributeTargets.Parameter
    | AttributeTargets.Property
    | AttributeTargets.Delegate
    | AttributeTargets.Field
    | AttributeTargets.Event
    | AttributeTargets.Class
    | AttributeTargets.Interface
    | AttributeTargets.GenericParameter)]
  public sealed class CanBeNullAttribute : Attribute { }
}
