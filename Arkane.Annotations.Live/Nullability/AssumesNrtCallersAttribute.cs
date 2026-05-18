// Licensed under the MIT License. See LICENSE.md at the repository root:
// https://github.com/arkane-systems/Arkane.Annotations.Live/blob/master/LICENSE.md

#nullable disable

using System;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Diagnostics;
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
  /// Applied to an assembly to declare that all consumers of that assembly are nullable
  /// reference type (NRT) aware.
  /// </summary>
  /// <remarks>
  /// <para>
  /// When this attribute is present, applying <see cref="NotNullAttribute"/> to a
  /// non-nullable reference type emits a compile-time warning (<c>AAL0202</c>), because
  /// the NRT annotation already enforces non-nullability for aware callers and the
  /// runtime guard is redundant.
  /// </para>
  /// <para>
  /// Applying <see cref="CanBeNullAttribute"/> to a nullable type (<c>T?</c>) always
  /// emits a compile-time warning (<c>AAL0201</c>) regardless of this attribute, as it
  /// is unconditionally redundant.
  /// </para>
  /// <para>
  /// This attribute is mutually exclusive with
  /// <see cref="EnforceNullabilityForPreNrtCallersAttribute"/>. Applying both to the
  /// same assembly is a compile-time error (<c>AAL0203</c>).
  /// </para>
  /// <para>
  /// When this attribute is present, the <c>[EnforceNullabilityForPreNrtCallers]</c>
  /// fabric (Phase 4) does not run.
  /// </para>
  /// </remarks>
  [AttributeUsage(AttributeTargets.Assembly)]
  public sealed class AssumesNrtCallersAttribute : Attribute, IAspect<IAssembly>
  {
    /// <summary>AAL0203: both NRT control attributes applied to the same assembly.</summary>
    internal static readonly DiagnosticDefinition<(string first, string second)> ErrorMutuallyExclusive =
      new(
        "AAL0203",
        Severity.Error,
        "[{0}] and [{1}] are mutually exclusive and cannot both be applied to the same assembly.",
        "Mutually exclusive NRT assembly attributes",
        "Arkane.Annotations.Live");

    /// <inheritdoc />
    public void BuildAspect(IAspectBuilder<IAssembly> builder)
    {
      if (builder.Target.Attributes.Any(typeof(EnforceNullabilityForPreNrtCallersAttribute)))
        builder.Diagnostics.Report(
          ErrorMutuallyExclusive.WithArguments(
            (nameof(AssumesNrtCallersAttribute), nameof(EnforceNullabilityForPreNrtCallersAttribute))));
    }
  }
}
