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
  /// Indicates that the marked parameter is a message template where placeholders are to be replaced by
  /// the following arguments in the order in which they appear.
  /// </summary>
  /// <example><code>
  /// void LogInfo([StructuredMessageTemplate]string message, params object[] args) { ... }
  /// 
  /// void Foo()
  /// {
  ///   LogInfo("User created: {username}"); // Warning: Non-existing argument in format string
  /// }
  /// </code></example>
  /// <seealso cref="StringFormatMethodAttribute"/>
  [AttributeUsage(AttributeTargets.Parameter)]
  public sealed class StructuredMessageTemplateAttribute : Attribute { }
}
