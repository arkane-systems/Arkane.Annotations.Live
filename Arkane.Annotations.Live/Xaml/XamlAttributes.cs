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
  /// XAML attribute. Indicates the type that has an <c>ItemsSource</c> property and should be treated
  /// as an <c>ItemsControl</c>-derived type, to enable inner items <c>DataContext</c> type resolution.
  /// </summary>
  [AttributeUsage(AttributeTargets.Class)]
  public sealed class XamlItemsControlAttribute : Attribute { }

  /// <summary>
  /// XAML attribute. Indicates the property of some <c>BindingBase</c>-derived type, that
  /// is used to bind some item of an <c>ItemsControl</c>-derived type. This annotation will
  /// enable the <c>DataContext</c> type resolution for XAML bindings for such properties.
  /// </summary>
  /// <remarks>
  /// The property should have a tree ancestor of the <c>ItemsControl</c> type or
  /// marked with the <see cref="XamlItemsControlAttribute"/> attribute.
  /// </remarks>
  [AttributeUsage(AttributeTargets.Property)]
  public sealed class XamlItemBindingOfItemsControlAttribute : Attribute { }

  /// <summary>
  /// XAML attribute. Indicates the property of some <c>Style</c>-derived type that
  /// is used to style items of an <c>ItemsControl</c>-derived type. This annotation will
  /// enable the <c>DataContext</c> type resolution in XAML bindings for such properties.
  /// </summary>
  /// <remarks>
  /// Property should have a tree ancestor of the <c>ItemsControl</c> type or
  /// marked with the <see cref="XamlItemsControlAttribute"/> attribute.
  /// </remarks>
  [AttributeUsage(AttributeTargets.Property)]
  public sealed class XamlItemStyleOfItemsControlAttribute : Attribute { }

  /// <summary>
  /// XAML attribute. Indicates that DependencyProperty has <c>OneWay</c> binding mode by default.
  /// </summary>
  /// <remarks>
  /// This attribute must be applied to DependencyProperty's CLR accessor property if it is present,
  /// or to a DependencyProperty descriptor field otherwise.
  /// </remarks>
  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
  public sealed class XamlOneWayBindingModeByDefaultAttribute : Attribute { }

  /// <summary>
  /// XAML attribute. Indicates that DependencyProperty has <c>TwoWay</c> binding mode by default.
  /// </summary>
  /// <remarks>
  /// This attribute must be applied to DependencyProperty's CLR accessor property if it is present,
  /// or to a DependencyProperty descriptor field otherwise.
  /// </remarks>
  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
  public sealed class XamlTwoWayBindingModeByDefaultAttribute : Attribute { }
}
