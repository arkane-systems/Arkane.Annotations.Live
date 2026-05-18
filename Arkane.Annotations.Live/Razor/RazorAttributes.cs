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
  [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.Field)]
  public sealed class HtmlElementAttributesAttribute : Attribute
  {
    public HtmlElementAttributesAttribute() { }

    public HtmlElementAttributesAttribute([NotNull] string name)
    {
      Name = name;
    }

    [CanBeNull] public string Name { get; }
  }

  [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Field | AttributeTargets.Property)]
  public sealed class HtmlAttributeValueAttribute : Attribute
  {
    public HtmlAttributeValueAttribute([NotNull] string name)
    {
      Name = name;
    }

    [NotNull] public string Name { get; }
  }

  /// <summary>
  /// Razor attribute. Indicates that the marked parameter or method is a Razor section.
  /// Use this attribute for custom wrappers similar to
  /// <c>System.Web.WebPages.WebPageBase.RenderSection(String)</c>.
  /// </summary>
  [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Method)]
  public sealed class RazorSectionAttribute : Attribute { }

  [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
  public sealed class RazorImportNamespaceAttribute : Attribute
  {
    public RazorImportNamespaceAttribute([NotNull] string name)
    {
      Name = name;
    }

    [NotNull] public string Name { get; }
  }

  [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
  public sealed class RazorInjectionAttribute : Attribute
  {
    public RazorInjectionAttribute([NotNull] string type, [NotNull] string fieldName)
    {
      Type = type;
      FieldName = fieldName;
    }

    [NotNull] public string Type { get; }

    [NotNull] public string FieldName { get; }
  }

  [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
  public sealed class RazorDirectiveAttribute : Attribute
  {
    public RazorDirectiveAttribute([NotNull] string directive)
    {
      Directive = directive;
    }

    [NotNull] public string Directive { get; }
  }

  [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
  public sealed class RazorPageBaseTypeAttribute : Attribute
  {
    public RazorPageBaseTypeAttribute([NotNull] string baseType)
    {
      BaseType = baseType;
    }

    public RazorPageBaseTypeAttribute([NotNull] string baseType, string pageName)
    {
      BaseType = baseType;
      PageName = pageName;
    }

    [NotNull] public string BaseType { get; }
    [CanBeNull] public string PageName { get; }
  }

  [AttributeUsage(AttributeTargets.Method)]
  public sealed class RazorHelperCommonAttribute : Attribute { }

  [AttributeUsage(AttributeTargets.Property)]
  public sealed class RazorLayoutAttribute : Attribute { }

  [AttributeUsage(AttributeTargets.Method)]
  public sealed class RazorWriteLiteralMethodAttribute : Attribute { }

  [AttributeUsage(AttributeTargets.Method)]
  public sealed class RazorWriteMethodAttribute : Attribute { }

  [AttributeUsage(AttributeTargets.Parameter)]
  public sealed class RazorWriteMethodParameterAttribute : Attribute { }
}
