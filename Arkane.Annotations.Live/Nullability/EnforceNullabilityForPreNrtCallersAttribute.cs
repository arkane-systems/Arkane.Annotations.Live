// Licensed under the MIT License. See LICENSE.md at the repository root:
// https://github.com/arkane-systems/Arkane.Annotations.Live/blob/master/LICENSE.md

#nullable disable

using System;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
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
  /// Applied to an assembly to activate runtime null-guard injection inferred from nullable
  /// reference type (NRT) annotations on the public API surface.
  /// </summary>
  /// <remarks>
  /// <para>
  /// When this attribute is present, a Metalama fabric (added in Phase 4) walks the public
  /// API surface of the assembly and automatically applies <see cref="NotNullAttribute"/>
  /// and <see cref="CanBeNullAttribute"/> aspects to members whose NRT annotations indicate
  /// non-nullability or nullability respectively. This provides runtime null guards for
  /// callers that are not NRT-aware or that suppress NRT compiler warnings.
  /// </para>
  /// <para>
  /// The fabric only processes members that do not already carry an explicit
  /// <see cref="NotNullAttribute"/> or <see cref="CanBeNullAttribute"/>; explicit
  /// annotations always take precedence.
  /// </para>
  /// <para>
  /// This attribute is mutually exclusive with <see cref="AssumesNrtCallersAttribute"/>.
  /// Applying both to the same assembly is a compile-time error (<c>AAL0203</c>).
  /// </para>
  /// <para>
  /// <b>Phase 4 note:</b> In the current phase the attribute may be applied without
  /// triggering any fabric behavior. The fabric implementation is deferred to Phase 4.
  /// </para>
  /// </remarks>
  [AttributeUsage(AttributeTargets.Assembly)]
  public sealed class EnforceNullabilityForPreNrtCallersAttribute : Attribute, IAspect<IAssembly>
  {
    /// <inheritdoc />
    public void BuildAspect(IAspectBuilder<IAssembly> builder)
    {
      if (builder.Target.Attributes.Any(typeof(AssumesNrtCallersAttribute)))
        builder.Diagnostics.Report(
          AssumesNrtCallersAttribute.ErrorMutuallyExclusive.WithArguments(
            (nameof(EnforceNullabilityForPreNrtCallersAttribute), nameof(AssumesNrtCallersAttribute))));
    }
  }
}
