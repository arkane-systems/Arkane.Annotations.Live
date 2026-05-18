# Arkane.Annotations.Live — Copilot Instructions

## Project Overview

This project enhances the JetBrains ReSharper annotation attributes (originally passive IDE hints compiled out via `[Conditional("JETBRAINS_ANNOTATIONS")]`) into **live Metalama aspects** that actively enforce their semantics at compile time in downstream projects.

The source file `Arkane.Annotations.Live\Annotations.cs` was downloaded from the JetBrains repository and contains ~2491 lines declaring all attributes originally in the `JetBrains.Annotations` namespace.

**Tech stack:**
- Target framework: `net10.0`
- C# 13+ features permitted (nullable enabled, implicit usings enabled)
- Metalama: `Metalama.Framework` 2026.x (already referenced)
- Test framework: xUnit (`Arkane.Annotations.Live.Tests` project)
- Delivery: NuGet package; consumers take the Metalama dependency

---

## Project Decisions (summary)
- Namespace: All public types live in Arkane.Annotations.Live (do not preserve JetBrains.Annotations). Type forwarding is a fallback if downstream compatibility issues arise.
- File layout: Group subfolders under `Arkane.Annotations.Live\` (e.g., `Nullability\`, `Contracts\`); one attribute + supporting types per file; subfolders are non-namespace-providers (`RootNamespace = Arkane.Annotations.Live`). License text in `LICENSE.md`; per-file header is a short reference only.
- Nullability: Use Metalama null guards for `[NotNull]`, `[CanBeNull]`, `[ItemNotNull]`, `[ItemCanBeNull]`. Two assembly-level attributes (`[AssumesNrtCallers]` and `[EnforceNullabilityForPreNrtCallers]`) control NRT interop behaviour (see Nullability section and Phase 4).
- Annotation-only attributes: Keep IDE-only groups (ASP.NET MVC, Razor, XAML, SourceTemplates, legacy ASP.NET) as plain attributes in this project.
- Tests: Use xUnit in the Arkane.Annotations.Live.Tests project to validate aspect behavior (both positive and negative cases).
- Delivery: Package as a NuGet package; consumers must reference Metalama at least as a dependency.
- Research priorities: Revisit `[ContractAnnotation]` and `[Pure]/[MustUseReturnValue]` for deeper analysis before implementing full aspects; likely partial implementations initially.

---

## Decided Architecture

### Namespace

All types live in **`Arkane.Annotations.Live`**. The original `JetBrains.Annotations` namespace is not preserved. If downstream compatibility becomes an issue, type forwarding is the fallback — but do not implement it preemptively.

ReSharper supports configuring alternate annotation namespaces, which should be sufficient for IDE recognition.

### File / Folder Layout

Source files are split into **group subfolders** directly under `Arkane.Annotations.Live\`. Each subfolder corresponds to one logical group (e.g., `Nullability\`, `Contracts\`, `AspNetMvc\`). Within a subfolder, each file contains one attribute and its directly supporting types (e.g., a paired enum or flags type), or a small set of inseparably related attributes where splitting would be artificial.

The subfolders must **not** introduce extra namespace segments — all types stay in `Arkane.Annotations.Live`. In `.csproj`, this is enforced by the top-level `<RootNamespace>Arkane.Annotations.Live</RootNamespace>`; no additional `<Compile>` or namespace overrides are needed as long as files do not use folder-derived namespace declarations.

### Nullability

Use **Metalama null guards** for `[NotNull]` / `[CanBeNull]` / `[ItemNotNull]` / `[ItemCanBeNull]`. The rationale: nullable reference types (NRT) only protect within a fully nullable-aware build; callers and callees compiled without NRT are still unsafe.

#### Assembly-level NRT control attributes (Phase 2)

Two mutually exclusive assembly-level attributes govern NRT interop behaviour. Both are defined in `Nullability\` and their mutual exclusivity is enforced by a `[BaseTypeRequired]`-style self-validation check at compile time.

- **`[AssumesNrtCallers]`** — Applied to an assembly to declare that all consumers of that assembly are NRT-aware. Effect: `[NotNull]` on a non-nullable reference type emits a compile-time **warning** (`AAL02xx`: *"[NotNull] is redundant on a non-nullable type in an [AssumesNrtCallers] assembly; the NRT annotation already enforces non-nullability for aware callers."*). `[CanBeNull]` on a nullable type (`T?`) always emits this warning regardless of `[AssumesNrtCallers]` (it is unconditionally redundant). The Phase 4 nullability fabric does **not** run when this attribute is present.

- **`[EnforceNullabilityForPreNrtCallers]`** — Applied to an assembly to activate the Phase 4 nullability fabric, which walks the public API surface and applies `[NotNull]`/`[CanBeNull]` aspects inferred from NRT annotations. Intended for libraries that enable NRT but whose consumers may not. Must not be combined with `[AssumesNrtCallers]`.

#### Default behaviour (neither attribute present)

- `[NotNull]` on a non-nullable reference type: **no warning** (defence-in-depth; the runtime guard catches callers who ignore or suppress the compiler).
- `[CanBeNull]` on a nullable type (`T?`): **warning** (`AAL02xx`) — unconditionally redundant.

### Annotation-Only Attributes

IDE-only attribute groups (ASP.NET MVC helpers, Razor, XAML, SourceTemplates, legacy ASP.NET) are **kept in this project** to minimize complexity. They will be plain attributes with no Metalama logic.

---

## Project Phases

### Phase 1 — Split and Prepare

**Goal:** Divide `Annotations.cs` into focused files; rename namespace; remove `[Conditional]` attributes.

**Rules for splitting:**
- Files go in group subfolders directly under `Arkane.Annotations.Live\` (e.g., `Nullability\`, `Contracts\`). One attribute (plus its directly supporting types such as enums or flag types) per file; pairs of inseparably related attributes may share a file.
- Subfolders must **not** introduce extra namespace segments; all types stay in `Arkane.Annotations.Live`.
- Change `namespace JetBrains.Annotations` → `namespace Arkane.Annotations.Live` in every file.
- Begin each file with the short license reference header (see **File Header Template** below) — do **not** copy the full license text.
- Preserve all `#pragma warning disable`, `// ReSharper disable` directives, and the `#nullable disable` directive.
- Remove every `[Conditional("JETBRAINS_ANNOTATIONS")]` attribute — this allows the attributes to be compiled into consuming assemblies unconditionally.
- `DebuggerGlobalWatchAttribute` has **two** `[Conditional]` attributes (`"DEBUG"` and `"JETBRAINS_ANNOTATIONS"`); keep `[Conditional("DEBUG")]` and remove only `[Conditional("JETBRAINS_ANNOTATIONS")]`.
- Keep any `[Obsolete]` attributes intact.
- Do **not** change any class names, member names, constructors, or `[AttributeUsage]` at this stage.
- The original `Annotations.cs` file must be deleted once all content has been extracted.

