using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ekstrand.Collections.Generic
{
    interface ITwoKeyDictionary<TKeyA, TKeyB, TValue> : ICollection<TwoKeyValueTriple<TKeyA, TKeyB, TValue>>
    {
        // Interfaces are not serializable
        // The Item property provides methods to read and edit entries 
        // in the Dictionary.
        TValue this[TKeyA keyA]
        {
            get;
            set;
        }

        TValue this[TKeyB keyB]
        {
            get;
            set;
        }

        // Returns a collections of the keys in this dictionary.
        ICollection<TKeyA> AKeys
        {
            get;
        }

        ICollection<TKeyB> BKeys
        {
            get;
        }

        // Returns a collections of the values in this dictionary.
        ICollection<TValue> Values
        {
            get;
        }

        // Returns whether this dictionary contains a particular key.
        //
        bool ContainsKeyA(TKeyA keyA);

        bool ContainsKeyB(TKeyB keyB);

        // Adds a key-value pair to the dictionary.
        // 
        void Add(TKeyA keyA, TKeyB keyB, TValue value);

        // Removes a particular key from the dictionary.
        //
        bool RemoveKeyA(TKeyA keyA);

        bool RemoveKeyB(TKeyB keyB);

        bool TryGetValueKeyA(TKeyA keyA, out TValue value);

        bool TryGetValueKeyB(TKeyB keyB, out TValue value);
    }
}
