# Phase 2 Plan: Cqrs

## Summary

Two attributes are compile-time validators (naming convention checks); three are
annotation-only. No full aspects are planned.

## Analysis

The XML docs explicitly promise that naming and adherence to the CQRS pattern will be
checked. The JetBrains example comments reveal two distinct categories of check:

1. **Naming conventions** (declaration-site) — commands should end in `Command`, queries
   in `Query`, handlers in `CommandHandler`/`QueryHandler`. These are pure string checks
   against the declaration name and are a perfect fit for a Metalama compile-time
   validator.

2. **Cross-role calling** (body/call-graph) — e.g. a `[CqrsCommand]` method calling a
   `[CqrsQuery]` method. This requires inter-procedural call-graph analysis that is only
   available to full IDE data-flow engines. Not feasible for a Metalama aspect at this
   time.

## Attribute Plans

### `CqrsCommandAttribute` — Compile-time validator

Validate that the marked declaration's name ends with `Command` (for methods, types, and
properties) or follows the `Handle`/`Execute` pattern for constructors.

**Checks:**
- Method/property/type name ends with `Command` (warning if not; the user may have a
  deliberate alternative name, so this is a configurable severity).
- Constructor: no naming constraint (constructors take the class name).

**Diagnostic:** `AAL1101` — `[CqrsCommand] declaration '{0}' does not follow the
recommended 'Command' naming suffix.`

### `CqrsQueryAttribute` — Compile-time validator

Validate that the marked declaration's name ends with `Query` (same targets and rationale
as above).

**Checks:**
- Method/property/type name ends with `Query`.
- Constructor: no naming constraint.

**Diagnostic:** `AAL1102` — `[CqrsQuery] declaration '{0}' does not follow the
recommended 'Query' naming suffix.`

### `CqrsCommandHandlerAttribute` — Annotation-only

Deferred. Naming convention (`CommandHandler` suffix) is structurally identical to the
command/query validators above and could be added in the same implementation pass. However,
the more valuable check — that the class actually handles a `[CqrsCommand]` type (e.g.
implements `ICommandHandler<TCommand>`) — requires knowledge of the specific CQRS
framework in use, which varies by project. Kept annotation-only until a framework
integration story is clearer.

### `CqrsQueryHandlerAttribute` — Annotation-only

Same rationale as `CqrsCommandHandlerAttribute`.

### `CqrsExcludeFromAnalysisAttribute` — Annotation-only

This attribute exists solely to suppress cross-role calling analysis. Since that analysis
is not implementable as a Metalama aspect (body/call-graph required), this attribute has
no enforcement role and remains annotation-only.

## Deferred Work

- Cross-role calling analysis (`[CqrsCommand]` method calling `[CqrsQuery]` method) — IDE
  only; not feasible for declaration-site Metalama validation.
- Handler-to-command/query type binding checks — dependent on the specific CQRS framework
  used by the consuming project.
- Naming-convention severity configuration — consider making the warning level
  configurable via a fabric option in a future pass.

## Diagnostic Code Range

`AAL1101`–`AAL1110` reserved for this folder.

## Implementation Sequence

1. `CqrsCommandAttribute` validator (naming suffix check, `AAL1101`)
2. `CqrsQueryAttribute` validator (naming suffix check, `AAL1102`)
3. Tests for both validators covering: correct name, missing suffix, constructor target
   (no warning), `CqrsExcludeFromAnalysisAttribute` suppression (no-op at this stage)

## Attribute Inventory

| Attribute | Status | Notes |
|---|---|---|
| `CqrsCommandAttribute` | Compile-time validator | Naming suffix check (`Command`) |
| `CqrsQueryAttribute` | Compile-time validator | Naming suffix check (`Query`) |
| `CqrsCommandHandlerAttribute` | Annotation-only | Framework-dependent handler check deferred |
| `CqrsQueryHandlerAttribute` | Annotation-only | Framework-dependent handler check deferred |
| `CqrsExcludeFromAnalysisAttribute` | Annotation-only | Suppresses cross-role analysis; analysis not yet implemented |
