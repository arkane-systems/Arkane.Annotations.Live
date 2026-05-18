# Phase 2 Plan: AspNetRouting

## Conclusion: Annotation-Only

All 13 attributes in this folder are annotation-only. No Metalama work is planned.

## Rationale

These attributes are IDE hints for ASP.NET routing features — route-template syntax
highlighting, URI string completion and navigation, and Minimal API endpoint discovery.
Enforcing them meaningfully at the C# declaration site would require:

- Full routing-table knowledge (conventional routing, attribute routing, Minimal API maps)
- Cross-project project-model analysis to resolve controller/action names and route values
- Understanding of the application's `Startup`/`WebApplication` configuration at compile time

None of this is available to a Metalama aspect operating at the declaration site, and
attempting partial structural checks (e.g. validating that a `[RouteParameterConstraint]`
class implements `IRouteConstraint`) would require a hard dependency on ASP.NET framework
assemblies that this library deliberately avoids.

## Attribute Inventory

| Attribute | Status | Notes |
|---|---|---|
| `RouteTemplateAttribute` | Annotation-only | IDE route-template syntax hint |
| `RouteParameterConstraintAttribute` | Annotation-only | Could validate `IRouteConstraint` but requires ASP.NET reference |
| `UriStringAttribute` | Annotation-only | IDE URI string completion hint |
| `AspRouteConventionAttribute` | Annotation-only | IDE routing-convention discovery hint |
| `AspDefaultRouteValuesAttribute` | Annotation-only | IDE parameter-role hint |
| `AspRouteValuesConstraintsAttribute` | Annotation-only | IDE parameter-role hint |
| `AspRouteOrderAttribute` | Annotation-only | IDE routing-order hint |
| `AspRouteVerbsAttribute` | Annotation-only | IDE HTTP-verb hint |
| `AspAttributeRoutingAttribute` | Annotation-only | IDE attribute-routing discovery hint |
| `AspMinimalApiDeclarationAttribute` | Annotation-only | IDE Minimal API endpoint discovery hint |
| `AspMinimalApiGroupAttribute` | Annotation-only | IDE Minimal API group discovery hint |
| `AspMinimalApiHandlerAttribute` | Annotation-only | IDE Minimal API handler hint |
| `AspMinimalApiImplicitEndpointDeclarationAttribute` | Annotation-only | IDE implicit endpoint declaration hint |
