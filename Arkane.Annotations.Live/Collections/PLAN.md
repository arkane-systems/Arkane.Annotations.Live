# Phase 2 Plan: `Collections\`

## Attributes in scope

| File | Attribute | Category |
|------|-----------|----------|
| `CollectionAccessAttribute.cs` | `[CollectionAccess]` | **Annotation-only** |
| `CollectionAccessAttribute.cs` | `CollectionAccessType` (enum) | **No change** |

No overlap with `Metalama.Patterns.Contracts`. No Phase 3 implementation work required.

---

## Rationale: annotation-only

`[CollectionAccess]` describes the *read/write effect* a method has on the collection it
belongs to or accepts. The IDE uses this information to perform **whole-program data-flow
analysis** across all annotated methods on a collection type, detecting patterns like:

- A collection that is only ever read from (contents never updated — possibly a logic error)
- A method that mutates content when the caller only expected a read
- A return value that is an exclusively-owned new collection vs. a shared reference

Every meaningful enforcement scenario requires the IDE to track collection state *across
multiple call sites* — correlating which methods read vs. write the same collection instance,
and whether all writes are reachable from all reads. This is the kind of inter-procedural
data-flow analysis that Roslyn analyzers and ReSharper's inspection engine are built for.

A Metalama `ContractAspect` can only see the single declaration it is applied to. It has no
access to the call graph or the set of other methods on the same type that carry
`[CollectionAccess]` annotations. There is no local structural check on the annotated method
alone that adds useful enforcement.

The one conceivable compile-time check would be: *"Is this attribute applied to a method on
a type that implements `IEnumerable<T>` or a collection interface?"* — but `[CollectionAccess]`
is deliberately applicable to any method that happens to work with a collection (including
extension methods and static factory methods), so this check would produce excessive false
positives. Not worth adding.

**Implementation:** Keep as a plain attribute and enum. No Metalama logic.

**Test cases:**
- Verify the attribute and enum compile
- Verify the attribute can be applied to `Method`, `Constructor`, `Property`, and
  `ReturnValue` targets with all `CollectionAccessType` flag values

---

## Implementation sequence for Phase 3

No implementation work required. All types remain as-is from Phase 1.
