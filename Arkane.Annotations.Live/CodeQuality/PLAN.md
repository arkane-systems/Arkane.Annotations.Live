# Phase 2 Plan: `CodeQuality\`

## Attributes in scope

| File | Attribute | Category |
|------|-----------|----------|
| `EqualityAndTypeConstraintAttributes.cs` | `[CannotApplyEqualityOperator]` | **Annotation-only** |
| `EqualityAndTypeConstraintAttributes.cs` | `[DefaultEqualityUsage]` | **Annotation-only** |
| `EqualityAndTypeConstraintAttributes.cs` | `[BaseTypeRequired]` | **Compile-time validator (meta-attribute)** |
| `EqualityAndTypeConstraintAttributes.cs` | `[NoReorder]` | **Annotation-only** |
| `LocalizationRequiredAttribute.cs` | `[LocalizationRequired]` | **Annotation-only** |

No overlap with `Metalama.Patterns.Contracts`.

---

## Attribute-by-attribute plan

### `[CannotApplyEqualityOperator]` — Annotation-only

**Semantics:** Marks a class/struct/interface where `==` and `!=` operators should not be
used for comparison (use `Equals()` instead); null comparisons are always exempt.

**Rationale for annotation-only:**

The enforcement here is *call-site*: the IDE warns when `==` or `!=` is used on an expression
whose type carries this attribute. Metalama aspects run at the *declaration* site, not at every
operator usage site across the codebase. A Metalama `ReferenceValidator` could theoretically
intercept usages, but it would need to detect binary expressions in arbitrary caller code — far
beyond the appropriate scope for this library, and better served by a dedicated Roslyn analyzer.

**Implementation:** Keep as a plain attribute. No Metalama logic.

**Test cases:**
- Verify the attribute compiles and can be applied to class, interface, and struct

---

### `[DefaultEqualityUsage]` — Annotation-only

**Semantics:** Applied to a generic parameter, method parameter, or return type to signal that
default (structural/reference) equality of that type is relied upon. Informs the IDE to warn
when the type is instantiated with a type that has no custom `Equals`/`GetHashCode` override.

**Rationale for annotation-only:**

Like `[CannotApplyEqualityOperator]`, the meaningful check is call-site data-flow analysis —
whether the concrete type argument at an instantiation site has adequate equality semantics.
Metalama cannot inspect caller instantiations from the declaration site. Annotation-only.

**Implementation:** Keep as a plain attribute. No Metalama logic.

**Test cases:**
- Verify the attribute compiles and can be applied to `GenericParameter`, `Parameter`, and
  `ReturnValue` targets

---

### `[BaseTypeRequired]` — Compile-time validator (meta-attribute)

**Semantics:** A meta-attribute — applied to a *custom attribute class* — that declares a
required base type or interface. Any type that is later decorated with that custom attribute
must implement or inherit the specified type; the IDE warns otherwise.

Example:
```csharp
[BaseTypeRequired(typeof(IComponent))]
class ComponentAttribute : Attribute { }

[Component] // Valid: MyComponent implements IComponent
class MyComponent : IComponent { }

[Component] // Warning: does not implement IComponent
class BadComponent { }
```

**Live behavior to add:**

`[BaseTypeRequired]` is the most architecturally interesting attribute in this subfolder
because its enforcement is inherently *two-level*: the attribute is applied to an attribute
class, and the check fires at the application sites of *that* attribute class.

Implementation approach in Metalama: `BaseTypeRequiredAttribute` implements
`IAspect<INamedType>`, where the `INamedType` target is the *attribute class* it decorates
(e.g., `ComponentAttribute`). In `BuildAspect`, it uses Metalama's compilation query API to
find all types in the compilation that have the decorated attribute applied, and for each one
validates that it inherits or implements `BaseType`.

Specifically:
1. In `BuildAspect(IAspectBuilder<INamedType> builder)`, enumerate
   `builder.Target.Compilation.AllTypes` filtered to those that have an attribute of the
   target type applied.
2. For each such type, check `type.AllImplementedInterfaces` and `type.BaseTypes` against
   `BaseType`.
