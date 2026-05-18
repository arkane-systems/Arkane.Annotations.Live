# Diagnostic Code Registry

All compile-time diagnostics emitted by Arkane.Annotations.Live aspects use the prefix `AAL`
followed by a four-digit code. Codes are grouped by subfolder so errors are easy to trace back
to their source.

## Code Ranges

| Range | Subfolder | Attribute(s) |
|---|---|---|
| `AAL01xx` | `Formatting\` | `[StringFormatMethod]` |
| `AAL02xx` | `Nullability\` | `[NotNull]` (redundancy / eligibility warnings) |
| `AAL03xx` | `ValueConstraints\` | `[NonNegativeValue]`, `[ValueRange]` |
| `AAL04xx` | `Contracts\` | `[ContractAnnotation]` |
| `AAL05xx` | `Contracts\` | `[NotifyPropertyChangedInvocator]` |
| `AAL06xx` | `CodeQuality\` | `[BaseTypeRequired]` |
| `AAL07xx` | `MethodBehavior\` | `[Pure]`, `[MustUseReturnValue]`, `[MustDisposeResource]` |
| `AAL08xx` | `Assertions\` | `[AssertionMethod]`, `[AssertionCondition]` |
| `AAL09xx` | `Strings\` | `[RegexPattern]` |
| `AAL10xx` | `SourceTemplates\` | `[SourceTemplate]`, `[Macro]` |
| `AAL11xx` | `Cqrs\` | `[CqrsCommand]`, `[CqrsQuery]` |

## Individual Codes

Codes within each range are assigned during Phase 3 implementation and recorded here as each
attribute is implemented. The table below will be populated incrementally.

| Code | Severity | Subfolder | Attribute | Message |
|---|---|---|---|---|
| `AAL0101` | Error | `Formatting\` | `[StringFormatMethod]` | No parameter named '{0}' exists on '{1}'. |
| `AAL0102` | Warning | `Formatting\` | `[StringFormatMethod]` | Parameter '{0}' on '{1}' is not of type string; [StringFormatMethod] may not work as expected. |
| `AAL0201` | Warning | `Nullability\` | `[CanBeNull]` | [CanBeNull] is redundant on a nullable type; the NRT annotation already communicates nullability. |
| `AAL0202` | Warning | `Nullability\` | `[NotNull]` | [NotNull] is redundant on a non-nullable type in an [AssumesNrtCallers] assembly; the NRT annotation already enforces non-nullability for aware callers. |
| `AAL0203` | Error | `Nullability\` | `[AssumesNrtCallers]` + `[EnforceNullabilityForPreNrtCallers]` | [AssumesNrtCallers] and [EnforceNullabilityForPreNrtCallers] are mutually exclusive and cannot both be applied to the same assembly. |

## Conventions

- **Errors** (`DiagnosticSeverity.Error`): The annotated declaration is structurally invalid
  and the aspect cannot work correctly. The consuming code will not behave as documented.
- **Warnings** (`DiagnosticSeverity.Warning`): The annotated declaration is unusual or
  contradictory but not necessarily broken. The aspect may still operate partially.
- **Code assignment rule**: Within each `AALxx` range, assign codes sequentially starting
  from `xx01`. Reserve `xx00` as unused (avoids ambiguity with the range identifier itself).
- **No runtime codes**: All `AAL` codes are compile-time only. Runtime violations (from full
  aspects such as `[NotNull]`, `[NonNegativeValue]`, `[ValueRange]`) throw standard .NET
  exceptions (`ArgumentNullException`, `ArgumentOutOfRangeException`,
  `PostconditionViolationException`) and are not assigned `AAL` codes.
