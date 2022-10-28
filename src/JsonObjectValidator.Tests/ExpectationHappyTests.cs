namespace JsonObjectValidator.Tests;

public class ExpectationHappyTests
{
    [Test]
    public void ExpectNull()
    {
        "{ \"TestProperty\": null }"
            .JsonShouldLookLike(new
        {
            TestProperty = JsonMatcher.ExpectNull()
        });
    }

    [Test]
    public void ExpectAnyInt()
    {
        "{ \"TestProperty\": 5 }"
            .JsonShouldLookLike(new
        {
            TestProperty = JsonMatcher.ExpectAny<int>()
        });
    }

    [Test]
    public void ExpectNullableValues()
    {
        "[ { \"TestProperty\": null }, { \"TestProperty\": 5 } ]"
            .JsonShouldLookLike(new Object[]
        {
            new
            {
                TestProperty = JsonMatcher.Expect<int?>(val => val is null)
            },
            new
            {
                TestProperty = 5
            }
        });
    }

    [Test]
    public void ExpectInt()
    {
        "{ \"TestProperty\": 5 }"
            .JsonShouldLookLike(new
        {
            TestProperty = JsonMatcher.Expect<int>(val => val == 5)
        });
    }

    [Test]
    public void ExpectPartialValidation()
    {
        "{ \"TestProperty\": 15, \"ExpectedProperty\": 5 }"
            .JsonShouldLookLike(new
        {
            TestProperty = 15,
            ExpectedProperty = JsonMatcher.Expect<int>(val => val is > 4 and < 15)
        });
    }

    [Test]
    public void ExampleForReadme()
    {
        @"
        { 
            ""WebsiteIpAddress"": ""192.168.0.3"",
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
    }
}