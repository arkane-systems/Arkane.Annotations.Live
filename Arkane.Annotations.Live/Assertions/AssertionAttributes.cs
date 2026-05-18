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
  /// Indicates that the marked method is an assertion method, i.e. it halts the control flow if
  /// one of the conditions is satisfied. To set the condition, mark one of the parameters with
  /// <see cref="AssertionConditionAttribute"/> attribute.
  /// </summary>
  [AttributeUsage(AttributeTargets.Method)]
  public sealed class AssertionMethodAttribute : Attribute { }

  /// <summary>
  /// Indicates the condition parameter of the assertion method. The method itself should be
  /// marked by the <see cref="AssertionMethodAttribute"/> attribute. The mandatory argument of
  /// the attribute is the assertion type.
  /// </summary>
  [AttributeUsage(AttributeTargets.Parameter)]
  public sealed class AssertionConditionAttribute : Attribute
  {
    public AssertionConditionAttribute(AssertionConditionType conditionType)
    {
      ConditionType = conditionType;
    }

    public AssertionConditionType ConditionType { get; }
  }

  /// <summary>
  /// Specifies the assertion type. If the assertion method argument satisfies the condition,
  /// then the execution continues. Otherwise, execution is assumed to be halted.
  /// </summary>
  public enum AssertionConditionType
  {
    /// <summary>Marked parameter should be evaluated to true.</summary>
    IS_TRUE = 0,

    /// <summary>Marked parameter should be evaluated to false.</summary>
    IS_FALSE = 1,

    /// <summary>Marked parameter should be evaluated to null value.</summary>
    IS_NULL = 2,

    /// <summary>Marked parameter should be evaluated to not null value.</summary>
    IS_NOT_NULL = 3,
  }

  /// <summary>
  /// Indicates that the marked method unconditionally terminates control flow execution.
  /// For example, it could unconditionally throw an exception.
  /// </summary>
  [Obsolete("Use [ContractAnnotation('=> halt')] instead")]
  [AttributeUsage(AttributeTargets.Method)]
  public sealed class TerminatesProgramAttribute : Attribute { }
}
