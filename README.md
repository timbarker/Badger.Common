# Badger.Common
Set of useful C# utilities

[![Build Status](https://travis-ci.org/timbarker/Badger.Common.svg?branch=master)](https://travis-ci.org/timbarker/Badger.Common)
[![Nuget Package](https://img.shields.io/nuget/v/Badger.Common.svg?style=flat)](https://www.nuget.org/packages/Badger.Common/)

## Table of contents

* [Optional](#optional)
* [Result](#result)
* [Disposable](#disposable)
* [Extensions to IList](#extensions-to-ilist)
* [Extensions to IDictionary](#extensions-to-idictionary)
* [EventBus](#eventbus)

## Optional

Wraps a value that may or may not be present. The only way to access the value is to use the methods provided.

```csharp
// Division is not defined when b is zero
Optional<int> Divide(int a, int b)
{
    if (b == 0) 
        return Optional.None<int>();
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

// returns the result of calling the some function when Some, or the none function when None
optional.Match(some: v => v, none: () => 0);

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

// using Apply:
class Person
{
    public string FirstName { get; }
    public string LastName { get; }
    public DateTime Dob { get; }

    private Person(string firstName, string lastName, DateTime dob)
    {
        FirstName = firstName;
        LastName = lastName;
        Dob = dob;
    }

    public static readonly Func<string, string, DateTime, Person> Create = (f, l, dob) => new Person(f, l, dob);
}

Optional<string> ValidateFirstName(string firstName) 
{
    if (string.IsNullOrWhiteSpace(firstName)) return Optional.None<string>();

    Optional.Some(firstName);   
}

Optional<string> ValidateLastName(string lastName) 
{
    if (string.IsNullOrWhiteSpace(lastName)) return Optional.None<string>();

    Optional.Some(lastName);   
}

Optional<DateTime> ValidateDob(DateTime dob)
{
    if (dob > DateTime.UtcNow) return Optional.None<DateTime>();

    Optional.Some(dob);
}

// creates a person object only if all the parameters pass validation, else returns None
Optional<Person> Create(string firstName, string lastName, DateTime dob)
{
    return ValidateDob(dob)
            .Apply(ValidateLastName(lastName)
                .Apply(ValidateFirstName(firstName)
                    .Map(Person.Create.Curry())))
}
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

// multiplies the result by 2 only if result is Ok
result.Map(r => r * 2);

// calls Divide again only if result is Ok
result.FlatMap(r => Divide(250, r));

// changes the error string to "Whoops" if result is an Error
result.MapError(e => "Whoops");

// returns the result of calling the ok function when Ok, or the error function when Error
optional.Match(ok: v => v, error: e => 0);

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

// apply exampler
class Person
{
    public string FirstName { get; }
    public string LastName { get; }
    public DateTime Dob { get; }

    private Person(string firstName, string lastName, DateTime dob)
    {
        FirstName = firstName;
        LastName = lastName;
        Dob = dob;
    }
    public static readonly Func<string, string, DateTime, Person> Create = (f, l, dob) => new Person(f, l, dob);
}

Result<string, string> ValidateFirstName(string firstName) 
{
    if (string.IsNullOrWhiteSpace(firstName)) return Result.Error<string, string>("First name is empty");

    else return Result.Ok<string, string>(firstName); 
}

Result<string, string> ValidateLastName(string lastName) 
{
    if (string.IsNullOrWhiteSpace(lastName)) return Result.Error<string, string>("Last name is empty");

    else return Result.Ok<string, string>(lastName);  
}

Result<DateTime, string> ValidateDob(DateTime dob)
{
    if (dob > DateTime.UtcNow) return Result.Error<DateTime, string>("date of birth is in the future");

    return Result.Ok<DateTime, string>(dob);
}

// creates a person object only if all the parameters pass validation, else returns the first validation error
Result<Person, string> Create(string firstName, string lastName, DateTime dob)
{
    return ValidateDob(dob)
            .Apply(ValidateLastName(lastName)
                .Apply(ValidateFirstName(firstName)
                    .Map(Person.Create.Curry())));
}

```

## Validator
Wraps a successful validation or a list of validation errors, collects validation errors when applied.
This is similar to Result, but Result will only hold one error and stops on the first error, Validation collates errors as all validations are applied

```csharp
Validation<int, string> InRange(int a, int min, int max)
{
    if (a < min || a >= max) 
        return Validation.Error<int, string>($"{a} is outside of range {min} to {max}");

    return Validation.Success<int, string>(a);
}

var validated = InRange(20, 0, 100);

// multiplies the validated value by 2 only if Success
validated.Map(r => r * 2);

// calls InRange again only if result is Success
validated.FlatMap(r => InRange(r, 0, 25));

// changes the error string to "Whoops" if result is an Error
result.MapError(e => "Whoops");

// returns the result of calling the success function when Success, or the error function when Error
optional.Match(success: v => v, error: e => 0);

// invokes the supplied action if the result is Success
result.WhenSuccess(s => Console.WriteLine(s));

// invokes the supplied action if the result is an Error
result.WhenError(e => Console.WriteLine(e));

// example chaining validators and constructing a validated object

class Person
{
    public string FirstName { get; }
    public string LastName { get; }
    public DateTime Dob { get; }

    private Person(string firstName, string lastName, DateTime dob)
    {
        FirstName = firstName;
        LastName = lastName;
        Dob = dob;
    }
    public static readonly Func<string, string, DateTime, Person> Create = (f, l, dob) => new Person(f, l, dob);
}

Validation<string, string> ValidateFirstName(string firstName)
{
    if (string.IsNullOrWhiteSpace(firstName)) return Validation.Error<string, string>("First name is empty");

    else return Validation.Success<string, string>(firstName);
}

Validation<string, string> ValidateLastName(string lastName)
{
    if (string.IsNullOrWhiteSpace(lastName)) return Validation.Error<string, string>("Last name is empty");

    else return Validation.Success<string, string>(lastName);
}

Validation<DateTime, string> ValidateDob(DateTime dob)
{
    if (dob > DateTime.UtcNow) return Validation.Error<DateTime, string>("date of birth is in the future");

    return Validation.Success<DateTime, string>(dob);
}

Validation<Person, string> Create(string firstName, string lastName, DateTime dob)
{
    return ValidateDob(dob)
            .Apply(ValidateLastName(lastName)
                .Apply(ValidateFirstName(firstName)
                    .Map(Person.Create.Curry())));
}

// creates a Success value with a Person object as all validators were Success
var successfullyValidatedPerson = Create("Joe", "Bloggs", new DateTime(2000, 1, 1));

// did not create a Person object and all validation error values are collated
var unsuccessfullyValidatedPerson = Create("", "", new DateTime(3000, 1, 1));

// prints out "First name is empty" "Last name is empty" "date of birth is in the future"
unsuccessfullyValidatedPerson.WhenError(errors => 
{
    foreach (var error in errors) 
        System.Console.WriteLine(error);
});

```

## Extensions to IList
```csharp
var list = new List<int> { 1, 2, 3, 4, 5 };

// finds the first value that matches the prdicate. Returns Some if match, else None
var result = list.FindValue(i => i == 2);

// returns the first result where Some is returned
var result = list.Pick(i => i == 2 ? Optional.Some("Badger") : Optional.None<string>())
```

## Extensions to IDictionary

```csharp
var dictionary = new Dictionary<string, int> { ["Badger"] = 42 };

// gets the value for the key specified. Returns Some if key exists, else None
var result = dictionary.Find("Badger");

// gets the key for the first key/value that returns true. None is retuned if nothing matched
var result = dictionary.FindKey((k, v) => k == "Badger" && v == 42);

// returns the first result where Some is returned
var result = dictionary.Pick((k, v) => k == "Badger" ? Optional.Some(42) : Optional.None<int>());
```

## Disposable

Helpers for creating IDisposable objects

```csharp
// Wraps an action in a disposable
IDisposable disposable = Disposable.From(() => Console.WriteLine("Disposing"));

// prints "Disposing"
disposable.Dispose();
```

```csharp
// Creates a composite disposable that disposes all the disposables passed into it
IDisposable disposable = Disposable.From(disposable1, disposable2);

// calls disposable1.Dispose() and disposable2.Dispose()
disposable.Dispose()
```

## EventBus

A simple thread safe event bus

```csharp

// create a new bus
var bus = new EventBus();

// subscribe to "string" events
bus.Subscribe<string>(s => Console.WriteLine(s));

// calls all the "string" subscriptions
bus.Publish("Hello World");
```

Any type can be rasied on the bus

```csharp
class MyEvent
{
    // ...
}

bus.Subscribe<MyEvent>(e => Console.WriteLine("MyEvent raised"));

bus.Publish(new MyEvent());
```

Subscriptions are managed via a subscription object returned from the Subscribe method

```csharp
// a subscription object is returned when subscribing
var subscription = bus.Subscribe<int>(i => Console.WriteLine(i));

// to stop subscribing call Dispose
subscription.Dispose();

// disposed subscription is not called
bus.Publish(42);

```

An object can be subscribed to the bus and any methods with the Subscribe attribute that accept one argument and return void will be subscribed

```csharp

class MyObject
{
    [Subscribe]
    public void HandleStrings(string s)
    {
        Console.WriteLine(s);
    }

    [Subscribe]
    private void AlsoHandleInts(int i)
    {
        Console.WriteLine(i)
    }
}

var myObject = new MyObject();

var subscription = bus.Subscribe(myObject);

// invokes myObject.HandleStrings()
bus.Publish("badger");

// invokes myObject.AlsoHandleInts()
bus.Publish(42);

```

If there is no subscriptions to an event then the DeadEvent will be raised on the bus
```csharp
var bus = new EventBus();

bus.Subscribe<EventBus.DeadEvent>(e => Console.WriteLine(e.Event));

// causes DeadEvent to be raised with this event on the Event property
bus.Publish("badger");

```

If there is an exception in a subscription then the Error event handler on the bus will be invoked.
An exception in one subscriber does not stop other subscribers from being invoked and the exception is
not propagated back to the Publish call

```csharp

var bus = new EventBus();
bus.Error += (s, e) => Console.WriteLine("Error: " + ex.Message);

bus.Subscribe<string>(s => throw new Exception(s));
bus.Subscribe<string>(s => Console.WriteLine(s));

// this will invoke the throwing subscriber and cause the Error event to be raised
// the second string subscriber will still be invoked
bus.Publish("badger");
