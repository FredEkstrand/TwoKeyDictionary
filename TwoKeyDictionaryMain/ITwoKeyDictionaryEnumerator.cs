using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ekstrand.Collections
{
    [System.Runtime.InteropServices.ComVisible(true)]
    public interface ITwoKeyDictionaryEnumerator : IEnumerator
    {
        // Returns the a-key of the current element of the enumeration. The returned
        // value is undefined before the first call to GetNext and following
        // a call to GetNext that returned false. Multiple calls to
        // GetKey with no intervening calls to GetNext will return
        // the same object.
        // 
        Object AKey
        {
            get;
        }

        // Returns the b-key of the current element of the enumeration. The returned
        // value is undefined before the first call to GetNext and following
        // a call to GetNext that returned false. Multiple calls to
        // GetKey with no intervening calls to GetNext will return
        // the same object.
        // 
        Object BKey
        {
            get;
        }

        // Returns the value of the current element of the enumeration. The
        // returned value is undefined before the first call to GetNext and
        // following a call to GetNext that returned false. Multiple calls
        // to GetValue with no intervening calls to GetNext will
        // return the same object.
        // 
        Object Value
        {
            get;
        }

        // GetBlock will copy dictionary values into the given Array.  It will either
        // fill up the array, or if there aren't enough elements, it will
        // copy as much as possible into the Array.  The number of elements
        // copied is returned.
        // 
        TwoKeyDictionaryEntry Entry
        {
            get;
        }
    }

    // A DictionaryEntry holds a key and a value from a dictionary.
    // It is returned by IDictionaryEnumerator::GetEntry().
    [System.Runtime.InteropServices.ComVisible(true)]
    [Serializable]
    public struct TwoKeyDictionaryEntry
    {
        private Object _Akey;
        private Object _Bkey;
        private Object _value;

        // Constructs a new DictionaryEnumerator by setting the Key
        // and Value fields appropriately.
        public TwoKeyDictionaryEntry(Object AKey, Object Bkey, Object value)
        {
            _Akey = AKey;
            _Bkey = Bkey;
            _value = value;
        }

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
