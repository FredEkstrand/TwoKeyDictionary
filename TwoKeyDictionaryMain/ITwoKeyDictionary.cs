using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ekstrand.Collections.Generic
{
    /// <summary>
    /// Represents a generic collection of A-key, B-key/value triple.
    /// </summary>
    /// <typeparam name="TKeyA">The type of A-key in the dictionary.</typeparam>
    /// <typeparam name="TKeyB">The type of B-key in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
    interface ITwoKeyDictionary<TKeyA, TKeyB, TValue> : ICollection<TwoKeyValueTriple<TKeyA, TKeyB, TValue>>
    {
        // Interfaces are not serializable
        // The Item property provides methods to read and edit entries 
        // in the Dictionary.
        /// <summary>
        /// Gets or sets the element with the specified key.
        /// </summary>
        /// <param name="keyA">The key of the element to get or set.</param>
        /// <returns>The element with the specified key.</returns>
        TValue this[TKeyA keyA]
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the element with the specified key.
        /// </summary>
        /// <param name="keyB">The key of the element to get or set.</param>
        /// <returns>The element with the specified key.</returns>
        TValue this[TKeyB keyB]
        {
            get;
            set;
        }

        // Returns a collections of the keys in this dictionary.
        /// <summary>
        /// Gets a collection of A-key in this dictionary.
        /// </summary>
        ICollection<TKeyA> AKeys
        {
            get;
        }

        /// <summary>
        ///  Gets a collection of B-key in this dictionary.
        /// </summary>
        ICollection<TKeyB> BKeys
        {
            get;
        }

        // Returns a collections of the values in this dictionary.
        /// <summary>
        ///  Gets a collection of Values in this dictionary.
        /// </summary>
        ICollection<TValue> Values
        {
            get;
        }

        // Returns whether this dictionary contains a particular key.
        /// <summary>
        /// Determines whether the ITwoKeyDictionary&lt;TKeyA,TKeyB,TValue&gt; contains an element with the specified key.
        /// </summary>
        /// <param name="keyA">The key to locate in the ITwoKeyDictionary&lt;TKeyA,TKeyB,TValue&gt;.</param>
        /// <returns>true if the ITwoKeyDictionary&lt;TKeyA,TKeyB,TValue&gt; contains an element with the key; otherwise, false.</returns>
        bool ContainsKeyA(TKeyA keyA);

        /// <summary>
        /// Determines whether the ITwoKeyDictionary&lt;TKeyA,TKeyB,TValue&gt; contains an element with the specified key.
        /// </summary>
        /// <param name="keyB">The key to locate in the ITwoKeyDictionary&lt;TKeyA,TKeyB,TValue&gt;.</param>
        /// <returns>true if the ITwoKeyDictionary&lt;TKeyA,TKeyB,TValue&gt; contains an element with the key; otherwise, false.</returns>
        bool ContainsKeyB(TKeyB keyB);

        // Adds a key-value pair to the dictionary.
        /// <summary>
        /// Adds an element with the provided key and value to the ITwoKeyDictionary&lt;TKeyA,TKeyB,TValue&gt;.
        /// </summary>
        /// <param name="keyA">The object to use as the A-key of the element to add.</param>
        /// <param name="keyB">The object to use as the B-key of the element to add.</param>
        /// <param name="value">The object to use as the value of the element to add.</param>
        void Add(TKeyA keyA, TKeyB keyB, TValue value);

        // Removes a particular key from the dictionary.
        /// <summary>
        /// Removes the element with the specified key from the ITwoKeyDictionary&lt;TKeyA,TKeyB,TValue&gt;.
        /// </summary>
        /// <param name="keyA">The A-key of the element to remove.</param>
        /// <returns>true if the element is successfully removed; otherwise, false. This method also returns false if key was not found in the original ITwoKeyDictionary&lt;TKeyA,TKeyB,TValue&gt;.</returns>
        bool RemoveKeyA(TKeyA keyA);

        /// <summary>
        /// Removes the element with the specified key from the ITwoKeyDictionary&lt;TKeyA,TKeyB,TValue&gt;.
        /// </summary>
        /// <param name="keyB">The B-key of the element to remove.</param>
        /// <returns>true if the element is successfully removed; otherwise, false. This method also returns false if key was not found in the original ITwoKeyDictionary&lt;TKeyA,TKeyB,TValue&gt;.</returns>
        bool RemoveKeyB(TKeyB keyB);

        /// <summary>
        /// Gets the value associated with the specified A-key.
        /// </summary>
        /// <param name="keyA">The A-key whose value to get.</param>
        /// <param name="value">When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the value parameter. This parameter is passed uninitialized.</param>
        /// <returns>true if the object that implements ITwoKeyDictionary&lt;TKeyA,TKeyB,TValue&gt; contains an element with the specified key; otherwise, false.</returns>
        bool TryGetValueKeyA(TKeyA keyA, out TValue value);

        /// <summary>
        /// Gets the value associated with the specified A-key.
        /// </summary>
        /// <param name="keyB">The B-key whose value to get.</param>
        /// <param name="value">When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the value parameter. This parameter is passed uninitialized.</param>
        /// <returns>true if the object that implements ITwoKeyDictionary&lt;TKeyA,TKeyB,TValue&gt; contains an element with the specified key; otherwise, false.</returns>
        bool TryGetValueKeyB(TKeyB keyB, out TValue value);
    }
}
