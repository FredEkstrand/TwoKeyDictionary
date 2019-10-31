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

        public TwoKeyDictionary() : this(0, null, null) { }

        public TwoKeyDictionary(int capacity) : this(capacity, null, null) { }

        public TwoKeyDictionary(IEqualityComparer<TKeyA> comparerA, IEqualityComparer<TKeyB> comparerB) : this(0, comparerA, comparerB) { }

        public TwoKeyDictionary(IEqualityComparer<TKeyA> comparerA) : this(0, comparerA, null) { }

        public TwoKeyDictionary(IEqualityComparer<TKeyB> comparerB) : this(0, null, comparerB) { }

        public TwoKeyDictionary(int capacity, IEqualityComparer<TKeyA> comparerA, IEqualityComparer<TKeyB> comparerB)
        {
            if (capacity < 0) throw new ArgumentOutOfRangeException("Capacity");
            if (capacity > 0) Initialize(capacity);
            this.comparerA = comparerA ?? EqualityComparer<TKeyA>.Default;
            this.comparerB = comparerB ?? EqualityComparer<TKeyB>.Default;
        }

        public TwoKeyDictionary(TwoKeyDictionary<TKeyA, TKeyB, TValue> dictionary) : this(dictionary, null, null) { }

        public TwoKeyDictionary(TwoKeyDictionary<TKeyA, TKeyB, TValue> dictionary, IEqualityComparer<TKeyA> comparerA, IEqualityComparer<TKeyB> comparerB) :
            this(dictionary != null ? dictionary.Count : 0, comparerA, comparerB)
        {

            if (dictionary == null)
            {
                throw new ArgumentNullException("TwoKeyDictionary");
            }

            foreach (TwoKeyValueTriple<TKeyA, TKeyB, TValue> triple in dictionary)
            {
                Clone(triple.KeyA, triple.KeyB, triple.Value, true);
            }
        }

        protected TwoKeyDictionary(SerializationInfo info, StreamingContext context)
        {
            //We can't do anything with the keys and values until the entire graph has been deserialized
            //and we have a reasonable estimate that GetHashCode is not going to fail.  For the time being,
            //we'll just cache this.  The graph is not valid until OnDeserialization has been called.
            HashHelpers.SerializationInfoTable.Add(this, info);
        }

        #endregion Constructors

        #region Properties

        public ICollection<TKeyA> AKeys
        {
            get
            {
                if (Akeys == null) Akeys = new KeyACollection(this);
                return Akeys;
            }
        }

        ICollection ITwoKeyDictionary.AKeys
        {
            get
            {
                if (Akeys == null) Akeys = new KeyACollection(this);
                return Akeys;
            }
        }
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
        public bool IsFixedSize { get { return false; } }
        public bool IsReadOnly { get { return false; } }
        public bool IsSynchronized { get { return false; } }
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
                            int k = FindEntry(entries[i].keyB);
                            if (k >= 0)
                            {
                                return entries[k].value;
                            }
                            else
                            {
                                throw new KeyNotFoundException();
                            }

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
                            return entries[i].value;
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

        public TValue this[TKeyB key]
        {
            get
            {
                int i = FindEntry(key);
                if (i >= 0) return entries[i].value;
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
        public TValue this[TKeyA key]
        {
            get
            {
                int i = FindEntry(key);
                if (i >= 0)
                {
                    TKeyB kb = entries[i].keyB;
                    return this[kb];
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

        public void Add(TKeyA keyA, TKeyB keyB, TValue value)
        {
            Insert(keyA, keyB, value, true);
        }

        public void Add(TwoKeyValueTriple<TKeyA, TKeyB, TValue> item)
        {
            Add(item.KeyA, item.KeyB, item.Value); 
        }

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

        private void Clone(TKeyA keyA, TKeyB keyB, TValue value, bool add)
        {

            if (buckets == null) Initialize(0);
            int hashCodeB = comparerB.GetHashCode(keyB) & 0x7FFFFFFF;
            int targetBucketB = hashCodeB % buckets.Length;
            int hashCodeA = comparerA.GetHashCode(keyA) & 0x7FFFFFFF;
            int targetBucketA = hashCodeA % buckets.Length;

            int indexB;
            int indexA;

            if (freeCount > 0)
            {
                indexB = freeList;
                freeList = entries[indexB].next;
                freeCount--;

                indexA = freeList;
                freeList = entries[indexA].next;
                freeCount--;
            }
            else
            {
                if (count == entries.Length)
                {
                    Resize();
                    targetBucketB = hashCodeB % buckets.Length;
                    targetBucketA = hashCodeA % buckets.Length;
                }
                indexB = count;
                count++;
                if (count == entries.Length)
                {
                    Resize();
                    targetBucketB = hashCodeB % buckets.Length;
                    targetBucketA = hashCodeA % buckets.Length;
                }
                indexA = count;
                count++;
            }

            entries[indexB].hashCode = hashCodeB;
            entries[indexB].next = buckets[targetBucketB];
            entries[indexB].keyA = default(TKeyA);
            entries[indexB].keyB = keyB;
            entries[indexB].IsBkey = true;
            entries[indexB].value = value;
            buckets[targetBucketB] = indexB;
            version++;

            entries[indexA].hashCode = hashCodeA;
            entries[indexA].next = buckets[targetBucketA];
            entries[indexA].keyA = keyA;
            entries[indexA].keyB = keyB;
            entries[indexA].IsBkey = false;
            buckets[targetBucketA] = indexA;
            version++;
        }

        public bool Contains(TwoKeyValueTriple<TKeyA, TKeyB, TValue> item)
        {
            int i = FindEntry(item.KeyB);
            if (i >= 0 && EqualityComparer<TValue>.Default.Equals(entries[i].value, item.Value))
            {
                return true;
            }
            return false;
        }

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

        public bool ContainsKeyA(TKeyA keyA)
        {
            return FindEntry(keyA) >= 0;
        }

        public bool ContainsKeyB(TKeyB keyB)
        {
            return FindEntry(keyB) >= 0;
        }

        public bool ContainsValue(TValue value)
        {
            if (value == null)
            {
                for (int i = 0; i < count; i++)
                {
                    if (entries[i].hashCode >= 0 && entries[i].value == null) return true;
                }
            }
            else
            {
                EqualityComparer<TValue> c = EqualityComparer<TValue>.Default;
                for (int i = 0; i < count; i++)
                {
                    if (entries[i].hashCode >= 0 && c.Equals(entries[i].value, value)) return true;
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
                    array[index++] = new TwoKeyValueTriple<TKeyA, TKeyB, TValue>(entries[i].keyA, entries[i].keyB, entries[i].value);

                }                
            }
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<TwoKeyValueTriple<TKeyA, TKeyB, TValue>> GetEnumerator()
        {
            return new Enumerator(this, Enumerator.KeyValueTriple);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this, Enumerator.KeyValueTriple);
        }

        ITwoKeyDictionaryEnumerator ITwoKeyDictionary.GetEnumerator()
        {
            return new Enumerator(this, Enumerator.KeyValueTriple);
        }

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
                return entries[i].value;
            }
            return default(TValue);
        }

        internal TValue GetValueOrDefault(TKeyB key)
        {
            int i = FindEntry(key);
            if (i >= 0)
            {
                return entries[i].value;
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

            // get b-key hash value and index
            int hashCodeB = comparerB.GetHashCode(keyB) & 0x7FFFFFFF;
            int targetBucketB = hashCodeB % buckets.Length;

            // get a-key hash value and index
            int hashCodeA = comparerA.GetHashCode(keyA) & 0x7FFFFFFF;
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
                    entries[i].value = value;
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
                    entries[i].value = value;
                    version++;
                    return;
                }
            }

            int indexB;
            int indexA;


            if (freeCount > 0)
            {
                indexA = freeList;
                freeList = entries[indexA].next;
                freeCount--;
            }
            else
            {
                if (count == entries.Length - 1)
                {
                    Resize();
                    targetBucketA = hashCodeA % buckets.Length;
                }
                indexA = count;
                count++;
            }

            if (freeCount > 0)
            {
                indexB = freeList;
                freeList = entries[indexB].next;
                freeCount--;
            }
            else
            {
                if (count == entries.Length - 1)
                {
                    Resize();
                    targetBucketB = hashCodeB % buckets.Length;
                }
                indexB = count;
                count++;
            }

            entries[indexB].hashCode = hashCodeB;
            entries[indexB].next = buckets[targetBucketB];
            entries[indexB].keyA = keyA;
            entries[indexB].keyB = keyB;
            entries[indexB].IsBkey = true;
            entries[indexB].value = value;
            buckets[targetBucketB] = indexB;
            version++;

            entries[indexA].hashCode = hashCodeA;
            entries[indexA].next = buckets[targetBucketA];
            entries[indexA].keyA = keyA;
            entries[indexA].keyB = keyB;
            entries[indexA].IsBkey = false;
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
                    entries[i].value = value;
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
                    entries[i].value = value;
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

        public bool Remove(TwoKeyValueTriple<TKeyA, TKeyB, TValue> item)
        {
            int i = FindEntry(item.KeyB);
            if(i >= 0)
            {
                return RemoveKeyB(item.KeyB);
            }
            return false;
        }

        public bool Remove(TKeyA key)
        {
            return RemoveKeyA(key);
        }

        public bool Remove(TKeyB key)
        {
            return RemoveKeyB(key);
        }

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
                        entries[i].value = default(TValue);
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
                        entries[i].value = default(TValue);
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

        public bool TryGetValueKeyA(TKeyA keyA, out TValue value)
        {
            int i = FindEntry(keyA);
            if (i >= 0)
            {
                // b-key holds the value
                i = FindEntry(entries[i].keyB);
                value = entries[i].value;
                return true;
            }
            value = default(TValue);
            return false;
        }

        public bool TryGetValueKeyB(TKeyB keyB, out TValue value)
        {
            int i = FindEntry(keyB);
            if (i >= 0)
            {
                value = entries[i].value;
                return true;
            }
            value = default(TValue);
            return false;
        }

        #endregion Methods

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

            public TwoKeyValueTriple<TKeyA, TKeyB, TValue> Current
            {
                get { return current; }
            }

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

            public void Dispose()
            {
            }

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
                        //current = new TwoKeyValueTriple<TKeyA, TKeyB, TValue>(dictionary.entries[index].keyA, dictionary.entries[index].keyB, dictionary.entries[index].value);
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
                current = new TwoKeyValueTriple<TKeyA, TKeyB, TValue>(dictionary.entries[index].keyA, dictionary.entries[index].keyB, dictionary.entries[index].value);

            }
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

        [DebuggerTypeProxy(typeof(TwoKeyDictionaryDebugView<,,>))]
        [DebuggerDisplay("Count = {Count}")]
        [Serializable]
        public sealed class KeyACollection : ICollection<TKeyA>, ICollection, IReadOnlyCollection<TKeyA>
        {

            #region Fields

            private TwoKeyDictionary<TKeyA, TKeyB, TValue> dictionary;

            #endregion Fields

            #region Constructors

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

            public int Count
            {
                get { return dictionary.Count; }
            }

            bool ICollection<TKeyA>.IsReadOnly
            {
                get { return true; }
            }

            public bool IsReadOnly
            {
                get { return true; }
            }

            bool ICollection.IsSynchronized
            {
                get { return false; }
            }

            Object ICollection.SyncRoot
            {
                get { return ((ICollection)dictionary).SyncRoot; }
            }

            #endregion Properties

            #region Methods

            void ICollection<TKeyA>.Add(TKeyA item)
            {
                throw new NotSupportedException("NotSupported Key Collection Set");
            }

            void ICollection<TKeyA>.Clear()
            {
                throw new NotSupportedException("NotSupported Key Collection Set");
            }

            bool ICollection<TKeyA>.Contains(TKeyA item)
            {
                return dictionary.ContainsKeyA(item);
            }

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

            public Enumerator GetEnumerator()
            {
                return new Enumerator(dictionary);
            }
            IEnumerator<TKeyA> IEnumerable<TKeyA>.GetEnumerator()
            {
                return new Enumerator(dictionary);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return new Enumerator(dictionary);
            }

            bool ICollection<TKeyA>.Remove(TKeyA item)
            {
                throw new NotSupportedException("NotSupported Key Collection Set");
            }

            #endregion Methods

            #region Structs + Classes + Enums

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

                internal Enumerator(TwoKeyDictionary<TKeyA, TKeyB, TValue> dictionary)
                {
                    this.dictionary = dictionary;
                    version = dictionary.version;
                    index = 0;
                    currentKey = default(TKeyA);
                }

                #endregion Constructors

                #region Properties

                public TKeyA Current
                {
                    get
                    {
                        return currentKey;
                    }
                }

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

                public void Dispose()
                {
                }

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
                            currentKey = dictionary.entries[index].keyA;
                            index++;
                            return true;
                        }
                        index++;
                    }

                    index = dictionary.count + 1;
                    currentKey = default(TKeyA);
                    return false;
                }
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

        [DebuggerTypeProxy(typeof(TwoKeyDictionaryDebugView<,,>))]
        [DebuggerDisplay("Count = {Count}")]
        [Serializable]
        public sealed class KeyBCollection : ICollection<TKeyB>, ICollection, IReadOnlyCollection<TKeyB>
        {

            #region Fields

            private TwoKeyDictionary<TKeyA, TKeyB, TValue> dictionary;

            #endregion Fields

            #region Constructors

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

            public int Count
            {
                get { return dictionary.Count; }
            }

            bool ICollection<TKeyB>.IsReadOnly
            {
                get { return true; }
            }

            public bool IsReadOnly { get { return true; } }

            bool ICollection.IsSynchronized
            {
                get { return false; }
            }

            Object ICollection.SyncRoot
            {
                get { return ((ICollection)dictionary).SyncRoot; }
            }

            #endregion Properties

            #region Methods

            void ICollection<TKeyB>.Add(TKeyB item)
            {
                throw new NotSupportedException("NotSupported Key Collection Set");
            }

            void ICollection<TKeyB>.Clear()
            {
                throw new NotSupportedException("NotSupported Key Collection Set");
            }

            bool ICollection<TKeyB>.Contains(TKeyB item)
            {
                return dictionary.ContainsKeyB(item);
            }

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

            public Enumerator GetEnumerator()
            {
                return new Enumerator(dictionary);
            }
            IEnumerator<TKeyB> IEnumerable<TKeyB>.GetEnumerator()
            {
                return new Enumerator(dictionary);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return new Enumerator(dictionary);
            }

            bool ICollection<TKeyB>.Remove(TKeyB item)
            {
                throw new NotSupportedException("NotSupported Key Collection Set");
            }

            #endregion Methods

            #region Structs + Classes + Enums

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

                internal Enumerator(TwoKeyDictionary<TKeyA, TKeyB, TValue> dictionary)
                {
                    this.dictionary = dictionary;
                    version = dictionary.version;
                    index = 0;
                    currentKey = default(TKeyB);
                }

                #endregion Constructors

                #region Properties

                public TKeyB Current
                {
                    get
                    {
                        return currentKey;
                    }
                }

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

                public void Dispose()
                {
                }

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
                            currentKey = dictionary.entries[index].keyB;
                            index++;
                            return true;
                        }
                        index++;
                    }

                    index = dictionary.count + 1;
                    currentKey = default(TKeyB);
                    return false;
                }
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

        public sealed class ValueCollection : ICollection<TValue>, ICollection, IReadOnlyCollection<TValue>
        {

            #region Fields

            private TwoKeyDictionary<TKeyA, TKeyB, TValue> dictionary;

            #endregion Fields

            #region Constructors

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

            public int Count
            {
                get { return dictionary.Count; }
            }

            bool ICollection<TValue>.IsReadOnly
            {
                get { return true; }
            }

            bool ICollection.IsSynchronized
            {
                get { return false; }
            }

            Object ICollection.SyncRoot
            {
                get { return ((ICollection)dictionary).SyncRoot; }
            }

            #endregion Properties

            #region Methods

            void ICollection<TValue>.Add(TValue item)
            {
                throw new NotSupportedException("NotSupported Value Collection Set");
            }

            void ICollection<TValue>.Clear()
            {
                throw new NotSupportedException("NotSupported Value Collection Set");
            }

            bool ICollection<TValue>.Contains(TValue item)
            {
                return dictionary.ContainsValue(item);
            }

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
                        array[index++] = entries[i].value;
                    }
                }
            }

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
                            if (entries[i].hashCode >= 0) objects[index++] = entries[i].value;
                        }
                    }
                    catch (ArrayTypeMismatchException)
                    {
                        throw new ArgumentException("Invalid Array Type");
                    }
                }
            }

            public Enumerator GetEnumerator()
            {
                return new Enumerator(dictionary);
            }
            IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator()
            {
                return new Enumerator(dictionary);
            }

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

                internal Enumerator(TwoKeyDictionary<TKeyA, TKeyB, TValue> dictionary)
                {
                    this.dictionary = dictionary;
                    version = dictionary.version;
                    index = 0;
                    currentValue = default(TValue);
                }

                #endregion Constructors

                #region Properties

                public TValue Current
                {
                    get
                    {
                        return currentValue;
                    }
                }

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

                public void Dispose()
                {
                }

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
                            currentValue = dictionary.entries[index].value;
                            index++;
                            return true;
                        }
                        index++;
                    }
                    index = dictionary.count + 1;
                    currentValue = default(TValue);
                    return false;
                }
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

        private struct Entry
        {

            #region Fields

            public int hashCode;        // Lower 31 bits of hash code, -1 if unused
            public bool IsBkey;			// is entry a B or A key
            public TKeyA keyA;          // Key A entry
            public TKeyB keyB;          // Key B entry                  
            public int next;            // Index of next entry, -1 if last key of entry
            public TValue value;

            #endregion Fields
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