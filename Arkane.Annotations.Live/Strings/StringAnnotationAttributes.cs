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
  /// Indicates that the marked parameter, field or property is a regular expression pattern.
  /// </summary>
  [AttributeUsage(
    AttributeTargets.Parameter
    | AttributeTargets.Field
    | AttributeTargets.Property)]
  public sealed class RegexPatternAttribute : Attribute { }

  /// <summary>
  /// Indicates that the string literal passed as an argument to this parameter
  /// should not be checked for spelling or grammar errors.
  /// </summary>
  [AttributeUsage(AttributeTargets.Parameter)]
  public sealed class IgnoreSpellingAndGrammarErrorsAttribute : Attribute { }

  /// <summary>
  /// Language of the injected code fragment inside a string literal marked by the <see cref="LanguageInjectionAttribute"/>.
  /// </summary>
  public enum InjectedLanguage
  {
    CSS = 0,
    HTML = 1,
    JAVASCRIPT = 2,
    JSON = 3,
    XML = 4
  }

  /// <summary>
  /// Indicates that the marked parameter, field, or property accepts string literals
  /// containing code fragments in a specified language.
  /// </summary>
  /// <example><code>
  /// void Foo([LanguageInjection(InjectedLanguage.CSS, Prefix = "body{", Suffix = "}")] string cssProps)
  /// {
  ///   // cssProps should only contain a list of CSS properties
  /// }
  /// </code></example>
  /// <example><code>
  /// void Bar([LanguageInjection("json")] string json)
  /// {
  /// }
  /// </code></example>
  [AttributeUsage(
    AttributeTargets.Parameter
    | AttributeTargets.Field
    | AttributeTargets.Property
    | AttributeTargets.ReturnValue)]
  public sealed class LanguageInjectionAttribute : Attribute
  {
    public LanguageInjectionAttribute(InjectedLanguage injectedLanguage)
    {
      InjectedLanguage = injectedLanguage;
    }

    public LanguageInjectionAttribute([NotNull] string injectedLanguage)
    {
      InjectedLanguageName = injectedLanguage;
    }

    /// <summary>Specifies the language of the injected code fragment.</summary>
    public InjectedLanguage InjectedLanguage { get; }

    /// <summary>Specifies the language name of the injected code fragment.</summary>
    [CanBeNull] public string InjectedLanguageName { get; }

    /// <summary>Specifies the string that 'precedes' the injected string literal.</summary>
    [CanBeNull] public string Prefix { get; set; }

    /// <summary>Specifies the string that 'follows' the injected string literal.</summary>
    [CanBeNull] public string Suffix { get; set; }
  }
}
