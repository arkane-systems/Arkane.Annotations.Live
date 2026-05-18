# Phase 2 Plan: `Nullability\`

## Attributes in scope

| File | Attribute | Category |
|------|-----------|----------|
| `NotNullAttribute.cs` | `[NotNull]` | **Full aspect** |
| `CanBeNullAttribute.cs` | `[CanBeNull]` | **Annotation-only** (with optional compile-time diagnostic) |
| `ItemNotNullAttribute.cs` | `[ItemNotNull]` | **Annotation-only** (see rationale below) |
| `ItemCanBeNullAttribute.cs` | `[ItemCanBeNull]` | **Annotation-only** |

---

## Dependency: `Metalama.Patterns.Contracts`

`Metalama.Patterns.Contracts` (same version family as `Metalama.Framework`) should be added as a
package reference to `Arkane.Annotations.Live.csproj`. This allows our `[NotNull]` aspect to use
the same violation infrastructure — specifically `PostconditionViolationException` for return-value
postconditions and `ContractTemplates` for customizable violation handling — ensuring that users
who configure `ContractTemplates` globally get consistent behavior from both our attributes and
Metalama's own contracts.

### Name-conflict note

`Metalama.Patterns.Contracts` also defines `NotNullAttribute`. Users who reference both packages
will have an ambiguous `NotNullAttribute` in scope and will need a `using` alias (e.g.,
`using Arkane = Arkane.Annotations.Live`). This is an expected and acceptable tradeoff: the two
attributes are semantically identical by design, and users who already have
`Metalama.Patterns.Contracts` are unlikely to additionally apply `[NotNull]` from this library —
they would use whichever is already in scope. Document this explicitly in the package README and
in XML doc on the attribute class.

---

## Attribute-by-attribute plan

### `[NotNull]` — Full aspect

**Semantics:** The marked element must never be null at the point of use.

**Application targets (from `[AttributeUsage]`):**
- `Parameter` (input direction): inject `ArgumentNullException` guard at method/constructor entry
- `Method` / `Delegate` (return value, output direction): inject `PostconditionViolationException`
  guard before each return
- `Property` / `Field` / `Event`: inject null guard on assignment (setter / field write)
- `Class` / `Interface` / `GenericParameter`: annotation-only at type level (no runtime injection
  makes sense here; these targets exist for IDE flow analysis)

**Implementation approach:**

Implement `NotNullAttribute` as a `ContractAspect` (from `Metalama.Framework.Aspects`), using
the `ContractTemplates.OnNotNullContractViolated` template from `Metalama.Patterns.Contracts`
to generate the violation code. This ensures identical violation behavior to
`Metalama.Patterns.Contracts.NotNullAttribute`.

Direction handling within `ContractAspect.Validate`:
- `ContractDirection.Input` (parameter, property setter, field assignment): call
  `ContractTemplates.OnNotNullContractViolated` which throws `ArgumentNullException`
- `ContractDirection.Output` (return value): call
  `ContractTemplates.OnNotNullContractViolated` which throws `PostconditionViolationException`

Eligibility: the aspect should be ineligible on value types (where null is structurally impossible)
and should emit a compile-time warning (matching LAMA5002 behavior from Metalama.Patterns.Contracts)
if applied to a nullable reference type (`string?`) in a nullable-enabled context, since the
annotation is redundant there.

**Test cases:**
- Parameter, non-null value passed → no exception
- Parameter, null passed → `ArgumentNullException` with correct parameter name
- Return value, method returns non-null → no exception
- Return value, method returns null → `PostconditionViolationException`
- Property setter, non-null assigned → no exception
- Property setter, null assigned → `ArgumentNullException`
- Applied to value type → compile-time eligibility error
- Applied to `string?` in nullable context → LAMA5002-equivalent compile-time warning

---

### `[CanBeNull]` — Annotation-only (with optional compile-time diagnostic)

**Semantics:** The marked element *may* be null; callers/consumers must null-check before use.

**Rationale for annotation-only:**
There is no meaningful runtime enforcement for "this *may* be null" — the annotation documents
intent and enables IDE flow analysis. Injecting anything at the call site would be wrong (null is
explicitly allowed).

**Optional compile-time diagnostic:**
Consider emitting a low-severity (informational/warning) diagnostic if `[CanBeNull]` is applied to
a declaration that is already nullable-annotated (`string?`, `T?`) in a nullable-enabled project.
In that context the attribute is redundant and may indicate a misunderstanding. Decision: **defer
this diagnostic to a future iteration** unless it proves useful during Phase 3 implementation.

**Implementation:** Keep as a plain attribute. No Metalama logic. No `Metalama` using directives.

**Test cases:**
- Verify the attribute compiles and can be applied to all declared target types
- No runtime behavior to test

---

### `[ItemNotNull]` — Annotation-only

**Semantics:** Elements within the marked collection, enumerable, or async-result are never null.

**Rationale for annotation-only:**
Runtime enforcement would require iterating the entire collection at every call boundary, which
is prohibitively expensive for general use and inappropriate as a default. Per-element null
checking also cannot be done lazily without wrapping the collection in a custom enumerator, which
changes the returned type. For `Task<T>` where T is a collection, the check would have to be
deferred to continuation, adding async complexity.

