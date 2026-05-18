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
  /// Indicates that the integral value falls into the specified interval.
  /// It is allowed to specify multiple non-intersecting intervals.
  /// Values of interval boundaries are included in the interval.
  /// </summary>
  /// <example><code>
  /// void Foo([ValueRange(0, 100)] int value)
  /// {
  ///   if (value == -1) // Warning: Expression is always 'false'
  ///   {
  ///     ...
  ///   }
  /// }
  /// </code></example>
  [AttributeUsage(
    AttributeTargets.Parameter
    | AttributeTargets.Field
    | AttributeTargets.Property
    | AttributeTargets.Method
    | AttributeTargets.Delegate,
    AllowMultiple = true)]
  public sealed class ValueRangeAttribute : Attribute
  {
    public object From { get; }
    public object To { get; }

    public ValueRangeAttribute(long from, long to)
    {
      From = from;
      To = to;
    }

    public ValueRangeAttribute(ulong from, ulong to)
    {
      From = from;
      To = to;
    }

    public ValueRangeAttribute(long value)
    {
      From = To = value;
    }

    public ValueRangeAttribute(ulong value)
    {
      From = To = value;
    }
  }
}
