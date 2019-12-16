# Generic Two Key Dictionary
![Version 1.0.0](https://img.shields.io/badge/Version-1.0.0-brightgreen.svg) ![License MIT](https://img.shields.io/badge/Licence-MIT-blue.svg)  [![Build status](https://ci.appveyor.com/api/projects/status/toavtx40noln89q9?svg=true)](https://ci.appveyor.com/project/FredEkstrand/twokeydictionary-0129g)

![image](https://github.com/FredEkstrand/ImageFiles/raw/master/TwoKeyDictionary/TwoKeyDictionaryImage.png)


# Overview
A Two Key Dictionary is a data structure that represents a collection of a-keys, b-keys and values triple data. You only need either a-key or b-key to return the value. The two key dictionary have the restriction that a/b-key cannot have the same value weather they map to the same key-value or not. **_Note:_** the current version doesn't provide any checks for duplicate key values between a-key and b-key. If so, the results would be nondeterministic. 

# Features
The Two Key Dictionary provide the following features:
* The two key dictionary requires only a-key or b-key to return the mapped value.
* The two key dictionary a-key or b-key cannot be null, but value can be.
* The two key dictionary indexer by, a-key or b-key, to access individual item.
* Provided TryGetValue() method to get the value of a key to avoid possible runtime exceptions. 
* The two key dictionary cannot contain duplicate a-keys or b-key.
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

	TwoKeyDictionary<int, string, string> tkd = new TwoKeyDictionary<int, string, string>();

	// Add elements to the two key dictionary
	tkd.Add(33024, "LJ02-026XN-PEP2F-M88L", "7FwCTLnD0ZdnDmYRPbZW");
	tkd.Add(66571, "LJ02-026XN-PEP2F-M88N", "Y4cE253SCT3agPC96Fhd");
	tkd.Add(86280, "LJ02-026XN-PEP2F-M88T", "cGnsZLmKK8xKDQnCprKY");
	tkd.Add(58647, "LJ02-026XN-PEP2F-M88R", "TWAggDF0jZVH454RRvrs");
	tkd.Add(87303, "LJ02-026XN-PEP2F-M88Q", "TuGEgtXSm9WQ6JLFGGLW");
	tkd.Add(86891, "LJ02-026XN-PEP2F-M88P", "ExmwnpRHWWx39dEkP6Ay");
	tkd.Add(69992, "LJ02-026XN-PEP2F-M88M", "cQ6RNcQcEm1KFXqRkBth");

	// Access by index by A-Key
	string result = string.Empty;
	result = tkd[58647];        // Result would be: TWAggDF0jZVH454RRvrs
	Console.WriteLine("Index by A-Key: {0}", result);

	// Access by index by B-Key
	result = tkd["LJ02-026XN-PEP2F-M88M"];  // Result would be: cQ6RNcQcEm1KFXqRkBth
	Console.WriteLine("Index by B-Key: {0}", result);

	// Contains key for A/B-Key
	bool bResult = false;
	bResult = tkd.ContainsKeyA(86280);  // Result would be: true

	bResult = tkd.ContainsKeyB("LJ02-026XN-PEP2F-M881");    // Result would be: false;
	Console.WriteLine("Contains BKey: {0}, Result: {1}", "LJ02-026XN-PEP2F-M881", bResult);

	// TwoKeyDictionaryRemovalKeyAB
	tkd.RemoveKeyA(86280);    // Removes A-Key and B-Key with value from the two key dictionary.
	bResult = tkd.ContainsKeyB("LJ02-026XN-PEP2F-M88T");	// Result would be: false;
	Console.WriteLine("Contains BKey: {0}", bResult);

	int count = tkd.Count;  // Result would be: 6 After the removal of A-Key & B-Key with value.
	Console.WriteLine("Two Key Dictionary Count: {0}", count);
	Console.WriteLine();

	// Enumeration of entries from TwoKeyValueTriple
	foreach (TwoKeyValueTriple<int, string, string> item in tkd)
	{
		Console.WriteLine("{0},  {1},  {2}", item.KeyA, item.KeyB, item.Value);
	}
	/* Output
		33024, LJ02-026XN-PEP2F-M88L, 7FwCTLnD0ZdnDmYRPbZW
		66571, LJ02-026XN-PEP2F-M88N, Y4cE253SCT3agPC96Fhd
		58647, LJ02-026XN-PEP2F-M88R, TWAggDF0jZVH454RRvrs
		87303, LJ02-026XN-PEP2F-M88Q, TuGEgtXSm9WQ6JLFGGLW
		86891, LJ02-026XN-PEP2F-M88P, ExmwnpRHWWx39dEkP6Ay
		69992, LJ02-026XN-PEP2F-M88M, cQ6RNcQcEm1KFXqRkBth	
	*/

	Console.WriteLine();

	// Enumeration of A-Keys
	foreach (int item in tkd.AKeys)
	{
		Console.WriteLine("{0}", item);
	}

	/* Output
		33024
		66571
		58647
		87303
		86891
		69992	
	*/
	Console.ReadLine();
}
```

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
