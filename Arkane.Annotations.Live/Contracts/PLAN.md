# Phase 2 Plan: `Contracts\`

## Attributes in scope

| File | Attribute | Category |
|------|-----------|----------|
| `ContractAnnotationAttribute.cs` | `[ContractAnnotation]` | **Compile-time validator** (FDT syntax check) |
| `InvokerParameterNameAttribute.cs` | `[InvokerParameterName]` | **Annotation-only** |
| `NotifyPropertyChangedInvocatorAttribute.cs` | `[NotifyPropertyChangedInvocator]` | **Compile-time validator** |

No overlap with `Metalama.Patterns.Contracts` — that library has no FDT, invoker, or INPC
concepts.

---

## Attribute-by-attribute plan

### `[ContractAnnotation]` — Compile-time validator (FDT syntax check)

**Semantics:** Encodes a Function Definition Table (FDT) describing the relationship between
inputs and outputs of the annotated method. Used by ReSharper/Rider's data flow analysis to
propagate nullability and termination information through call sites.

**Why not a runtime aspect:**

The FDT language is a *static analysis annotation*, not a runtime contract. Each FDT row
describes what the IDE should *assume* about the method; it does not describe code that should
be *executed*. For example:

- `"=> halt"` — the method never returns; the IDE treats call sites as unreachable. There is
  no runtime enforcement needed: if the method doesn't throw, the caller's code continues, which
  is correct runtime behaviour.
- `"s:null => true"` — if `s` is null, the return is `true`. Enforcing this at runtime would
  mean throwing if the developer violates their own documented contract, which is backwards.
- `"null => null; notnull => notnull"` — a nullability passthrough. No runtime check applies.

A full FDT interpreter at runtime is both out of scope and conceptually wrong for these
semantics. The only meaningful live value is **catching invalid FDT strings at compile time**
rather than silently accepting malformed contracts.

**Implementation approach:**

Implement `ContractAnnotationAttribute` as an `IAspect<IMethod>` with `BuildAspect` performing
compile-time-only validation of the `Contract` string:

1. Parse the FDT string against the formal grammar documented in the attribute's XML doc:
   - Rows separated by `;`
   - Each row: `Input => Output` or `Output <= Input`
   - `Input` / `Output` tokens: `ParameterName: Value` pairs, where `Value` ∈
	 `{true, false, null, notnull, canbenull}` and output-only additionally allows
	 `{halt, stop, void, nothing}`
