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
  /// Indicates that a parameter is a path to a file or a folder within a web project.
  /// Path can be relative or absolute, starting from web root (~).
  /// </summary>
  [AttributeUsage(
    AttributeTargets.Parameter
    | AttributeTargets.Field
    | AttributeTargets.Property)]
  public sealed class PathReferenceAttribute : Attribute
  {
    public PathReferenceAttribute() { }

    public PathReferenceAttribute([NotNull, PathReference] string basePath)
    {
      BasePath = basePath;
    }

    [CanBeNull] public string BasePath { get; }
  }

  /// <summary>
  /// Defines a code search pattern using the Structural Search and Replace syntax.
  /// It allows you to find and, if necessary, replace blocks of code that match a specific pattern.
  /// </summary>
  /// <remarks>
  /// Search and replace patterns consist of a textual part and placeholders.
  /// Textural part must contain only identifiers allowed in the target language and will be matched exactly
  /// (whitespaces, tabulation characters, and line breaks are ignored).
  /// Placeholders allow matching variable parts of the target code blocks.
  /// <br/>
  /// A placeholder has the following format:
  /// <c>$placeholder_name$</c> - where <c>placeholder_name</c> is an arbitrary identifier.
  /// </remarks>
  /// <seealso href="https://www.jetbrains.com/help/resharper/Navigation_and_Search__Structural_Search_and_Replace.html">
  /// Structural Search and Replace</seealso>
  /// <seealso href="https://www.jetbrains.com/help/resharper/Code_Analysis__Find_and_Update_Obsolete_APIs.html">
  /// Find and update deprecated APIs</seealso>
  [AttributeUsage(
    AttributeTargets.Method
    | AttributeTargets.Constructor
    | AttributeTargets.Property
    | AttributeTargets.Field
    | AttributeTargets.Event
    | AttributeTargets.Interface
    | AttributeTargets.Class
    | AttributeTargets.Struct
    | AttributeTargets.Enum,
    AllowMultiple = true,
    Inherited = false)]
  public sealed class CodeTemplateAttribute : Attribute
  {
    public CodeTemplateAttribute(string searchTemplate)
    {
      SearchTemplate = searchTemplate;
    }

    /// <summary>
    /// Structural search pattern.
    /// </summary>
    /// <remarks>
    /// The pattern includes a textual part, which must only contain identifiers allowed in the target language
    /// and placeholders to match variable parts of the target code blocks.
    /// </remarks>
    public string SearchTemplate { get; }

    /// <summary>
    /// Message to show when a code block matching the search pattern was found.
    /// </summary>
    /// <remarks>
    /// You can also prepend the message text with 'Error:', 'Warning:', 'Suggestion:' or 'Hint:' prefix
    /// to specify the pattern severity.
    /// Code patterns with replace templates have the 'Suggestion' severity by default.
    /// If a replace pattern is not provided, the pattern will have the 'Warning' severity.
    ///</remarks>
    public string Message { get; set; }

    /// <summary>
    /// Replace pattern to use for replacing a matched pattern.
    /// </summary>
    public string ReplaceTemplate { get; set; }

    /// <summary>
    /// Replace message to show in the light bulb.
    /// </summary>
    public string ReplaceMessage { get; set; }

    /// <summary>
    /// Apply code formatting after code replacement.
    /// </summary>
    public bool FormatAfterReplace { get; set; } = true;

    /// <summary>
    /// Whether similar code blocks should be matched.
    /// </summary>
    public bool MatchSimilarConstructs { get; set; }

    /// <summary>
    /// Automatically insert namespace import directives or remove qualifiers
    /// that become redundant after the template is applied.
    /// </summary>
    public bool ShortenReferences { get; set; }

    /// <summary>
    /// The string to use as a suppression key.
    /// By default, the following suppression key is used: <c>CodeTemplate_SomeType_SomeMember</c>,
    /// where 'SomeType' and 'SomeMember' are names of the associated containing type and member,
    /// to which this attribute is applied.
    /// </summary>
    public string SuppressionKey { get; set; }
  }

  /// <summary>
  /// Static properties annotated with this attribute are automatically discovered by IDE debugger
  /// and included to Watches window.
  /// </summary>
  /// <example><code>
  /// public static class DispatcherDebugWatch
  /// {
  ///   [DebuggerGlobalWatch(Name = "UI Thread")]
  ///   public static bool IsUIThread =>
  ///     System.Windows.Application.Current.Dispatcher.CheckAccess();
  /// }
  /// </code></example>
  [AttributeUsage(AttributeTargets.Property)]
  [Conditional("DEBUG")]
  public sealed class DebuggerGlobalWatchAttribute : Attribute
  {
    /// <summary>
    /// Specifies the display name for the watch entry.
    /// </summary>
    public string Name { get; set; }
  }
}
