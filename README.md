# Badger.Common
Set of useful C# utilities

[![Build Status](https://travis-ci.org/timbarker/Badger.Common.svg?branch=master)](https://travis-ci.org/timbarker/Badger.Common)
[![Nuget Package](https://img.shields.io/nuget/v/Badger.Common.svg?style=flat)](https://www.nuget.org/packages/Badger.Common/)

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
    if (b == 0) 
        return Optional.None<int>();
    else 
        return Optional.Some(a / b);
}

var optional = Divide(100, 2);

// returns the result or a supplied default if result is None
optional.ValueOr(42); 

// returns the result or invokes a function to get a default if result is None
optional.ValueOr(() => 42);

// multiplies the result by 100 only if result is not None
optional.Map(v => v * 100); 

// calls Divide again only if result is not None
optional.FlatMap(v => Divide(250, v)); 

// returns Some only if the predicate holds true and result is not None, else it results None
optional.Filter(v => v % 2 == 0); 

// invokes the action supplied if the result is not None
optional.WhenSome(r => Console.WriteLine(r));

// invokes the action supplied if the result is None
optional.WhenNone(() => Console.WriteLine("None"))

// converts to nullable type (for value types only)
optional.ToNullable(); 

// returns true if the result has a value
optional.HasValue;

// if you have a nullable value there is a helper to convert to an Optional
int? x = 42;
var optional = Optional.FromNullable(x);

// converts to an IEnumerable<T>, has one item if Some, else is empty
optional.AsEnumerable();

```

## Result
Wraps a Ok value or an Error value. The only way to access the error or the ok value is to use the methods provided.

```csharp
Result<int, string> Divide(int a, int b)
{
    if (b == 0) 
        return Result.Error<int, string>("Division by zero");
    else 
        return Result.Ok<int, string>(a / b);
}

var result = Divide(100, 2);

// multiplies the result by 100 only if result is Ok
result.Map(r => r * 2);

// calls Divide again only if result is Ok
result.FlatMap(r => Divide(250, r));

// changes the error string to "Whoops" if result is an Error
result.MapError(e => "Whoops");

// invokes the supplied action if the result is Ok
result.WhenOk(s => Console.WriteLine(s));

// invokes the supplied action if the result is an Error
result.WhenError(e => Console.WriteLine(e));

// returns the Ok value or throws (only if TError is an exception)
result.ValueOrThrow();

// returns the Ok value or the supplied default
result.ValueOr(42);

// returns the Ok value or invokes a function to get a default
result.ValueOr(() => 42);

// returns the Ok value or invokes a function with the error to get a default 
result.ValueOr(e => 42);

// returns true if the result is Ok
result.HasValue;

// if calling a method that throws a specific there is Result.Try to automatically wrap the exception
var result = Result.Try<int, FormatException>(() => int.Parse("123"));

// or if you want to catch any exception
var result = Result.Try(() => int.Parse("123"));

// converts to an IEnumerable<T>, has one item if Ok, else is empty
result.AsEnumerable();
```

## Disposable

Wraps an action creating an IDisposable

```csharp
IDisposable disposable = Disposable.From(() => Console.WriteLine("Disposing"));

// prints "Disposing"
disposable.Dispose();
```
