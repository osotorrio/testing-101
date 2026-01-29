# What are automated tests?

It is a technical application that test your business application. That's it.

# What is a unit test?

Is an **automated piece of code** that invokes a **unit of work** being tested, and checks some assumptions about a **single end result** of that unit.

# What is a unit of work? (demystifying your brain)

*"There are parts of the first edition that today I do not agree with, for example,
that a unit refers to a method...It can be as small as a method, or as big as several classes (possibly assemblies)"* ([Roy Osherove, The Art Of Unit Test](https://www.artofunittesting.com))

*Although I start with the notion of the unit being a class, I often take a bunch of closely related classes and treat them as a single unit. Rarely I might take a subset of methods in a class as a unit.* ([Martin Fowler](https://martinfowler.com/bliki/UnitTest.html))

# What is an integration test then?

Is an **automated piece of code** that invokes a unit of work **without having full control over all** of it. Like network servers, time, and so on.