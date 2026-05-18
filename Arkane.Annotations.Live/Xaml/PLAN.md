# Phase 2 Plan: Xaml

## Conclusion: Annotation-Only

All 5 attributes in this folder are annotation-only. No Metalama work is planned.

## Rationale

These attributes are XAML tooling hints consumed by the WPF/MAUI designer and IDE for
`DataContext` type resolution in bindings and `DependencyProperty` binding-mode defaults.
Meaningful enforcement would require:

- A hard dependency on WPF or MAUI framework assemblies (deliberately avoided)
- Understanding of the visual tree and `ItemsControl` hierarchy at compile time
- XAML compiler integration beyond what is available to a C# Metalama aspect

The only structural check that could be considered without a framework reference is
validating that `XamlOneWayBindingModeByDefaultAttribute` and
`XamlTwoWayBindingModeByDefaultAttribute` are not applied simultaneously to the same
member. This is a trivial edge case not worth the implementation cost; the IDE already
highlights the redundancy.

## Attribute Inventory

| Attribute | Status | Notes |
|---|---|---|
| `XamlItemsControlAttribute` | Annotation-only | IDE `DataContext` resolution hint for `ItemsControl`-like types |
| `XamlItemBindingOfItemsControlAttribute` | Annotation-only | IDE binding `DataContext` resolution hint |
| `XamlItemStyleOfItemsControlAttribute` | Annotation-only | IDE style `DataContext` resolution hint |
| `XamlOneWayBindingModeByDefaultAttribute` | Annotation-only | IDE `DependencyProperty` binding-mode hint |
| `XamlTwoWayBindingModeByDefaultAttribute` | Annotation-only | IDE `DependencyProperty` binding-mode hint |
