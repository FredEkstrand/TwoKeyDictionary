using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.Serialization;
using Ekstrand.Collections;
using Ekstrand.Collections.Generic;

namespace Ekstrand.Collections.Generic
{
    /// <summary>
    /// Represents a collection of A-keys, B-Keys and values.
    /// </summary>
    /// <typeparam name="TKeyA">The type of the A-keys in the dictionary.</typeparam>
    /// <typeparam name="TKeyB">The type of the B-keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
    [DebuggerTypeProxy(typeof(TwoKeyDictionaryDebugView<,,>))]
    [Serializable]
    [System.Runtime.InteropServices.ComVisible(false)]
    public class TwoKeyDictionary<TKeyA, TKeyB, TValue> : ITwoKeyDictionary<TKeyA, TKeyB, TValue>, ITwoKeyDictionary, IReadOnlyTwoKeyDictionary<TKeyA, TKeyB, TValue>, ISerializable, IDeserializationCallback
    {

        #region Fields

        private const String ComparerName = "Comparer";

        private const String HashSizeName = "HashSize";

        // Must save buckets.Length
        private const String KeyValuePairsName = "KeyValueTriple";

        // constants for serialization
        private const String VersionName = "Version";

        private Object _syncRoot;

        private KeyACollection Akeys;

        private KeyBCollection Bkeys;

        private int[] buckets;

        private IEqualityComparer<TKeyA> comparerA;

        private IEqualityComparer<TKeyB> comparerB;

        private int count;

        private Entry[] entries;

        private int freeCount;

        private int freeList;

        private ValueCollection values;

        private int version;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the Dictionary&lt;TKeyA,TKeyB,TValue&gt; class that is empty, has the default initial capacity, and uses the default equality comparer for the key type.
        /// </summary>
        public TwoKeyDictionary() : this(0, null, null) { }

        /// <summary>
        /// Initializes a new instance of the Dictionary&lt;TKeyA,TKeyB,TValue&gt; class that is empty, has the specified initial capacity, and uses the default equality comparer for the key type.
        /// </summary>
        /// <param name="capacity">The initial number of elements that the Dictionary&lt;TKeyA,TKeyB,TValue&gt; can contain.</param>
        public TwoKeyDictionary(int capacity) : this(capacity, null, null) { }

        /// <summary>
        /// Initializes a new instance of the Dictionary&lt;TKeyA,TKeyB,TValue&gt; class that is empty, has the default initial capacity, and uses the specified IEqualityComparer&lt;TKeyA&gt; and IEqualityComparer&lt;TKeyB&gt;.
        /// </summary>
        /// <param name="comparerA">The IEqualityComparer&lt;TKeyA&gt; implementation to use when comparing keys, or null to use the default IEqualityComparer&lt;TKeyA&gt; for the type of the key.</param>
        /// <param name="comparerB">The IEqualityComparer&lt;TKeyB&gt; implementation to use when comparing keys, or null to use the default IEqualityComparer&lt;TKeyB&gt; for the type of the key.</param>
        public TwoKeyDictionary(IEqualityComparer<TKeyA> comparerA, IEqualityComparer<TKeyB> comparerB) : this(0, comparerA, comparerB) { }

        /// <summary>
        /// Initializes a new instance of the Dictionary&lt;TKeyA,TKeyB,TValue&gt; class that is empty, has the default initial capacity, and uses the specified IEqualityComparer&lt;TKeyA&gt;.
        /// </summary>
        /// <param name="comparerA">The IEqualityComparer&lt;TKeyA&gt; implementation to use when comparing keys, or null to use the default IEqualityComparer&lt;TKeyA&gt; for the type of the key.</param>
        public TwoKeyDictionary(IEqualityComparer<TKeyA> comparerA) : this(0, comparerA, null) { }

        /// <summary>
        /// Initializes a new instance of the Dictionary&lt;TKeyA,TKeyB,TValue&gt; class that is empty, has the default initial capacity, and uses the specified IEqualityComparer&lt;TKeyB&gt;.
        /// </summary>
        /// <param name="comparerB">The IEqualityComparer&lt;TKeyB&gt; implementation to use when comparing keys, or null to use the default EqualityComparer&lt;TKeyB&gt; for the type of the key.</param>
        public TwoKeyDictionary(IEqualityComparer<TKeyB> comparerB) : this(0, null, comparerB) { }

        /// <summary>
        /// Initializes a new instance of the Dictionary&lt;TKeyA,TKeyB,TValue&gt; class that is empty, has the specified initial capacity, and uses the specified IEqualityComparer&lt;TKeyA&gt; and IEqualityComparer&lt;TKeyB&gt;.
        /// </summary>
        /// <param name="capacity">The initial number of elements that the Dictionary&lt;TKeyA,TKeyB,TValue&gt; can contain.</param>
        /// <param name="comparerA">The IEqualityComparer&lt;TKeyA&gt; implementation to use when comparing keys, or null to use the default EqualityComparer&lt;TKeyA&gt; for the type of the key.</param>
        /// <param name="comparerB">The IEqualityComparer&lt;TKeyB&gt; implementation to use when comparing keys, or null to use the default EqualityComparer&lt;TKeyB&gt;> for the type of the key.</param>
        public TwoKeyDictionary(int capacity, IEqualityComparer<TKeyA> comparerA, IEqualityComparer<TKeyB> comparerB)
        {
            if (capacity < 0) throw new ArgumentOutOfRangeException("Capacity");
            if (capacity > 0) Initialize(capacity);
            this.comparerA = comparerA ?? EqualityComparer<TKeyA>.Default;
            this.comparerB = comparerB ?? EqualityComparer<TKeyB>.Default;
        }

        /// <summary>
        /// Initializes a new instance of the Dictionary&lt;TKeyA,TKeyB,TValue&gt; and uses the default equality comparer for the key type.
        /// </summary>
        /// <param name="dictionary">The Dictionary&lt;TKeyA,TKeyB,TValue&gt; whose elements are copied to the new Dictionary&lt;TKeyA,TKeyB,TValue&gt;.</param>
        public TwoKeyDictionary(TwoKeyDictionary<TKeyA, TKeyB, TValue> dictionary) : this(dictionary, null, null) { }

        /// <summary>
        /// Initializes a new instance of the Dictionary&lt;TKeyA,TKeyB,TValue&gt; and uses the IEqualityComparer&lt;TKeyA&gt; and IEqualityComparer&lt;TKeyB&gt;.
        /// </summary>
        /// <param name="dictionary">The Dictionary&lt;TKeyA,TKeyB,TValue&gt; whose elements are copied to the new Dictionary&lt;TKeyA,TKeyB,TValue&gt;.</param>
        /// <param name="comparerA">The IEqualityComparer&lt;TKeyA&gt; implementation to use when comparing keys, or null to use the default EqualityComparer&lt;TKeyA&gt; for the type of the key.</param>
        /// <param name="comparerB">The IEqualityComparer&lt;TKeyB&gt; implementation to use when comparing keys, or null to use the default EqualityComparer&lt;TKeyB&gt; for the type of the key.</param>
        public TwoKeyDictionary(TwoKeyDictionary<TKeyA, TKeyB, TValue> dictionary, IEqualityComparer<TKeyA> comparerA, IEqualityComparer<TKeyB> comparerB) :
            this(dictionary != null ? dictionary.Count : 0, comparerA, comparerB)
        {

            if (dictionary == null)
            {
                throw new ArgumentNullException("TwoKeyDictionary");
            }

            foreach (TwoKeyValueTriple<TKeyA, TKeyB, TValue> triple in dictionary)
            {
                Clone(triple.KeyA, triple.KeyB, triple.Value);
            }
        }

