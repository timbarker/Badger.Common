# Badger.Common
Set of useful C# utilities

## SystemTime

Useful in test code to freeze time

```csharp
// Freeze time now (or optionally specify the DateTime to freeze at)
using (SystemTime.Freeze())
{
    // Always returns the same value until the using block exit
    Console.WriteLine(SystemTime.UtcNow); 
}

// Normally the test setup would freeze the time and the teardown would dispose the 
// resylt of calling Freeze. Application code would use SystemTime.UtcNow 
// over DateTime.UtcNow
```

## Optional

Wraps a value that may or may not be present 

```csharp
// Division is not defined when b is zero
Optional<int> Divide(int a, int b)
{
    if (b == 0) return Optional.None<int>();
    else return Optional.Some(a / b);
}

var result = Divide(100, 2);

// returns the result or 0 if result is None
result.ValueOr(0); 

// multiplies the result by 100 only if result is not None
result.Map(v => v * 100); 

// calls Divide again only if result is not None
result.FlatMap(v => Divide(250, v)); 

// returns Some only if the predicate holds true and result is not None, else it results None
result.Filter(v => v % 2 == 0); 

// converts to nullable type (for value types only)
result.ToNullable(); 

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
Wraps a Success value or an Error value

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

// returns the success value or throws (only if TError is an exception)
result.SuccessOrThrow();
```

### LINQ query expressions

```csharp
// same as for Optional, query evaluation is short circuited as soon as there is an error
from r1 in SomethingThatCouldFail(100)
from r2 in SomethingThatCouldFail(r1)
select r2 * 15;

```