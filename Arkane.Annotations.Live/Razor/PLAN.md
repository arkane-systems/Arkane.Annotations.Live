# Phase 2 Plan: Razor

## Conclusion: Annotation-Only

All 12 attributes in this folder are annotation-only. No Metalama work is planned.

## Rationale

These attributes are consumed by the Razor compiler and IDE tooling — they declare
namespace imports, service injections, directive names, page base types, and the
identity of Razor helper/write/layout methods. They operate at the Razor compilation
layer, not the C# declaration layer that Metalama aspects can observe. There is no
meaningful structural constraint a Metalama aspect could enforce here without replicating
the Razor compiler's own project-model analysis.

## Attribute Inventory

| Attribute | Status | Notes |
|---|---|---|
| `HtmlElementAttributesAttribute` | Annotation-only | IDE HTML attribute-dictionary hint |
| `HtmlAttributeValueAttribute` | Annotation-only | IDE HTML attribute-value completion hint |
| `RazorSectionAttribute` | Annotation-only | Razor section marker for IDE |
| `RazorImportNamespaceAttribute` | Annotation-only | Assembly-level Razor namespace import hint |
| `RazorInjectionAttribute` | Annotation-only | Assembly-level Razor service-injection hint |
| `RazorDirectiveAttribute` | Annotation-only | Assembly-level Razor directive hint |
| `RazorPageBaseTypeAttribute` | Annotation-only | Assembly-level Razor base-type hint |
| `RazorHelperCommonAttribute` | Annotation-only | Marks method as a Razor common helper |
| `RazorLayoutAttribute` | Annotation-only | Marks property as the Razor layout property |
| `RazorWriteLiteralMethodAttribute` | Annotation-only | Marks literal-write method for Razor tooling |
| `RazorWriteMethodAttribute` | Annotation-only | Marks write method for Razor tooling |
| `RazorWriteMethodParameterAttribute` | Annotation-only | Marks write-method parameter for Razor tooling |