        /// <summary>
        /// Initializes a new instance of the Dictionary&lt;TKeyA,TKeyB,TValue&gt; class with serialized data.
        /// </summary>
        /// <param name="info">A SerializationInfo object containing the information required to serialize the Dictionary&lt;TKeyA,TKeyB,TValue&gt;.</param>
        /// <param name="context">A StreamingContext structure containing the source and destination of the serialized stream associated with the Dictionary&lt;TKeyA,TKeyB,TValue&gt;.</param>
        protected TwoKeyDictionary(SerializationInfo info, StreamingContext context)
        {
            //We can't do anything with the keys and values until the entire graph has been deserialized
            //and we have a reasonable estimate that GetHashCode is not going to fail.  For the time being,
            //we'll just cache this.  The graph is not valid until OnDeserialization has been called.
            HashHelpers.SerializationInfoTable.Add(this, info);
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets a collection containing the A-keys in the Dictionary&lt;TKeyA,TKeyB,TValue&gt;.
        /// </summary>
        public ICollection<TKeyA> AKeys
        {
            get
            {
                if (Akeys == null) Akeys = new KeyACollection(this);
                return Akeys;
            }
        }

        /// <summary>
        /// Gets a collection containing the A-keys in the Dictionary&lt;TKeyA,TKeyB,TValue&gt;.
        /// </summary>
        ICollection ITwoKeyDictionary.AKeys
        {
            get
            {
                if (Akeys == null) Akeys = new KeyACollection(this);
                return Akeys;
            }
        }

        /// <summary>
        /// Gets a collection containing the A-keys of the IReadOnlyDictionary&lt;TKeyA,TKeyB,TValue&gt;.
        /// </summary>
        IEnumerable<TKeyA> IReadOnlyTwoKeyDictionary<TKeyA, TKeyB, TValue>.AKeys
        {
            get
            {
                if (Akeys == null)
                {
                    Akeys = new KeyACollection(this);
                }
                return Akeys;
            }
        }

        /// <summary>
        /// Gets a collection containing the B-keys in the Dictionary&lt;TKeyA,TKeyB,TValue&gt;.
        /// </summary>
        public ICollection<TKeyB> BKeys
        {
            get
            {
                Contract.Ensures(Contract.Result<KeyBCollection>() != null);
                if (Bkeys == null)
                {
                    Bkeys = new KeyBCollection(this);
                }
                return Bkeys;
            }
        }

        /// <summary>
        /// Gets a collection containing the B-keys in the Dictionary&lt;TKeyA,TKeyB,TValue&gt;.
        /// </summary>
        ICollection ITwoKeyDictionary.BKeys
        {
            get
            {
                Contract.Ensures(Contract.Result<KeyBCollection>() != null);
                if (Bkeys == null)
                {
                    Bkeys = new KeyBCollection(this);
                }
                return Bkeys;
            }
        }

        /// <summary>
        /// Gets a collection containing the B-keys of the IReadOnlyDictionary&lt;TKeyA,TKeyB,TValue&gt;.
        /// </summary>
        IEnumerable<TKeyB> IReadOnlyTwoKeyDictionary<TKeyA, TKeyB, TValue>.BKeys
        {
            get
            {
                if (Bkeys == null)
                {
                    Bkeys = new KeyBCollection(this);
                }
                return Bkeys;
            }
        }

        /// <summary>
        /// Gets the number of A-key/B-Key/value triple contained in the Dictionary&lt;TKeyA,TKeyB,TValue&gt;.
        /// </summary>
        public int Count
        {
            get
            {
                if ((count - freeCount) < 2)
                {
                    return count - freeCount;
                }

                return (count - freeCount) / 2;
            }
        }

        /// <summary>
        /// Gets a value that indicates whether the IDictionary has a fixed size.
        /// </summary>
        public bool IsFixedSize { get { return false; } }

        /// <summary>
        /// Gets a value that indicates whether the dictionary is read-only.
        /// </summary>
        public bool IsReadOnly { get { return false; } }

        /// <summary>
        /// Gets a value that indicates whether access to the ICollection is synchronized(thread safe).
        /// </summary>
        public bool IsSynchronized { get { return false; } }

        /// <summary>
        /// Gets an object that can be used to synchronize access to the ICollection.
        /// </summary>
        public object SyncRoot
        {
            get
            {
                if (_syncRoot == null)
                {
                    System.Threading.Interlocked.CompareExchange<Object>(ref _syncRoot, new Object(), null);
                }
                return _syncRoot;
            }
        }

        /// <summary>
        /// Gets a collection containing the values in the Dictionary&lt;TKeyA,TKeyB,TValue&gt;.
        /// </summary>
        public ICollection<TValue> Values
        {
            get
            {
                if (values == null)
                {
                    values = new ValueCollection(this);
                }
                return values;
            }
        }

        /// <summary>
        ///  Gets a collection containing the values in the Dictionary&lt;TKeyA,TKeyB,TValue&gt;.
        /// </summary>
        ICollection ITwoKeyDictionary.Values
        {
            get
            {
                if (values == null)
                {
                    values = new ValueCollection(this);
                }
                return values;
            }
        }

        /// <summary>
        /// Gets a collection containing the values of the IReadOnlyDictionary&lt;TKeyA,TKeyB,TValue&gt;.
        /// </summary>
        IEnumerable<TValue> IReadOnlyTwoKeyDictionary<TKeyA, TKeyB, TValue>.Values
        {
            get
            {
                if (values == null) values = new ValueCollection(this);
                return values;
            }
        }

        #endregion Properties

        #region Indexers

        /// <summary>
        /// Gets or sets the value with the specified key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object this[object key]
        {
            get
            {

                if (key is TKeyA)
                {
                    TKeyA ak = (TKeyA)key;
                    if (this.Contains(ak))
                    {
                        int i = FindEntry(ak);
                        if (i >= 0)
                        {
                            return entries[i].node.Next.Value; 
                        }
                        else
                        {
                            throw new KeyNotFoundException();
                        }
                    }
                }

                if (key is TKeyB)
                {
                    TKeyB bk = (TKeyB)key;
                    if (this.Contains(bk))
                    {
                        int i = FindEntry(bk);
                        if (i >= 0)
                        {
                            return entries[i].node.Value;
                        }
                        else
                        {
                            throw new KeyNotFoundException();
                        }
                    }
                }
                else
                {
                    throw new KeyNotFoundException();
                }

                return default(TValue);
            }
            set
            {
                if (key is TKeyA)
                {
                    TKeyA ka = (TKeyA)key;
                    if (value is TValue)
                    {
                        TValue tv = (TValue)value;
                        Insert(ka, tv, false);
                    }
                    else
                    {
                        throw new InvalidOperationException();
                    }
                }
                if (key is TKeyB)
                {
                    TKeyB kb = (TKeyB)key;
                    if (value is TValue)
                    {
                        TValue tv = (TValue)value;
                        Insert(kb, tv, false);
                    }
                    else
                    {
                        throw new InvalidOperationException();
                    }

                }
            }
        }

        /// <summary>
        /// Gets or sets the value associated with the specified B-key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public TValue this[TKeyB key]
        {
            get
            {
                int i = FindEntry(key);
                if (i >= 0) return entries[i].node.Value;
                {
                    throw new KeyNotFoundException();
                }
                return default(TValue);
            }
            set
            {
                Insert(key, value, false);
            }
        }

        /// <summary>
        /// Gets or sets the value associated with the specified A-key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public TValue this[TKeyA key]
        {
            get
            {
                int i = FindEntry(key);
                if (i >= 0)
                {
                    return entries[i].node.Next.Value;
                }
                else
                {
                    throw new KeyNotFoundException(String.Format("Key: {0}", key));
                }
                return default(TValue);
            }
            set
            {
                Insert(key, value, false);
            }
        }

        #endregion Indexers

        #region Methods

        /// <summary>
        /// Adds the specified key and value to the dictionary.
        /// </summary>
        /// <param name="keyA">The A-key of the element to add.</param>
        /// <param name="keyB">The B-key of the element to add.</param>
        /// <param name="value">The value of the element to add. The value can be null for reference types.</param>
        public void Add(TKeyA keyA, TKeyB keyB, TValue value)
        {
            Insert(keyA, keyB, value, true);
        }

        /// <summary>
        /// Adds the specified value to the ICollection&lt;T&gt; with the specified key.
        /// </summary>
        /// <param name="item">The TwoKeyValueTriple&lt;TKeyA,TKeyB,TValue&gt; structure representing the key and value to add to the Dictionary&lt;TKeyA,TKeyB,TValue&gt;.</param>
        public void Add(TwoKeyValueTriple<TKeyA, TKeyB, TValue> item)
        {
            Add(item.KeyA, item.KeyB, item.Value); 
        }

        /// <summary>
        /// Adds the specified key and value to the dictionary.
        /// </summary>
        /// <param name="keyA">The object to use as the A-key.</param>
        /// <param name="keyB">The object to use as the B-key.</param>
        /// <param name="value">The object to use as the value.</param>
        public void Add(object keyA, object keyB, object value)
        {
            TKeyA tempKeyA;
            TKeyB tempKeyB;
            TValue tempValue;
            if (keyA == null || keyB == null)
            {
                throw new ArgumentNullException("key is null.");
            }

            try
            {
                tempKeyA = (TKeyA)keyA;
            }
            catch (InvalidCastException)
            {
                throw new ArgumentException(String.Format("Arg_WrongType", keyA, typeof(TKeyA), "TKeyA"));
            }

            try
            {
                tempKeyB = (TKeyB)keyB;
            }
            catch (InvalidCastException)
            {
                throw new ArgumentException(String.Format("Arg_WrongType", keyB, typeof(TKeyB), "TkeyB"));
            }

            try
            {
                tempValue = (TValue)value;
            }
            catch (InvalidCastException)
            {
                throw new ArgumentException(String.Format("Arg_WrongType", value, typeof(TValue), "TValue"));
            }

            Add(tempKeyA, tempKeyB, tempValue);

        }

        /// <summary>
        /// Removes all keys and values from the Dictionary&lt;TKeyA,TKeyB,TValue&gt;.
        /// </summary>
        public void Clear()
        {
            if (count > 0)
            {
                for (int i = 0; i < buckets.Length; i++) buckets[i] = -1;
                Array.Clear(entries, 0, count);
                freeList = -1;
                count = 0;
                freeCount = 0;
                version++;
            }
        }

        private void Clone(TKeyA keyA, TKeyB keyB, TValue value)
        {

            if (buckets == null)
            {
                Initialize(0);
            }

            int indexB;
            int indexA;

            // Get key-A/B hash value and index
            int hashCodeB = comparerB.GetHashCode(keyB) & 0x7FFFFFFF;
            int hashCodeA = comparerA.GetHashCode(keyA) & 0x7FFFFFFF;
            int targetBucketB = hashCodeB % buckets.Length;
            int targetBucketA = hashCodeA % buckets.Length;            

            if (freeCount > 0)
            {
                indexA = freeList;
                freeList = entries[indexA].next;
                freeCount--;

                indexB = freeList;
                freeList = entries[indexB].next;
                freeCount--;
            }
            else
            {
                if (count == entries.Length - 1)
                {
                    Resize();
                    targetBucketA = hashCodeA % buckets.Length;
                    targetBucketB = hashCodeB % buckets.Length;
                }
                indexA = count;
                count++;

                indexB = count;
                count++;
            }

            entries[indexB].hashCode = hashCodeB;
            entries[indexB].next = buckets[targetBucketB];
            entries[indexB].keyA = keyA;
            entries[indexB].keyB = keyB;
            entries[indexB].IsBkey = true;
            entries[indexB].node = new EntryNode(value);
            buckets[targetBucketB] = indexB;
            version++;

            entries[indexA].hashCode = hashCodeA;
            entries[indexA].next = buckets[targetBucketA];
            entries[indexA].keyA = keyA;
            entries[indexA].keyB = keyB;
            entries[indexA].IsBkey = false;
            entries[indexA].node = new EntryNode();
            entries[indexA].node.Next = entries[indexB].node;
            buckets[targetBucketA] = indexA;
            version++;
        }

        /// <summary>
        /// Determines whether the ICollection&lt;T&gt; contains a specific key and value.
        /// </summary>
        /// <param name="item">The KeyValuePair&lt;TKeyA,TKeyB,TValue&gt; structure to locate in the ICollection&lt;T&gt;.</param>
        /// <returns></returns>
        public bool Contains(TwoKeyValueTriple<TKeyA, TKeyB, TValue> item)
        {
            int i = FindEntry(item.KeyB);
            if (i >= 0 && EqualityComparer<TValue>.Default.Equals(entries[i].node.Value, item.Value))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// The key to locate in the IDictionary.
        /// </summary>
        /// <param name="key">The key to locate in the IDictionary.</param>
        /// <returns>true if the IDictionary contains an element with the specified key; otherwise, false.</returns>
        public bool Contains(object key)
        {
            if (key is TKeyA)
            {
                TKeyA ta = (TKeyA)key;
                return ContainsKeyA(ta);
            }
            else
            {
                if (key is TKeyB)
                {
                    TKeyB ta = (TKeyB)key;
                    return this.ContainsKeyB(ta);
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }

        }

        /// <summary>
        ///  Determines whether the Dictionary&lt;TKeyA,TKeyB,TValue&gt; contains the specified A-key.
        /// </summary>
        /// <param name="keyA">The A-key to locate in the Dictionary&lt;TKeyA,TKeyB,TValue&gt;.</param>
        /// <returns>true if the TwoKeyDictionary&lt;TKeyA,TKeyB,TValue&gt; contains an element with the specified key; otherwise, false.</returns>
        public bool ContainsKeyA(TKeyA keyA)
        {
            return FindEntry(keyA) >= 0;
        }

        /// <summary>
        /// Determines whether the Dictionary&lt;TKeyA,TKeyB,TValue&gt; contains the specified B-key.
        /// </summary>
        /// <param name="keyB">The B-key to locate in the Dictionary&lt;TKeyA,TKeyB,TValue&gt;.</param>
        /// <returns>true if the TwoKeyDictionary&lt;TKeyA,TKeyB,TValue&gt; contains an element with the specified key; otherwise, false.</returns>
        public bool ContainsKeyB(TKeyB keyB)
        {
            return FindEntry(keyB) >= 0;
        }

        /// <summary>
        /// Determines whether the Dictionary&lt;TKeyA,TKeyB,TValue&gt; contains a specific value.
        /// </summary>
        /// <param name="value">The value to locate in the Dictionary&lt;TKeyA,TKeyB,TValue&gt;. The value can be null for reference types.</param>
        /// <returns>true if theDictionary&lt;TKeyA,TKeyB,TValue&gt; contains an element with the specified value; otherwise, false.</returns>
        public bool ContainsValue(TValue value)
        {
            if (value == null)
            {
                for (int i = 0; i < count; i++)
                {
                    if (entries[i].hashCode >= 0 && entries[i].node == null) return true;
                }
            }
            else
            {
                EqualityComparer<TValue> c = EqualityComparer<TValue>.Default;
                for (int i = 0; i < count; i++)
                {
                    if (entries[i].hashCode >= 0 && c.Equals(entries[i].node.Value, value)) return true;
                }
            }
            return false;
        }

        private int FindEntry(TKeyB key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("KeyB");
            }

            if (buckets != null)
            {
                int hashCode = comparerB.GetHashCode(key) & 0x7FFFFFFF;
                for (int i = buckets[hashCode % buckets.Length]; i >= 0; i = entries[i].next)
                {
                    if (entries[i].hashCode == hashCode && comparerB.Equals(entries[i].keyB, key))
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        private int FindEntry(TKeyA key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("KeyA");
            }

            if (buckets != null)
            {
                int hashCode = comparerA.GetHashCode(key) & 0x7FFFFFFF;
                for (int i = buckets[hashCode % buckets.Length]; i >= 0; i = entries[i].next)
                {
                    if (entries[i].hashCode == hashCode && comparerA.Equals(entries[i].keyA, key))
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        /// <summary>
        /// Copies the elements of the ICollection&lt;T&gt; to an array of type TwoKeyValueTriple&lt;TKeyA,TKeyB,TValue&gt;, starting at the specified array index.
        /// </summary>
        /// <param name="array">The one-dimensional array of type TwoKeyValueTriple&lt;TKeyA,TKeyB,TValue&gt; that is the destination of the TwoKeyValueTriple&lt;TKeyA,TKeyB,TValue&gt; elements copied from the ICollection&lt;T&gt;. The array must have zero-based indexing.</param>
        /// <param name="index">The zero-based index in array at which copying begins.</param>
        public void CopyTo(TwoKeyValueTriple<TKeyA, TKeyB, TValue>[] array, int index)
        {
            if (array == null)
            {
                throw new ArgumentNullException("Array");
            }

            if (index < 0 || index > array.Length)
            {
                throw new ArgumentOutOfRangeException($"Index: {index}", new ArgumentOutOfRangeException("Need Non Negative Number."));
            }

            if (array.Length - index < Count)
            {
                throw new ArgumentException("Array Plus Off Too Small");
            }

            int count = this.count;
            Entry[] entries = this.entries;
            for (int i = 0; i < count; i++)
            {
                if (entries[i].IsBkey)
                {
                    array[index++] = new TwoKeyValueTriple<TKeyA, TKeyB, TValue>(entries[i].keyA, entries[i].keyB, entries[i].node.Value);

                }                
            }
        }

        /// <summary>
        /// Copies the elements of the ICollection&lt;T&gt; to an array, starting at the specified array index.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of the elements copied from ICollection&lt;T&gt;. The array must have zero-based indexing.</param>
        /// <param name="index">The zero-based index in array at which copying begins.</param>
        /// <remarks>This method is not implemented</remarks>
        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the Dictionary&lt;TKeyA,TKeyB,TValue&gt;.
        /// </summary>
        /// <returns>A TwoKeyValueTriple&lt;TKeyA,TKeyB,TValue&gt;. Enumerator structure for the Dictionary&lt;TKeyA,TKeyB,TValue&gt;.</returns>
        public IEnumerator<TwoKeyValueTriple<TKeyA, TKeyB, TValue>> GetEnumerator()
        {
            return new Enumerator(this, Enumerator.KeyValueTriple);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the Dictionary&lt;TKeyA,TKeyB,TValue&gt;.
        /// </summary>
        /// <returns>>A Dictionary&lt;TKeyA,TKeyB,TValue&gt;.Enumerator structure for the Dictionary&lt;TKeyA,TKeyB,TValue&gt;.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this, Enumerator.KeyValueTriple);
        }

        /// <summary>
        /// Returns an IDictionaryEnumerator for the IDictionary.
        /// </summary>
        /// <returns>An IDictionaryEnumerator for the IDictionary.</returns>
        ITwoKeyDictionaryEnumerator ITwoKeyDictionary.GetEnumerator()
        {
            return new Enumerator(this, Enumerator.KeyValueTriple);
        }

        /// <summary>
        /// Implements the ISerializable interface and returns the data needed to serialize the Dictionary&lt;TKeyA,TKeyB,TValue&gt; instance.
        /// </summary>
        /// <param name="info">A SerializationInfo object that contains the information required to serialize the Dictionary&lt;TKeyA,TKeyB,TValue&gt; instance.</param>
        /// <param name="context">A StreamingContext structure that contains the source and destination of the serialized stream associated with the Dictionary&lt;TKeyA,TKeyB,TValue&gt; instance.</param>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException(info.ToString());
            }
            info.AddValue(VersionName, version);
            info.AddValue(ComparerName, HashHelpers.GetEqualityComparerForSerialization(comparerA), typeof(IEqualityComparer<TKeyA>));
            info.AddValue(ComparerName, HashHelpers.GetEqualityComparerForSerialization(comparerB), typeof(IEqualityComparer<TKeyB>));
            info.AddValue(HashSizeName, buckets == null ? 0 : buckets.Length); //This is the length of the bucket array.
            if (buckets != null)
            {
                TwoKeyValueTriple<TKeyA, TKeyB, TValue>[] array = new TwoKeyValueTriple<TKeyA, TKeyB, TValue>[Count];
                CopyTo(array, 0);
                info.AddValue(KeyValuePairsName, array, typeof(TwoKeyValueTriple<TKeyA, TKeyB, TValue>[]));
            }
        }

        // This is a convenience method for the internal callers that were converted from using Hashtable.
        // Many were combining key doesn't exist and key exists but null value (for non-value types) checks.
        // This allows them to continue getting that behavior with minimal code delta. This is basically
        // TryGetValue without the out param
        internal TValue GetValueOrDefault(TKeyA key)
        {
            int i = FindEntry(key);
            if (i >= 0)
            {
                return entries[i].node.Next.Value;
            }
            return default(TValue);
        }

        internal TValue GetValueOrDefault(TKeyB key)
        {
            int i = FindEntry(key);
            if (i >= 0)
            {
                return entries[i].node.Value;
            }
            return default(TValue);
        }

        private void Initialize(int capacity)
        {
            int size = HashHelpers.GetPrime(capacity);
            buckets = new int[size];
            for (int i = 0; i < buckets.Length; i++)
            {
                buckets[i] = -1;
            }
            entries = new Entry[size];
            freeList = -1;
        }

        private void Insert(TKeyA keyA, TKeyB keyB, TValue value, bool add)
        {

            if (keyA == null || keyB == null)
            {
                throw new ArgumentNullException("Key is null");
            }

            if (buckets == null)
            {
                Initialize(0);
            }

            int indexB;
            int indexA;

            // Get key-A/B hash value and index
            int hashCodeB = comparerB.GetHashCode(keyB) & 0x7FFFFFFF;
            int hashCodeA = comparerA.GetHashCode(keyA) & 0x7FFFFFFF;
            int targetBucketB = hashCodeB % buckets.Length;
            int targetBucketA = hashCodeA % buckets.Length;

            // checking for duplicate keyAs
            for (int i = buckets[targetBucketA]; i >= 0; i = entries[i].next)
            {
                if (entries[i].hashCode == hashCodeA && comparerA.Equals(entries[i].keyA, keyA))
                {
                    if (add)
                    {
                        throw new ArgumentException("Adding Duplicate AKey");
                    }
                    entries[i].node.Value = value;
                    version++;
                    return;
                }
            }

            // checking for duplicate keyBs. If not add then update value at key;
            for (int i = buckets[targetBucketB]; i >= 0; i = entries[i].next)
            {
                if (entries[i].hashCode == hashCodeB && comparerB.Equals(entries[i].keyB, keyB))
                {
                    if (add)
                    {
                        throw new ArgumentException("Adding Duplicate BKey");
                    }
                    entries[i].node.Value = value;
                    version++;
                    return;
                }
            }

            if (freeCount > 0)
            {
                indexA = freeList;
                freeList = entries[indexA].next;
                freeCount--;

                indexB = freeList;
                freeList = entries[indexB].next;
                freeCount--;
            }
            else
            {
                if (count == entries.Length - 1)
                {
                    Resize();
                    targetBucketA = hashCodeA % buckets.Length;
                    targetBucketB = hashCodeB % buckets.Length;
                }
                indexA = count;
                count++;

                indexB = count;
                count++;
            }

            entries[indexB].hashCode = hashCodeB;
            entries[indexB].next = buckets[targetBucketB];
            entries[indexB].keyA = keyA;
            entries[indexB].keyB = keyB;
            entries[indexB].IsBkey = true;
            entries[indexB].node = new EntryNode(value);
            buckets[targetBucketB] = indexB;
            version++;

            entries[indexA].hashCode = hashCodeA;
            entries[indexA].next = buckets[targetBucketA];
            entries[indexA].keyA = keyA;
            entries[indexA].keyB = keyB;
            entries[indexA].IsBkey = false;
            entries[indexA].node = new EntryNode();
            entries[indexA].node.Next = entries[indexB].node;
            buckets[targetBucketA] = indexA;
            version++;
        }

        private void Insert(TKeyA keyA, TValue value, bool add)
        {

            if (keyA == null)
            {
                throw new ArgumentNullException("Key is null");
            }

            if (FindEntry(keyA) == 0)
            {
                throw new ArgumentException("KeyA not member of set");
            }

            int hashCode = comparerA.GetHashCode(keyA) & 0x7FFFFFFF;
            int targetBucket = hashCode % buckets.Length;

            for (int i = buckets[targetBucket]; i >= 0; i = entries[i].next)
            {
                if (entries[i].hashCode == hashCode && comparerA.Equals(entries[i].keyA, keyA))
                {
                    entries[i].node.Next.Value = value;
                    version++;
                    return;
                }
            }
        }

        private void Insert(TKeyB keyB, TValue value, bool add)
        {

            if (keyB == null)
            {
                throw new ArgumentNullException("Key is null");
            }

            if (FindEntry(keyB) == 0)
            {
                throw new ArgumentException("KeyA not member of set");
            }

            int hashCode = comparerB.GetHashCode(keyB) & 0x7FFFFFFF;
            int targetBucket = hashCode % buckets.Length;

            for (int i = buckets[targetBucket]; i >= 0; i = entries[i].next)
            {
                if (entries[i].hashCode == hashCode && comparerB.Equals(entries[i].keyB, keyB))
                {
                    entries[i].node.Value = value;
                    version++;
                    return;
                }
            }
        }

        private static bool IsCompatibleKeyA(object key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("Object Key.");
            }
            return (key is TKeyA);
        }

        private static bool IsCompatibleKeyB(object key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("Object Key.");
            }
            return (key is TKeyB);
        }

        /// <summary>
        /// Implements the ISerializable interface and raises the deserialization event when the deserialization is complete.
        /// </summary>
        /// <param name="sender">The source of the deserialization event.</param>
        public void OnDeserialization(object sender)
        {
            SerializationInfo siInfo;
            HashHelpers.SerializationInfoTable.TryGetValue(this, out siInfo);

            if (siInfo == null)
            {
                // It might be necessary to call OnDeserialization from a container if the container object also implements
                // OnDeserialization. However, remoting will call OnDeserialization again.
                // We can return immediately if this function is called twice. 
                // Note we set remove the serialization info from the table at the end of this method.
                return;
            }

            int realVersion = siInfo.GetInt32(VersionName);
            int hashsize = siInfo.GetInt32(HashSizeName);
            comparerA = (IEqualityComparer<TKeyA>)siInfo.GetValue(ComparerName, typeof(IEqualityComparer<TKeyA>));
            comparerB = (IEqualityComparer<TKeyB>)siInfo.GetValue(ComparerName, typeof(IEqualityComparer<TKeyB>));

            if (hashsize != 0)
            {
                buckets = new int[hashsize];
                for (int i = 0; i < buckets.Length; i++) buckets[i] = -1;
                entries = new Entry[hashsize];
                freeList = -1;

                TwoKeyValueTriple<TKeyA, TKeyB, TValue>[] array = (TwoKeyValueTriple<TKeyA, TKeyB, TValue>[])
                    siInfo.GetValue(KeyValuePairsName, typeof(TwoKeyValueTriple<TKeyA, TKeyB, TValue>[]));

                if (array == null)
                {
                    throw new SerializationException("Missing Keys.");
                }

                for (int i = 0; i < array.Length; i++)
                {
                    if (array[i].KeyA == null || array[i].KeyB == null)
                    {
                        throw new SerializationException("Null Key.");
                    }

                    Insert(array[i].KeyA, array[i].KeyB, array[i].Value, true);
                }
            }
            else
            {
                buckets = null;
            }

            version = realVersion;
            HashHelpers.SerializationInfoTable.Remove(this);
        }

        /// <summary>
        /// Removes a A-key, B-key and value from the dictionary.
        /// </summary>
        /// <param name="item">The TwoKeyValueTriple&lt;TKeyA,TKeyB,TValue&gt; structure representing the key and value to remove from the Dictionary&lt;TKeyA,TKeyB,TValue&gt;.</param>
        /// <returns>true if the key and value represented by TwoKeyValueTriple is successfully found and removed; otherwise, false. This method returns false if keyValuePair is not found in the ICollection&lt;T&gt;.</returns>
        public bool Remove(TwoKeyValueTriple<TKeyA, TKeyB, TValue> item)
        {
            int i = FindEntry(item.KeyB);
            if(i >= 0)
            {
                return RemoveKeyB(item.KeyB);
            }
            return false;
        }

        /// <summary>
        /// Removes the value with the specified A-key from the Dictionary&lt;TKeyA,TKeyB,TValue&gt;.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <returns>true if the element is successfully found and removed; otherwise, false. This method returns false if key is not found in the Dictionary&lt;TKeyA,TKeyB,TValue&gt;.</returns>
        public bool Remove(TKeyA key)
        {
            return RemoveKeyA(key);
        }

        /// <summary>
        /// Removes the value with the specified B-key from the Dictionary&lt;TKeyA,TKeyB,TValue&gt;.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <returns>true if the element is successfully found and removed; otherwise, false. This method returns false if key is not found in the Dictionary&lt;TKeyA,TKeyB,TValue&gt;.</returns>
        public bool Remove(TKeyB key)
        {
            return RemoveKeyB(key);
        }

        /// <summary>
        /// Removes the element with the specified key from the IDictionary.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        public void Remove(object key)
        {

            if (IsCompatibleKeyA(key))
            {
                Remove((TKeyA)key);
            }

            if (IsCompatibleKeyB(key))
            {
                Remove((TKeyB)key);
            }
        }

        /// <summary>
        /// Removes the value with the specified A-key from the Dictionary&lt;TKeyA,TKeyB,TValue&gt;.
        /// </summary>
        /// <param name="keyA">The key of the element to remove.</param>
        /// <returns>true if the element is successfully found and removed; otherwise, false. This method returns false if key is not found in the Dictionary&lt;TKeyA,TKeyB,TValue&gt;.</returns>
        public bool RemoveKeyA(TKeyA keyA)
        {
            if (keyA == null)
            {
                throw new ArgumentNullException("KeyA");
            }

            if (buckets != null)
            {
                int hashCode = comparerA.GetHashCode(keyA) & 0x7FFFFFFF;
                int bucket = hashCode % buckets.Length;
                int last = -1;
                TKeyB keyb;
                for (int i = buckets[bucket]; i >= 0; last = i, i = entries[i].next)
                {
                    if (entries[i].hashCode == hashCode && comparerA.Equals(entries[i].keyA, keyA))
                    {
                        if (last < 0)
                        {
                            buckets[bucket] = entries[i].next;
                        }
                        else
                        {
                            entries[last].next = entries[i].next;
                        }
                        entries[i].hashCode = -1;
                        entries[i].next = freeList;
                        keyb = entries[i].keyB;
                        entries[i].keyA = default(TKeyA);
                        entries[i].keyB = default(TKeyB);
                        entries[i].node.Value = default(TValue);
                        entries[i].IsBkey = false;
                        freeList = i;
                        freeCount++;
                        version++;
                        Remove(keyb);
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Removes the value with the specified B-key from the Dictionary&lt;TKeyA,TKeyB,TValue&gt;.
        /// </summary>
        /// <param name="keyB">The key of the element to remove.</param>
        /// <returns>true if the element is successfully found and removed; otherwise, false. This method returns false if key is not found in the Dictionary&lt;TKeyA,TKeyB,TValue&gt;.</returns>
        public bool RemoveKeyB(TKeyB keyB)
        {
            if (keyB == null)
            {
                throw new ArgumentNullException("KeyB");
            }

            if (buckets != null)
            {
                int hashCode = comparerB.GetHashCode(keyB) & 0x7FFFFFFF;
                int bucket = hashCode % buckets.Length;
                int last = -1;
                TKeyA keyA;
                for (int i = buckets[bucket]; i >= 0; last = i, i = entries[i].next)
                {
                    if (entries[i].hashCode == hashCode && comparerB.Equals(entries[i].keyB, keyB))
                    {
                        if (last < 0)
                        {
                            buckets[bucket] = entries[i].next;
                        }
                        else
                        {
                            entries[last].next = entries[i].next;
                        }
                        entries[i].hashCode = -1;
                        entries[i].next = freeList;
                        keyA = entries[i].keyA;
                        entries[i].keyA = default(TKeyA);
                        entries[i].keyB = default(TKeyB);
                        entries[i].node.Value = default(TValue);
                        entries[i].IsBkey = false;
                        freeList = i;
                        freeCount++;
                        version++;
                        Remove(keyA);
                        return true;
                    }
                }
            }
            return false;
        }

        private void Resize()
        {
            Resize(HashHelpers.ExpandPrime(count), false);
        }

        private void Resize(int newSize, bool forceNewHashCodes)
        {
            Contract.Assert(newSize >= entries.Length);
            int[] newBuckets = new int[newSize];
            for (int i = 0; i < newBuckets.Length; i++)
            {
                newBuckets[i] = -1;
            }

            Entry[] newEntries = new Entry[newSize];
            Array.Copy(entries, 0, newEntries, 0, count);

            if (forceNewHashCodes)
            {
                for (int i = 0; i < count; i++)
                {
                    if (newEntries[i].hashCode != -1)
                    {
                        if (newEntries[i].IsBkey)
                        {
                            newEntries[i].hashCode = (comparerB.GetHashCode(newEntries[i].keyB) & 0x7FFFFFFF);
                        }
                        else
                        {
                            newEntries[i].hashCode = (comparerA.GetHashCode(newEntries[i].keyA) & 0x7FFFFFFF);
                        }
                    }
                }
            }

            for (int i = 0; i < count; i++)
            {
                if (newEntries[i].hashCode >= 0)
                {
                    int bucket = newEntries[i].hashCode % newSize;
                    newEntries[i].next = newBuckets[bucket];
                    newBuckets[bucket] = i;
                }
            }
            buckets = newBuckets;
            entries = newEntries;
        }

        /// <summary>
        /// Gets the value associated with the specified A-key.
        /// </summary>
        /// <param name="keyA">The key of the value to get.</param>
        /// <param name="value">When this method returns, contains the value associated with the specified key, if the key is found; otherwise, the default value for the type of the value parameter. This parameter is passed uninitialized.</param>
        /// <returns>true if the Dictionary&lt;TKeyA,TKeyB,TValue&gt; contains an element with the specified key; otherwise, false.</returns>
        public bool TryGetValueKeyA(TKeyA keyA, out TValue value)
        {
            int i = FindEntry(keyA);
            if (i >= 0)
            {
                value = entries[i].node.Next.Value;
                return true;
            }
            value = default(TValue);
            return false;
        }

        /// <summary>
        /// Gets the value associated with the specified B-key.
        /// </summary>
        /// <param name="keyB">The key of the value to get.</param>
        /// <param name="value">When this method returns, contains the value associated with the specified key, if the key is found; otherwise, the default value for the type of the value parameter. This parameter is passed uninitialized.</param>
        /// <returns>true if the Dictionary&lt;TKeyA,TKeyB,TValue&gt; contains an element with the specified key; otherwise, false.</returns>
        public bool TryGetValueKeyB(TKeyB keyB, out TValue value)
        {
            int i = FindEntry(keyB);
            if (i >= 0)
            {
                value = entries[i].node.Value;
                return true;
            }
            value = default(TValue);
            return false;
        }

        #endregion Methods

        /// <summary>
        /// Enumerates the elements of a Dictionary&lt;TKeyA,TKeyB,TValue&gt;.
        /// </summary>
        [Serializable]
        public struct Enumerator : IEnumerator<TwoKeyValueTriple<TKeyA, TKeyB, TValue>>, ITwoKeyDictionaryEnumerator
        {

            #region Fields

            internal const int DictEntry = 1;
            internal const int KeyValueTriple = 2;
            private TwoKeyValueTriple<TKeyA, TKeyB, TValue> current;
            private TwoKeyDictionary<TKeyA, TKeyB, TValue> dictionary;
            private int getEnumeratorRetType;
            private int index;
            private int version;

            #endregion Fields

            #region Constructors

            internal Enumerator(TwoKeyDictionary<TKeyA, TKeyB, TValue> dictionary, int getEnumeratorRetType)
            {
                this.dictionary = dictionary;
                version = dictionary.version;
                index = 0;
                this.getEnumeratorRetType = getEnumeratorRetType;
                current = new TwoKeyValueTriple<TKeyA, TKeyB, TValue>();
            }

            #endregion Constructors

            #region Properties

            /// <summary>
            /// The element in the Dictionary&lt;TKeyA,TKeyB,TValue&gt; at the current position of the enumerator.
            /// </summary>
            public TwoKeyValueTriple<TKeyA, TKeyB, TValue> Current
            {
                get { return current; }
            }

            /// <summary>
            /// The element in the collection at the current position of the enumerator, as an Object.
            /// </summary>
            object IEnumerator.Current
            {
                get
                {
                    if (index == 0 || (index == dictionary.count + 1))
                    {
                        throw new InvalidOperationException("Enumerator Operation Cant Happen");
                    }

                    if (getEnumeratorRetType == DictEntry)
                    {
                        return new TwoKeyDictionaryEntry(current.KeyA, current.KeyB, current.Value);
                    }
                    else
                    {
                        return new TwoKeyValueTriple<TKeyA, TKeyB, TValue>(current.KeyA, current.KeyB, current.Value);
                    }
                }
            }

            /// <summary>
            /// The element in the dictionary at the current position of the enumerator, as a DictionaryEntry.
            /// </summary>
            TwoKeyDictionaryEntry ITwoKeyDictionaryEnumerator.Entry
            {
                get
                {
                    if (index == 0 || (index == dictionary.count + 1))
                    {
                        throw new InvalidOperationException("Enumerator Operation Cant Happen");
                    }

                    return new TwoKeyDictionaryEntry(current.KeyA, current.KeyB, current.Value);
                }
            }

            /// <summary>
            /// Gets the A-key of the element at the current position of the enumerator.
            /// </summary>
            /// <exception cref="InvalidOperationException">The enumerator is positioned before the first element of the collection or after the last element.</exception>
            object ITwoKeyDictionaryEnumerator.AKey
            {
                get
                {
                    if (index == 0 || (index == dictionary.count + 1))
                    {
                        throw new InvalidOperationException("Enumerator Operation Cant Happen");
                    }

                    return current.KeyA;
                }
            }

            /// <summary>
            /// Gets the B-key of the element at the current position of the enumerator.
            /// </summary>
            /// /// <exception cref="InvalidOperationException">The enumerator is positioned before the first element of the collection or after the last element.</exception>
            object ITwoKeyDictionaryEnumerator.BKey
            {
                get
                {
                    if (index == 0 || (index == dictionary.count + 1))
                    {
                        throw new InvalidOperationException("Enumerator Operation Cant Happen");
                    }

                    return current.KeyB;
                }
            }

            /// <summary>
            /// Gets the value of the element at the current position of the enumerator.
            /// </summary>
            /// <exception cref="InvalidOperationException">The enumerator is positioned before the first element of the collection or after the last element.</exception>
            object ITwoKeyDictionaryEnumerator.Value
            {
                get
                {
                    if (index == 0 || (index == dictionary.count + 1))
                    {
                        throw new InvalidOperationException("Enumerator Operation Cant Happen");
                    }

                    return current.Value;
                }
            }

            #endregion Properties

            #region Methods

            /// <summary>
            /// Releases all resources used by the TwoKeyDictionary&lt;TKeyA,TKeyB,TValue&gt;.Enumerator.
            /// </summary>
            public void Dispose()
            {
            }

            /// <summary>
            /// Advances the enumerator to the next element of the Dictionary&lt;TKeyA,TKeyB,TValue&gt;.
            /// </summary>
            /// <returns>true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.</returns>
            /// <exception cref="InvalidOperationException">The collection was modified after the enumerator was created.</exception>
            public bool MoveNext()
            {
                if (version != dictionary.version)
                {
                    throw new InvalidOperationException("Enumerator Failed Version");
                }

                // Use unsigned comparison since we set index to dictionary.count+1 when the enumeration ends.
                // dictionary.count+1 could be negative if dictionary.count is Int32.MaxValue
                while ((uint)index < (uint)dictionary.count)
                {
                    if (dictionary.entries[index].hashCode >= 0)
                    {
                        Next();
                        index += 2;
                        return true;
                    }
                    index += 2;
                }

                index = dictionary.count + 1;
                current = new TwoKeyValueTriple<TKeyA, TKeyB, TValue>();
                return false;
            }

            private void Next()
            {
                if (!dictionary.entries[index].IsBkey)
                {
                    index ++;
                    Next();
                }
                current = new TwoKeyValueTriple<TKeyA, TKeyB, TValue>(dictionary.entries[index].keyA, dictionary.entries[index].keyB, dictionary.entries[index].node.Value);
            }

            /// <summary>
            /// Sets the enumerator to its initial position, which is before the first element in the collection.
            /// </summary>
            /// <exception cref="InvalidOperationException">The collection was modified after the enumerator was created.</exception>
            void IEnumerator.Reset()
            {
                if (version != dictionary.version)
                {
                    throw new InvalidOperationException("Enumerator Failed Version");
                }

                index = 0;
                current = new TwoKeyValueTriple<TKeyA, TKeyB, TValue>();
            }

            #endregion Methods

        }

        /// <summary>
        /// Represents the collection of A-keys in a Dictionary&lt;TKeyA,TKeyB,TValue&gt;. This class cannot be inherited.
        /// </summary>
        [DebuggerTypeProxy(typeof(TwoKeyDictionaryDebugView<,,>))]
        [DebuggerDisplay("Count = {Count}")]
        [Serializable]
        public sealed class KeyACollection : ICollection<TKeyA>, ICollection, IReadOnlyCollection<TKeyA>
        {

            #region Fields

            private TwoKeyDictionary<TKeyA, TKeyB, TValue> dictionary;

            #endregion Fields

            #region Constructors

            /// <summary>
            /// Initializes a new instance of the Dictionary&lt;TKeyA,TKeyB,TValue&gt;.KeyACollection class that reflects the A-keys in the specified Dictionary&lt;TKeyA,TKeyB,TValue&gt;.
            /// </summary>
            /// <param name="dictionary">The Dictionary&lt;TKeyA,TKeyB,TValue&gt; whose keys are reflected in the new Dictionary&lt;TKeyA,TKeyB,TValue&gt;.KeyACollection.</param>
            public KeyACollection(TwoKeyDictionary<TKeyA, TKeyB, TValue> dictionary)
            {
                if (dictionary == null)
                {
                    throw new ArgumentNullException("TwoKeyDictionary is null.");
                }
                this.dictionary = dictionary;
            }

            #endregion Constructors

            #region Properties

            /// <summary>
            /// Gets the number of elements contained in the Dictionary&lt;TKeyA,TKeyB,TValue&gt;.KeyACollection.
            /// </summary>
            public int Count
            {
                get { return dictionary.Count; }
            }

            /// <summary>
            /// Gets a value indicating whether the ICollection&lt;T&gt; is read-only.
            /// </summary>
            bool ICollection<TKeyA>.IsReadOnly
            {
                get { return true; }
            }

            /// <summary>
            /// Gets a value indicating whether the ICollection&lt;T&gt; is read-only.
            /// </summary>
            public bool IsReadOnly
            {
                get { return true; }
            }

            /// <summary>
            /// Gets a value indicating whether access to the ICollection is synchronized (thread safe).
            /// </summary>
            bool ICollection.IsSynchronized
            {
                get { return false; }
            }

            /// <summary>
            /// Gets an object that can be used to synchronize access to the ICollection.
            /// </summary>
            Object ICollection.SyncRoot
            {
                get { return ((ICollection)dictionary).SyncRoot; }
            }

            #endregion Properties

            #region Methods

            /// <summary>
            /// Adds an item to the ICollection&lt;T&gt;. This implementation always throws NotSupportedException.
            /// </summary>
            /// <param name="item">The object to add to the ICollection&lt;T&gt;.</param>
            void ICollection<TKeyA>.Add(TKeyA item)
            {
                throw new NotSupportedException("NotSupported Key Collection Set");
            }

            /// <summary>
            /// Removes all items from the ICollection&lt;T&gt;. This implementation always throws NotSupportedException.
            /// </summary>
            void ICollection<TKeyA>.Clear()
            {
                throw new NotSupportedException("NotSupported Key Collection Set");
            }

            /// <summary>
            /// Determines whether the ICollection&lt;T&gt; contains a specific value.
            /// </summary>
            /// <param name="item">The object to locate in the ICollection&lt;T&gt;.</param>
            /// <returns>true if item is found in the ICollection&lt;T&gt;; otherwise, false.</returns>
            bool ICollection<TKeyA>.Contains(TKeyA item)
            {
                return dictionary.ContainsKeyA(item);
            }

            /// <summary>
            /// Copies the Dictionary&lt;TKeyA,TKeyB,TValue&gt;.KeyACollection elements to an existing one-dimensional Array, starting at the specified array index.
            /// </summary>
            /// <param name="array">The one-dimensional Array that is the destination of the elements copied from Dictionary&lt;TKeyA,TKeyB,TValue&gt;.KeyACollection. The Array must have zero-based indexing.</param>
            /// <param name="index">The zero-based index in array at which copying begins.</param>
            public void CopyTo(TKeyA[] array, int index)
            {
                if (array == null)
                {
                    throw new ArgumentNullException("Array");
                }

                if (index < 0 || index > array.Length)
                {
                    throw new ArgumentOutOfRangeException($"Index: {index}", new ArgumentOutOfRangeException("Need Non-Negative Number"));
                }

                if (array.Length - index < dictionary.Count)
                {
                    throw new ArgumentException("Array too small.");
                }

                int count = dictionary.count;
                Entry[] entries = dictionary.entries;
                for (int i = 0; i < count; i++)
                {
                    if (entries[i].hashCode >= 0)
                    {
                        if (entries[i].IsBkey == false)
                        {
                            array[index++] = entries[i].keyA;
                        }
                    }
                }
            }

            /// <summary>
            /// Copies the elements of the ICollection to an Array, starting at a particular Array index.
            /// </summary>
            /// <param name="array">The one-dimensional Array that is the destination of the elements copied from ICollection. The Array must have zero-based indexing.</param>
            /// <param name="index">The zero-based index in array at which copying begins.</param>
            void ICollection.CopyTo(Array array, int index)
            {
                if (array == null)
                {
                    throw new ArgumentNullException("Array");
                }

                if (array.Rank != 1)
                {
                    throw new ArgumentException("MultiDim Not Supported");
                }

                if (array.GetLowerBound(0) != 0)
                {
                    throw new ArgumentException("Non Zero Lower Bound");
                }

                if (index < 0 || index > array.Length)
                {
                    throw new ArgumentOutOfRangeException($"Index: {index}", new ArgumentOutOfRangeException("Need Non-Negative Number"));
                }

                if (array.Length - index < dictionary.Count)
                {
                    throw new ArgumentException("Argument Array Plus Off Too Small");
                }

                TKeyA[] keys = array as TKeyA[];
                if (keys != null)
                {
                    CopyTo(keys, index);
                }
                else
                {
                    object[] objects = array as object[];
                    if (objects == null)
                    {
                        throw new ArgumentException("Argument Invalid Array Type");
                    }

                    int count = dictionary.count;
                    Entry[] entries = dictionary.entries;
                    try
                    {
                        for (int i = 0; i < count; i++)
                        {
                            if (entries[i].hashCode >= 0) objects[index++] = entries[i].keyA;
                        }
                    }
                    catch (ArrayTypeMismatchException)
                    {
                        throw new ArgumentException("Argument Invalid Array Type");
                    }
                }
            }

            /// <summary>
            /// Returns an enumerator that iterates through the Dictionary&lt;TKeyA,TKeyB,TValue&gt;.KeyACollection.
            /// </summary>
            /// <returns>A Dictionary&lt;TKeyA,TKeyB,TValue&gt;.KeyACollection.Enumerator for the Dictionary&lt;TKeyA,TKeyB,TValue&gt;.KeyACollection.</returns>
            public Enumerator GetEnumerator()
            {
                return new Enumerator(dictionary);
            }

            /// <summary>
            /// Returns an enumerator that iterates through a collection.
            /// </summary>
            /// <returns>An IEnumerator that can be used to iterate through the collection.</returns>
            IEnumerator<TKeyA> IEnumerable<TKeyA>.GetEnumerator()
            {
                return new Enumerator(dictionary);
            }

            /// <summary>
            ///  Returns an enumerator that iterates through a collection.
            /// </summary>
            /// <returns>An IEnumerator that can be used to iterate through the collection.</returns>
            IEnumerator IEnumerable.GetEnumerator()
            {
                return new Enumerator(dictionary);
            }

            /// <summary>
            /// Removes the first occurrence of a specific object from the ICollection&lt;T&gt;. This implementation always throws NotSupportedException.
            /// </summary>
            /// <param name="item">The object to remove from the ICollection&lt;T&gt;.</param>
            /// <returns>true if item was successfully removed from the ICollection&lt;T&gt;; otherwise, false. This method also returns false if item was not found in the original ICollection&lt;T&gt;.</returns>
            bool ICollection<TKeyA>.Remove(TKeyA item)
            {
                throw new NotSupportedException("NotSupported Key Collection Set");
            }

            #endregion Methods

            #region Structs + Classes + Enums

            /// <summary>
            /// Enumerates the elements of a TwoKeyDictionary&lt;TKeyA,TKeyB,TValue&gt;.KeyACollection.
            /// </summary>
            [Serializable]
            public struct Enumerator : IEnumerator<TKeyA>, System.Collections.IEnumerator
            {

                #region Fields

                private TKeyA currentKey;
                private TwoKeyDictionary<TKeyA, TKeyB, TValue> dictionary;
                private int index;
                private int version;

                #endregion Fields

                #region Constructors
                /// <summary>
                ///  Enumerates the elements of a TwoKeyDictionary&lt;TKeyA,TKeyB,TValue&gt;.KeyACollection.
                /// </summary>
                /// <param name="dictionary">TwoKeyDictionary&lt;TKeyA,TKeyB,TValue&gt; to enumerate TKeyA.</param>
                internal Enumerator(TwoKeyDictionary<TKeyA, TKeyB, TValue> dictionary)
                {
                    this.dictionary = dictionary;
                    version = dictionary.version;
                    index = 0;
                    currentKey = default(TKeyA);
                }

                #endregion Constructors

                #region Properties

                /// <summary>
                /// Gets the element at the current position of the enumerator.
                /// </summary>
                public TKeyA Current
                {
                    get
                    {
                        return currentKey;
                    }
                }

                /// <summary>
                /// Gets the element at the current position of the enumerator.
                /// </summary>
                Object System.Collections.IEnumerator.Current
                {
                    get
                    {
                        if (index == 0 || (index == dictionary.count + 1))
                        {
                            throw new InvalidOperationException("InvalidOperation Enumerator Operation Can't Happen");
                        }

                        return currentKey;
                    }
                }

                #endregion Properties

                #region Methods

                /// <summary>
                /// Releases all resources used by the TwoKeyDictionary&lt;TKeyA,TKeyB,TValue&gt;.ValueCollection.Enumerator.
                /// </summary>
                public void Dispose()
                {
                }

                /// <summary>
                /// Advances the enumerator to the next element of the TwoKeyDictionary&lt;TKeyA,TKeyB,TValue&gt;.KeyACollection.
                /// </summary>
                /// <returns>true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.</returns>
                public bool MoveNext()
                {
                    if (version != this.dictionary.version)
                    {
                        throw new InvalidOperationException("InvalidOperation Enumerator Failed Version");
                    }

                    while ((uint)index < (uint)this.dictionary.count)
                    {
                        if (dictionary.entries[index].hashCode >= 0)
                        {
                            Next();
                            index += 2;
                            return true;
                        }
                        index += 2;
                    }

                    index = this.dictionary.count + 1;
                    currentKey = default(TKeyA);
                    return false;
                }

                private void Next()
                {
                    if (dictionary.entries[index].IsBkey)
                    {
                        index++;
                        Next();
                    }
                    currentKey = dictionary.entries[index].keyA;
                }

                /// <summary>
                /// Sets the enumerator to its initial position, which is before the first element in the collection.
                /// </summary>
                void System.Collections.IEnumerator.Reset()
                {
                    if (version != dictionary.version)
                    {
                        throw new InvalidOperationException("InvalidOperation Enumerator Failed Version");
                    }

                    index = 0;
                    currentKey = default(TKeyA);
                }

                #endregion Methods

            }

            #endregion Structs + Classes + Enums

        }

        /// <summary>
        /// Represents the collection of A-keys in a Dictionary&lt;TKeyA,TKeyB,TValue&gt;. This class cannot be inherited.
        /// </summary>
        [DebuggerTypeProxy(typeof(TwoKeyDictionaryDebugView<,,>))]
        [DebuggerDisplay("Count = {Count}")]
        [Serializable]
        public sealed class KeyBCollection : ICollection<TKeyB>, ICollection, IReadOnlyCollection<TKeyB>
        {

            #region Fields

            private TwoKeyDictionary<TKeyA, TKeyB, TValue> dictionary;

            #endregion Fields

            #region Constructors

            /// <summary>
            /// Initializes a new instance of the Dictionary&lt;TKeyA,TKeyB,TValue&gt;.KeyBCollection class that reflects the A-keys in the specified Dictionary&lt;TKeyA,TKeyB,TValue&gt;
            /// </summary>
            /// <param name="dictionary">The Dictionary&lt;TKeyA,TKeyB,TValue&gt; whose keys are reflected in the new Dictionary&lt;TKeyA,TKeyB,TValue&gt;.KeyBCollection.</param>
            public KeyBCollection(TwoKeyDictionary<TKeyA, TKeyB, TValue> dictionary)
            {
                if (dictionary == null)
                {
                    throw new ArgumentNullException("TwoKeyDictionary is null.");
                }
                this.dictionary = dictionary;
            }

            #endregion Constructors

            #region Properties

            /// <summary>
            /// Gets the number of elements contained in the Dictionary&lt;TKeyA,TKeyB,TValue&gt;.KeyBCollection.
            /// </summary>
            public int Count
            {
                get { return dictionary.Count; }
            }

            /// <summary>
            /// Gets a value indicating whether the ICollection&lt;T&gt; is read-only.
            /// </summary>
            bool ICollection<TKeyB>.IsReadOnly
            {
                get { return true; }
            }

            /// <summary>
            /// Gets a value indicating whether the ICollection&lt;T&gt; is read-only.
            /// </summary>
            public bool IsReadOnly { get { return true; } }

            /// <summary>
            /// Gets a value indicating whether access to the ICollection is synchronized (thread safe).
            /// </summary>
            bool ICollection.IsSynchronized
            {
                get { return false; }
            }

            /// <summary>
            /// Gets an object that can be used to synchronize access to the ICollection.
            /// </summary>
            Object ICollection.SyncRoot
            {
                get { return ((ICollection)dictionary).SyncRoot; }
            }

            #endregion Properties

            #region Methods

            /// <summary>
            /// Adds an item to the ICollection&lt;T&gt;. This implementation always throws NotSupportedException.
            /// </summary>
            /// <param name="item">The object to add to the ICollection&lt;T&gt;.</param>
            void ICollection<TKeyB>.Add(TKeyB item)
            {
                throw new NotSupportedException("NotSupported Key Collection Set");
            }

            /// <summary>
            /// Removes all items from the ICollection&lt;T&gt;. This implementation always throws NotSupportedException.
            /// </summary>
            void ICollection<TKeyB>.Clear()
            {
                throw new NotSupportedException("NotSupported Key Collection Set");
            }

            /// <summary>
            /// Determines whether the ICollection&lt;T&gt; contains a specific value.
            /// </summary>
            /// <param name="item">The object to locate in the ICollection&lt;T&gt;.</param>
            /// <returns>true if item is found in the ICollection&lt;T&gt;; otherwise, false.</returns>
            bool ICollection<TKeyB>.Contains(TKeyB item)
            {
                return dictionary.ContainsKeyB(item);
            }

            /// <summary>
            /// Copies the Dictionary&lt;TKeyA,TKeyB,TValue&gt;.KeyBCollection elements to an existing one-dimensional Array, starting at the specified array index.
            /// </summary>
            /// <param name="array">The one-dimensional Array that is the destination of the elements copied from Dictionary&lt;TKeyA,TKeyB,TValue&gt;.KeyBCollection. The Array must have zero-based indexing.</param>
            /// <param name="index">The zero-based index in array at which copying begins.</param>
            public void CopyTo(TKeyB[] array, int index)
            {
                if (array == null)
                {
                    throw new ArgumentNullException("Array");
                }

                if (index < 0 || index > array.Length)
                {
                    throw new ArgumentOutOfRangeException($"Index: {index}", new ArgumentOutOfRangeException("Need Non-Negative Number"));
                }

                if (array.Length - index < dictionary.Count)
                {
                    throw new ArgumentException("Array too small.");
                }

                int count = dictionary.count;
                Entry[] entries = dictionary.entries;
                for (int i = 0; i < count; i++)
                {
                    if (entries[i].IsBkey == true)
                    {
                        array[index++] = entries[i].keyB;
                    }
                }
            }

            /// <summary>
            /// Copies the elements of the ICollection to an Array, starting at a particular Array index.
            /// </summary>
            /// <param name="array">The one-dimensional Array that is the destination of the elements copied from ICollection. The Array must have zero-based indexing.</param>
            /// <param name="index">The zero-based index in array at which copying begins.</param>
            void ICollection.CopyTo(Array array, int index)
            {
                if (array == null)
                {
                    throw new ArgumentNullException("Array");
                }

                if (array.Rank != 1)
                {
                    throw new ArgumentException("MultiDim Not Supported");
                }

                if (array.GetLowerBound(0) != 0)
                {
                    throw new ArgumentException("Non Zero Lower Bound");
                }

                if (index < 0 || index > array.Length)
                {
                    throw new ArgumentOutOfRangeException($"Index: {index}", new ArgumentOutOfRangeException("Need Non-Negative Number"));
                }

                if (array.Length - index < dictionary.Count)
                {
                    throw new ArgumentException("Argument Array Plus Off Too Small");
                }

                TKeyB[] keys = array as TKeyB[];
                if (keys != null)
                {
                    CopyTo(keys, index);
                }
                else
                {
                    object[] objects = array as object[];
                    if (objects == null)
                    {
                        throw new ArgumentException("Argument Invalid Array Type");
                    }

                    int count = dictionary.count;
                    Entry[] entries = dictionary.entries;
                    try
                    {
                        for (int i = 0; i < count; i++)
                        {
                            if (entries[i].hashCode >= 0) objects[index++] = entries[i].keyB;
                        }
                    }
                    catch (ArrayTypeMismatchException)
                    {
                        throw new ArgumentException("Argument Invalid Array Type");
                    }
                }
            }

            /// <summary>
            /// Returns an enumerator that iterates through the Dictionary&lt;TKeyA,TKeyB,TValue&gt;.KeyBCollection.
            /// </summary>
            /// <returns>A Dictionary&lt;TKeyA,TKeyB,TValue&gt;.KeyBCollection.Enumerator for the Dictionary&lt;TKeyA,TKeyB,TValue&gt;.KeyBCollection.</returns>
            public Enumerator GetEnumerator()
            {
                return new Enumerator(dictionary);
            }

            /// <summary>
            /// Returns an enumerator that iterates through a collection.
            /// </summary>
            /// <returns>An IEnumerator that can be used to iterate through the collection.</returns>
            IEnumerator<TKeyB> IEnumerable<TKeyB>.GetEnumerator()
            {
                return new Enumerator(dictionary);
            }

            /// <summary>
            ///  Returns an enumerator that iterates through a collection.
            /// </summary>
            /// <returns>An IEnumerator that can be used to iterate through the collection.</returns>
            IEnumerator IEnumerable.GetEnumerator()
            {
                return new Enumerator(dictionary);
            }

            /// <summary>
            /// Removes the first occurrence of a specific object from the ICollection&lt;T&gt;. This implementation always throws NotSupportedException.
            /// </summary>
            /// <param name="item">The object to remove from the ICollection&lt;T&gt;.</param>
            /// <returns>true if item was successfully removed from the ICollection&lt;T&gt;; otherwise, false. This method also returns false if item was not found in the original ICollection&lt;T&gt;.</returns>
            bool ICollection<TKeyB>.Remove(TKeyB item)
            {
                throw new NotSupportedException("NotSupported Key Collection Set");
            }

            #endregion Methods

            #region Structs + Classes + Enums

            /// <summary>
            /// Enumerates the elements of a TwoKeyDictionary&lt;TKeyA,TKeyB,TValue&gt;.KeyBCollection.
            /// </summary>
            [Serializable]
            public struct Enumerator : IEnumerator<TKeyB>, System.Collections.IEnumerator
            {

                #region Fields

                private TKeyB currentKey;
                private TwoKeyDictionary<TKeyA, TKeyB, TValue> dictionary;
                private int index;
                private int version;

                #endregion Fields

                #region Constructors

                /// <summary>
                /// Enumerates the elements of a TwoKeyDictionary&lt;TKeyA,TKeyB,TValue&gt;.KeyCollection.
                /// </summary>
                /// <param name="dictionary">TwoKeyDictionary&lt;TKeyA,TKeyB,TValue&gt; to enumerate TKeyB.</param>
                internal Enumerator(TwoKeyDictionary<TKeyA, TKeyB, TValue> dictionary)
                {
                    this.dictionary = dictionary;
                    version = dictionary.version;
                    index = 0;
                    currentKey = default(TKeyB);
                }

                #endregion Constructors

                #region Properties
                /// <summary>
                /// Gets the element at the current position of the enumerator.
                /// </summary>
                public TKeyB Current
                {
                    get
                    {
                        return currentKey;
                    }
                }

                /// <summary>
                /// Gets the element at the current position of the enumerator.
                /// </summary>
                Object System.Collections.IEnumerator.Current
                {
                    get
                    {
                        if (index == 0 || (index == dictionary.count + 1))
                        {
                            throw new InvalidOperationException("InvalidOperation Enumerator Operation Can't Happen");
                        }

                        return currentKey;
                    }
                }

                #endregion Properties

                #region Methods

                /// <summary>
                /// Releases all resources used by the TwoKeyDictionary&lt;TKeyA,TKeyB,TValue&gt;.ValueCollection.Enumerator.
                /// </summary>
                public void Dispose()
                {
                }

                /// <summary>
                /// Advances the enumerator to the next element of the TwoKeyDictionary&lt;TKeyA,TKeyB,TValue&gt;.KeyBCollection.
                /// </summary>
                /// <returns>true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.</returns>
                public bool MoveNext()
                {
                    if (version != dictionary.version)
                    {
                        throw new InvalidOperationException("InvalidOperation Enumerator Failed Version");
                    }

                    while ((uint)index < (uint)dictionary.count)
                    {
                        if (dictionary.entries[index].hashCode >= 0)
                        {
                            Next();
                            index += 2;
                            return true;
                        }
                        index += 2;
                    }

                    index = dictionary.count + 1;
                    currentKey = default(TKeyB);
                    return false;
                }

                private void Next()
                {
                    if (!dictionary.entries[index].IsBkey)
                    {
                        index++;
                        Next();
                    }
                    currentKey = dictionary.entries[index].keyB;
                }

                /// <summary>
                /// Sets the enumerator to its initial position, which is before the first element in the collection.
                /// </summary>
                void System.Collections.IEnumerator.Reset()
                {
                    if (version != dictionary.version)
                    {
                        throw new InvalidOperationException("InvalidOperation Enumerator Failed Version");
                    }

                    index = 0;
                    currentKey = default(TKeyB);
                }

                #endregion Methods

            }

            #endregion Structs + Classes + Enums

        }

        /// <summary>
        /// Represents the collection of values in a Dictionary&lt;TKeyA,TKeyB,TValue&gt;. This class cannot be inherited.
        /// </summary>
        public sealed class ValueCollection : ICollection<TValue>, ICollection, IReadOnlyCollection<TValue>
        {

            #region Fields

            private TwoKeyDictionary<TKeyA, TKeyB, TValue> dictionary;

            #endregion Fields

            #region Constructors

            /// <summary>
            /// Initializes a new instance of the Dictionary&lt;TKeyA,TKeyB,TValue&gt;.ValueCollection class that reflects the values in the specified Dictionary&lt;TKeyA,TKeyB,TValue&gt;.
            /// </summary>
            /// <param name="dictionary">The Dictionary&lt;TKeyA,TKeyB,TValue&gt; whose values are reflected in the new Dictionary&lt;TKeyA,TKeyB,TValue&gt;.ValueCollection.</param>
            public ValueCollection(TwoKeyDictionary<TKeyA, TKeyB, TValue> dictionary)
            {
                if (dictionary == null)
                {
                    throw new ArgumentNullException("dictionary");
                }
                this.dictionary = dictionary;
            }

            #endregion Constructors

            #region Properties

            /// <summary>
            /// Gets the number of elements contained in the Dictionary&lt;TKeyA,TKeyB,TValue&gt;.ValueCollection.
            /// </summary>
            public int Count
            {
                get { return dictionary.Count; }
            }

            /// <summary>
            /// Gets a value indicating whether the ICollection&lt;T&gt; is read-only.
            /// </summary>
            bool ICollection<TValue>.IsReadOnly
            {
                get { return true; }
            }

            /// <summary>
            /// Gets a value indicating whether access to the ICollection is synchronized (thread safe).
            /// </summary>
            bool ICollection.IsSynchronized
            {
                get { return false; }
            }

            /// <summary>
            /// Gets an object that can be used to synchronize access to the ICollection.
            /// </summary>
            Object ICollection.SyncRoot
            {
                get { return ((ICollection)dictionary).SyncRoot; }
            }

            #endregion Properties

            #region Methods

            /// <summary>
            /// Adds an item to the ICollection&lt;T&gt;. This implementation always throws NotSupportedException.
            /// </summary>
            /// <param name="item">The object to add to the ICollection&lt;T&gt;.</param>
            void ICollection<TValue>.Add(TValue item)
            {
                throw new NotSupportedException("NotSupported Value Collection Set");
            }

            /// <summary>
            /// Removes all items from the ICollection&lt;T&gt;.This implementation always throws NotSupportedException.
            /// </summary>
            void ICollection<TValue>.Clear()
            {
                throw new NotSupportedException("NotSupported Value Collection Set");
            }

            /// <summary>
            /// Determines whether the ICollection&lt;T&gt; contains a specific value.
            /// </summary>
            /// <param name="item"></param>
            /// <returns></returns>
            bool ICollection<TValue>.Contains(TValue item)
            {
                return dictionary.ContainsValue(item);
            }

            /// <summary>
            /// Copies the elements of the ICollection to an Array, starting at a particular Array index.
            /// </summary>
            /// <param name="array">The one-dimensional Array that is the destination of the elements copied from ICollection. The Array must have zero-based indexing.</param>
            /// <param name="index">The zero-based index in array at which copying begins.</param>
            public void CopyTo(TValue[] array, int index)
            {
                if (array == null)
                {
                    throw new ArgumentNullException("Array");
                }

                if (index < 0 || index > array.Length)
                {
                    throw new ArgumentOutOfRangeException($"Index: {index}", new ArgumentOutOfRangeException("Need Non-Negative Number"));
                }

                if (array.Length - index < dictionary.Count)
                {
                    throw new ArgumentException("Array Plus Off Too Small");
                }

                int count = dictionary.count;
                Entry[] entries = dictionary.entries;
                for (int i = 0; i < count; i++)
                {
                    if (entries[i].IsBkey == true)
                    {
                        array[index++] = entries[i].node.Value;
                    }
                }
            }

            /// <summary>
            /// Copies the Dictionary&lt;TKeyA,TKeyB,TValue&gt;.ValueCollection elements to an existing one-dimensional Array, starting at the specified array index.
            /// </summary>
            /// <param name="array">The one-dimensional Array that is the destination of the elements copied from Dictionary&lt;TKeyA,TKeyB,TValue&gt;.ValueCollection. The Array must have zero-based indexing.</param>
            /// <param name="index">The zero-based index in array at which copying begins.</param>
            void ICollection.CopyTo(Array array, int index)
            {
                if (array == null)
                {
                    throw new ArgumentNullException("Array");
                }

                if (array.Rank != 1)
                {
                    throw new ArgumentException("MultiDim Array Not Supported");
                }

                if (array.GetLowerBound(0) != 0)
                {
                    throw new ArgumentException("NonZero Lower Bound");
                }

                if (index < 0 || index > array.Length)
                {
                    throw new ArgumentOutOfRangeException($"Index: {index}", new ArgumentOutOfRangeException("Need NonNegative Number"));
                }

                if (array.Length - index < dictionary.Count)
                    throw new ArgumentException("Array Plus Off Too Small");

                TValue[] values = array as TValue[];
                if (values != null)
                {
                    CopyTo(values, index);
                }
                else
                {
                    object[] objects = array as object[];
                    if (objects == null)
                    {
                        throw new ArgumentException("Invalid Array Type");
                    }

                    int count = dictionary.count;
                    Entry[] entries = dictionary.entries;
                    try
                    {
                        for (int i = 0; i < count; i++)
                        {
                            if (entries[i].hashCode >= 0) objects[index++] = entries[i].node;
                        }
                    }
                    catch (ArrayTypeMismatchException)
                    {
                        throw new ArgumentException("Invalid Array Type");
                    }
                }
            }

            /// <summary>
            /// Returns an enumerator that iterates through the Dictionary&lt;TKeyA,TKeyB,TValue&gt;.ValueCollection.
            /// </summary>
            /// <returns>A Dictionary&lt;TKeyA,TKeyB,TValue&gt;.ValueCollection.Enumerator for the Dictionary&lt;TKeyA,TKeyB,TValue&gt;.ValueCollection.</returns>
            public Enumerator GetEnumerator()
            {
                return new Enumerator(dictionary);
            }

            /// <summary>
            /// Returns an enumerator that iterates through a collection.
            /// </summary>
            /// <returns>An IEnumerator&lt;T&gt; that can be used to iterate through the collection.</returns>
            IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator()
            {
                return new Enumerator(dictionary);
            }

            /// <summary>
            /// Returns an enumerator that iterates through a collection.
            /// </summary>
            /// <returns>An IEnumerator&lt;T&gt; that can be used to iterate through the collection.</returns>
            IEnumerator IEnumerable.GetEnumerator()
            {
                return new Enumerator(dictionary);
            }

            bool ICollection<TValue>.Remove(TValue item)
            {
                throw new NotSupportedException("NotSupported Value Collection Set");
            }

            #endregion Methods

            #region Structs + Classes + Enums

            /// <summary>
            /// Enumerates the elements of a TwoKeyDictionary&lt;TKeyA,TKeyB,TValue&gt;.ValueCollection.
            /// </summary>
            [Serializable]
            public struct Enumerator : IEnumerator<TValue>, System.Collections.IEnumerator
            {

                #region Fields

                private TValue currentValue;
                private TwoKeyDictionary<TKeyA, TKeyB, TValue> dictionary;
                private int index;
                private int version;

                #endregion Fields

                #region Constructors

                /// <summary>
                /// Enumerates the elements of a Dictionary&lt;TKeyA,TKeyB,TValue&gt;.ValueCollection.
                /// </summary>
                /// <param name="dictionary">Dictionary&lt;TKeyA,TKeyB,TValue&gt; to enumerate over its collection for TValues.</param>
                internal Enumerator(TwoKeyDictionary<TKeyA, TKeyB, TValue> dictionary)
                {
                    this.dictionary = dictionary;
                    version = dictionary.version;
                    index = 0;
                    currentValue = default(TValue);
                }

                #endregion Constructors

                #region Properties

                /// <summary>
                /// Gets the element at the current position of the enumerator.
                /// </summary>
                public TValue Current
                {
                    get
                    {
                        return currentValue;
                    }
                }

                /// <summary>
                /// Gets the element at the current position of the enumerator.
                /// </summary>
                Object System.Collections.IEnumerator.Current
                {
                    get
                    {
                        if (index == 0 || (index == dictionary.count + 1))
                        {
                            throw new InvalidOperationException("Enumerator Operation Cant Happen");
                        }

                        return currentValue;
                    }
                }

                #endregion Properties

                #region Methods

                /// <summary>
                /// Releases all resources used by the TwoKeyDictionary&lt;TKeyA,TKeyB,TValue&gt;.ValueCollection.Enumerator.
                /// </summary>
                public void Dispose()
                {
                }

                /// <summary>
                /// Advances the enumerator to the next element of the TwoKeyDictionary&lt;TKeyA,TKeyB,TValue&gt;.ValueCollection.
                /// </summary>
                /// <returns>true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.</returns>
                public bool MoveNext()
                {
                    if (version != dictionary.version)
                    {
                        throw new InvalidOperationException("Enumerator Failed Version");
                    }

                    while ((uint)index < (uint)dictionary.count)
                    {
                        if (dictionary.entries[index].hashCode >= 0)
                        {
                            Next();
                            index += 2;
                            return true;
                        }
                        index += 2;
                    }
                    index = dictionary.count + 1;
                    currentValue = default(TValue);
                    return false;
                }

                private void Next()
                {
                    if (!dictionary.entries[index].IsBkey)
                    {
                        index++;
                        Next();
                    }
                    currentValue = dictionary.entries[index].node.Value;
                }

                /// <summary>
                /// Sets the enumerator to its initial position, which is before the first element in the collection.
                /// </summary>
                void System.Collections.IEnumerator.Reset()
                {
                    if (version != dictionary.version)
                    {
                        throw new InvalidOperationException("Enumerator Failed Version");
                    }
                    index = 0;
                    currentValue = default(TValue);
                }

                #endregion Methods

            }

            #endregion Structs + Classes + Enums
        }

        // used to place a pointer to from key-a
        internal class EntryNode
        {
            public EntryNode()
            {
                Value = default(TValue);

            }
            public EntryNode(TValue value)
            {
                Value = value;
            }

            public TValue Value;
            public EntryNode Next;
        }

        private struct Entry
        {
            public int hashCode;        // Lower 31 bits of hash code, -1 if unused
            public bool IsBkey;			// is entry a B or A key
            public TKeyA keyA;          // Key A entry
            public TKeyB keyB;          // Key B entry                  
            public int next;            // Index of next entry, -1 if last key of entry
            public EntryNode node;      // EntryNode for holding TValue and to take a reference from key-a
        }
    }

    internal class TwoKeyDictionaryDebugView<TKeyA, TKeyB, TValue>
    {

        #region Constructors

        public TwoKeyDictionaryDebugView(TKeyA keyA, TKeyB keyB, TValue value)
        {
            KeyA = keyA;
            KeyB = keyB;
            Value = value;
        }

        #endregion Constructors

        #region Properties

        public TKeyA KeyA { get; set; }
        public TKeyB KeyB { get; set; }

        public TValue Value { get; set; }

        #endregion Properties

        #region Methods

        public override string ToString()
        {
            return $"{KeyA.ToString()}, {KeyB.ToString()}, {Value.ToString()}";
        }

        #endregion Methods

    }
}