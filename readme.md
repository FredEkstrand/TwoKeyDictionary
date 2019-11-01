# TwoKeyDictionary<TKeyA,TKeyB,TValue>
![Version 1.0.0](https://img.shields.io/badge/Version-1.0.0-brightgreen.svg) ![License MIT](https://img.shields.io/badge/Licence-MIT-blue.svg)
[Project Visuals] Include project screen shot, logo, etc.	

# Overview
A Two Key Dictionary is a data structure that represents a collection of (key-a + key-b)s and values triple of data.  Then use either key-a or key-b to return the value. The two key dictionary have the restriction that key-a and key-b cannot have the same value weather they map to the same value or not. **_Note:_** the current version doesn't provide any checks for duplicate key values between key-a and key-b. If so, the results would be nondeterministic. 

A Dictionary class is a data structure that represents a collection of keys and values pair of data.

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
Basic code examples:
```csharp
static void Main(string[] args)
{
	// Key-A = color name, Key-B = enum , Value = string with hex color value.
	// TwoKeyDictionary<string, CrayolaCrayons, string> CrayolaColors = init(); // loads up the dictionary with key,key and values	
	int count = CrayolaColors.Count; // count is 120
	
	
	// Example 1: Index by A-Key for hex color value.   
	string result = "CrayolaColors["AntiqueBrass"]; 	// result is "FFCD9575".		

	// Example 2: Index by B-Key for hex color value.	
	result = CrayolaColors[CrayolaCrayons.Fern];        // result is "FF71BC78".

	// Example 3: Remove Key-A.
	CrayolaColors.RemoveKeyA("Bittersweet"); // goodbye 

	// Now check if the Key-A is removed.
	bool bresult = CrayolaColors.ContainsKeyA("Bittersweet"); // bresult is false.

	// Now check if the corresponding B Key is removed as well.");
	bresult = CrayolaColors.ContainsKeyB(CrayolaCrayons.Bittersweet); // bresult is false.
	
	count = CrayolaColors.Count; // count is now 119
	
}
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