2. If the string fails to parse: report a compile-time **error** (`AAL04xx`: *"[ContractAnnotation]
   contract string '{0}' is syntactically invalid."*)
3. If a named parameter in the FDT does not match any parameter on the method: report a
   compile-time **warning** (`AAL04xx`: *"Parameter '{0}' referenced in contract does not
   exist on '{1}'."*) — analogous to `[StringFormatMethod]` parameter-name checking.

No runtime code is injected.

**FDT parser scope:** The parser needs to handle only the grammar above — it does not need to
evaluate or reason about the semantics of the FDT rows, only their syntax and parameter name
references. A simple tokenizer/recursive-descent parser implemented as a `[CompileTime]`
helper class is appropriate.

**Test cases:**
- `[ContractAnnotation("=> halt")]` → no diagnostic
- `[ContractAnnotation("null => null; notnull => notnull")]` → no diagnostic
- `[ContractAnnotation("s:null => true")]` on a method with parameter `s` → no diagnostic
- `[ContractAnnotation("s:null => true")]` on a method with no parameter `s` → `AAL04xx`
  warning
- `[ContractAnnotation("=> xyzzy")]` (invalid value token) → `AAL04xx` error
- `[ContractAnnotation("")]` (empty string) → `AAL04xx` error
- `[ContractAnnotation("null => null; notnull => notnull; canbenull => canbenull")]`
  (multiple rows) → no diagnostic
- Multiple `[ContractAnnotation]` on same method (AllowMultiple = true) → each validated
  independently

---

### `[InvokerParameterName]` — Annotation-only

**Semantics:** Marks a `string` parameter that is expected to receive a string literal matching
the name of one of the *caller's* parameters — the canonical example being the first argument
to `new ArgumentNullException("paramName")`.

**Rationale for annotation-only:**

The IDE validation (ReSharper/Rider warns when the passed literal doesn't match any caller
parameter) is purely a call-site static analysis concern. There is no viable compile-time
Metalama equivalent, because Metalama aspects run at the *declaration* site, not the *call
site* — we cannot inspect the caller's parameter list from the decorated parameter's aspect.

The modern .NET equivalent is `[CallerArgumentExpression]` (C# 10+), which captures the
expression text of the passed argument at compile time. However, silently redirecting this
attribute's users to `[CallerArgumentExpression]` would be a breaking semantic change —
`[CallerArgumentExpression]` fills in the expression automatically; `[InvokerParameterName]`
documents that the caller *should* pass a name manually. These are different contracts.

**Implementation:** Keep as a plain attribute. No Metalama logic.

**Note for documentation:** Add an XML doc `<remarks>` note pointing users to
`[CallerArgumentExpression]` as the modern built-in alternative when automatic capture
(rather than documentation) is what they need.

**Test cases:**
- Verify the attribute compiles and can be applied to a `string` parameter
- No runtime behavior to test

---

### `[NotifyPropertyChangedInvocator]` — Compile-time validator

**Semantics:** Marks the method within an `INotifyPropertyChanged`-implementing type that is
responsible for raising `PropertyChanged`. The optional `ParameterName` argument names the
method parameter that receives the property name string. The IDE uses this to:
- Warn when the caller passes a property name that doesn't match the enclosing property setter
- Offer to generate `NotifyChanged(nameof(Property))` automatically

**Live behavior to add:**

Four compile-time validators:

1. **Type check** (`AAL05xx`): Verify that the declaring type of the annotated method
   implements `System.ComponentModel.INotifyPropertyChanged`. If not: compile-time **error**
   (*"[NotifyPropertyChangedInvocator] can only be applied to methods in types that implement
   INotifyPropertyChanged."*).

2. **Static check** (`AAL05xx`): Verify the method is non-static. If static: compile-time
   **error** (*"[NotifyPropertyChangedInvocator] cannot be applied to a static method; the
   method must be an instance method to raise instance events."*).

3. **Signature check** (`AAL05xx`): Verify the method conforms to one of the five supported
   signatures documented by JetBrains. If not: compile-time **warning** (*"Method '{0}' does
   not match any recognised [NotifyPropertyChangedInvocator] signature; the IDE may not use
   this annotation."*). Warning rather than error because a non-matching method is not
   necessarily broken at runtime — it simply won't benefit from IDE integration.

   The five recognised signatures are:
   - `NotifyChanged(string)` — single `string` parameter
   - `NotifyChanged(params string[])` — single `params string[]` parameter
   - `NotifyChanged<T>(Expression<Func<T>>)` — one generic type parameter, one
     `Expression<Func<T>>` parameter
   - `NotifyChanged<T,U>(Expression<Func<T,U>>)` — two generic type parameters, one
     `Expression<Func<T,U>>` parameter
   - `SetProperty<T>(ref T, T, string)` — one generic type parameter, three parameters:
     `ref T`, `T`, `string`

   Matching is on *shape* (parameter count, types, generic arity, `ref`-ness), not on method
   name. The `Expression<Func<T>>` and `Expression<Func<T,U>>` variants require careful
   open-generic type matching via Metalama's compile-time type APIs.

4. **Parameter name check** (`AAL05xx`): If `ParameterName` is specified (non-null), verify
   that a parameter of that name exists on the method. If not: compile-time **warning**
   (*"No parameter named '{0}' exists on '{1}'."*) — same pattern as `[StringFormatMethod]`.

No runtime code is injected: the method body is responsible for raising the event; we only
validate its declaration context is correct.

**Implementation approach:**

Implement `NotifyPropertyChangedInvocatorAttribute` as an `IAspect<IMethod>`. In `BuildAspect`:
1. Resolve `INotifyPropertyChanged` via `ICompilation.GetType(typeof(INotifyPropertyChanged))`
2. Check `method.DeclaringType.ImplementedInterfaces` includes it
3. Check `method.IsStatic` is false
4. Check `method.Parameters` shape against the five signature patterns
5. If `ParameterName != null`, check `method.Parameters` contains a match

**Test cases:**
- Applied to an instance method on a class that implements `INotifyPropertyChanged`, matching
  signature `(string)`, no `ParameterName` → no diagnostic
- Applied to a method on a class that does *not* implement `INotifyPropertyChanged` → `AAL05xx`
  error
- Applied to a `static` method → `AAL05xx` error
- Applied to a method with signature `(string propertyName)` → no signature warning
- Applied to a method with signature `(params string[] names)` → no signature warning
- Applied to a method with signature `<T>(Expression<Func<T>> expr)` → no signature warning
- Applied to a method with signature `<T,U>(Expression<Func<T,U>> expr)` → no signature
  warning
- Applied to a method with signature `<T>(ref T field, T value, string name)` → no signature
  warning
- Applied to a method with signature `(int value)` (no match) → `AAL05xx` warning
- Applied with `ParameterName = "propertyName"` where that parameter exists → no diagnostic
- Applied with `ParameterName = "propName"` where no such parameter exists → `AAL05xx` warning
- Applied with no `ParameterName` → no parameter-name diagnostic (null is allowed per
  `[CanBeNull]` on the property)

---

## Implementation sequence for Phase 3

1. Implement FDT parser (`[CompileTime]` helper) and `[ContractAnnotation]` compile-time
   validator; define `AAL04xx` diagnostics
2. Write tests for `[ContractAnnotation]` FDT validation
3. Implement `[NotifyPropertyChangedInvocator]` compile-time validator with four checks
   (INPC type, non-static, signature shape, parameter name); define `AAL05xx` diagnostics;
   implement `[CompileTime]` signature-matching helper for the five recognised patterns
4. Write tests for `[NotifyPropertyChangedInvocator]`
5. `[InvokerParameterName]` requires no changes

---

## Open questions / deferred decisions

- **`[ContractAnnotation]` FDT parser complexity**: The grammar is well-defined but a
  hand-written parser is non-trivial. Consider whether a regex-based tokenizer suffices for
  the relatively small token set, or whether a proper recursive-descent parser is needed.
  Decide during Phase 3 implementation — start simple, escalate if edge cases require it.
- **Diagnostic code ranges**: `AAL04xx` for `Contracts\`, `AAL05xx` for
  `NotifyPropertyChangedInvocator`. Confirm when the overall `AAL` numbering scheme is settled.
- **`[InvokerParameterName]` XML doc update**: Add a `<remarks>` note mentioning
  `[CallerArgumentExpression]` as the modern built-in alternative. This is a doc-only change,
  no Metalama work.
