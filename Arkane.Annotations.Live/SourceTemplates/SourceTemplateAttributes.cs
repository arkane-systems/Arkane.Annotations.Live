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
  /// An extension method marked with this attribute is processed by code completion
  /// as a 'Source Template'. When the extension method is completed over some expression, its source code
  /// is automatically expanded like a template at the call site.
  /// </summary>
  /// <remarks>
  /// Template method bodies can contain valid source code and/or special comments starting with '$'.
  /// Text inside these comments is added as source code when the template is applied. Template parameters
  /// can be used either as additional method parameters or as identifiers wrapped in two '$' signs.
  /// Use the <see cref="MacroAttribute"/> attribute to specify macros for parameters.
  /// The expression to be used in the expansion can be adjusted
  /// by the <see cref="SourceTemplateAttribute.Target"/> parameter.
  /// </remarks>
  /// <example>
  /// In this example, the <c>forEach</c> method is a source template available over all values
  /// of enumerable types, producing an ordinary C# <c>foreach</c> statement and placing the caret inside the block:
  /// <code>
  /// [SourceTemplate]
  /// public static void forEach&lt;T&gt;(this IEnumerable&lt;T&gt; xs)
  /// {
  ///   foreach (var x in xs)
  ///   {
  ///     //$ $END$
  ///   }
  /// }
  /// </code>
  /// </example>
  [AttributeUsage(AttributeTargets.Method)]
  public sealed class SourceTemplateAttribute : Attribute
  {
    /// <summary>
    /// Allows specifying the expression to capture for template execution if more than one expression
    /// is available at the expansion point.
    /// If not specified, <see cref="SourceTemplateTargetExpression.Inner"/> is assumed.
    /// </summary>
    public SourceTemplateTargetExpression Target { get; set; }
  }

  /// <summary>
  /// Provides a value for the <see cref="SourceTemplateAttribute"/> to define how to capture
  /// the expression at the point of expansion
  /// </summary>
  public enum SourceTemplateTargetExpression
  {
    /// <summary>Selects inner expression</summary>
    /// <example><c>value > 42.{caret}</c> captures <c>42</c></example>
    /// <example><c>_args = args.{caret}</c> captures <c>args</c></example>
    Inner = 0,

    /// <summary>Selects outer expression</summary>
    /// <example><c>value > 42.{caret}</c> captures <c>value > 42</c></example>
    /// <example><c>_args = args.{caret}</c> captures whole assignment</example>
    Outer = 1
  }

  /// <summary>
  /// Allows specifying a macro for a parameter of a <see cref="SourceTemplateAttribute">source template</see>.
  /// </summary>
  /// <remarks>
  /// You can apply the attribute to the whole method or to any of its additional parameters. The macro expression
  /// is defined in the <see cref="MacroAttribute.Expression"/> property. When applied to a method, the target
  /// template parameter is defined in the <see cref="MacroAttribute.Target"/> property. To apply the macro silently
  /// for the parameter, set the <see cref="MacroAttribute.Editable"/> property value to -1.
  /// </remarks>
  [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Method, AllowMultiple = true)]
  public sealed class MacroAttribute : Attribute
  {
    /// <summary>
    /// Allows specifying a macro that will be executed for a <see cref="SourceTemplateAttribute">source template</see>
    /// parameter when the template is expanded.
    /// </summary>
    [CanBeNull] public string Expression { get; set; }

    /// <summary>
    /// Allows specifying the occurrence of the target parameter that becomes editable when the template is deployed.
    /// </summary>
    /// <remarks>
    /// If the target parameter is used several times in the template, only one occurrence becomes editable;
    /// other occurrences are changed synchronously. To specify the zero-based index of the editable occurrence,
    /// use values >= 0. To make the parameter non-editable when the template is expanded, use -1.
    /// </remarks>
    public int Editable { get; set; }

    /// <summary>
    /// Identifies the target parameter of a <see cref="SourceTemplateAttribute">source template</see> if the
    /// <see cref="MacroAttribute"/> is applied to a template method.
    /// </summary>
    [CanBeNull] public string Target { get; set; }
  }
}
