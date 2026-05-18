# Phase 4 Plan: Fabrics

## Overview

Phase 4 adds Metalama fabrics — compilation-wide aspects that automate annotation or
enforcement across whole assemblies. Fabrics unlock behaviors that are impossible at the
individual declaration site because they require access to the full compilation graph, method
bodies, or the public API surface as a whole.

Phase 4 is planned but not yet scheduled. It begins after Phase 3 (per-attribute
implementation) is substantially complete.

---

## Fabric 1: Nullability Fabric

**Activating attribute:** `[EnforceNullabilityForPreNrtCallers]` (assembly-level, defined in
`Nullability\` during Phase 2)

**Suppressed by:** `[AssumesNrtCallers]` (the two attributes are mutually exclusive; the
fabric checks for `[AssumesNrtCallers]` and does nothing if it is present)

### Purpose

In a NRT-enabled assembly, `string` means non-null and `string?` means nullable. Pre-NRT
consumers (or consumers who suppress NRT warnings) can still pass null and violate the
contract without receiving a runtime error. This fabric injects `[NotNull]` and `[CanBeNull]`
aspects inferred directly from the NRT annotations on the public API surface, providing
runtime guards for any caller that bypasses the compiler.

### Scope

- **Public API surface only** by default (public and protected members of public types).
- Value types are skipped (null is impossible).
- Members that already carry an explicit `[NotNull]` or `[CanBeNull]` attribute are skipped
  (the explicit annotation wins; double-injection is not allowed).

### Implementation approach

Implement as a `TransitiveProjectFabric`, since consumers are the target for the behavior). In `AmendProject`:

1. Check for `[AssumesNrtCallers]` on the assembly; exit immediately if present.
2. Enumerate all public types and their public/protected members.
3. For each parameter, return value, field, or property whose declared type is a non-nullable
   reference type (NRT-aware): apply `[NotNull]` if not already present.
4. For each parameter, return value, field, or property whose declared type is a nullable
   reference type (`T?`, NRT-aware): apply `[CanBeNull]` if not already present.
5. Skip members in types that carry `[GeneratedCode]` or Metalama's own internal attributes.

### Deferred decisions

- Whether to support an opt-out per-member attribute (e.g., `[NrtFabricSkip]`) to suppress
  fabric injection on specific members.
- Whether to expose a scope option on `[EnforceNullabilityForPreNrtCallers]` to extend
  injection to internal members.

---

## Fabric 2: `[NotifyPropertyChangedInvocator]` Auto-Application Fabric

**Activating attribute:** TBD (possibly unconditional, or activated by a new assembly-level
attribute `[AutoApplyInpcAnnotations]` for opt-in)

### Purpose

Find all types implementing `System.ComponentModel.INotifyPropertyChanged` that contain a
method matching one of the five recognised `[NotifyPropertyChangedInvocator]` signatures but
are missing the explicit `[NotifyPropertyChangedInvocator]` annotation. Either apply the
annotation automatically or emit a warning.

This saves INPC implementers from having to remember to apply the attribute manually to every
`OnPropertyChanged` / `NotifyChanged` / `SetProperty` method.

### The five recognised signatures (from `Contracts\PLAN.md`)

1. `NotifyChanged(string)`
2. `NotifyChanged(params string[])`
3. `NotifyChanged<T>(Expression<Func<T>>)`
4. `NotifyChanged<T,U>(Expression<Func<T,U>>)`
5. `SetProperty<T>(ref T, T, string)`

Matching is on shape (parameter count, types, generic arity, `ref`-ness), not method name.

### Implementation approach

Implement as a `ProjectFabric`. In `AmendProject`:

1. Enumerate all types implementing `INotifyPropertyChanged`.
2. For each type, scan instance methods for signature matches against the five patterns.
3. For each matching method that lacks `[NotifyPropertyChangedInvocator]`: apply the
   attribute programmatically via the Metalama fabric API.

### Deferred decisions

- Whether to warn-only (emit a diagnostic suggesting the annotation) vs auto-apply. Auto-apply
  is more powerful but less transparent; warn-only lets the developer see and confirm the
  annotation.
- Activation model: unconditional (always runs) may generate unexpected behavior for some INPC
  patterns. An opt-in assembly attribute is safer.

---

## Fabric 3: CQRS Cross-Role Calling Enforcement

**Activating attribute:** `[CqrsCommand]` / `[CqrsQuery]` already present on methods.
No additional fabric attribute needed — the fabric activates whenever these attributes are
found in the compilation.

### Purpose

Detect `[CqrsCommand]` methods calling `[CqrsQuery]` methods, or `[CqrsQuery]` methods
calling `[CqrsCommand]` methods. This cross-role calling analysis was explicitly deferred
from `Cqrs\PLAN.md` because it requires body/call-graph analysis — precisely what a
compilation-wide fabric can provide.

### Implementation approach

Implement as a `ProjectFabric`. In `AmendProject`:

1. Collect all methods marked `[CqrsCommand]` and all methods marked `[CqrsQuery]`.
2. For each `[CqrsCommand]` method, inspect its body (via Metalama's `IMethod.Body` /
   `IMethod.GetBody()` compile-time API) for calls to any method in the `[CqrsQuery]` set.
3. For each `[CqrsQuery]` method, likewise inspect for calls to `[CqrsCommand]` methods.
4. Report a compile-time **warning** (`AAL11xx`) for each cross-role call found:
   *"[CqrsCommand] method '{0}' calls [CqrsQuery] method '{1}'; this violates CQRS
   separation of concerns."*
5. Members marked `[CqrsExcludeFromAnalysis]` are skipped.

### Deferred decisions

- Confirm that Metalama's compile-time body inspection API (available in 2026.x) covers
  the call-detection pattern needed without excessive complexity.
- Decide whether to also detect indirect cross-role calls (A calls B calls C where C is
  the cross-role method) — direct calls only for the initial implementation.

---

## Deferred / Out of Scope for Phase 4

The following fabric ideas were considered and deferred beyond Phase 4. They are better
suited as user-defined fabrics in consuming projects because they are highly
project-specific:

- **`[Pure]` propagation**: Automatically applying `[Pure]` to all methods of immutable or
  record types. Too project-specific; no reliable heuristic for "this type is pure" exists
  in the general case.
- **`[MustDisposeResource]` factory-method auto-application**: Applying `[MustDisposeResource]`
  to all factory methods / constructors returning `IDisposable` types. Legitimate but
  project-specific; the pattern of what counts as a "factory method" varies too much.

Both are good candidates for a future "example fabrics" section in the project documentation.

---

## Phase 4 Implementation Sequence (tentative)

1. Implement nullability fabric (`[EnforceNullabilityForPreNrtCallers]` + mutual-exclusion
   check with `[AssumesNrtCallers]`)
2. Write tests for the nullability fabric (NRT member gets guard; explicit annotation skipped;
   `[AssumesNrtCallers]` suppresses fabric; value types skipped)
3. Implement `[NotifyPropertyChangedInvocator]` auto-application fabric; decide warn-vs-apply
   during implementation
4. Write tests for the INPC fabric
5. Implement CQRS cross-calling fabric; extend `AAL11xx` diagnostic range
6. Write tests for the CQRS fabric (direct cross-role call detected; `[CqrsExcludeFromAnalysis]`
   suppresses; same-role call not flagged)
