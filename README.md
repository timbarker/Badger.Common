# Badger.Common
Set of useful C# utilities

[![Build Status](https://travis-ci.org/timbarker/Badger.Common.svg?branch=master)](https://travis-ci.org/timbarker/Badger.Common)

## SystemTime

Useful in test code to freeze time. Normally the test setup would freeze the time and the teardown would dispose the result returned from calling Freeze. Application code would use SystemTime.UtcNow instead of DateTime.UtcNow.

```csharp
// Freeze time now (or optionally specify the DateTime to freeze at)
using (SystemTime.Freeze())
{
    // Always returns the same value until the using block exit
    Console.WriteLine(SystemTime.UtcNow); 
}
```

## Optional

Wraps a value that may or may not be present. The only way to access the value is to use the methods provided.

```csharp
// Division is not defined when b is zero
Optional<int> Divide(int a, int b)
{
    if (b == 0) return Optional.None<int>();
    else return Optional.Some(a / b);
}

var result = Divide(100, 2);

// returns the result or a supplied default if result is None
result.ValueOr(42); 

// returns the result or invokes a function to get a default if result is None
result.ValueOr(() => 42);

// multiplies the result by 100 only if result is not None
result.Map(v => v * 100); 

// calls Divide again only if result is not None
result.FlatMap(v => Divide(250, v)); 

// returns Some only if the predicate holds true and result is not None, else it results None
result.Filter(v => v % 2 == 0); 

// invokes the action supplied if the result is not None
result.WhenSome(r => Console.WriteLine(r));

// invokes the action supplied if the result is None
result.WhenNone(() => Console.WriteLine("None"))

// converts to nullable type (for value types only)
result.ToNullable(); 

// returns true if the result has a value
result.HasValue;

// if you have a nullable value there is a helper to convert to an Optional
int? x = 42;
var result = Optional.FromNullable(x);

```

### LINQ query expressions

```csharp
// continues evaluating the query if each part is not None, when part of the query returns a 
// None result the query stops evaluating and returns a None result. If all parts return a 
// Some value then the query will have a Some result
from v1 in Divide(100, 2)
from v2 in Divide(500, v1)
where v2 > 5
select v2 * 2; 
```

## Result
Wraps a Success value or an Error value. The only way to access the error or the success value is to use the methods provided.

```csharp
Result<int, string> SomethingThatCouldFail(int someData)
{
    try 
    {
        // lets pretend this could fail sometimes
        return Result.Ok<int, string>(someData * 2);
    }
    catch (Exception ex)
    {
        return Result.Error<int, string>(ex.Message);
    }
}

var result = SomethingThatCouldFail(200);

// multiplies the result by 100 only if result is Success
result.Map(r => r * 2);

// calls SomethingThatCouldFail again only if result is Success
result.FlatMap(r => SomethingThatCouldFail(r));

// changes the error string to "Whoops" if result is an Error
result.MapError(e => "Whoops");

// invokes the supplied action if the result is Success
result.WhenSuccess(s => Console.WriteLine(s));

// invokes the supplied action if the result is an Error
result.WhenError(e => Console.WriteLine(e));

// returns the success value or throws (only if TError is an exception)
result.SuccessOrThrow();

// returns true if the result is Success
result.IsSuccess;

// returns true if the result is an Error
result.IsError;

// if calling a method that throws there is Result.Try to automatically wrap the exception
var result = Result.Try<int, FormatException>(() => int.Parse("123"));
```

### LINQ query expressions

```csharp
// same as for Optional, query evaluation is short circuited as soon as there is an error
from r1 in SomethingThatCouldFail(100)
from r2 in SomethingThatCouldFail(r1)
select r2 * 15;

```
