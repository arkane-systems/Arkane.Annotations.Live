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
  /// Specifies a type being tested by a test class or a test method.
  /// </summary>
  /// <remarks>
  /// This information can be used by the IDE to navigate between tests and tested types,
  /// or by test runners to group tests by subject and to provide better test reports.
  /// </remarks>
  [AttributeUsage(
    AttributeTargets.Method
    | AttributeTargets.Property
    | AttributeTargets.Class
    | AttributeTargets.Interface,
    AllowMultiple = true)]
  public sealed class TestSubjectAttribute : Attribute
  {
    /// <summary>
    /// Gets the type being tested.
    /// </summary>
    [NotNull] public Type Subject { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TestSubjectAttribute"/> class with the specified tested type.
    /// </summary>
    /// <param name="subject">The type being tested.</param>
    public TestSubjectAttribute([NotNull] Type subject)
    {
      Subject = subject;
    }
  }

  /// <summary>
  /// Marks a generic argument as the test subject for a test class.
  /// </summary>
  /// <remarks>
  /// Can be applied to a generic parameter of a base test class to indicate that
  /// the type passed as the argument is the class being tested. This information can be used by the IDE
  /// to navigate between tests and tested types,
  /// or by test runners to group tests by subject and to provide better test reports.
  /// </remarks>
  /// <example><code>
  /// public class BaseTestClass&lt;[MeansTestSubject] T&gt;
  /// {
  ///   protected T Component { get; }
  /// }
  /// 
  /// public class CalculatorAdditionTests : BaseTestClass&lt;Calculator&gt;
  /// {
  ///   [Test]
  ///   public void Should_add_two_numbers()
  ///   {
  ///     Assert.That(Component.Add(2, 3), Is.EqualTo(5));
  ///   }
  /// }
  /// </code></example>
  [AttributeUsage(AttributeTargets.GenericParameter)]
  public sealed class MeansTestSubjectAttribute : Attribute { }
}
