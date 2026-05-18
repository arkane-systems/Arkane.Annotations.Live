# Phase 2 Plan: Testing

## Conclusion: Annotation-Only

Both attributes in this folder are annotation-only. No Metalama work is planned.

## Rationale

`TestSubjectAttribute` and `MeansTestSubjectAttribute` are IDE/test-runner navigation and
grouping hints. The compiler already guarantees the `Type` argument to `TestSubjectAttribute`
is valid (it is always supplied as a `typeof()` expression). `MeansTestSubjectAttribute` is
a meta-attribute applied to a generic parameter to mark it as meaning "the type under test";
this is consumed entirely by the IDE for test-to-type navigation.

There is no structural contract a Metalama aspect can enforce at the C# declaration site
that adds value beyond what the compiler and IDE already provide.

## Attribute Inventory

| Attribute | Status | Notes |
|---|---|---|
| `TestSubjectAttribute` | Annotation-only | IDE/test-runner subject navigation hint; `Type` arg is compiler-validated |
| `MeansTestSubjectAttribute` | Annotation-only | Meta-attribute marking a generic parameter as the test subject type |
