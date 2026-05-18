# Phase 2 Plan: `AspNetMvc\`

## Attributes in scope

Twenty attributes across two categories:

**View location format attributes** (8, all structurally identical):
`[AspMvcAreaMasterLocationFormat]`, `[AspMvcAreaPartialViewLocationFormat]`,
`[AspMvcAreaViewComponentViewLocationFormat]`, `[AspMvcAreaViewLocationFormat]`,
`[AspMvcMasterLocationFormat]`, `[AspMvcPartialViewLocationFormat]`,
`[AspMvcViewComponentViewLocationFormat]`, `[AspMvcViewLocationFormat]`

**MVC element marker attributes** (12):
`[AspMvcAction]`, `[AspMvcArea]`, `[AspMvcController]`, `[AspMvcMaster]`,
`[AspMvcModelType]`, `[AspMvcPartialView]`, `[AspMvcSuppressViewError]`,
`[AspMvcDisplayTemplate]`, `[AspMvcEditorTemplate]`, `[AspMvcTemplate]`,
`[AspMvcView]`, `[AspMvcViewComponent]`, `[AspMvcViewComponentView]`,
`[AspMvcActionSelector]`

No overlap with `Metalama.Patterns.Contracts`. No Phase 3 implementation work required.

---

## Rationale: all annotation-only

All twenty attributes are MVC/ASP.NET Core IDE integration hints for ReSharper/Rider's
MVC-aware code analysis. They fall into two groups:

**View location format attributes**: Register custom view search paths with the IDE's
MVC view resolution engine. The `Format` strings use `{0}` placeholders for area/controller
names — a format string validation check would be conceivable, but the placeholder schema is
MVC-engine-specific and would require knowledge of which placeholders are valid in each
context. The marginal value is low; the IDE validates these at design time.

**MVC element marker attributes**: Tell the IDE that a `string` parameter, field, or
property holds the name of an MVC action, controller, view, area, view component, etc.
The IDE uses these to provide navigation, rename refactoring, and "cannot resolve" warnings.
All of the meaningful enforcement — checking that the named action/controller/view actually
exists — requires the IDE to walk the MVC routing table and the project's controller/view
file structure. This is a project-model analysis, not a compile-time structural constraint.

The same conclusion applies as for `AspNet\`: these are designed for a specific framework
integration and their enforcement is entirely the IDE's responsibility.

**Implementation:** Keep all twenty as plain attributes. No Metalama logic.

**Test cases:**
- Verify each attribute compiles and can be applied to its declared target types

---

## Implementation sequence for Phase 3

No implementation work required. All attributes remain as-is from Phase 1.