**File groupings:**

Each group gets its own subfolder under `Arkane.Annotations.Live\`. Within each subfolder, files are small: one attribute + its directly supporting types per file, or a pair of closely related attributes where splitting them would be artificial.

**`Nullability\`**
| File | Contents |
|------|----------|
| `CanBeNullAttribute.cs` | `CanBeNullAttribute` |
| `NotNullAttribute.cs` | `NotNullAttribute` |
| `ItemNotNullAttribute.cs` | `ItemNotNullAttribute` |
| `ItemCanBeNullAttribute.cs` | `ItemCanBeNullAttribute` |

**`Formatting\`**
| File | Contents |
|------|----------|
| `StringFormatMethodAttribute.cs` | `StringFormatMethodAttribute` |
| `StructuredMessageTemplateAttribute.cs` | `StructuredMessageTemplateAttribute` |

**`ValueConstraints\`**
| File | Contents |
|------|----------|
| `ValueProviderAttribute.cs` | `ValueProviderAttribute` |
| `ValueRangeAttribute.cs` | `ValueRangeAttribute` |
| `NonNegativeValueAttribute.cs` | `NonNegativeValueAttribute` |

**`Contracts\`**
| File | Contents |
|------|----------|
| `ContractAnnotationAttribute.cs` | `ContractAnnotationAttribute` |
| `InvokerParameterNameAttribute.cs` | `InvokerParameterNameAttribute` |
| `NotifyPropertyChangedInvocatorAttribute.cs` | `NotifyPropertyChangedInvocatorAttribute` |

**`CodeQuality\`**
| File | Contents |
|------|----------|
| `LocalizationRequiredAttribute.cs` | `LocalizationRequiredAttribute` |
| `CannotApplyEqualityOperatorAttribute.cs` | `CannotApplyEqualityOperatorAttribute` |
| `DefaultEqualityUsageAttribute.cs` | `DefaultEqualityUsageAttribute` |
| `BaseTypeRequiredAttribute.cs` | `BaseTypeRequiredAttribute` |

**`ImplicitUse\`**
| File | Contents |
|------|----------|
| `UsedImplicitlyAttribute.cs` | `UsedImplicitlyAttribute`, `ImplicitUseKindFlags`, `ImplicitUseTargetFlags` |
| `MeansImplicitUseAttribute.cs` | `MeansImplicitUseAttribute` |
| `PublicAPIAttribute.cs` | `PublicAPIAttribute` |

**`MethodBehavior\`**
| File | Contents |
|------|----------|
| `PureAttribute.cs` | `PureAttribute` |
| `MustUseReturnValueAttribute.cs` | `MustUseReturnValueAttribute` |
| `InstantHandleAttribute.cs` | `InstantHandleAttribute` |
| `MustDisposeResourceAttribute.cs` | `MustDisposeResourceAttribute` |
| `HandlesResourceDisposalAttribute.cs` | `HandlesResourceDisposalAttribute` |
| `RequireStaticDelegateAttribute.cs` | `RequireStaticDelegateAttribute` |
| `ProvidesContextAttribute.cs` | `ProvidesContextAttribute` |

**`Collections\`**
| File | Contents |
|------|----------|
| `CollectionAccessAttribute.cs` | `CollectionAccessAttribute`, `CollectionAccessType` |

**`Assertions\`**
| File | Contents |
|------|----------|
| `AssertionMethodAttribute.cs` | `AssertionMethodAttribute` |
| `AssertionConditionAttribute.cs` | `AssertionConditionAttribute`, `AssertionConditionType` |
| `TerminatesProgramAttribute.cs` | `TerminatesProgramAttribute` (obsolete) |

**`Linq\`**
| File | Contents |
|------|----------|
| `LinqTunnelAttribute.cs` | `LinqTunnelAttribute` |
| `NoEnumerationAttribute.cs` | `NoEnumerationAttribute` |

**`Strings\`**
| File | Contents |
|------|----------|
| `RegexPatternAttribute.cs` | `RegexPatternAttribute` |
| `LanguageInjectionAttribute.cs` | `LanguageInjectionAttribute`, `InjectedLanguage` |

**`SourceTemplates\`**
| File | Contents |
|------|----------|
| `SourceTemplateAttribute.cs` | `SourceTemplateAttribute`, `SourceTemplateTargetExpression` |
| `MacroAttribute.cs` | `MacroAttribute` |

**`IdeTools\`**
| File | Contents |
|------|----------|
| `PathReferenceAttribute.cs` | `PathReferenceAttribute` |
| `NoReorderAttribute.cs` | `NoReorderAttribute` |
| `CodeTemplateAttribute.cs` | `CodeTemplateAttribute` |
| `IgnoreSpellingAndGrammarErrorsAttribute.cs` | `IgnoreSpellingAndGrammarErrorsAttribute` |
| `DebuggerGlobalWatchAttribute.cs` | `DebuggerGlobalWatchAttribute` |

**`AspNet\`** *(annotation-only)*
| File | Contents |
|------|----------|
| `AspChildControlTypeAttribute.cs` | `AspChildControlTypeAttribute` |
| `AspDataFieldAttributes.cs` | `AspDataFieldAttribute`, `AspDataFieldsAttribute` |
| `AspMethodPropertyAttribute.cs` | `AspMethodPropertyAttribute` |
| `AspRequiredAttributeAttribute.cs` | `AspRequiredAttributeAttribute` |
| `AspTypePropertyAttribute.cs` | `AspTypePropertyAttribute` |

**`AspNetMvc\`** *(annotation-only)*
| File | Contents |
|------|----------|
| `AspMvcLocationFormats.cs` | All `AspMvcArea*LocationFormat`, `AspMvc*LocationFormat` attributes (8 location-format attributes) |
| `AspMvcActionAttribute.cs` | `AspMvcActionAttribute` |
| `AspMvcAreaAttribute.cs` | `AspMvcAreaAttribute` |
| `AspMvcControllerAttribute.cs` | `AspMvcControllerAttribute` |
| `AspMvcViewAttributes.cs` | `AspMvcMasterAttribute`, `AspMvcModelTypeAttribute`, `AspMvcPartialViewAttribute`, `AspMvcSuppressViewErrorAttribute`, `AspMvcDisplayTemplateAttribute`, `AspMvcEditorTemplateAttribute`, `AspMvcTemplateAttribute`, `AspMvcViewAttribute`, `AspMvcViewComponentAttribute`, `AspMvcViewComponentViewAttribute` |
| `AspMvcActionSelectorAttribute.cs` | `AspMvcActionSelectorAttribute` |

**`AspNetRouting\`** *(annotation-only)*
| File | Contents |
|------|----------|
| `RouteTemplateAttribute.cs` | `RouteTemplateAttribute` |
| `RouteParameterConstraintAttribute.cs` | `RouteParameterConstraintAttribute` |
| `UriStringAttribute.cs` | `UriStringAttribute` |
| `AspRouteConventionAttribute.cs` | `AspRouteConventionAttribute` |
| `AspRouteAttributes.cs` | `AspDefaultRouteValuesAttribute`, `AspRouteValuesConstraintsAttribute`, `AspRouteOrderAttribute`, `AspRouteVerbsAttribute` |
| `AspAttributeRoutingAttribute.cs` | `AspAttributeRoutingAttribute` |
| `AspMinimalApiAttributes.cs` | `AspMinimalApiDeclarationAttribute`, `AspMinimalApiGroupAttribute`, `AspMinimalApiHandlerAttribute`, `AspMinimalApiImplicitEndpointDeclarationAttribute` |

**`Razor\`** *(annotation-only)*
| File | Contents |
|------|----------|
| `HtmlAttributes.cs` | `HtmlElementAttributesAttribute`, `HtmlAttributeValueAttribute` |
| `RazorAttributes.cs` | `RazorSectionAttribute`, `RazorImportNamespaceAttribute`, `RazorInjectionAttribute`, `RazorDirectiveAttribute`, `RazorPageBaseTypeAttribute`, `RazorHelperCommonAttribute`, `RazorLayoutAttribute`, `RazorWriteLiteralMethodAttribute`, `RazorWriteMethodAttribute`, `RazorWriteMethodParameterAttribute` |

**`Xaml\`** *(annotation-only)*
| File | Contents |
|------|----------|
| `XamlItemsControlAttribute.cs` | `XamlItemsControlAttribute` |
| `XamlItemBindingOfItemsControlAttribute.cs` | `XamlItemBindingOfItemsControlAttribute` |
| `XamlItemStyleOfItemsControlAttribute.cs` | `XamlItemStyleOfItemsControlAttribute` |
| `XamlBindingModeAttributes.cs` | `XamlOneWayBindingModeByDefaultAttribute`, `XamlTwoWayBindingModeByDefaultAttribute` |

**`Testing\`**
| File | Contents |
|------|----------|
| `TestSubjectAttribute.cs` | `TestSubjectAttribute` |
| `MeansTestSubjectAttribute.cs` | `MeansTestSubjectAttribute` |

**`Cqrs\`**
| File | Contents |
|------|----------|
| `CqrsCommandAttribute.cs` | `CqrsCommandAttribute` |
| `CqrsQueryAttribute.cs` | `CqrsQueryAttribute` |
| `CqrsCommandHandlerAttribute.cs` | `CqrsCommandHandlerAttribute` |
| `CqrsQueryHandlerAttribute.cs` | `CqrsQueryHandlerAttribute` |
| `CqrsExcludeFromAnalysisAttribute.cs` | `CqrsExcludeFromAnalysisAttribute` |

---

### Phase 2 — Conversion Planning

**Goal:** Produce a detailed plan for which attributes can/should become live Metalama aspects, and what each aspect should enforce.

**Categorization to apply to each attribute:**

1. **Full aspect** — Implemented as a Metalama compile-time aspect (e.g., injects null guards, range checks, `ArgumentOutOfRangeException`).
2. **Partial** — Adds a compile-time warning/error via Metalama `IDiagnostic` without full code weaving (e.g., call-site validation).
3. **Annotation-only** — Meaningful only as an IDE hint; no runtime enforcement makes sense (e.g., `SourceTemplate`, ASP.NET MVC/Razor/XAML helpers). Keep as plain attributes.
4. **Obsolete/skip** — Already deprecated upstream; keep the `[Obsolete]` marker but add no aspect logic (e.g., `TerminatesProgram`).

**Per-attribute notes:**
- **`[NotNull]` / `[CanBeNull]` / `[ItemNotNull]` / `[ItemCanBeNull]`:** Full aspect — inject null guards. See the Nullability architecture section for `[AssumesNrtCallers]` / `[EnforceNullabilityForPreNrtCallers]` and the redundancy warning rules.
- **`[AssumesNrtCallers]`** and **`[EnforceNullabilityForPreNrtCallers]`:** New assembly-level attributes defined in `Nullability\` as part of Phase 2 (see Nullability section above).
- **`[ValueRange]` / `[NonNegativeValue]`:** Full aspect — inject `ArgumentOutOfRangeException` guards on parameters; warn on impossible-range constants at call sites if feasible.
- **`[MustDisposeResource]` / `[HandlesResourceDisposal]`:** Full or partial — compile-time checks via Metalama validators that ownership is handled.
- **`[ContractAnnotation]`:** The FDT mini-language is complex. **Revisit in depth before implementing.** Likely partial at best; full FDT enforcement is probably out of scope. Annotate the decision in the Phase 2 plan.
- **`[Pure]` / `[MustUseReturnValue]`:** Require call-site analysis (referential validator). **Revisit in depth before implementing** — Metalama validators can do this but the design needs careful thought to avoid false positives. Annotate the decision in the Phase 2 plan.
- **`[AssertionMethod]` / `[AssertionCondition]`:** Partial — Metalama can propagate nullability postconditions, but full flow-sensitive enforcement is compiler territory.
- All ASP.NET MVC, Razor, XAML, SourceTemplate, legacy ASP.NET, IDE-tool attributes: **Annotation-only**.

---

### Phase 3 — Implementation (One Attribute at a Time)

**Goal:** For each attribute selected for conversion, implement the Metalama aspect and add unit tests.

**Implementation rules:**
- Aspect implementation code goes in the same file as the attribute class (the split file from Phase 1), below the attribute class itself, unless the file becomes unwieldy — in that case, create a companion `*.Aspects.cs` file alongside.
- The attribute class should either **be** the aspect (by inheriting from the appropriate Metalama base) or be accompanied by a companion fabric/validator.
- Metalama aspects inherit from `Metalama.Framework.Aspects.*` base classes (e.g., `OverrideMethodAspect`, `ContractAspect`, `TypeAspect`).
- Test files go in `Arkane.Annotations.Live.Tests\`, mirroring source file names: `Annotations.Nullability.Tests.cs`, etc.
- Tests must include positive cases (aspect applied correctly, no diagnostic) and negative cases (aspect triggers the expected diagnostic or runtime exception).
- For compile-time aspects, use `Metalama.Testing.AspectTesting` (`TestAspect`) for compile-time unit tests.

---

### Phase 4 — Fabrics

**Goal:** Add Metalama fabrics that automate annotation across whole assemblies, unlocking enforcement that is impossible at the individual declaration site.

The detailed fabric designs live in `Phase4\PLAN.md`. The confirmed Phase 4 deliverables are:

1. **Nullability fabric** (`[EnforceNullabilityForPreNrtCallers]`): Walk the public API surface of the consuming assembly and apply `[NotNull]`/`[CanBeNull]` aspects inferred from NRT annotations. Does not run if `[AssumesNrtCallers]` is present (the two attributes are mutually exclusive).
2. **`[NotifyPropertyChangedInvocator]` auto-application fabric**: Find all types implementing `INotifyPropertyChanged` that have a method matching one of the five recognised signatures but lack the `[NotifyPropertyChangedInvocator]` annotation, and apply it automatically (or warn that it is missing).
3. **CQRS cross-calling fabric**: Walk method bodies compilation-wide to detect `[CqrsCommand]` methods calling `[CqrsQuery]` methods (or vice versa) — the cross-role enforcement that is impossible from a declaration-site aspect alone.

The following were considered and **deferred** beyond Phase 4 (better suited as user-defined fabrics in consuming projects than as library-provided ones):
- `[Pure]` propagation across immutable types
- `[MustDisposeResource]` factory-method auto-application

---

## Metalama Conventions

- Use `Metalama.Framework.Aspects` and `Metalama.Framework.Diagnostics` namespaces.
- Prefer `ContractAspect` for parameter/return value validation (null checks, range checks).
- Use `IAspect<T>` + `BuildAspect` for type-level or member-level logic.
- Report custom diagnostics with `DiagnosticDefinition<T>` — severity should match what ReSharper used (Warning or Error).
- Use `[CompileTime]` on helpers that must only run during compilation.
- Never add runtime overhead beyond what the enforced contract strictly requires.
- Do **not** add Metalama `using` directives to files that contain only annotation-only plain attributes.

## Metalama.Patterns.Contracts Compatibility

`Metalama.Patterns.Contracts` (same version family as `Metalama.Framework`) is a first-party Metalama library that provides ready-made contract aspects (`NotNullAttribute`, `NonNegativeAttribute`, `RangeAttribute`, etc.) and a shared violation infrastructure (`ContractTemplates`, `PostconditionViolationException`, `ContractOptions`).

**Rules when implementing aspects in this project:**

- **Reuse violation infrastructure**: Use `ContractTemplates` methods (e.g., `OnNotNullContractViolated`) from `Metalama.Patterns.Contracts` to generate violation code rather than writing custom `throw` statements. This ensures that users who customize `ContractTemplates` globally (a supported Metalama extensibility pattern) get consistent behavior from both our attributes and Metalama's own contracts.
- **Match exception types**: Precondition violations (input parameters, property setters) throw `ArgumentNullException` or `ArgumentOutOfRangeException`; postcondition violations (return values) throw `PostconditionViolationException` — same as `Metalama.Patterns.Contracts`.
- **Warn consistently**: Emit compile-time warnings using the same LAMA-prefixed diagnostic codes where a direct equivalent exists (e.g., LAMA5002 for `[NotNull]` on a nullable type). Where no direct code exists, define a new diagnostic code in the `AAL` prefix range.
- **Document name conflicts explicitly**: Where `Metalama.Patterns.Contracts` defines an identically-named attribute (e.g., `NotNullAttribute`), add an XML doc note and README note that warns users of the ambiguity and describes the `using` alias pattern to resolve it.
- **Add `Metalama.Patterns.Contracts` as a package reference** in `Arkane.Annotations.Live.csproj` when implementing the first aspect that uses its violation infrastructure (i.e., when implementing `[NotNull]`).

---

## File Header Template

The full license text lives in [`LICENSE.md`](https://github.com/arkane-systems/Arkane.Annotations.Live/blob/master/LICENSE.md) at the repository root. Do **not** duplicate it in source files.

Every split file must begin with the following short header, then the `using` directives, then the `namespace` declaration:

```csharp
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
  // ... attributes ...
}
```

Files that implement Metalama aspects add `using Metalama.Framework.Aspects;` and related namespaces after the standard `using` block.

---

## Build and Test Workflow

```
# Build
dotnet build

