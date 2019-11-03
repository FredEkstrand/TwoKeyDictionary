using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ekstrand.Collections.Generic
{
    /// <summary>
    /// Represents a generic read-only collection of A-key,B-key/value triple.
    /// </summary>
    /// <typeparam name="TKeyA">The type of A-keys in the read-only dictionary.</typeparam>
    /// <typeparam name="TKeyB">The type of B-keys in the read-only dictionary.</typeparam>
    /// <typeparam name="TValue">The type of values in the read-only dictionary.</typeparam>
    interface IReadOnlyTwoKeyDictionary<TKeyA, TKeyB, TValue> : IReadOnlyCollection<TwoKeyValueTriple<TKeyA, TKeyB, TValue>>
    {
        /// <summary>
        /// Determines whether the read-only dictionary contains an element that has the specified A-key.
        /// </summary>
        /// <param name="keyA">The A-key to locate.</param>
        /// <returns>true if the read-only dictionary contains an element that has the specified key; otherwise, false.</returns>
        bool ContainsKeyA(TKeyA keyA);

        /// <summary>
        /// Determines whether the read-only dictionary contains an element that has the specified B-key.
        /// </summary>
        /// <param name="keyB">The B-key to locate.</param>
        /// <returns>true if the read-only dictionary contains an element that has the specified key; otherwise, false.</returns>
        bool ContainsKeyB(TKeyB keyB);

        /// <summary>
        /// Gets the value that is associated with the specified A-key.
        /// </summary>
        /// <param name="keyA">The A-key to locate.</param>
        /// <param name="value">When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the value parameter. This parameter is passed uninitialized.</param>
        /// <returns>true if the object that implements the IReadOnlyTwoKeyDictionary&lt;TKeyA,TKeyB,TValue&gt; interface contains an element that has the specified key; otherwise, false.</returns>
        bool TryGetValueKeyA(TKeyA keyA, out TValue value);

        /// <summary>
        /// Gets the value that is associated with the specified A-key.
        /// </summary>
        /// <param name="keyB">The B-key to locate.</param>
        /// <param name="value">When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the value parameter. This parameter is passed uninitialized.</param>
        /// <returns>true if the object that implements the IReadOnlyTwoKeyDictionary&lt;TKeyA,TKeyB,TValue&gt; interface contains an element that has the specified key; otherwise, false.</returns>
        bool TryGetValueKeyB(TKeyB keyB, out TValue value);

        /// <summary>
        /// Gets the element that has the specified key in the read-only dictionary.
        /// </summary>
        /// <param name="keyA">The A-key to locate.</param>
        /// <returns>The element that has the specified key in the read-only dictionary.</returns>
        TValue this[TKeyA keyA] { get; }

        /// <summary>
        /// Gets the element that has the specified key in the read-only dictionary.
        /// </summary>
        /// <param name="keyB">The B-key to locate.</param>
        /// <returns>The element that has the specified key in the read-only dictionary.</returns>
        TValue this[TKeyB keyB] { get; }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        IEnumerable<TKeyA> AKeys { get; }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        IEnumerable<TKeyB> BKeys { get; }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        IEnumerable<TValue> Values { get; }
    }

    /// <summary>
    /// Represents a strongly-typed, read-only collection of elements.
    /// </summary>
    /// <typeparam name="T">The type of the elements.</typeparam>
    public interface IReadOnlyCollection<out T> : IEnumerable<T>
    {
        /// <summary>
        /// Gets the number of elements in the collection.
        /// </summary>
        int Count { get; }
    }
}
