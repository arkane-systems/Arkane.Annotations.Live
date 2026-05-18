# Phase 2 Plan: `AspNet\`

## Attributes in scope

| File | Attribute | Category |
|------|-----------|----------|
| `AspNetAttributes.cs` | `[AspChildControlType]` | **Annotation-only** |
| `AspNetAttributes.cs` | `[AspDataField]` | **Annotation-only** |
| `AspNetAttributes.cs` | `[AspDataFields]` | **Annotation-only** |
| `AspNetAttributes.cs` | `[AspMethodProperty]` | **Annotation-only** |
| `AspNetAttributes.cs` | `[AspRequiredAttribute]` | **Annotation-only** |
| `AspNetAttributes.cs` | `[AspTypeProperty]` | **Annotation-only** |

No overlap with `Metalama.Patterns.Contracts`. No Phase 3 implementation work required.

---

## Rationale: all annotation-only

All six attributes are Web Forms / ASP.NET designer-time IDE hints from the legacy ASP.NET
Web Forms era. They inform the ReSharper/Rider designer integration about the structure of
server controls — child control types, data-bindable fields, required HTML attributes, and
type-resolution properties. Their semantics are:

- Consumed exclusively by the IDE's ASP.NET designer/code-analysis subsystem
- Specific to Web Forms server-control authoring patterns that are not present in modern
  ASP.NET Core
- Entirely declarative: they annotate the *shape* of a control for the designer, with no
  runtime or compile-time structural contract

There are no meaningful structural checks to add:
- `[AspChildControlType]` maps a tag name string to a `Type` — a type-existence check at
  compile time is conceivable but the mapping is validated by the ASP.NET runtime/designer,
  not by the annotation
- `[AspRequiredAttribute]` names an HTML attribute string — no compile-time resolution is
  possible for arbitrary HTML attribute names
- The remaining attributes are pure markers with no parameters to validate

Given that Web Forms is out of mainstream use and these attributes are provided for
backwards compatibility only, adding any Metalama logic here offers negligible value.

**Implementation:** Keep all six as plain attributes. No Metalama logic.

**Test cases:**
- Verify each attribute compiles and can be applied to its declared target types

---

## Implementation sequence for Phase 3

No implementation work required. All attributes remain as-is from Phase 1.
