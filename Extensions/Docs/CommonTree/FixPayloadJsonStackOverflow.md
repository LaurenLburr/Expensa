# Fix PayloadJson Stack Overflow

`AddinTreePayloadBase.PayloadJson` previously serialized `this`.

That caused recursion:

```text
PayloadJson serializes object
serializer sees PayloadJson
PayloadJson serializes object
serializer sees PayloadJson
...
```

The fix:

```csharp
[JsonIgnore]
public virtual string PayloadJson => JsonSerializer.Serialize(ToPayloadSnapshot(), ...);
```

`ToPayloadSnapshot()` returns only safe payload fields and avoids serializing `PayloadJson` itself.

Derived payloads can override `ToPayloadSnapshot()` later if they need richer detail output.
