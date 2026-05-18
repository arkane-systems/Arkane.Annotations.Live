# Phase 2 Plan: `Assertions\`

## Attributes in scope

| File | Attribute | Category |
|------|-----------|----------|
| `AssertionAttributes.cs` | `[AssertionMethod]` | **Compile-time validator** |
| `AssertionAttributes.cs` | `[AssertionCondition]` | **Compile-time validator** |
| `AssertionAttributes.cs` | `AssertionConditionType` (enum) | **No change** |
| `AssertionAttributes.cs` | `[TerminatesProgram]` | **Annotation-only** (obsolete) |

No overlap with `Metalama.Patterns.Contracts`.

---

## Attribute-by-attribute plan

### `[AssertionMethod]` + `[AssertionCondition]` — Compile-time validators (paired)

**Semantics:** `[AssertionMethod]` marks a method as an assertion that halts control flow if
a condition is violated. `[AssertionCondition]` marks which parameter carries the condition
and what kind (`IS_TRUE`, `IS_FALSE`, `IS_NULL`, `IS_NOT_NULL`). The IDE uses this pair to:
- Treat call sites as unreachable when the assertion fails (e.g., code after a failed assert)
- Narrow nullability and boolean state after the assertion passes

These two attributes are tightly coupled and must be validated together.

**Live behavior to add — compile-time validators on `[AssertionMethod]`:**

1. **Has at least one `[AssertionCondition]` parameter** (`AAL08xx` warning): An
   `[AssertionMethod]` with no `[AssertionCondition]`-marked parameter is a declaration the
   IDE cannot use — it doesn't know which parameter carries the condition. Warn: *"Method
   '{0}' is marked [AssertionMethod] but has no parameter marked with
   [AssertionCondition]."*

2. **Return type is `void` or `bool`** (`AAL08xx` warning): Assertion methods that return
   non-void, non-bool values are unusual. The IDE handles `void` (execution resumes only if
   the condition holds) and `bool` (return value reflects the assertion result). Other return
   types are not meaningful in this context. Warn: *"[AssertionMethod] is typically applied
   to void or bool-returning methods; the return type '{0}' may not be handled correctly by
   the IDE."*

**Live behavior to add — compile-time validators on `[AssertionCondition]`:**

3. **Parameter is on an `[AssertionMethod]`** (`AAL08xx` warning): `[AssertionCondition]`
   applied to a parameter whose containing method is not marked `[AssertionMethod]` is an
   orphaned annotation the IDE cannot use. Warn: *"[AssertionCondition] is applied to a
   parameter of '{0}', which is not marked [AssertionMethod]."*

4. **Parameter type matches `ConditionType`** (`AAL08xx` warning):
   - `IS_TRUE` / `IS_FALSE`: parameter should be `bool` or `bool?`
   - `IS_NULL` / `IS_NOT_NULL`: parameter should be a reference type or nullable value type

   Warn if the condition type is incompatible with the parameter type: *"AssertionConditionType.{0}
   is not compatible with parameter type '{1}'."*

**Implementation approach:**

Both attributes implement `IAspect<IMethod>` (for `[AssertionMethod]`) and
`IAspect<IParameter>` (for `[AssertionCondition]`). Because the validations are cross-attribute
(each needs to check for the other's presence), the checks should look up sibling attributes
via `parameter.ContainingMember.Attributes` / `method.Parameters[i].Attributes` at compile
time.

**Test cases:**
- `[AssertionMethod]` on a `void` method with one `[AssertionCondition(IS_TRUE)]` `bool`
  parameter → no diagnostic
- `[AssertionMethod]` on a `bool`-returning method with one `[AssertionCondition]` parameter
  → no diagnostic
- `[AssertionMethod]` on a method with no `[AssertionCondition]` parameter → `AAL08xx`
  warning
- `[AssertionMethod]` on a method returning `string` → `AAL08xx` warning
- `[AssertionCondition(IS_TRUE)]` on a `bool` parameter of a non-`[AssertionMethod]` method
  → `AAL08xx` warning
- `[AssertionCondition(IS_TRUE)]` on an `int` parameter → `AAL08xx` warning
- `[AssertionCondition(IS_NULL)]` on a `string` parameter → no diagnostic (reference type)
- `[AssertionCondition(IS_NULL)]` on an `int` parameter (non-nullable value type) →
  `AAL08xx` warning

---

### `[TerminatesProgram]` — Annotation-only (obsolete)

**Semantics:** Indicates the method unconditionally terminates control flow (throws or
terminates the process). Superseded by `[ContractAnnotation("=> halt")]`.

The attribute is already marked `[Obsolete]` in the source. No further Metalama logic is
appropriate — the `[Obsolete]` attribute already causes the IDE to warn at every usage site.
Adding further validation would be redundant.

The `[Obsolete]` marker and its message ("Use `[ContractAnnotation("=> halt")]` instead")
should be preserved exactly as-is.

**Implementation:** Keep as a plain attribute. No Metalama logic.

---

## Implementation sequence for Phase 3

1. Implement `[AssertionMethod]` as `IAspect<IMethod>` with checks 1 and 2; define `AAL08xx`
   diagnostics
2. Implement `[AssertionCondition]` as `IAspect<IParameter>` with checks 3 and 4
3. Write tests for both (including cross-attribute orphan checks)
4. `[TerminatesProgram]` requires no changes

---

## Open questions / deferred decisions

- **Diagnostic code range**: `AAL08xx` for `Assertions\`. Confirm when overall scheme is
  settled.
- **Multiple `[AssertionCondition]` parameters**: A method could theoretically have more than
  one `[AssertionCondition]`-marked parameter (e.g., a combined assert). The JetBrains
  tooling supports this. The "no condition parameter" check should fire only when *none* are
  present, not when there is exactly one.
