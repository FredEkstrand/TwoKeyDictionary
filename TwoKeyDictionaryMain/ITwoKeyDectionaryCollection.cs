using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ekstrand.Collections
{
    public interface ITwoKeyDictionary : ICollection
    {
        // Interfaces are not serializable
        // The Item property provides methods to read and edit entries 
        // in the Dictionary.
        Object this[Object key]
        {
            get;
            set;
        }

        // Returns a collections of the keys in this dictionary.
        ICollection AKeys
        {
            get;
        }

        ICollection BKeys
        {
            get;
        }

        // Returns a collections of the values in this dictionary.
        ICollection Values
        {
            get;
        }

        // Returns whether this dictionary contains a particular key.
        //
        bool Contains(Object key);

        // Adds a key-value pair to the dictionary.
        // 
        void Add(Object keyA, Object keyB, Object value);

        // Removes all pairs from the dictionary.
        void Clear();

        bool IsReadOnly
        { get; }

        bool IsFixedSize
        { get; }

        // Returns an IDictionaryEnumerator for this dictionary.
        new ITwoKeyDictionaryEnumerator GetEnumerator();

        // Removes a particular key from the dictionary.
        //
        void Remove(Object key);
    }
}
