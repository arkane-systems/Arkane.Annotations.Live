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
  /// A declaration marked with this attribute will be recognized as a CQRS Command.
  /// Its naming and adherence to the CQRS pattern will be checked.
  /// </summary>
  /// <example><code>
  /// public class User
  /// {
  ///   private string Name;
  /// 
  ///   [CqrsCommand]
  ///   public void SetUserNameCommand(string newName)
  ///   {
  ///     if (newName == GetUserName()) // Warning about 'GetUserName' is called from Command but belongs to the Query
  ///       return;
  /// 
  ///     Name = newName;
  ///   }
  /// 
  ///   [CqrsQuery]
  ///   public string GetUserName() // Suggestion to rename it to the 'GetUserNameQuery'
  ///   {
  ///     return Name;
  ///   }
  /// }
  /// </code></example>
  [AttributeUsage(
    AttributeTargets.Class
    | AttributeTargets.Struct
    | AttributeTargets.Interface
    | AttributeTargets.Constructor
    | AttributeTargets.Method
    | AttributeTargets.Property)]
  public sealed class CqrsCommandAttribute : Attribute { }

  /// <summary>
  /// A declaration marked with this attribute will be recognized as a CQRS Query.
  /// Its naming and adherence to the CQRS pattern will be checked.
  /// </summary>
  /// <example><code>
  /// public class User
  /// {
  ///   private string Name;
  /// 
  ///   [CqrsCommand]
  ///   public void SetUserNameCommand(string newName)
  ///   {
  ///     if (newName == GetUserName()) // Warning about 'GetUserName' is called from Command but belongs to the Query
  ///       return;
  /// 
  ///     Name = newName;
  ///   }
  /// 
  ///   [CqrsQuery]
  ///   public string GetUserName() // Suggestion to rename it to the 'GetUserNameQuery'
  ///   {
  ///     return Name;
  ///   }
  /// }
  /// </code></example>
  [AttributeUsage(
    AttributeTargets.Class
    | AttributeTargets.Struct
    | AttributeTargets.Interface
    | AttributeTargets.Constructor
    | AttributeTargets.Method
    | AttributeTargets.Property)]
  public sealed class CqrsQueryAttribute : Attribute { }

  /// <summary>
  /// A declaration marked with this attribute will be recognized as a CQRS CommandHandler.
  /// Its naming and adherence to the CQRS pattern will be checked.
  /// </summary>
  [AttributeUsage(
    AttributeTargets.Class
    | AttributeTargets.Struct
    | AttributeTargets.Interface
    | AttributeTargets.Constructor
    | AttributeTargets.Method
    | AttributeTargets.Property)]
  public sealed class CqrsCommandHandlerAttribute : Attribute { }

  /// <summary>
  /// A declaration marked with this attribute will be recognized as a CQRS QueryHandler.
  /// Its naming and adherence to the CQRS pattern will be checked.
  /// </summary>
  [AttributeUsage(
    AttributeTargets.Class
    | AttributeTargets.Struct
    | AttributeTargets.Interface
    | AttributeTargets.Constructor
    | AttributeTargets.Method
    | AttributeTargets.Property)]
  public sealed class CqrsQueryHandlerAttribute : Attribute { }

  /// <summary>
  /// Indicates that the marked element must be excluded from CQRS-related analysis.
  /// </summary>
  [AttributeUsage(
    AttributeTargets.Class
    | AttributeTargets.Struct
    | AttributeTargets.Interface
    | AttributeTargets.Constructor
    | AttributeTargets.Method
    | AttributeTargets.Property)]
  public sealed class CqrsExcludeFromAnalysisAttribute : Attribute { }
}
