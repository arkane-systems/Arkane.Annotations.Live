# Phase 2 Plan: `ValueConstraints\`

## Attributes in scope

| File | Attribute | Category |
|------|-----------|----------|
| `NonNegativeValueAttribute.cs` | `[NonNegativeValue]` | **Full aspect** (via Metalama.Patterns.Contracts) |
| `ValueRangeAttribute.cs` | `[ValueRange]` | **Full aspect** (via Metalama.Patterns.Contracts) |
| `ValueProviderAttribute.cs` | `[ValueProvider]` | **Annotation-only** (with optional compile-time validator) |

---

## Metalama.Patterns.Contracts overlap

This subfolder has the **strongest** overlap with `Metalama.Patterns.Contracts` of any group
in Phase 2. Two directly analogous types exist:

| Our attribute | Metalama.Patterns.Contracts equivalent |
|---|---|
| `[NonNegativeValue]` | `NonNegativeAttribute` |
| `[ValueRange(long, long)]` | `RangeAttribute(long, long, ...)` |

In both cases we should use `ContractTemplates.OnRangeContractViolated` (for `[ValueRange]`)
and the equivalent non-negative violation template to generate injected code, ensuring
consistent `ArgumentOutOfRangeException` behavior and global `ContractTemplates` customizability.

**Key difference between our attributes and theirs:**

- `Metalama.Patterns.Contracts.RangeAttribute` supports a wide range of numeric types
  (int, uint, short, ushort, byte, sbyte, long, ulong, decimal via `decimalPlaces`, float,
  double) and includes `minAllowed`/`maxAllowed` strictness options.
- Our `[ValueRange]` only stores `From`/`To` as `object` (boxing `long` or `ulong`) and
  has no strictness concept — bounds are always inclusive.
- `Metalama.Patterns.Contracts.NonNegativeAttribute` supports all numeric types;
  our `[NonNegativeValue]` is documented as "integral value" only.

**Strategy: wrap / delegate to `Metalama.Patterns.Contracts` internals where possible.**
Since both packages share the same `ContractTemplates` violation infrastructure, our aspects
can call the same `ContractTemplates` methods directly rather than re-implementing injection
logic.

---

## Attribute-by-attribute plan

### `[NonNegativeValue]` — Full aspect

**Semantics:** The marked integral value must be ≥ 0. Equivalent to
`[ValueRange(0, long.MaxValue)]` for signed types.

**Implementation approach:**

Implement `NonNegativeValueAttribute` as a `ContractAspect`. In `Validate`:
- Check the target's type at compile time to confirm it is a supported integral numeric type
  (`int`, `long`, `short`, `byte`, `sbyte`, `uint`, `ulong`, `ushort`). Report a
  compile-time error (`AAL03xx`) if applied to a non-numeric type.
- Inject: `if (value < 0) ContractTemplates.OnRangeContractViolated(value, range, context)`
  — reusing the same violation template as `Metalama.Patterns.Contracts.NonNegativeAttribute`.
  (Unsigned types are inherently non-negative; consider reporting a compile-time warning
  `AAL03xx` for applying `[NonNegativeValue]` to `uint`, `ulong`, `ushort`, or `byte` as
  redundant — same as LAMA5002 rationale.)

**Application targets** (from `[AttributeUsage]`):
- `Parameter` (input): inject at method/constructor entry
- `Field` / `Property`: inject on assignment (setter / field write)
- `Method` / `Delegate` (return value): inject postcondition before return

**Test cases:**
- Non-negative int passed as parameter → no exception
- Negative int passed as parameter → `ArgumentOutOfRangeException` with correct parameter name
- Return value non-negative → no exception
- Return value negative → `PostconditionViolationException`
- Property setter: non-negative → no exception; negative → `ArgumentOutOfRangeException`
- Applied to `uint` parameter → compile-time redundancy warning
- Applied to `string` parameter → compile-time error

---

### `[ValueRange]` — Full aspect

**Semantics:** The integral value falls within `[From, To]` (inclusive bounds, always).
`AllowMultiple = true` — a declaration may carry several non-intersecting `[ValueRange]`
intervals.

**Constructors and stored types:**
- `(long from, long to)` — signed range
- `(ulong from, ulong to)` — unsigned range
- `(long value)` / `(ulong value)` — single exact value (from == to)

