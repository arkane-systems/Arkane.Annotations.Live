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
  /// Indicates that the marked symbol is used implicitly (via reflection, in an external library, and so on),
  /// so this symbol will be ignored by usage-checking inspections. <br/>
  /// You can use <see cref="ImplicitUseKindFlags"/> and <see cref="ImplicitUseTargetFlags"/>
  /// to configure how this attribute is applied.
  /// </summary>
  /// <example><code>
  /// [UsedImplicitly]
  /// public class TypeConverter { }
  /// 
  /// public class SummaryData
  /// {
  ///   [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature)]
  ///   public SummaryData() { }
  /// }
  /// 
  /// [UsedImplicitly(ImplicitUseTargetFlags.WithInheritors | ImplicitUseTargetFlags.Default)]
  /// public interface IService { }
  /// </code></example>
  [AttributeUsage(AttributeTargets.All)]
  public sealed class UsedImplicitlyAttribute : Attribute
  {
    public UsedImplicitlyAttribute()
      : this(ImplicitUseKindFlags.Default, ImplicitUseTargetFlags.Default) { }

    public UsedImplicitlyAttribute(ImplicitUseKindFlags useKindFlags)
      : this(useKindFlags, ImplicitUseTargetFlags.Default) { }

    public UsedImplicitlyAttribute(ImplicitUseTargetFlags targetFlags)
      : this(ImplicitUseKindFlags.Default, targetFlags) { }

    public UsedImplicitlyAttribute(ImplicitUseKindFlags useKindFlags, ImplicitUseTargetFlags targetFlags)
    {
      UseKindFlags = useKindFlags;
      TargetFlags = targetFlags;
    }

    public ImplicitUseKindFlags UseKindFlags { get; }

    public ImplicitUseTargetFlags TargetFlags { get; }

    public string Reason { get; set; }
  }

  /// <summary>
  /// Can be applied to attributes, type parameters, and parameters of a type assignable from <see cref="System.Type"/>.
  /// When applied to an attribute, the decorated attribute behaves the same as <see cref="UsedImplicitlyAttribute"/>.
  /// When applied to a type parameter or to a parameter of type <see cref="System.Type"/>,
  /// indicates that the corresponding type is used implicitly.
  /// </summary>
  [AttributeUsage(
    AttributeTargets.Class
    | AttributeTargets.GenericParameter
    | AttributeTargets.Parameter)]
  public sealed class MeansImplicitUseAttribute : Attribute
  {
    public MeansImplicitUseAttribute()
      : this(ImplicitUseKindFlags.Default, ImplicitUseTargetFlags.Default) { }

    public MeansImplicitUseAttribute(ImplicitUseKindFlags useKindFlags)
      : this(useKindFlags, ImplicitUseTargetFlags.Default) { }

    public MeansImplicitUseAttribute(ImplicitUseTargetFlags targetFlags)
      : this(ImplicitUseKindFlags.Default, targetFlags) { }

    public MeansImplicitUseAttribute(ImplicitUseKindFlags useKindFlags, ImplicitUseTargetFlags targetFlags)
    {
      UseKindFlags = useKindFlags;
      TargetFlags = targetFlags;
    }

    [UsedImplicitly] public ImplicitUseKindFlags UseKindFlags { get; }

    [UsedImplicitly] public ImplicitUseTargetFlags TargetFlags { get; }
  }

  /// <summary>
  /// Specifies the details of an implicitly used symbol when it is marked
  /// with <see cref="MeansImplicitUseAttribute"/> or <see cref="UsedImplicitlyAttribute"/>.
  /// </summary>
  [Flags]
  public enum ImplicitUseKindFlags
  {
    Default = Access | Assign | InstantiatedWithFixedConstructorSignature,

    /// <summary>Only entity marked with attribute considered used.</summary>
    Access = 1,

    /// <summary>Indicates implicit assignment to a member.</summary>
    Assign = 2,

    /// <summary>
    /// Indicates implicit instantiation of a type with a fixed constructor signature.
    /// That means any unused constructor parameters will not be reported as such.
    /// </summary>
    InstantiatedWithFixedConstructorSignature = 4,

    /// <summary>Indicates implicit instantiation of a type.</summary>
    InstantiatedNoFixedConstructorSignature = 8,
  }

  /// <summary>
  /// Specifies what is considered to be used implicitly when marked
  /// with <see cref="MeansImplicitUseAttribute"/> or <see cref="UsedImplicitlyAttribute"/>.
  /// </summary>
  [Flags]
  public enum ImplicitUseTargetFlags
  {
    Default = Itself,

    /// <summary>Code entity itself.</summary>
    Itself = 1,

    /// <summary>Members of the type marked with the attribute are considered used.</summary>
    Members = 2,

    /// <summary> Inherited entities are considered used. </summary>
    WithInheritors = 4,

    /// <summary>Entity marked with the attribute and all its members considered used.</summary>
    WithMembers = Itself | Members
  }

  /// <summary>
  /// This attribute is intended to mark publicly available APIs
  /// that should not be removed and therefore should never be reported as unused.
  /// </summary>
  [MeansImplicitUse(ImplicitUseTargetFlags.WithMembers)]
  [AttributeUsage(AttributeTargets.All, Inherited = false)]
  public sealed class PublicAPIAttribute : Attribute
  {
    public PublicAPIAttribute() { }

    public PublicAPIAttribute([NotNull] string comment)
    {
      Comment = comment;
    }

    [CanBeNull] public string Comment { get; }
  }
}
