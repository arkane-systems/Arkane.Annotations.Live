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
  /// Indicates that the value of the marked element can never be <c>null</c>.
  /// </summary>
  /// <example><code>
  /// [NotNull] object Foo()
  /// {
  ///   return null; // Warning: Possible 'null' assignment
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
  public sealed class NotNullAttribute : Attribute { }
}
