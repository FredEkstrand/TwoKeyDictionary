# TwoKeyDictionary<TKeyA,TKeyB,TValue>
[Badges] Version, Code Coverage, Build, License
[Project Visuals] Include project screen shot, logo, etc.	

# Overview
The two key dictionary is a data structure to allow one to map two keys to a single value. Then use either key-a or key-b to return the value. The two key dictionary have the restriction that key-a and key-b cannot have the same value weather they map to the same value or not. **_Note:_** the current version doesn't provide any checks for duplicate key values between key-a and key-b. If so, the results would be nondeterministic. 

# Features
The Two Key Dictionary provide the following features:
* The two key dictionary requires only key-a or key-b to return the mapped value.
* The two key dictionary key-a or key-b cannot be null, but value can be.
* The two key dictionary is an ordered collection.
* The two key dictionary indexer by, key-a or key-b, to access individual item.
* Provided TryGetValue() method to get the value of a key to avoid possible runtime exceptions. 
* The two key dictionary cannot contain duplicate a-keys or b-keys.
* The two key dictionary cannot contain duplicate value for a-key and b-key.
* The two key dictionary is not thread-safe.

# Getting Started
The source code is written in C# and targeted for the .Net Framework 4.0 and later. Download the entire project and compile.

# Usage
Once you have compiled the project reference the dll in your Visual Studio project. Then in your code file add the following to the collection of using statement.

```csharp
using Ekstrand.Collections.Generic;
```

# Code Documentation
MSDN-style code documentation can be found  [here](#).

# History
 1.0.0 Initial release into the wild.

# Contributing

If you'd like to contribute, please fork the repository and use a feature
branch. Pull requests are always welcome.

# Contact
Fred Ekstrand
email: fredekstrandgithub@gmail.com

# Licensing

This project is licensed under the MIT License - see the LICENSE.md file for details.
