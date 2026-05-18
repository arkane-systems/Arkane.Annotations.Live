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
  /// Indicates whether the marked element should be localized.
  /// </summary>
  /// <example><code>
  /// [LocalizationRequiredAttribute(true)]
  /// class Foo
  /// {
  ///   string str = "my string"; // Warning: Localizable string
  /// }
  /// </code></example>
  [AttributeUsage(AttributeTargets.All)]
  public sealed class LocalizationRequiredAttribute : Attribute
  {
    public LocalizationRequiredAttribute() : this(true) { }

    public LocalizationRequiredAttribute(bool required)
    {
      Required = required;
    }

    public bool Required { get; }
  }
}
