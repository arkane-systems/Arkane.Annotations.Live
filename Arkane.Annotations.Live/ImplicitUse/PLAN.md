# Phase 2 Plan: `ImplicitUse\`

## Attributes in scope

| File | Attribute | Category |
|------|-----------|----------|
| `ImplicitUseAttributes.cs` | `[UsedImplicitly]` | **Annotation-only** |
| `ImplicitUseAttributes.cs` | `[MeansImplicitUse]` | **Annotation-only** |
| `ImplicitUseAttributes.cs` | `[PublicAPI]` | **Annotation-only** |
| `ImplicitUseAttributes.cs` | `ImplicitUseKindFlags` (enum) | **No change** |
| `ImplicitUseAttributes.cs` | `ImplicitUseTargetFlags` (enum) | **No change** |

No overlap with `Metalama.Patterns.Contracts`. No Phase 3 implementation work required.

---

## Rationale: all annotation-only

Every attribute in this subfolder communicates intent to the IDE's *unused symbol analysis*
engine — they suppress, redirect, or cascade "unused" warnings. This is fundamentally an IDE
inspection concern:

- **`[UsedImplicitly]`**: Tells ReSharper/Rider "do not warn about this symbol being
  unreferenced, because it is consumed via reflection, a DI container, an ORM, or similar
  implicit mechanism." There is no runtime enforcement to add: the symbol is genuinely
  unreferenced in static analysis terms, and that is correct.

- **`[MeansImplicitUse]`**: A meta-attribute — when applied to another attribute class, it
  makes the decorated attribute behave like `[UsedImplicitly]` at all its application sites.
  Like `[BaseTypeRequired]`, this is a two-level IDE annotation. Unlike `[BaseTypeRequired]`,
  there is no structural constraint to validate: the decorated attribute simply gains a new
  semantic meaning in the IDE, and there is nothing to check or enforce.

- **`[PublicAPI]`**: Marks symbols as part of the public API surface that must not be removed
  regardless of apparent non-usage. Suppresses "unused" warnings. Again, purely an IDE hint.
  A compile-time check for whether the symbol is actually accessible (`public`/`protected`)
  was considered — `[PublicAPI]` on an `internal` or `private` member is arguably a
  contradiction. However, `[PublicAPI]` is also legitimately used on `internal`-for-now
  members that are intended to be promoted, so this would cause false positives. Rejected.

In all three cases, the enforcement mechanism is the IDE's inspection engine reading the
annotation — not generated code, not a runtime guard, and not a compile-time structural
check we can usefully add. **No Metalama logic in this subfolder.**

---

## Implementation sequence for Phase 3

No implementation work required. All attributes remain as-is from Phase 1.

---

## Notes

- The `[PublicAPI]` attribute itself uses `[MeansImplicitUse(ImplicitUseTargetFlags.WithMembers)]`
  in its declaration, which is correct and self-consistent. This self-referential use should
  be preserved as-is.
- The `[UsedImplicitly]` attribute's `Reason` property (a settable `string?`) is an
  undocumented but useful extension for recording *why* the symbol is used implicitly.
  No action required; preserve as-is.
