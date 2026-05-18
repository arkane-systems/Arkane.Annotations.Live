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
  /// Indicates that the value of the marked type (or its derivatives)
  /// cannot be compared using <c>==</c> or <c>!=</c> operators, and <c>Equals()</c>
  /// should be used instead. However, using <c>==</c> or <c>!=</c> for comparison
  /// with <c>null</c> is always permitted.
  /// </summary>
  /// <example><code>
  /// [CannotApplyEqualityOperator]
  /// class NoEquality { }
  /// 
  /// class UsesNoEquality
  /// {
  ///   void Test()
  ///   {
  ///     var instance1 = new NoEquality();
  ///     var instance2 = new NoEquality();
  ///     if (instance1 != null) // OK
  ///     {
  ///       bool condition = instance1 == instance2; // Warning
  ///     }
  ///   }
  /// }
  /// </code></example>
  [AttributeUsage(
    AttributeTargets.Interface
    | AttributeTargets.Class
    | AttributeTargets.Struct)]
  public sealed class CannotApplyEqualityOperatorAttribute : Attribute { }

  /// <summary>
  /// Indicates that the method or type uses equality members of the annotated element.
  /// </summary>
  /// <remarks>
  /// When applied to the method's generic parameter, indicates that the equality of the annotated type is used,
  /// unless a custom equality comparer is passed when calling this method. The attribute can also be applied
  /// directly to the method's parameter or return type to specify equality usage for it.
  /// When applied to the type's generic parameter, indicates that type equality usage can happen anywhere
  /// inside this type, so the instantiation of this type is treated as equality usage, unless a custom
  /// equality comparer is passed to the constructor.
  /// </remarks>
  /// <example><code>
  /// struct StructWithDefaultEquality { /* no Equals &amp; GetHashCode override */ }
  /// 
  /// class MySet&lt;[DefaultEqualityUsage] T&gt; { ... }
  /// 
  /// static class Extensions
  /// {
  ///   public static MySet&lt;T&gt; ToMySet&lt;[DefaultEqualityUsage] T&gt;(this IEnumerable&lt;T&gt; items) { ... }
  /// }
  /// 
  /// class MyList&lt;T&gt;
  /// {
  ///   public int IndexOf([DefaultEqualityUsage] T item) { ... }
  /// }
  /// 
  /// class UsesDefaultEquality
  /// {
  ///   void Test()
  ///   {
  ///     var list = new MyList&lt;StructWithDefaultEquality&gt;();
  ///
  ///     // Warning: Default equality of struct 'StructWithDefaultEquality' is used
  ///     list.IndexOf(new StructWithDefaultEquality());
  ///
  ///     // Warning: Default equality of struct 'StructWithDefaultEquality' is used
  ///     var set1 = new MySet&lt;StructWithDefaultEquality&gt;();
  ///
  ///     // Warning: Default equality of struct 'StructWithDefaultEquality' is used
  ///     var set2 = new StructWithDefaultEquality[1].ToMySet();
  ///   }
  /// }
  /// </code></example>
  [AttributeUsage(
    AttributeTargets.GenericParameter
    | AttributeTargets.Parameter
    | AttributeTargets.ReturnValue)]
  public sealed class DefaultEqualityUsageAttribute : Attribute { }

  /// <summary>
  /// When applied to a target attribute, specifies a requirement for any type marked
  /// with the target attribute to implement or inherit the specific type or types.
  /// </summary>
  /// <example><code>
  /// [BaseTypeRequired(typeof(IComponent)] // Specify requirement
  /// class ComponentAttribute : Attribute { }
  /// 
  /// [Component] // ComponentAttribute requires implementing IComponent interface
  /// class MyComponent : IComponent { }
  /// </code></example>
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
  [BaseTypeRequired(typeof(Attribute))]
  public sealed class BaseTypeRequiredAttribute : Attribute
  {
    public BaseTypeRequiredAttribute([NotNull] Type baseType)
    {
      BaseType = baseType;
    }

    [NotNull] public Type BaseType { get; }
  }

  /// <summary>
  /// Prevents the Member Reordering feature in the IDE from tossing members of the marked class.
  /// </summary>
  /// <remarks>
  /// The attribute must be mentioned in your member reordering patterns.
  /// </remarks>
  [AttributeUsage(
    AttributeTargets.Class
    | AttributeTargets.Interface
    | AttributeTargets.Struct
    | AttributeTargets.Enum,
    AllowMultiple = true)]
  public sealed class NoReorderAttribute : Attribute { }
}
