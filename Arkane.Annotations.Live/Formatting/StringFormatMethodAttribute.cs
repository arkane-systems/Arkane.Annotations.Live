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
  /// Indicates that the marked method builds a string by the format pattern and (optional) arguments.
  /// The parameter that accepts the format string should be specified in the constructor. The format string
  /// should be in the <see cref="string.Format(IFormatProvider,string,object[])"/>-like form.
  /// </summary>
  /// <example><code>
  /// [StringFormatMethod("message")]
  /// void ShowError(string message, params object[] args) { ... }
  /// 
  /// void Foo()
  /// {
  ///   ShowError("Failed: {0}"); // Warning: Non-existing argument in format string
  /// }
  /// </code></example>
  /// <seealso cref="StructuredMessageTemplateAttribute"/>
  [AttributeUsage(
    AttributeTargets.Constructor
    | AttributeTargets.Method
    | AttributeTargets.Property
    | AttributeTargets.Delegate)]
  public sealed class StringFormatMethodAttribute : Attribute
  {
    /// <param name="formatParameterName">
    /// Specifies which parameter of an annotated method should be treated as the format string.
    /// </param>
    public StringFormatMethodAttribute([NotNull] string formatParameterName)
    {
      FormatParameterName = formatParameterName;
    }

    [NotNull] public string FormatParameterName { get; }
  }
}
