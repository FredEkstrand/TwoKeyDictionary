using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ekstrand.Collections
{
    /// <summary>
    /// Represents a generic collection of A-key, B-key/value triple.
    /// </summary>
    public interface ITwoKeyDictionary : ICollection
    {
        // Interfaces are not serializable
        // The Item property provides methods to read and edit entries 
        // in the Dictionary.
        /// <summary>
        /// Gets or sets the element with the specified key.
        /// </summary>
        /// <param name="key">The key of the element to get or set.</param>
        /// <returns>The element with the specified key.</returns>
        Object this[Object key]
        {
            get;
            set;
        }

        // Returns a collections of the keys in this dictionary.
        /// <summary>
        /// Gets an ICollection&lt;T&gt; containing the A-keys of the ITwoKeyValueTriple&lt;TKeyA,TKeyB,TValue&gt;.
        /// </summary>
        ICollection AKeys
        {
            get;
        }

        /// <summary>
        /// Gets an ICollection&lt;T&gt; containing the B-keys of the ITwoKeyValueTriple&lt;TKeyA,TKeyB,TValue&gt;.
        /// </summary>
        ICollection BKeys
        {
            get;
        }

        // Returns a collections of the values in this dictionary.
        /// <summary>
        /// Gets an ICollection&lt;T&gt; containing the values of the ITwoKeyValueTriple&lt;TKeyA,TKeyB,TValue&gt;.
        /// </summary>
        ICollection Values
        {
            get;
        }

        // Returns whether this dictionary contains a particular key.
        //
        /// <summary>
        /// Determines whether the ICollection&lt;T&gt; contains a specific A/B key.
        /// </summary>
        /// <param name="key">The object to locate in the ICollection&lt;T&gt;.</param>
        /// <returns>true if item is found in the ICollection&lt;T&gt;; otherwise, false.</returns>
        bool Contains(Object key);

        // Adds a key-value pair to the dictionary.
        /// <summary>
        /// Adds an element with the provided A-key, B-key and value to the ITwoKeyDictionary&lt;TKeyA,TKeyB,TValue&gt;.
        /// </summary>
        /// <param name="keyA">The object to use as the key of the element to add.</param>
        /// <param name="keyB">The object to use as the key of the element to add.</param>
        /// <param name="value">he object to use as the value of the element to add.</param>
        void Add(Object keyA, Object keyB, Object value);

        // Removes all pairs from the dictionary.
        /// <summary>
        /// Removes all items from the ICollection&lt;T&gt;.
        /// </summary>
        void Clear();

        /// <summary>
        /// Gets a value indicating whether the ICollection&lt;T&gt; is read-only.
        /// </summary>
        bool IsReadOnly
        { get; }

        /// <summary>
        /// Gets a value indicating whether the ICollection&lt;T&gt; is fixed size.
        /// </summary>
        bool IsFixedSize
        { get; }

        // Returns an IDictionaryEnumerator for this dictionary.
        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An IEnumerator object that can be used to iterate through the collection.</returns>
        new ITwoKeyDictionaryEnumerator GetEnumerator();

        // Removes a particular key from the dictionary.
        //
        /// <summary>
        /// Removes the element with the specified key from the ITwoKeyDictionary&lt;TKeyA,TKeyB,TValue&gt;.
        /// </summary>
        /// <param name="key"></param>
        void Remove(Object key);
    }
}