# Run tests
dotnet test

# Coverage
dotnet-coverage collect -f cobertura -o coverage.cobertura.xml dotnet test
```

Metalama aspects are validated at compile time; build warnings/errors from Metalama count as test failures.

---

## Diagnostic Code Scheme

All compile-time diagnostics use the prefix `AAL` followed by a four-digit code (`AALxxnn`),
where `xx` identifies the subfolder and `nn` is the specific code within that range.
The authoritative registry — including range assignments, individual codes, severities, and
messages — is maintained in [`DIAGNOSTICS.md`](../DIAGNOSTICS.md) at the repository root.

**Summary of range assignments:**

| Range | Subfolder |
|---|---|
| `AAL01xx` | `Formatting\` |
| `AAL02xx` | `Nullability\` |
| `AAL03xx` | `ValueConstraints\` |
| `AAL04xx` | `Contracts\` — `[ContractAnnotation]` |
| `AAL05xx` | `Contracts\` — `[NotifyPropertyChangedInvocator]` |
| `AAL06xx` | `CodeQuality\` |
| `AAL07xx` | `MethodBehavior\` |
| `AAL08xx` | `Assertions\` |
| `AAL09xx` | `Strings\` |
| `AAL10xx` | `SourceTemplates\` |
| `AAL11xx` | `Cqrs\` |

**Rules:**
- Assign codes sequentially from `xx01` within each range; `xx00` is reserved/unused.
- Record every new code in `DIAGNOSTICS.md` at the time of implementation.
- Runtime violations use standard .NET exceptions, not `AAL` codes.
