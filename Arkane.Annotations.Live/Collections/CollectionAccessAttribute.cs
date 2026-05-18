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
  /// Indicates how a method, constructor invocation, or property access
  /// over a collection type affects the contents of the collection.
  /// When applied to a return value of a method, indicates whether the returned collection
  /// is created exclusively for the caller (<c>CollectionAccessType.UpdatedContent</c>) or
  /// can be read/updated from outside (<c>CollectionAccessType.Read</c>/<c>CollectionAccessType.UpdatedContent</c>).
  /// Use <see cref="CollectionAccessType"/> to specify the access type.
  /// </summary>
  /// <remarks>
  /// Using this attribute only makes sense if all collection methods are marked with this attribute.
  /// </remarks>
  /// <example><code>
  /// public class MyStringCollection : List&lt;string&gt;
  /// {
  ///   [CollectionAccess(CollectionAccessType.Read)]
  ///   public string GetFirstString()
  ///   {
  ///     return this.ElementAt(0);
  ///   }
  /// }
  ///
  /// class Test
  /// {
  ///   public void Foo()
  ///   {
  ///     // Warning: Contents of the collection is never updated
  ///     var col = new MyStringCollection();
  ///     string x = col.GetFirstString();
  ///   }
  /// }
  /// </code></example>
  [AttributeUsage(
    AttributeTargets.Method
    | AttributeTargets.Constructor
    | AttributeTargets.Property
    | AttributeTargets.ReturnValue)]
  public sealed class CollectionAccessAttribute : Attribute
  {
    public CollectionAccessAttribute(CollectionAccessType collectionAccessType)
    {
      CollectionAccessType = collectionAccessType;
    }

    public CollectionAccessType CollectionAccessType { get; }
  }

  /// <summary>
  /// Provides a value for the <see cref="CollectionAccessAttribute"/> to define
  /// how the collection method invocation affects the contents of the collection.
  /// </summary>
  [Flags]
  public enum CollectionAccessType
  {
    /// <summary>Method does not use or modify the content of the collection.</summary>
    None = 0,

    /// <summary>Method only reads the content of the collection but does not modify it.</summary>
    Read = 1,

    /// <summary>Method can change the content of the collection but does not add new elements.</summary>
    ModifyExistingContent = 2,

    /// <summary>Method can add new elements to the collection.</summary>
    UpdatedContent = ModifyExistingContent | 4
  }
}