**Implementation approach:**

Implement `ValueRangeAttribute` as a `ContractAspect`. In `Validate`:
- At compile time, determine the declared type of the target and confirm it is a supported
  integral numeric type. Report `AAL03xx` error if not.
- At compile time, validate that `From ≤ To` (both stored as `object`; check at aspect
  construction). Report `AAL03xx` error if the range is inverted.
- Inject a runtime range check using `ContractTemplates.OnRangeContractViolated(value,
  new NumericRange(...), context)` — reusing the same template as
  `Metalama.Patterns.Contracts.RangeAttribute`.
- Because `AllowMultiple = true` and the JetBrains semantic is "value satisfies *at least one*
  of the intervals", multiple `[ValueRange]` instances on one target must be combined with
  logical OR at runtime: `if (!range1.Contains(v) && !range2.Contains(v)) throw`. This
  differs from Metalama.Patterns.Contracts which requires each `[Range]` to be satisfied
  independently. Document this explicitly.

**Application targets** (same analysis as `[NonNegativeValue]` above).

**Test cases:**
- Value in range → no exception
- Value below lower bound → `ArgumentOutOfRangeException`
- Value above upper bound → `ArgumentOutOfRangeException`
- Exact-value constructor (`long value`): matching value → no exception; other value → exception
- Two `[ValueRange]` intervals on one parameter: value in either interval → no exception;
  value in neither interval → exception (OR semantics: at least one interval must be satisfied)
- `from > to` in constructor → compile-time error
- Applied to `string` → compile-time error
- Return value postcondition variant
- Property setter variant
- `ulong` range constructor: `0, ulong.MaxValue` → no exception for any valid ulong

---

### `[ValueProvider]` — Annotation-only (with optional compile-time validator)

**Semantics:** Names a type whose `static` or `const` fields contain valid values for the
annotated property/field/parameter. Pure IDE completion hint.

**Rationale for annotation-only:**

There is no runtime contract to enforce — the attribute only tells the IDE which type to
pull completion suggestions from. Any runtime check (e.g., "the assigned value is equal to
one of the fields of the named type") would require reflection, would break for non-primitive
types, and would run on every assignment — entirely inappropriate as a default.

**Optional compile-time validator (defer to Phase 3 implementation decision):**

At compile time it would be possible to resolve the named type string and report an error if
it does not exist in the compilation. This is similar to what `[StringFormatMethod]` does for
its parameter name. This would catch typos. However:
- The type name is a string (not a `Type` argument) so resolution needs `ICompilation.FindType`
- The type might live in a different assembly not yet referenced
- This may produce false positives in partial builds

**Decision: start annotation-only; revisit adding the type-resolution check during Phase 3
implementation if it proves easy to do reliably.**

**Test cases:**
- Verify the attribute compiles and can be applied to all declared targets
- No runtime behavior to test

---

## Implementation sequence for Phase 3

1. Implement `[NonNegativeValue]` as a `ContractAspect`; define `AAL03xx` diagnostics
2. Write tests for `[NonNegativeValue]`
3. Implement `[ValueRange]` as a `ContractAspect`; extend `AAL03xx` diagnostics
4. Write tests for `[ValueRange]`, including multi-interval OR semantics
5. `[ValueProvider]` requires no changes (annotation-only)

---

## Open questions / deferred decisions

- **Diagnostic code range for `ValueConstraints\`**: Proposed `AAL03xx`. Confirm when the
  overall `AAL` numbering scheme is settled (flagged as an open question in `Formatting\PLAN.md`).
- **`[ValueRange]` multi-interval OR semantics**: ✅ **Decided.** Multiple `[ValueRange]`
  attributes on a single target are evaluated as OR — the value must satisfy *at least one*
  of the intervals. This matches JetBrains' original intent and observed IDE behavior.
- **Floating-point support**: `Metalama.Patterns.Contracts.RangeAttribute` supports `float`
  and `double`; our `[ValueRange]` constructors only accept `long`/`ulong`. We do not
  extend to floating-point — the JetBrains original never did — but document this limitation.
- **`[NonNegativeValue]` on unsigned types**: Decide at implementation time whether to emit
  a redundancy warning or silently succeed (no injection needed for inherently non-negative types).
