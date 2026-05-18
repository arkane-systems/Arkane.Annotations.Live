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
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
  public sealed class AspChildControlTypeAttribute : Attribute
  {
    public AspChildControlTypeAttribute([NotNull] string tagName, [NotNull] Type controlType)
    {
      TagName = tagName;
      ControlType = controlType;
    }

    [NotNull] public string TagName { get; }

    [NotNull] public Type ControlType { get; }
  }

  [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method)]
  public sealed class AspDataFieldAttribute : Attribute { }

  [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method)]
  public sealed class AspDataFieldsAttribute : Attribute { }

  [AttributeUsage(AttributeTargets.Property)]
  public sealed class AspMethodPropertyAttribute : Attribute { }

  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
  public sealed class AspRequiredAttributeAttribute : Attribute
  {
    public AspRequiredAttributeAttribute([NotNull] string attribute)
    {
      Attribute = attribute;
    }

    [NotNull] public string Attribute { get; }
  }

  [AttributeUsage(AttributeTargets.Property)]
  public sealed class AspTypePropertyAttribute : Attribute
  {
    public bool CreateConstructorReferences { get; }

    public AspTypePropertyAttribute(bool createConstructorReferences)
    {
      CreateConstructorReferences = createConstructorReferences;
    }
  }
}
