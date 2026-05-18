# Phase 2 Plan: `Formatting\`

## Attributes in scope

| File | Attribute | Category |
|------|-----------|----------|
| `StringFormatMethodAttribute.cs` | `[StringFormatMethod]` | **Compile-time validator aspect** |
| `StructuredMessageTemplateAttribute.cs` | `[StructuredMessageTemplate]` | **Annotation-only** |

No overlap with `Metalama.Patterns.Contracts` — that library has no formatting-related types.

---

## Attribute-by-attribute plan

### `[StringFormatMethod]` — Compile-time validator aspect

**Semantics:** Marks a method (constructor, property getter, or delegate) that builds a string
by a format pattern (like `string.Format`). The constructor argument names the parameter that
receives the format string. The IDE uses this to warn when format placeholders and arguments
don't match.

**Live behavior to add:**

The most actionable live enforcement is a **compile-time validation** that the name supplied
in the constructor (`FormatParameterName`) actually corresponds to a parameter on the attributed
method. Currently a typo in the name is silently accepted. A secondary check is that the named
parameter is of type `string` (or `string?`/`string!`).

There is no meaningful *runtime* injection here: format-string correctness can only be analysed
statically, and we cannot generally reconstruct the original format string at runtime for
re-validation.

**Implementation approach:**

Implement `StringFormatMethodAttribute` as both an `Attribute` and an `IAspect<IMethod>`
(and likewise for `IConstructor` and `IProperty` given the `AttributeUsage`). In
`BuildAspect` / `BuildEligibility`, use Metalama compile-time APIs to:

1. Resolve the target's parameter list.
2. Check that a parameter named `FormatParameterName` exists.
   - If not: report a compile-time **error** diagnostic (`AAL0101`: *"No parameter named
	 '{0}' exists on '{1}'."*).
3. Check that the named parameter is of type `string` (or `string?`).
   - If not: report a compile-time **warning** diagnostic (`AAL0102`: *"Parameter '{0}' on
	 '{1}' is not of type string; [StringFormatMethod] may not work as expected."*).

No runtime code is injected; this is a pure compile-time validator.

**Note on `FormatParameterName` null guard:** The constructor already declares
`[NotNull] string formatParameterName`. Once `[NotNull]` is a live aspect (Phase 3,
`Nullability\`), this parameter will automatically get a null guard injected.
Do not add a manual null guard here.

**Eligibility:** The aspect is ineligible on `void` methods if the only format-string use
would be a return value (not applicable here — eligibility should be permissive to match
the existing `AttributeUsage` targets).

**Test cases:**
- `[StringFormatMethod("message")]` on a method that has a `string message` parameter → no
  compile-time diagnostic
- `[StringFormatMethod("msg")]` on a method that has no `msg` parameter → AAL0101 error
- `[StringFormatMethod("count")]` where `count` is an `int` parameter → AAL0102 warning
- Applied to a constructor with a matching parameter → no diagnostic
- Applied to a property → no diagnostic (properties cannot have parameters in the same sense;
  consider this edge case during implementation — may need to restrict to methods/constructors
  in the eligibility check and note the limitation)

---

### `[StructuredMessageTemplate]` — Annotation-only

**Semantics:** Marks a `string` parameter as a structured message template where named
placeholders (`{username}`, `{count}`) are matched to subsequent arguments. Used by
Serilog-aware tools and Rider/ReSharper to validate argument counts and names.

**Rationale for annotation-only:**

The placeholder-matching logic is entirely IDE/analyzer work. Runtime validation would require
parsing the template string at call time, which:
- Is non-trivial (especially for Serilog-style positional vs. named semantics)
- Duplicates work done far better by dedicated logging analyzers (e.g., `Serilog.Analyzers`)
- Would add measurable overhead on every logging call

A compile-time Metalama check that the decorated parameter is of type `string` would add
minor value, but the same check is trivially visible in code review, and applying
`[StructuredMessageTemplate]` to a non-string parameter would be an obvious error caught
immediately. **Defer this check; keep the attribute annotation-only for now.**

**Implementation:** Keep as a plain attribute. No Metalama logic. No `Metalama` using
directives.

**Test cases:**
- Verify the attribute compiles and can be applied to a `string` parameter
- No runtime behavior to test

---

## Implementation sequence for Phase 3

1. Convert `StringFormatMethodAttribute` to implement `IAspect<IMethod>` (and
   `IAspect<IConstructor>`) and define `AAL0101` / `AAL0102` diagnostics
2. Write compile-time tests verifying the diagnostics fire correctly
3. `StructuredMessageTemplateAttribute` requires no changes

---

## Open questions / deferred decisions

- **Property/Delegate targets for `[StringFormatMethod]`**: `AttributeUsage` includes
  `Property` and `Delegate`. It is unclear what "format parameter" means on a property
  (no parameters) or a delegate type. During implementation, consider restricting eligibility
  to `Method` and `Constructor` only, and documenting the restriction.
- **`AAL` diagnostic code range**: Establish a diagnostic code numbering scheme before
  Phase 3 begins. Suggested ranges: `AAL01xx` — Formatting, `AAL02xx` — Nullability,
  etc. (or a flat sequential scheme). Record the chosen scheme in `copilot-instructions.md`.