3. If the constraint is not satisfied: report a compile-time **error** (`AAL06xx`:
   *"Type '{0}' is marked with [{1}] but does not implement or inherit required base type
   '{2}'."*)

`BaseTypeRequired` already applies itself to itself in the source:
```csharp
[BaseTypeRequired(typeof(Attribute))]
public sealed class BaseTypeRequiredAttribute : Attribute { }
```
This self-referential constraint (requiring the target to be an `Attribute`) should also be
validated by the aspect — i.e., `[BaseTypeRequired]` must only be applied to `Attribute`
subclasses. If applied to a non-attribute class: compile-time **error** (`AAL06xx`:
*"[BaseTypeRequired] can only be applied to attribute classes (types that inherit from
System.Attribute)."*).

**`AllowMultiple = true`**: Multiple `[BaseTypeRequired]` instances on one attribute class
are validated independently — each `BaseType` must be satisfied.

**Implementation note:** This is the most complex compile-time validator in the project to
date because `BuildAspect` must query the *whole compilation* rather than just the target
declaration. This is a supported Metalama pattern (compilation-wide validators/fabrics) but
should be implemented carefully to avoid excessive compilation overhead.

**Test cases:**
- `[BaseTypeRequired(typeof(IComponent))]` on an attribute class; applied to a type that
  implements `IComponent` → no diagnostic
- Same setup; applied to a type that does *not* implement `IComponent` → `AAL06xx` error
- `[BaseTypeRequired(typeof(BaseClass))]`; applied to a type that inherits `BaseClass` →
  no diagnostic
- Two `[BaseTypeRequired]` on one attribute class; target satisfies both → no diagnostic
- Two `[BaseTypeRequired]`; target satisfies only one → `AAL06xx` error
- `[BaseTypeRequired]` applied to a non-attribute class → `AAL06xx` error (self-constraint)
- Self-referential case: `[BaseTypeRequired]` on `BaseTypeRequiredAttribute` itself (already
  in the source); verify it validates correctly

---

### `[NoReorder]` — Annotation-only

**Semantics:** Tells the ReSharper/Rider "Member Reordering" feature not to reorder members
in the marked type. A pure IDE tool configuration hint.

**Rationale for annotation-only:** No runtime semantics; no compile-time check adds value.
The attribute must be registered in user-defined member reordering patterns to take effect —
entirely outside the scope of this library's enforcement.

**Implementation:** Keep as a plain attribute. No Metalama logic.

**Test cases:**
- Verify the attribute compiles and can be applied to class, interface, struct, and enum

---

### `[LocalizationRequired]` — Annotation-only

**Semantics:** Marks a type or member as requiring localization (`Required = true`, the
default) or explicitly opting out (`Required = false`). The IDE warns about non-localized
string literals within a `[LocalizationRequired(true)]` context.

**Rationale for annotation-only:**

The meaningful check is whether string literals within the marked scope are wrapped in a
resource manager or similar localization call — a data-flow and heuristic analysis the IDE
performs, not a structural constraint we can express as a Metalama contract. Annotation-only.

A potential compile-time check (warn if `[LocalizationRequired(false)]` is applied to a
type/member where no ancestor has `[LocalizationRequired(true)]`, making the opt-out a no-op)
was considered but rejected: it would require walking the inheritance/enclosure hierarchy and
checking resource files, and the marginal value does not justify the complexity.

**Implementation:** Keep as a plain attribute. No Metalama logic.

**Test cases:**
- Verify the attribute compiles with default constructor (implies `Required = true`)
- Verify the attribute compiles with `Required = false`
- Verify it can be applied to `AttributeTargets.All` representative targets

---

## Implementation sequence for Phase 3

1. Implement `[BaseTypeRequired]` as an `IAspect<INamedType>` with compilation-wide query;
   define `AAL06xx` diagnostics
2. Write tests for `[BaseTypeRequired]` (valid, missing interface, missing base class,
   multi-constraint, non-attribute application, self-constraint)
3. All other attributes in this subfolder require no changes

---

## Open questions / deferred decisions

- **Compilation-wide query performance**: The `[BaseTypeRequired]` validator queries all types
  in the compilation. Confirm during implementation that Metalama's incremental compilation
  caching means this does not cause unacceptable build overhead in large solutions.
- **Diagnostic code range**: `AAL06xx` for `CodeQuality\`. Confirm when overall scheme is
  settled.
- **`[BaseTypeRequired]` on `interface` targets**: The `AttributeUsage` is `Class`, so the
  target must be a class (attribute classes are always classes). No additional eligibility
  handling needed for interface/struct.
