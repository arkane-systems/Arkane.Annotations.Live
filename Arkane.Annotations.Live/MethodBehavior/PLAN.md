# Phase 2 Plan: `MethodBehavior\`

## Attributes in scope

| File | Attribute | Category |
|------|-----------|----------|
| `MethodBehaviorAttributes.cs` | `[InstantHandle]` | **Annotation-only** |
| `MethodBehaviorAttributes.cs` | `[Pure]` | **Compile-time validator** |
| `MethodBehaviorAttributes.cs` | `[MustUseReturnValue]` | **Compile-time validator** |
| `MethodBehaviorAttributes.cs` | `[MustDisposeResource]` | **Compile-time validator** |
| `MethodBehaviorAttributes.cs` | `[HandlesResourceDisposal]` | **Annotation-only** |
| `MethodBehaviorAttributes.cs` | `[RequireStaticDelegate]` | **Annotation-only** |
| `MethodBehaviorAttributes.cs` | `[ProvidesContext]` | **Annotation-only** |

No overlap with `Metalama.Patterns.Contracts`.

---

## Attribute-by-attribute plan

### `[InstantHandle]` — Annotation-only

**Semantics:** The delegate/enumerable parameter is fully consumed during the method's
execution — it is not stored for later invocation. Optional `RequireAwait` property restricts
enforcement to async call sites.

**Rationale for annotation-only:** The IDE tracks lifetime of delegate captures across call
sites. Enforcing "not stored" as a runtime check is impossible: we cannot intercept arbitrary
field assignments that happen inside the callee's body from an aspect on the parameter.
Compile-time enforcement would require data-flow analysis of the method body — appropriate for
a Roslyn analyzer, not a Metalama contract aspect.

**Implementation:** Keep as a plain attribute. No Metalama logic.

---

### `[Pure]` — Compile-time validator

**Semantics:** The method makes no observable state changes. The IDE warns when the return
value of a `[Pure]` method call is discarded (since a call with no side effects and a
discarded return value is a no-op).

**Note:** `[Pure]` is equivalent to `System.Diagnostics.Contracts.PureAttribute` (noted in
the XML docs). Consider adding an XML doc `<remarks>` cross-reference to this, and to
`System.Runtime.CompilerServices.MethodImplOptions` for reference-only context.

**Live behavior to add — compile-time validators:**

1. **Return type check** (`AAL07xx` warning): `[Pure]` on a `void`-returning method is a
   contradiction (there is no return value to use or discard). Warn: *"[Pure] has no effect
   on a void-returning method; a pure void method is a no-op by definition."*

2. **State mutation check** (`AAL07xx` warning): At compile time, check whether the method
   body contains obvious state mutations that contradict the purity claim:
   - Assignments to non-local fields or properties of `this` (i.e., `this.field = ...`)
   - Calls to non-pure methods on `this`

   This check is inherently incomplete (cannot catch mutations via aliased references,
   out-parameters to non-pure callees, etc.), so the severity is **warning**, not error,
   and it should be opt-in or limited to clear-cut cases. **Defer the body-analysis check
   to Phase 3 implementation decision** — start with only the void-return check, which is
   unambiguous, and assess whether body analysis is feasible and valuable.

**Test cases:**
- `[Pure]` on a method returning `int` with no mutations → no diagnostic
- `[Pure]` on a `void` method → `AAL07xx` warning
- (Deferred) `[Pure]` on a method that assigns `this.field` → `AAL07xx` warning

---

### `[MustUseReturnValue]` — Compile-time validator

**Semantics:** The return value of the method must be observed by the caller. Unlike `[Pure]`,
the method may have side effects — the return value simply carries information the caller
cannot afford to ignore (e.g., a new immutable collection, a status code, a disposable).

**Live behavior to add — compile-time validator:**

1. **Return type check** (`AAL07xx` error): `[MustUseReturnValue]` on a `void`-returning
   method is meaningless — there is no return value. Error: *"[MustUseReturnValue] cannot
   be applied to a void-returning method."*

2. **`IsFluentBuilderMethod` type check** (`AAL07xx` warning): The `IsFluentBuilderMethod`
   property is documented as requiring the return type to match the receiver type (i.e., the
   method returns `this`). At compile time, verify that when `IsFluentBuilderMethod = true`,
   the method's return type is assignable from `method.DeclaringType`. If not: warning:
   *"IsFluentBuilderMethod = true is only meaningful when the return type is assignable from
   the declaring type."*

**Test cases:**
- `[MustUseReturnValue]` on a method returning `string` → no diagnostic
- `[MustUseReturnValue]` on a `void` method → `AAL07xx` error
- `[MustUseReturnValue(IsFluentBuilderMethod = true)]` on a method that returns the
  declaring type → no diagnostic
- `[MustUseReturnValue(IsFluentBuilderMethod = true)]` on a method that returns `string`
  (not the declaring type) → `AAL07xx` warning

---

### `[MustDisposeResource]` — Compile-time validator

