# Phase 2 Plan: `Strings\`

## Attributes in scope

| File | Attribute | Category |
|------|-----------|----------|
| `StringAnnotationAttributes.cs` | `[RegexPattern]` | **Compile-time validator** |
| `StringAnnotationAttributes.cs` | `[IgnoreSpellingAndGrammarErrors]` | **Annotation-only** |
| `StringAnnotationAttributes.cs` | `[LanguageInjection]` | **Annotation-only** |
| `StringAnnotationAttributes.cs` | `InjectedLanguage` (enum) | **No change** |

No overlap with `Metalama.Patterns.Contracts`.

---

## Attribute-by-attribute plan

### `[RegexPattern]` — Compile-time validator

**Semantics:** The marked `string` parameter, field, or property holds a regular expression
pattern. The IDE uses this to provide regex syntax highlighting, validation, and completion
at string literal assignment sites.

**Live behavior to add — compile-time validator:**

1. **Target type check** (`AAL09xx` error): The annotated member must be of type `string`
   (or `string?`). Applying `[RegexPattern]` to a non-string field, property, or parameter
   is a clear mistake. Error: *"[RegexPattern] can only be applied to string members; '{0}'
   is of type '{1}'."*

**Optional — literal value validation** (defer to Phase 3 implementation decision):

When a field or property is initialized with a string literal, or a parameter has a default
value that is a string literal, it is *theoretically* possible to validate the literal against
`Regex` at compile time. However:
- Fields and properties are commonly assigned at runtime, making compile-time validation
  of the initial value a partial check at best
- Parameters rarely have regex default values
- This would require `[CompileTime]` access to constant values, which Metalama supports but
  adds complexity

**Decision: start with the type check only; defer literal validation.**

**Test cases:**
- `[RegexPattern]` on a `string` parameter → no diagnostic
- `[RegexPattern]` on a `string` field → no diagnostic
- `[RegexPattern]` on a `string` property → no diagnostic
- `[RegexPattern]` on an `int` parameter → `AAL09xx` error
- `[RegexPattern]` on a `Regex` parameter → `AAL09xx` error (the attribute is for the
  *string* representation, not an already-compiled `Regex` object)

---

### `[IgnoreSpellingAndGrammarErrors]` — Annotation-only

**Semantics:** Suppresses spell/grammar inspection for the string literal passed to this
parameter — useful for identifiers, codes, and technical strings that are not natural
language. Pure IDE inspection configuration; no runtime or structural enforcement possible.

**Implementation:** Keep as a plain attribute. No Metalama logic.

---

### `[LanguageInjection]` — Annotation-only

**Semantics:** Declares that string literals assigned to the marked parameter, field,
property, or return value contain embedded code in a specified language (CSS, HTML, JS,
JSON, XML, or an arbitrary named language). The IDE activates language injection — syntax
highlighting, completion, and validation — for those string literals.

**Rationale for annotation-only:** Language injection is entirely an IDE editor feature.
There is no runtime behavior to inject, and no structural constraint we could usefully
validate.

The one tempting compile-time check — *"verify the target is of type `string`"* — is the
same check as `[RegexPattern]`, and is equally valid here. However, `[LanguageInjection]`
is also legitimately used on `StringBuilder`, interpolated string handlers, or custom
string-like types in some scenarios. Given the broader applicability, this check would
produce more false positives than `[RegexPattern]`, and the marginal value is lower.
**Defer this check; keep annotation-only for now.**

**Implementation:** Keep as a plain attribute and enum. No Metalama logic.

---

## Implementation sequence for Phase 3

1. Implement `[RegexPattern]` as `IAspect<IFieldOrPropertyOrIndexer>` and
   `IAspect<IParameter>` with target-type string check; define `AAL09xx` diagnostics
2. Write tests for `[RegexPattern]`
3. All other attributes require no changes

---

## Open questions / deferred decisions

- **`[RegexPattern]` literal validation**: Assess during Phase 3 whether Metalama's
  compile-time constant-value API makes it easy to validate string literal initial values
  against `new Regex(value)`. If straightforward, add as a second `AAL09xx` warning.
- **`[LanguageInjection]` type check**: Revisit if `[RegexPattern]`'s type check proves
  unambiguously valuable in practice — the same check could be added to `[LanguageInjection]`
  with a more permissive target-type set.
- **Diagnostic code range**: `AAL09xx` for `Strings\`. Confirm when overall scheme is
  settled.
