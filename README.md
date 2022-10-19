# JsonObjectValidator

This allows validation of JSON objects by comparing with an anonymous object.

## How to use

### Partial Comparisons

Only the properties in the object that's passed in are compared.
```csharp
"{ \"TestedProperty\": \"TestedValue\", \"UntestedProperty\": 5 }"
    .JsonShouldLookLike(new
{
    TestedProperty = "TestedValue"
});
```

### Compare arrays

Arrays can be compared too. The individual values will be compared in order.
```csharp
// Arrays on their own
"[ 10, 5, 6 ]".JsonShouldLookLike(new[] { 10, 5, 6 });

// Arrays of objects
"[ { \"TestProperty\": 1 }, { \"TestProperty\": 2 } ]".JsonShouldLookLike(new[]
{
    new
    {
        TestProperty = 1
    },
    new
    {
        TestProperty = 2
    }
});
```

### Custom Comparisons

For values that needs custom matching, an `Expectation` can be created. This is useful in situations when the exact value is not known such as comparing timestamps.

```csharp
"{ \"TestProperty\": 15, \"ExpectedProperty\": 5 }"
    .JsonShouldLookLike(new
{
    TestProperty = 15,
    ExpectedProperty = JsonMatcher.Expect<int>(val => val is > 4 and < 15)
});
```

For more examples, see the tests.

### Custom Deserialization

`JsonSerializerOptions` can be passed in to verify custom JSON. For an example see the GraphQL response data below.
```csharp
// Custom JsonSerializerOptions
"{ \"testProperty\": 15 }"
    .JsonShouldLookLike(new
{
    TestProperty = 15,
}, new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    });
    
    
// Verify JsonNode
JsonSerializer
  .Deserialize<JsonNode>("{ \"InnerObject\": { \"TestProperty\": 15 } }")!
  ["InnerObject"]
  .JsonShouldLookLike(new
  {
      TestProperty = 15,
  });
```


## Known Limitations

The following scenarios aren't supported at the moment.

- Verifying lists (can use arrays instead)
- Verifying against concrete classes 
  - Mostly out of scope here but if the equality comparison operators are implemented it should work
  - Records and structs should work as well
  - `IEquatable`, `IComparable` interfaces aren't supported
- Verifying lists of different types
  - It should be possible to create an `Expectation` that expects a `JsonNode` or a `JsonElement` and do custom deserialization but it'll be non-trivial

## TODO

- Setup as a nuget package
- Push to Nuget.org
- Add releases
- Setup github CI

## How it works

This was inspired by the wonderful [ExpectedObject](https://github.com/derekgreer/expectedObjects) and [SpecsFor](https://github.com/MattHoneycutt/SpecsFor) projects.

It uses reflection to get the properties of the expected anonymous object and to get individual values of arrays. Then it deserializes the actual object to expected type specified in the expected object and compared against it.