The primary value of `[ItemNotNull]` is IDE flow analysis (ReSharper/Rider null propagation within
foreach, LINQ, await), which does not require runtime enforcement.

**Future consideration:** A separate opt-in attribute or a Roslyn analyzer could offer per-element
checking for use in debug/test builds. This is out of scope for Phase 3.

**Implementation:** Keep as a plain attribute. No Metalama logic.

**Test cases:**
- Verify the attribute compiles and can be applied to all declared target types
- No runtime behavior to test

---

### `[ItemCanBeNull]` — Annotation-only

**Semantics:** Elements within the marked collection, enumerable, or async-result may be null.

**Rationale:** Same as `[CanBeNull]` — this is a documentation/IDE hint. There is no enforcement
to apply.

**Implementation:** Keep as a plain attribute. No Metalama logic.

**Test cases:**
- Verify the attribute compiles and can be applied to all declared target types
- No runtime behavior to test

---

## Implementation sequence for Phase 3

1. Add `Metalama.Patterns.Contracts` package reference
2. Implement `[NotNull]` as a `ContractAspect` in `NotNullAttribute.cs`
3. Add `Arkane.Annotations.Live.Tests` tests for `[NotNull]`
4. Build and verify — `[CanBeNull]`, `[ItemNotNull]`, `[ItemCanBeNull]` remain as-is from Phase 1

---

## Additional Phase 2 items: NRT control attributes

Two assembly-level attributes must be defined (as plain attributes, no Metalama logic) during
Phase 2. They live in `Nullability\` alongside the nullability attributes.

### `[AssumesNrtCallers]` — Assembly-level attribute (Phase 2, plain attribute)

**Semantics:** Applied to an assembly to declare that all consumers are NRT-aware.

**Effect on `[NotNull]`:** When this attribute is present on the consuming assembly,
`[NotNull]` on a non-nullable reference type emits a compile-time **warning** (`AAL02xx`):
*"[NotNull] is redundant on a non-nullable type in an [AssumesNrtCallers] assembly; the NRT
annotation already enforces non-nullability for aware callers."*

**Effect on Phase 4 fabric:** The `[EnforceNullabilityForPreNrtCallers]` fabric does **not**
run if `[AssumesNrtCallers]` is present. The two attributes are mutually exclusive; applying
both is a compile-time error.

**File:** `Nullability\AssumesNrtCallersAttribute.cs`

**`[AttributeUsage]`:** `AttributeTargets.Assembly`, `AllowMultiple = false`.

### `[EnforceNullabilityForPreNrtCallers]` — Assembly-level attribute (Phase 2, plain attribute)

**Semantics:** Applied to an assembly to activate the Phase 4 nullability fabric, which walks
the public API surface and applies `[NotNull]`/`[CanBeNull]` aspects inferred from NRT
annotations. Intended for NRT-enabled libraries whose consumers may not be NRT-aware.

**Effect in Phase 2:** Defined as a plain attribute only — no fabric behaviour yet (that is
Phase 4 work). The attribute may be applied in Phase 2 without triggering the fabric.

**File:** `Nullability\EnforceNullabilityForPreNrtCallersAttribute.cs`

**`[AttributeUsage]`:** `AttributeTargets.Assembly`, `AllowMultiple = false`.

### Redundancy warning — `[CanBeNull]` on a nullable type

Regardless of which (if either) NRT control attribute is present, applying `[CanBeNull]` to
a nullable type (`T?`) in a nullable-enabled context is unconditionally redundant. The
`[CanBeNull]` aspect should emit a compile-time **warning** (`AAL02xx`): *"[CanBeNull] is
redundant on a nullable type; the NRT annotation already communicates nullability."*

This is the one case where the "optional compile-time diagnostic" mentioned in the
`[CanBeNull]` section above is **decided**: implement it in Phase 3 alongside `[NotNull]`.

### Mutual exclusivity enforcement

Add a compile-time **error** when both `[AssumesNrtCallers]` and
`[EnforceNullabilityForPreNrtCallers]` are applied to the same assembly. This can be
implemented as a simple `IAspect` or fabric check on `[AssumesNrtCallers]` that queries for
the presence of the other attribute. Define this in Phase 3 alongside the `[NotNull]`
implementation.

---

## Open questions / deferred decisions

- **`[CanBeNull]` + nullable context warning**: Decide at implementation time whether a LAMA5002-
  style informational diagnostic is worth adding. Start without it; add only if it proves useful.
- **`[ItemNotNull]` runtime enforcement opt-in**: A future analyzer or conditional contract
  (debug-only) could enforce per-element non-nullability cheaply in test builds. Revisit post-Phase 3.
- **Name conflict documentation**: Add a note to the package README and to `NotNullAttribute`'s
  XML doc warning users that `Metalama.Patterns.Contracts.NotNullAttribute` is an identically-
  behaving alternative; a `using` alias is needed if both packages are referenced.
