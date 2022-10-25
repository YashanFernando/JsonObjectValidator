# JsonObjectValidator

[![CI](https://github.com/YashanFernando/JsonObjectValidator/actions/workflows/build.yml/badge.svg)](https://github.com/YashanFernando/JsonObjectValidator/actions/workflows/build.yml)
[![Nuget](https://img.shields.io/nuget/v/JsonObjectValidator)](https://www.nuget.org/packages/JsonObjectValidator)

This allows validation of JSON objects by comparing with an anonymous object.


## Why

The main goal of this library is to allow comparing a JSON string against an object graph built using anonymous objects. While it's easy to deserialize a JSON string to an anonymous object and compare the two objects, that doesn't allow us to perform any custom comparisons. Custom comparisons allow us to perform comparisons such as,
- Approximate values (Such as verifying a timestamp is within a known range)
- Advanced list comparisons (Verify a list length while ignoring the contents)

For an example,

```csharp
@"
{ 
    ""IpAddress"": ""192.168.0.3"",
    ""WebsiteName"": ""TestSite"",
    ""Citations"": null,
    ""Articles"": 
        [ 
            { 
                ""Author"": null, 
                ""Timestamp"": ""2012-04-23T18:25:43.511Z"",
                ""ViewCount"": 5
            }, 
            { 
                ""Author"": ""Jane Doe"", 
                ""Timestamp"": ""2015-04-23T18:25:43.511Z"",
                ""ViewCount"": 3
            }
        ] 
}"
.JsonShouldLookLike(new
{
    // Ignore the IP address field by not including it here

    // Compare the values directly
    WebsiteName = "TestSite",

    // Verify the field exists and it's null
    Citations = JsonMatcher.ExpectNull(),

    // THIS IS KEY.. We use use a generic list of objects to allow list items of different types
    Articles = new object[]
    {
        new
        {
            Author = JsonMatcher.ExpectNull(),

            // Custom validation
            Timestamp = JsonMatcher.Expect<DateTime>(d => d.Year == 2012),

            ViewCount = 5
        },
        new
        {
            Author = "Jane Doe",

            // Verify field exists and it contains a DateTime
            Timestamp = JsonMatcher.ExpectAny<DateTime>(),

            ViewCount = 3
        }
    }
});

```


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

### Compare arrays/lists

Arrays can be compared too. The individual values will be compared in order.

If the list contains expectations, or when verifying a list of different types use `new object[]` to allow mixing and matching expectations and types.

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


// Lists of different types
"[ 1, \"Two\", [ 3 ] ]".JsonShouldLookLike(new object[]
{
    1, "Two", new[] { 3 }
});

```

### Custom Comparisons

For values that needs custom matching, an `Expectation` can be created. This is useful in situations when the exact value is not known such as comparing timestamps.

```csharp
// Expect a value with custom behavior
"{ \"TestProperty\": 15, \"ExpectedProperty\": 5 }"
    .JsonShouldLookLike(new
{
    TestProperty = 15,
    ExpectedProperty = JsonMatcher.Expect<int>(val => val is > 4 and < 15)
});

// Expect any value of a given type
"{ \"ExpectedProperty\": 5 }"
    .JsonShouldLookLike(new
{
    ExpectedProperty = JsonMatcher.ExpectAny<int>()
});


// Expect the value to be null
"{ \"ExpectedProperty\": null }"
    .JsonShouldLookLike(new
{
    ExpectedProperty = JsonMatcher.ExpectNull()
});
```

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
```

### More examples
For more examples, [see the tests](/src/Validator.Tests).


## Known Limitations

The following scenarios aren't supported at the moment.

- Verifying against concrete classes 
  - Mostly out of scope here but if the equality comparison operators are implemented it should work
  - Records and structs should work as well
  - `IEquatable`, `IComparable` interfaces aren't supported


## How it works

This was inspired by the wonderful [ExpectedObject](https://github.com/derekgreer/expectedObjects) and [SpecsFor](https://github.com/MattHoneycutt/SpecsFor) projects.

It uses reflection to get the properties of the expected anonymous object and to get individual values of an enumerable. Then it deserializes the actual object to type specified in the expected object and compares against it.