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
  /// Indicates that the marked parameter, field, or property is a route template.
  /// </summary>
  /// <remarks>
  /// This attribute allows IDE to recognize the use of web frameworks' route templates
  /// to enable syntax highlighting, code completion, navigation, rename, and other features in string literals.
  /// </remarks>
  [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Field | AttributeTargets.Property)]
  public sealed class RouteTemplateAttribute : Attribute { }

  /// <summary>
  /// Indicates that the marked type is custom route parameter constraint,
  /// which is registered in the application's Startup with the name <c>ConstraintName</c>.
  /// </summary>
  /// <remarks>
  /// You can specify <c>ProposedType</c> if target constraint matches only route parameters of a specific type,
  /// it will allow IDE to create method's parameter from usage in route template
  /// with specified type instead of default <c>System.String</c>
  /// and check if constraint's proposed type conflicts with matched parameter's type.
  /// </remarks>
  [AttributeUsage(AttributeTargets.Class)]
  public sealed class RouteParameterConstraintAttribute : Attribute
  {
    [NotNull] public string ConstraintName { get; }
    [CanBeNull] public Type ProposedType { get; set; }

    public RouteParameterConstraintAttribute([NotNull] string constraintName)
    {
      ConstraintName = constraintName;
    }
  }

  /// <summary>
  /// Indicates that the marked parameter, field, or property is a URI string.
  /// </summary>
  /// <remarks>
  /// This attribute enables code completion, navigation, renaming, and other features
  /// in URI string literals assigned to annotated parameters, fields, or properties.
  /// </remarks>
  [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Field | AttributeTargets.Property)]
  public sealed class UriStringAttribute : Attribute
  {
    public UriStringAttribute() { }

    public UriStringAttribute(string httpVerb)
    {
      HttpVerb = httpVerb;
    }

    [CanBeNull] public string HttpVerb { get; }
  }

  /// <summary>
  /// Indicates that the marked method declares routing convention for ASP.NET.
  /// </summary>
  /// <remarks>
  /// The IDE will analyze all usages of methods marked with this attribute,
  /// and will add all routes to completion, navigation, and other features over URI strings.
  /// </remarks>
  [AttributeUsage(AttributeTargets.Method)]
  public sealed class AspRouteConventionAttribute : Attribute
  {
    public AspRouteConventionAttribute() { }

    public AspRouteConventionAttribute(string predefinedPattern)
    {
      PredefinedPattern = predefinedPattern;
    }

    [CanBeNull] public string PredefinedPattern { get; }
  }

  /// <summary>
  /// Indicates that the marked method parameter contains default route values of routing convention for ASP.NET.
  /// </summary>
  [AttributeUsage(AttributeTargets.Parameter)]
  public sealed class AspDefaultRouteValuesAttribute : Attribute { }

  /// <summary>
  /// Indicates that the marked method parameter contains constraints on route values of routing convention for ASP.NET.
  /// </summary>
  [AttributeUsage(AttributeTargets.Parameter)]
  public sealed class AspRouteValuesConstraintsAttribute : Attribute { }

  /// <summary>
  /// Indicates that the marked parameter or property contains routing order provided by ASP.NET routing attribute.
  /// </summary>
  [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
  public sealed class AspRouteOrderAttribute : Attribute { }

  /// <summary>
  /// Indicates that the marked parameter or property contains HTTP verbs provided by ASP.NET routing attribute.
  /// </summary>
  [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
  public sealed class AspRouteVerbsAttribute : Attribute { }

  /// <summary>
  /// Indicates that the marked attribute is used for attribute routing in ASP.NET.
  /// </summary>
  /// <remarks>
  /// The IDE will analyze all usages of attributes marked with this attribute,
  /// and will add all routes to completion, navigation, and other features over URI strings.
  /// </remarks>
  [AttributeUsage(AttributeTargets.Class)]
  public sealed class AspAttributeRoutingAttribute : Attribute
  {
    public string HttpVerb { get; set; }
  }

  /// <summary>
  /// Indicates that the marked method declares an ASP.NET Minimal API endpoint.
  /// </summary>
  /// <remarks>
  /// The IDE will analyze all usages of methods marked with this attribute,
  /// and will add all routes to completion, navigation, and other features over URI strings.
  /// </remarks>
  [AttributeUsage(AttributeTargets.Method)]
  public sealed class AspMinimalApiDeclarationAttribute : Attribute
  {
    public string HttpVerb { get; set; }
  }

  /// <summary>
  /// Indicates that the marked method declares an ASP.NET Minimal API endpoints group.
  /// </summary>
  [AttributeUsage(AttributeTargets.Method)]
  public sealed class AspMinimalApiGroupAttribute : Attribute { }

  /// <summary>
  /// Indicates that the marked parameter contains an ASP.NET Minimal API endpoint handler.
  /// </summary>
  [AttributeUsage(AttributeTargets.Parameter)]
  public sealed class AspMinimalApiHandlerAttribute : Attribute { }

  /// <summary>
  /// Indicates that the marked method contains Minimal API endpoint declaration.
  /// </summary>
  /// <remarks>
  /// The IDE will analyze all usages of methods marked with this attribute,
  /// and will add all declared in attributes routes to completion, navigation, and other features over URI strings.
  /// </remarks>
  [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
  public sealed class AspMinimalApiImplicitEndpointDeclarationAttribute : Attribute
  {
    public string HttpVerb { get; set; }

    public string RouteTemplate { get; set; }

    public Type BodyType { get; set; }

    /// <summary>
    /// Comma-separated list of query parameters defined for endpoint
    /// </summary>
    public string QueryParameters { get; set; }
  }
}
