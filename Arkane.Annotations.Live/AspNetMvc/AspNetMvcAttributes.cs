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
  [AttributeUsage(
    AttributeTargets.Assembly
    | AttributeTargets.Field
    | AttributeTargets.Property,
    AllowMultiple = true)]
  public sealed class AspMvcAreaMasterLocationFormatAttribute : Attribute
  {
    public AspMvcAreaMasterLocationFormatAttribute([NotNull] string format) { Format = format; }
    [NotNull] public string Format { get; }
  }

  [AttributeUsage(
    AttributeTargets.Assembly
    | AttributeTargets.Field
    | AttributeTargets.Property,
    AllowMultiple = true)]
  public sealed class AspMvcAreaPartialViewLocationFormatAttribute : Attribute
  {
    public AspMvcAreaPartialViewLocationFormatAttribute([NotNull] string format) { Format = format; }
    [NotNull] public string Format { get; }
  }

  [AttributeUsage(
    AttributeTargets.Assembly
    | AttributeTargets.Field
    | AttributeTargets.Property,
    AllowMultiple = true)]
  public sealed class AspMvcAreaViewComponentViewLocationFormatAttribute : Attribute
  {
    public AspMvcAreaViewComponentViewLocationFormatAttribute([NotNull] string format) { Format = format; }
    [NotNull] public string Format { get; }
  }

  [AttributeUsage(
    AttributeTargets.Assembly
    | AttributeTargets.Field
    | AttributeTargets.Property,
    AllowMultiple = true)]
  public sealed class AspMvcAreaViewLocationFormatAttribute : Attribute
  {
    public AspMvcAreaViewLocationFormatAttribute([NotNull] string format) { Format = format; }
    [NotNull] public string Format { get; }
  }

  [AttributeUsage(
    AttributeTargets.Assembly
    | AttributeTargets.Field
    | AttributeTargets.Property,
    AllowMultiple = true)]
  public sealed class AspMvcMasterLocationFormatAttribute : Attribute
  {
    public AspMvcMasterLocationFormatAttribute([NotNull] string format) { Format = format; }
    [NotNull] public string Format { get; }
  }

  [AttributeUsage(
    AttributeTargets.Assembly
    | AttributeTargets.Field
    | AttributeTargets.Property,
    AllowMultiple = true)]
  public sealed class AspMvcPartialViewLocationFormatAttribute : Attribute
  {
    public AspMvcPartialViewLocationFormatAttribute([NotNull] string format) { Format = format; }
    [NotNull] public string Format { get; }
  }

  [AttributeUsage(
    AttributeTargets.Assembly
    | AttributeTargets.Field
    | AttributeTargets.Property,
    AllowMultiple = true)]
  public sealed class AspMvcViewComponentViewLocationFormatAttribute : Attribute
  {
    public AspMvcViewComponentViewLocationFormatAttribute([NotNull] string format) { Format = format; }
    [NotNull] public string Format { get; }
  }

  [AttributeUsage(
    AttributeTargets.Assembly
    | AttributeTargets.Field
    | AttributeTargets.Property,
    AllowMultiple = true)]
  public sealed class AspMvcViewLocationFormatAttribute : Attribute
  {
    public AspMvcViewLocationFormatAttribute([NotNull] string format) { Format = format; }
    [NotNull] public string Format { get; }
  }

  /// <summary>
  /// ASP.NET MVC attribute. If applied to a parameter, indicates that the parameter
  /// is an MVC action. If applied to a method, the MVC action name is calculated
  /// implicitly from the context. Use this attribute for custom wrappers similar to
  /// <c>System.Web.Mvc.Html.ChildActionExtensions.RenderAction(HtmlHelper, String)</c>.
  /// </summary>
  [AttributeUsage(
    AttributeTargets.Parameter
    | AttributeTargets.Method
    | AttributeTargets.Field
    | AttributeTargets.Property)]
  public sealed class AspMvcActionAttribute : Attribute
  {
    public AspMvcActionAttribute() { }

    public AspMvcActionAttribute([NotNull] string anonymousProperty)
    {
      AnonymousProperty = anonymousProperty;
    }

    [CanBeNull] public string AnonymousProperty { get; }
  }

  /// <summary>
  /// ASP.NET MVC attribute. Indicates that the marked parameter is an MVC area.
  /// Use this attribute for custom wrappers similar to
  /// <c>System.Web.Mvc.Html.ChildActionExtensions.RenderAction(HtmlHelper, String)</c>.
  /// </summary>
  [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Field | AttributeTargets.Property)]
  public sealed class AspMvcAreaAttribute : Attribute
  {
    public AspMvcAreaAttribute() { }

    public AspMvcAreaAttribute([NotNull] string anonymousProperty)
    {
      AnonymousProperty = anonymousProperty;
    }

    [CanBeNull] public string AnonymousProperty { get; }
  }

  /// <summary>
  /// ASP.NET MVC attribute. If applied to a parameter, indicates that the parameter is
  /// an MVC controller. If applied to a method, the MVC controller name is calculated
  /// implicitly from the context. Use this attribute for custom wrappers similar to
  /// <c>System.Web.Mvc.Html.ChildActionExtensions.RenderAction(HtmlHelper, String, String)</c>.
  /// </summary>
  [AttributeUsage(
    AttributeTargets.Parameter
    | AttributeTargets.Method
    | AttributeTargets.Field
    | AttributeTargets.Property)]
  public sealed class AspMvcControllerAttribute : Attribute
  {
    public AspMvcControllerAttribute() { }

    public AspMvcControllerAttribute([NotNull] string anonymousProperty)
    {
      AnonymousProperty = anonymousProperty;
    }

    [CanBeNull] public string AnonymousProperty { get; }
  }

  /// <summary>
  /// ASP.NET MVC attribute. Indicates that the marked parameter is an MVC Master. Use this attribute
  /// for custom wrappers similar to <c>System.Web.Mvc.Controller.View(String, String)</c>.
  /// </summary>
  [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Field | AttributeTargets.Property)]
  public sealed class AspMvcMasterAttribute : Attribute { }

  /// <summary>
  /// ASP.NET MVC attribute. Indicates that the marked parameter is an MVC model type. Use this attribute
  /// for custom wrappers similar to <c>System.Web.Mvc.Controller.View(String, Object)</c>.
  /// </summary>
  [AttributeUsage(AttributeTargets.Parameter)]
  public sealed class AspMvcModelTypeAttribute : Attribute { }

  /// <summary>
  /// ASP.NET MVC attribute. If applied to a parameter, indicates that the parameter is an MVC
  /// partial view. If applied to a method, the MVC partial view name is calculated implicitly
  /// from the context. Use this attribute for custom wrappers similar to
  /// <c>System.Web.Mvc.Html.RenderPartialExtensions.RenderPartial(HtmlHelper, String)</c>.
  /// </summary>
  [AttributeUsage(
    AttributeTargets.Parameter
    | AttributeTargets.Method
    | AttributeTargets.Field
    | AttributeTargets.Property)]
  public sealed class AspMvcPartialViewAttribute : Attribute { }

  /// <summary>
  /// ASP.NET MVC attribute. Allows disabling inspections for MVC views within a class or a method.
  /// </summary>
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
  public sealed class AspMvcSuppressViewErrorAttribute : Attribute { }

  /// <summary>
  /// ASP.NET MVC attribute. Indicates that a parameter is an MVC display template.
  /// Use this attribute for custom wrappers similar to
  /// <c>System.Web.Mvc.Html.DisplayExtensions.DisplayForModel(HtmlHelper, String)</c>.
  /// </summary>
  [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Field | AttributeTargets.Property)]
  public sealed class AspMvcDisplayTemplateAttribute : Attribute { }

  /// <summary>
  /// ASP.NET MVC attribute. Indicates that the marked parameter is an MVC editor template.
  /// Use this attribute for custom wrappers similar to
  /// <c>System.Web.Mvc.Html.EditorExtensions.EditorForModel(HtmlHelper, String)</c>.
  /// </summary>
  [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Field | AttributeTargets.Property)]
  public sealed class AspMvcEditorTemplateAttribute : Attribute { }

  /// <summary>
  /// ASP.NET MVC attribute. Indicates that the marked parameter is an MVC template.
  /// Use this attribute for custom wrappers similar to
  /// <c>System.ComponentModel.DataAnnotations.UIHintAttribute(System.String)</c>.
  /// </summary>
  [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Field | AttributeTargets.Property)]
  public sealed class AspMvcTemplateAttribute : Attribute { }

  /// <summary>
  /// ASP.NET MVC attribute. If applied to a parameter, indicates that the parameter
  /// is an MVC view component. If applied to a method, the MVC view name is calculated implicitly
  /// from the context. Use this attribute for custom wrappers similar to
  /// <c>System.Web.Mvc.Controller.View(Object)</c>.
  /// </summary>
  [AttributeUsage(
    AttributeTargets.Parameter
    | AttributeTargets.Method
    | AttributeTargets.Field
    | AttributeTargets.Property)]
  public sealed class AspMvcViewAttribute : Attribute { }

  /// <summary>
  /// ASP.NET MVC attribute. If applied to a parameter, indicates that the parameter
  /// is an MVC view component name.
  /// </summary>
  [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Field | AttributeTargets.Property)]
  public sealed class AspMvcViewComponentAttribute : Attribute { }

  /// <summary>
  /// ASP.NET MVC attribute. If applied to a parameter, indicates that the parameter
  /// is an MVC view component view. If applied to a method, the MVC view component view name is default.
  /// </summary>
  [AttributeUsage(
    AttributeTargets.Parameter
    | AttributeTargets.Method
    | AttributeTargets.Field
    | AttributeTargets.Property)]
  public sealed class AspMvcViewComponentViewAttribute : Attribute { }

  /// <summary>
  /// ASP.NET MVC attribute. When applied to a parameter of an attribute,
  /// indicates that this parameter is an MVC action name.
  /// </summary>
  /// <example><code>
  /// [ActionName("Foo")]
  /// public ActionResult Login(string returnUrl)
  /// {
  ///   ViewBag.ReturnUrl = Url.Action("Foo"); // OK
  ///   return RedirectToAction("Bar"); // Error: Cannot resolve action
  /// }
  /// </code></example>
  [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
  public sealed class AspMvcActionSelectorAttribute : Attribute { }
}
