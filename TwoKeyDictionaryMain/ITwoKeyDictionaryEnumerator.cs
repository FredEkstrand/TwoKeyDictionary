using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ekstrand.Collections
{
    /// <summary>
    /// Enumerates the elements of a nongeneric dictionary.
    /// </summary>
    [System.Runtime.InteropServices.ComVisible(true)]
    public interface ITwoKeyDictionaryEnumerator : IEnumerator
    {
        // Returns the a-key of the current element of the enumeration. The returned
        // value is undefined before the first call to GetNext and following
        // a call to GetNext that returned false. Multiple calls to
        // GetKey with no intervening calls to GetNext will return
        // the same object.
        /// <summary>
        /// Gets the A-key of the current dictionary entry.
        /// </summary>
        Object AKey
        {
            get;
        }

        // Returns the b-key of the current element of the enumeration. The returned
        // value is undefined before the first call to GetNext and following
        // a call to GetNext that returned false. Multiple calls to
        // GetKey with no intervening calls to GetNext will return
        // the same object.
        /// <summary>
        /// Gets the B-key of the current dictionary entry.
        /// </summary>
        Object BKey
        {
            get;
        }

        // Returns the value of the current element of the enumeration. The
        // returned value is undefined before the first call to GetNext and
        // following a call to GetNext that returned false. Multiple calls
        // to GetValue with no intervening calls to GetNext will
        // return the same object.
        /// <summary>
        /// 
        /// </summary>
        Object Value
        {
            get;
        }

        // GetBlock will copy dictionary values into the given Array.  It will either
        // fill up the array, or if there aren't enough elements, it will
        // copy as much as possible into the Array.  The number of elements
        // copied is returned.
        /// <summary>
        /// Gets both the A-key, B-key and the value of the current dictionary entry.
        /// </summary>
        TwoKeyDictionaryEntry Entry
        {
            get;
        }
    }

    // A DictionaryEntry holds a key and a value from a dictionary.
    // It is returned by IDictionaryEnumerator::GetEntry().
    /// <summary>
    /// Defines a dictionary A-key, B-key/value triple that can be set or retrieved.
    /// </summary>
    [System.Runtime.InteropServices.ComVisible(true)]
    [Serializable]
    public struct TwoKeyDictionaryEntry
    {
        private Object _Akey;
        private Object _Bkey;
        private Object _value;

        // Constructs a new DictionaryEnumerator by setting the Key
        // and Value fields appropriately.
        /// <summary>
        /// Initializes an instance of the TwoKeyDictionaryEntry type with the specified A-key, B-key and value.
        /// </summary>
        /// <param name="AKey">The object defined in each A-key, B-key/value triple.</param>
        /// <param name="Bkey">The object defined in each A-key, B-key/value triple.</param>
        /// <param name="value">The definition associated with key.</param>
        public TwoKeyDictionaryEntry(Object AKey, Object Bkey, Object value)
        {
            _Akey = AKey;
            _Bkey = Bkey;
            _value = value;
        }

        /// <summary>
        /// Gets or sets the A-key in the A-key, B-key/value triple.
        /// </summary>
        public Object AKey
        {
            get
            {
                return _Akey;
            }

            set
            {
                _Akey = value;
            }
        }

        /// <summary>
        /// Gets or sets the A-key in the A-key, B-key/value triple.
        /// </summary>
        public Object BKey
        {
            get
            {
                return _Bkey;
            }

            set
            {
                _Bkey = value;
            }
        }

        /// <summary>
        /// Gets or sets the value in the A-key, B-key/value triple.
        /// </summary>
        public Object Value
        {
            get
            {
                return _value;
            }

            set
            {
                _value = value;
            }
        }
    }
}
