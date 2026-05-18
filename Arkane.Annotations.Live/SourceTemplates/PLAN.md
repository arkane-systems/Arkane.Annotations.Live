# Phase 2 Plan: `SourceTemplates\`

## Attributes in scope

| File | Attribute | Category |
|------|-----------|----------|
| `SourceTemplateAttributes.cs` | `[SourceTemplate]` | **Compile-time validator** |
| `SourceTemplateAttributes.cs` | `[Macro]` | **Compile-time validator** |
| `SourceTemplateAttributes.cs` | `SourceTemplateTargetExpression` (enum) | **No change** |

No overlap with `Metalama.Patterns.Contracts`.

---

## Attribute-by-attribute plan

### `[SourceTemplate]` — Compile-time validator

**Semantics:** Marks an extension method as a ReSharper/Rider Source Template — when
completed over an expression, the method body is expanded as a code snippet at the call site.
The IDE consumes the method body text; the method is never actually *called* at runtime.

**Live behavior to add — compile-time validators:**

1. **Must be a static extension method** (`AAL10xx` error): Source Templates are required to
   be extension methods (i.e., static methods with at least one parameter marked `this`).
   A `[SourceTemplate]` on a non-static method or a static method with no `this` parameter
   is unusable by the IDE. Error: *"[SourceTemplate] must be applied to a static extension
   method (a static method with a 'this' parameter)."*

2. **Must be in a static class** (`AAL10xx` error): Extension methods in C# must be declared
   in non-generic static classes. This is enforced by the compiler for any extension method,
   but an explicit check here provides a clearer diagnostic message in context. Since the
   compiler will also catch this, consider whether our error adds value over the compiler
   error — if not, omit this check to avoid redundant messages. **Defer to Phase 3
   implementation decision.**

3. **Return type is `void`** (`AAL10xx` warning): Source Templates are expanded in-place;
   the return value is never used. A non-`void` return type is not meaningfully consumable
   in the template expansion context. Warn: *"[SourceTemplate] methods conventionally return
   void; the return value '{0}' will not be available at the expansion site."*

**Test cases:**
- `[SourceTemplate]` on a `static void` extension method → no diagnostic
- `[SourceTemplate]` on a non-static method → `AAL10xx` error
- `[SourceTemplate]` on a `static void` method with no `this` parameter (not an extension
  method) → `AAL10xx` error
- `[SourceTemplate]` on a `static string` extension method → `AAL10xx` warning

---

### `[Macro]` — Compile-time validator

**Semantics:** Specifies a macro expression to be applied to a parameter within a Source
Template expansion. Can be applied to the method (with `Target` identifying the parameter
by name) or directly to a parameter.

**Live behavior to add — compile-time validators:**

1. **Must be within a `[SourceTemplate]` context** (`AAL10xx` warning): `[Macro]` on a
   parameter or method that is not a `[SourceTemplate]` (or on a parameter of a method that
   is not a `[SourceTemplate]`) is an orphaned annotation the IDE cannot use. Warn:
   *"[Macro] is applied to '{0}', which is not a [SourceTemplate] method or a parameter of
   one."*

2. **`Target` name check when applied to method** (`AAL10xx` warning): When `[Macro]` is
   applied to the method itself with a `Target` property set, verify that a parameter named
   `Target` actually exists on the method. Warn if not: *"[Macro] Target '{0}' does not
   match any parameter on '{1}'."* (Same pattern as `[StringFormatMethod]` and
   `[NotifyPropertyChangedInvocator]`.)

**Test cases:**
- `[Macro(Expression = "...")]` on a parameter of a `[SourceTemplate]` method → no
  diagnostic
- `[Macro(Expression = "...", Target = "x")]` on a `[SourceTemplate]` method that has
  parameter `x` → no diagnostic
- `[Macro]` on a parameter of a non-`[SourceTemplate]` method → `AAL10xx` warning
- `[Macro(Target = "missing")]` on a `[SourceTemplate]` method with no parameter named
  `missing` → `AAL10xx` warning

---

## Implementation sequence for Phase 3

1. Implement `[SourceTemplate]` as `IAspect<IMethod>` with extension-method and void-return
   checks; define `AAL10xx` diagnostics
2. Implement `[Macro]` as `IAspect<IParameter>` and `IAspect<IMethod>` with source-template
   context check and Target name check
3. Write tests for both attributes
4. Decide during Phase 3 whether to add the "must be in static class" check or rely on the
   compiler

---

## Open questions / deferred decisions

- **Static class check on `[SourceTemplate]`**: The compiler already enforces extension
  methods must be in static classes. Decide during Phase 3 whether our diagnostic adds
  meaningful value or just duplicates the compiler error with extra noise.
- **Diagnostic code range**: `AAL10xx` for `SourceTemplates\`. Confirm when overall scheme
  is settled.
