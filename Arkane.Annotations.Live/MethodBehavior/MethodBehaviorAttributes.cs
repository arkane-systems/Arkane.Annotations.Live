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
  /// Tells the code analysis engine if the parameter is completely handled when the invoked method is on stack.
  /// If the parameter is of the delegate type - indicates that the delegate can only be invoked during the method
  /// execution. The delegate can be invoked zero or multiple times, but not stored to some field and invoked later,
  /// when the containing method is no longer on the execution stack.
  /// If the parameter is of the enumerable type - indicates that it is enumerated while the method is executed.
  /// If <see cref="RequireAwait"/> is true - the attribute will only take effect if the method invocation
  /// is located under the <c>await</c> expression.
  /// </summary>
  [AttributeUsage(AttributeTargets.Parameter)]
  public sealed class InstantHandleAttribute : Attribute
  {
    /// <summary>
    /// Requires the method invocation to be used under the <c>await</c> expression for this attribute to take effect.
    /// Can be used for delegate/enumerable parameters of <c>async</c> methods.
    /// </summary>
    public bool RequireAwait { get; set; }
  }

  /// <summary>
  /// Indicates that the method does not make any observable state changes.
  /// The same as <see cref="T:System.Diagnostics.Contracts.PureAttribute"/>.
  /// </summary>
  /// <example><code>
  /// [Pure] int Multiply(int x, int y) => x * y;
  /// 
  /// void M()
  /// {
  ///   Multiply(123, 42); // Warning: Return value of pure method is not used
  /// }
  /// </code></example>
  [AttributeUsage(AttributeTargets.Method)]
  public sealed class PureAttribute : Attribute { }

  /// <summary>
  /// Indicates that the return value of the method invocation must be used.
  /// </summary>
  /// <remarks>
  /// Methods decorated with this attribute (in contrast to pure methods) might change state,
  /// but make no sense without using their return value. <br/>
  /// Similarly to <see cref="PureAttribute"/>, this attribute
  /// will help detect usages of the method when the return value is not used.
  /// Optionally, you can specify a message to use when showing warnings, e.g.
  /// <code>[MustUseReturnValue("Use the return value to...")]</code>.
  /// </remarks>
  [AttributeUsage(AttributeTargets.Method)]
  public sealed class MustUseReturnValueAttribute : Attribute
  {
    public MustUseReturnValueAttribute() { }

    public MustUseReturnValueAttribute([NotNull] string justification)
    {
      Justification = justification;
    }

    [CanBeNull] public string Justification { get; }

    /// <summary>
    /// Enables the special handling of the "fluent" APIs that perform mutations and return 'this' object.
    /// In this case the analysis checks the fluent invocations chain and only warns if the initial receiver value
    /// is probably a temporary value - in this case the very last fluent method return assumed to be temporary as well,
    /// therefore is a subject of warning if unused. If the initial receiver is a local variable or 'this' reference
    /// the analysis assumes that fluent invocations were used to mutate the existing value and warning will not be shown.
    /// </summary>
    /// <remarks>
    /// This property must only be used for methods with the return type matching the receiver type.
    /// </remarks>
    public bool IsFluentBuilderMethod { get; set; }
  }

  /// <summary>
  /// Indicates that the resource disposal must be handled at the use site,
  /// meaning that the resource ownership is transferred to the caller.
  /// This annotation can be used to annotate disposable types or their constructors individually to enable
  /// the IDE code analysis for resource disposal in every context where the new instance of this type is created.
  /// Factory methods and <c>out</c> parameters can also be annotated to indicate that the return value
  /// of the disposable type needs handling.
  /// </summary>
  /// <remarks>
  /// Annotation of input parameters with this attribute is meaningless.<br/>
  /// Constructors inherit this attribute from their type if it is annotated,
  /// but not from the base constructors they delegate to (if any).<br/>
  /// Resource disposal is expected via <c>using (resource)</c> statement,
  /// <c>using var</c> declaration, explicit <c>Dispose()</c> call, or passing the resource as an argument
  /// to a parameter annotated with the <see cref="HandlesResourceDisposalAttribute"/> attribute.
  /// </remarks>
  [AttributeUsage(
    AttributeTargets.Class
    | AttributeTargets.Struct
    | AttributeTargets.Constructor
    | AttributeTargets.Method
    | AttributeTargets.Parameter)]
  public sealed class MustDisposeResourceAttribute : Attribute
  {
    public MustDisposeResourceAttribute()
    {
      Value = true;
    }

    public MustDisposeResourceAttribute(bool value)
    {
      Value = value;
    }

    /// <summary>
    /// When set to <c>false</c>, disposing of the resource is not obligatory.
    /// The main use-case for explicit <c>[MustDisposeResource(false)]</c> annotation
    /// is to loosen the annotation for inheritors.
    /// </summary>
    public bool Value { get; }
  }

  /// <summary>
  /// Indicates that the method or class instance acquires resource ownership and will dispose it after use.
  /// </summary>
  /// <remarks>
  /// Annotation of <c>out</c> parameters with this attribute is meaningless.<br/>
  /// When an instance method is annotated with this attribute,
  /// it means that it is handling the resource disposal of the corresponding resource instance.<br/>
  /// When a field or a property is annotated with this attribute, it means that this type owns the resource
  /// and will handle the resource disposal properly (e.g. in own <c>IDisposable</c> implementation).
  /// </remarks>
  [AttributeUsage(
    AttributeTargets.Method
    | AttributeTargets.Parameter
    | AttributeTargets.Field
    | AttributeTargets.Property)]
  public sealed class HandlesResourceDisposalAttribute : Attribute { }

  /// <summary>
  /// This annotation allows enforcing allocation-less usage patterns of delegates for performance-critical APIs.
  /// When this annotation is applied to the parameter of a delegate type,
  /// the IDE checks the input argument of this parameter:
  /// * When a lambda expression or anonymous method is passed as an argument, the IDE verifies that the passed closure
  ///   has no captures of the containing local variables and the compiler is able to cache the delegate instance
  ///   to avoid heap allocations. Otherwise, a warning is produced.
  /// * The IDE warns when the method name or local function name is passed as an argument because this always results
  ///   in heap allocation of the delegate instance.
  /// </summary>
  /// <remarks>
  /// In C# 9.0+ code, the IDE will also suggest annotating the anonymous functions with the <c>static</c> modifier
  /// to make use of the similar analysis provided by the language/compiler.
  /// </remarks>
  [AttributeUsage(AttributeTargets.Parameter)]
  public sealed class RequireStaticDelegateAttribute : Attribute
  {
    public bool IsError { get; set; }
  }

  /// <summary>
  /// Indicates the type member or parameter of some type that should be used instead of all other ways
  /// to get the value of that type. This annotation is useful when you have some 'context' value evaluated
  /// and stored somewhere, meaning that all other ways to get this value must be consolidated with the existing one.
  /// </summary>
  /// <example><code>
  /// class Foo
  /// {
  ///   [ProvidesContext] IBarService _barService = ...;
  /// 
  ///   void ProcessNode(INode node)
  ///   {
  ///     DoSomething(node, node.GetGlobalServices().Bar);
  ///     //              ^ Warning: use value of '_barService' field
  ///   }
  /// }
  /// </code></example>
  [AttributeUsage(
    AttributeTargets.Field
    | AttributeTargets.Property
    | AttributeTargets.Parameter
    | AttributeTargets.Method
    | AttributeTargets.Class
    | AttributeTargets.Interface
    | AttributeTargets.Struct
    | AttributeTargets.GenericParameter)]
  public sealed class ProvidesContextAttribute : Attribute { }
}
