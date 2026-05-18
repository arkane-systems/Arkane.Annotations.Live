# Phase 2 Plan: `IdeTools\`

## Attributes in scope

| File | Attribute | Category |
|------|-----------|----------|
| `IdeToolAttributes.cs` | `[PathReference]` | **Annotation-only** |
| `IdeToolAttributes.cs` | `[CodeTemplate]` | **Annotation-only** |
| `IdeToolAttributes.cs` | `[DebuggerGlobalWatch]` | **Annotation-only** |

No overlap with `Metalama.Patterns.Contracts`. No Phase 3 implementation work required.

---

## Rationale: all annotation-only

All three attributes are IDE tooling configuration — they instruct specific IDE subsystems
how to treat the annotated element, with no runtime semantics and no structural constraints
that could usefully be validated at compile time.

**`[PathReference]`**: Activates web-project path resolution and completion for string
values at the annotated site. Entirely an IDE editor feature. A `string` type check analogous
to `[RegexPattern]` is conceivable, but `[PathReference]` is also used with a `basePath`
constructor parameter (itself already `[PathReference]`-annotated), so the boundary is
self-referential and a type check adds no practical value.

**`[CodeTemplate]`**: Registers a Structural Search and Replace (SSR) pattern with the IDE.
The `SearchTemplate` and optional `ReplaceTemplate` are SSR syntax strings consumed
exclusively by ReSharper/Rider's SSR engine — there is no compiler or runtime equivalent.
A compile-time SSR syntax validator would require embedding the full SSR parser, which is
JetBrains-proprietary. Not feasible.

**`[DebuggerGlobalWatch]`**: Registers a static property as a permanent entry in the
debugger's Watches window. Note that this attribute retains its `[Conditional("DEBUG")]`
marker (the one attribute in the project that does, by design — it should genuinely only
be present in debug builds). It is a debugger host configuration hint with no compile-time
or runtime contract to enforce.

A compile-time check that `[DebuggerGlobalWatch]` is applied only to `static` properties
was considered, since non-static properties cannot meaningfully appear in a global watch
(there is no instance context). However, this is already enforced implicitly: the IDE simply
ignores non-static applications. The marginal value of a compile-time warning does not
justify the implementation work.

**Implementation:** Keep all three as plain attributes. No Metalama logic.

**Test cases:**
- Verify each attribute compiles and can be applied to its declared target types

---

## Implementation sequence for Phase 3

No implementation work required. All attributes remain as-is from Phase 1.