**Semantics:** The returned or constructed resource (an `IDisposable` or `IAsyncDisposable`)
must be disposed at the call site. `Value = false` explicitly opts out (for use in
inheritance scenarios to loosen a parent's constraint).

**Live behavior to add — compile-time validators:**

1. **`IDisposable`/`IAsyncDisposable` type check** (`AAL07xx` warning): When `Value = true`
   (the default), the annotated target should produce or be a disposable type. Check that:
   - For a `Method` or `Constructor`: the return type (or constructed type) implements
	 `IDisposable` or `IAsyncDisposable`
   - For a `Class` or `Struct`: the type itself implements `IDisposable` or
	 `IAsyncDisposable`
   - For a `Parameter`: the parameter type implements `IDisposable` or `IAsyncDisposable`

   If not: **warning** (`AAL07xx`): *"[MustDisposeResource] is applied to a target whose
   type does not implement IDisposable or IAsyncDisposable."* Warning (not error) because
   there are edge cases where the type may implement disposal via a non-standard pattern,
   though these are unusual.

2. **Input parameter check** (`AAL07xx` warning): The XML docs note that "annotation of
   input parameters with this attribute is meaningless." At compile time, if the attribute
   is applied to a non-`out` parameter, report a warning: *"[MustDisposeResource] on an
   input parameter has no effect; apply it to the method, constructor, or an out parameter
   instead."*

**Test cases:**
- `[MustDisposeResource]` on a method returning `Stream` (implements `IDisposable`) → no
  diagnostic
- `[MustDisposeResource]` on a class implementing `IDisposable` → no diagnostic
- `[MustDisposeResource]` on a method returning `int` → `AAL07xx` warning
- `[MustDisposeResource]` on a regular input `string` parameter → `AAL07xx` warning
- `[MustDisposeResource]` on an `out IDisposable` parameter → no diagnostic
- `[MustDisposeResource(false)]` → no diagnostic (Value = false is always valid; it
  is a declaration of intent, not a constraint)

---

### `[HandlesResourceDisposal]` — Annotation-only

**Semantics:** The method, parameter, field, or property acquires ownership of a resource and
is responsible for disposing it. Companion annotation to `[MustDisposeResource]`.

**Rationale for annotation-only:** Like the disposal side of `[MustDisposeResource]`, the
meaningful check is data-flow — whether the resource actually reaches a `Dispose()` call or
`using` statement through all code paths. This is deep data-flow analysis, not a structural
constraint. Annotation-only.

**Implementation:** Keep as a plain attribute. No Metalama logic.

---

### `[RequireStaticDelegate]` — Annotation-only

**Semantics:** The lambda or method group passed to this parameter must not capture any
variables (so the compiler can cache the delegate instance and avoid heap allocation). The
`IsError` property controls whether a violation is an error or a warning.

**Rationale for annotation-only:** The meaningful check is whether the lambda argument at
each *call site* captures variables — this requires data-flow analysis of caller expressions.
Metalama aspects operate at the declaration site. Annotation-only; the IDE performs the
per-call-site check.

**Implementation:** Keep as a plain attribute. No Metalama logic.

---

### `[ProvidesContext]` — Annotation-only

**Semantics:** Marks a member, parameter, or type as the canonical source of a particular
contextual value — the IDE warns when the same type of value is obtained by other means
(e.g., calling a service locator when a field already holds the same service).

**Rationale for annotation-only:** This is a data-flow / value-source tracking annotation.
The IDE tracks all places where a value of the annotated type is obtained and cross-references
them against marked context providers. Entirely IDE-side analysis; no compile-time structural
constraint or runtime enforcement applies.

**Implementation:** Keep as a plain attribute. No Metalama logic.

---

## Implementation sequence for Phase 3

1. Implement `[Pure]` as an `IAspect<IMethod>` with void-return check; define `AAL07xx`
   diagnostics; defer body-mutation check
2. Write tests for `[Pure]`
3. Implement `[MustUseReturnValue]` as an `IAspect<IMethod>` with void-return and
   `IsFluentBuilderMethod` checks
4. Write tests for `[MustUseReturnValue]`
5. Implement `[MustDisposeResource]` as an `IAspect` with IDisposable type check and
   input-parameter check
6. Write tests for `[MustDisposeResource]`
7. All other attributes require no changes

---

## Open questions / deferred decisions

- **`[Pure]` body mutation check**: Starting with only the void-return check is safe and
  unambiguous. Assess during Phase 3 whether Metalama's `meta.Target.Method` body inspection
  APIs make the mutation check feasible without excessive implementation complexity.
- **`[Pure]` vs `System.Diagnostics.Contracts.PureAttribute`**: Consider whether the aspect
  should at compile time also check for (and warn about) the presence of the BCL
  `[Pure]` attribute on the same method, since they are semantically identical and duplicating
  both is redundant. Defer to Phase 3 implementation decision.
- **Diagnostic code range**: `AAL07xx` for `MethodBehavior\`. Confirm when overall scheme
  is settled.
