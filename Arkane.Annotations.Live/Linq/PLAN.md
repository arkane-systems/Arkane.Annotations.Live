# Phase 2 Plan: `Linq\`

## Attributes in scope

| File | Attribute | Category |
|------|-----------|----------|
| `LinqAttributes.cs` | `[LinqTunnel]` | **Annotation-only** |
| `LinqAttributes.cs` | `[NoEnumeration]` | **Annotation-only** |

No overlap with `Metalama.Patterns.Contracts`. No Phase 3 implementation work required.

---

## Rationale: both annotation-only

Both attributes exist to guide ReSharper/Rider's **multiple-enumeration** and
**`[InstantHandle]` propagation** analyses — inter-procedural data-flow inspections that
track `IEnumerable` lifetime across call chains.

**`[LinqTunnel]`**: Tells the IDE that this method is a lazy LINQ-style pipeline method
(like `Select` or `Where`) that passes enumeration through to the caller. The IDE uses this
to correctly propagate `[InstantHandle]` inference through chains of such methods. There is
no structural constraint to check at the declaration site, and no runtime behavior to inject —
the method body's laziness is a property of its implementation, not something we can assert or
enforce from an aspect.

**`[NoEnumeration]`**: Tells the IDE that the `IEnumerable` parameter is *not* enumerated
inside the method, suppressing false-positive "possible multiple enumeration" warnings at call
sites. Enforcing this at runtime — detecting whether enumeration occurred — would require
wrapping the passed enumerable in a tracking decorator and checking post-call, which would
add observable overhead and change the type of the argument in a way that could break
implementations. Annotation-only.

One superficially tempting compile-time check for `[NoEnumeration]` would be: *"verify the
parameter type is `IEnumerable` or compatible"*. However, the attribute's `AttributeUsage`
already restricts it to `Parameter`, and a parameter type check would frequently be wrong
(the attribute can apply to any parameter that happens to receive an enumerable, including
generic `T` parameters constrained to `IEnumerable<T>`). Not worth adding.

**Implementation:** Keep both as plain attributes. No Metalama logic.

**Test cases:**
- Verify `[LinqTunnel]` compiles and can be applied to a method
- Verify `[NoEnumeration]` compiles and can be applied to a parameter

---

## Implementation sequence for Phase 3

No implementation work required. Both attributes remain as-is from Phase 1.
