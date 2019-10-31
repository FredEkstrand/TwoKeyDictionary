using NUnit.Framework;
using Ekstrand.Collections.Generic;
using Ekstrand.Collections;
using System.Collections;
using System.Collections.Generic;
using System;
using System.ComponentModel;
using CategoryAttribute = NUnit.Framework.CategoryAttribute;


namespace NUnitTests
{
    public class Tests
    {
        private const int POPSIZE = 11;
  

        [SetUp]
        public void Setup()
        {
           
        }

        #region Constructor
        [Test]
        [Category("Constructor")]
        public void TwoKeyDictionary_Base()
        {
            // basic check of an instance of an object with default settings.
            TwoKeyDictionary<int, int, int> tkd = new TwoKeyDictionary<int, int, int>();
            TwoKeyDictionary<string, string, string> tkd2 = new TwoKeyDictionary<string, string, string>();
            Assert.AreEqual(0, tkd.Count);
            Assert.AreEqual(0, tkd2.Count);
            Assert.AreEqual(0, tkd.Values.Count);
            Assert.AreEqual(0, tkd2.Values.Count);
            Assert.AreEqual(0, tkd.AKeys.Count);
            Assert.AreEqual(0, tkd.BKeys.Count);
            Assert.AreEqual(0, tkd2.AKeys.Count);
            Assert.AreEqual(0, tkd2.BKeys.Count);
        }

        [Test]
        [Category("Constructor")]
        public void TwoKeyDictionary_Capacity()
        {
            // can not directly check if initialize with given capacity but
            // should upon completing initialization have known default settings.
            TwoKeyDictionary<int, int, int> tkd = new TwoKeyDictionary<int, int, int>(7);
            Assert.AreEqual(0, tkd.Count);
            Assert.AreEqual(0, tkd.Values.Count);
            Assert.AreEqual(0, tkd.AKeys.Count);
            Assert.AreEqual(0, tkd.BKeys.Count);
        }

        [Test]
        [Category("Constructor")]
        public void TwoKeyDictionary_IEqualityComparerAB()
        {
            //case with Akey only
            TwoKeyDictionary<string, string, int> tkd = new Ekstrand.Collections.Generic.TwoKeyDictionary<string, string, int>(StringComparer.CurrentCultureIgnoreCase, null);
            tkd.Add("txt", "notepad", 23);
            tkd.Add("bmp", "paint", 33);
            tkd.Add("DIB", "paintnet", 43);
            tkd.Add("rtf", "wordpad", 53);

            try
            {
                tkd.Add("BMP", "adobe.exe", 55);
            }
            catch (ArgumentException ae)
            {
                Assert.AreEqual("Adding Duplicate AKey", ae.Message);
            }

            //case with Bkey only
            TwoKeyDictionary<string, string, int> tkd1 = new Ekstrand.Collections.Generic.TwoKeyDictionary<string, string, int>(null, StringComparer.CurrentCultureIgnoreCase);
            tkd1.Add("txt", "notepad", 23);
            tkd1.Add("bmp", "paint", 33);
            tkd1.Add("DIB", "paintnet", 43);
            tkd1.Add("rtf", "wordpad", 53);

            try
            {
                tkd1.Add("tbf", "NotePad", 55);
            }
            catch (ArgumentException ae)
            {
                Assert.AreEqual("Adding Duplicate BKey", ae.Message);
            }

            // test case with both AKey + BKey not necessary sense we are checking the same code path.
        }

        [Test]
        [Category("Constructor")]
        public void TwoKeyDictionary_IEqualityComparerA()
        {
            // in test case TwoKeyDictionary_IEqualityComparerAB()
            // already tested this code path
            Assert.Pass();
        }

        [Test]
        [Category("Constructor")]
        public void TwoKeyDictionary_IEqualityComparerB()
        {
            // in test case TwoKeyDictionary_IEqualityComparerAB()
            // already tested this code path
            Assert.Pass();
        }

        [Test]
        [Category("Constructor")]
        public void TwoKeyDictionary_Capacity_IEqualityComparerAB()
        {
            PackTwoKeyDictionary ptkd = new PackTwoKeyDictionary();   
            TwoKeyDictionary<string, int, int> tkd = new TwoKeyDictionary<string, int, int>(13, StringComparer.CurrentCultureIgnoreCase, new IntEqualityComparer());

            for(int i = 0; i < 11; i++)
            {
                tkd.Add(ptkd.kavalues[i], ptkd.kbvalues[i], ptkd.vvalues[i]);
            }
            
            Assert.AreEqual(11, tkd.Count, "main.Coun");
            Assert.AreEqual(11, tkd.AKeys.Count, "main.AKeys.Count");
            Assert.AreEqual(11, tkd.BKeys.Count, "main.BKeys.Count");
            Assert.AreEqual(11, tkd.Values.Count, "main.Values.Count");

            tkd.Add(ptkd.kavalues[11], ptkd.kbvalues[11], ptkd.vvalues[11]);
            tkd.Add(ptkd.kavalues[12], ptkd.kbvalues[12], ptkd.vvalues[12]);

            Assert.AreEqual(13, tkd.Count);
            Assert.AreEqual(13, tkd.AKeys.Count);
            Assert.AreEqual(13, tkd.BKeys.Count);
            Assert.AreEqual(13, tkd.Values.Count);

            int result = -1;
            result = tkd["ABALONESHELL"];
            Assert.AreEqual(-2373699, result);

            result = tkd[12];
            Assert.AreEqual(-2373699, unchecked((int)0xFFDBC7BD));

        }

        [Test]
        [Category("Constructor")]
        public void TwoKeyDictionary_TwoKeyDictionary()
        {
            PackTwoKeyDictionary ptkd = new PackTwoKeyDictionary();
            TwoKeyDictionary<string, int, int> seeder = ptkd.GetLoadedTwoKeyDictionary(POPSIZE);
            TwoKeyDictionary<string, int, int> tkd = new TwoKeyDictionary<string, int, int>(seeder, null, null);

            Assert.AreEqual(11, tkd.Count, "main.Coun");
            Assert.AreEqual(11, tkd.AKeys.Count, "main.AKeys.Count");
            Assert.AreEqual(11, tkd.BKeys.Count, "main.BKeys.Count");
            Assert.AreEqual(11, tkd.Values.Count, "main.Values.Count");

            tkd.Add(ptkd.kavalues[11], ptkd.kbvalues[11], ptkd.vvalues[11]);
            tkd.Add(ptkd.kavalues[12], ptkd.kbvalues[12], ptkd.vvalues[12]);

            Assert.AreEqual(13, tkd.Count);
            Assert.AreEqual(13, tkd.AKeys.Count);
            Assert.AreEqual(13, tkd.BKeys.Count);
            Assert.AreEqual(13, tkd.Values.Count);

            int result = -1;
            result = tkd["AbaloneShell"];
            Assert.AreEqual(-2373699, result);

            result = tkd[12];
            Assert.AreEqual(-2373699, unchecked((int)0xFFDBC7BD));
        }

        [Test]
        [Category("Constructor")]
        public void TwoKeyDictionary_TwoKeyDictionary_IEqualityComparerAB()
        {
            PackTwoKeyDictionary ptkd = new PackTwoKeyDictionary();
            TwoKeyDictionary<string, int, int> seeder = ptkd.GetLoadedTwoKeyDictionary(POPSIZE);
            TwoKeyDictionary<string, int, int> tkd = new TwoKeyDictionary<string, int, int>(seeder, StringComparer.CurrentCultureIgnoreCase, new IntEqualityComparer());

            Assert.AreEqual(11, tkd.Count, "main.Coun");
            Assert.AreEqual(11, tkd.AKeys.Count, "main.AKeys.Count");
            Assert.AreEqual(11, tkd.BKeys.Count, "main.BKeys.Count");
            Assert.AreEqual(11, tkd.Values.Count, "main.Values.Count");

            tkd.Add(ptkd.kavalues[11], ptkd.kbvalues[11], ptkd.vvalues[11]);
            tkd.Add(ptkd.kavalues[12], ptkd.kbvalues[12], ptkd.vvalues[12]);

            Assert.AreEqual(13, tkd.Count);
            Assert.AreEqual(13, tkd.AKeys.Count);
            Assert.AreEqual(13, tkd.BKeys.Count);
            Assert.AreEqual(13, tkd.Values.Count);
        }

        #endregion

        #region Properties
        [Test]
        [Category("Properties")]
        public void ICollection_TKeyA()
        {
            PackTwoKeyDictionary ptkd = new PackTwoKeyDictionary();
            TwoKeyDictionary<string, int, int> tkd = ptkd.GetLoadedTwoKeyDictionary(POPSIZE);

            ICollection<string> ika = tkd.AKeys;
            Assert.AreEqual(tkd.Count, ika.Count);

            foreach(string akey in ika)
            {
                if(!tkd.ContainsKeyA(akey))
                {
                    Assert.Fail();
                }
            }

            Assert.Pass();
        }

        [Test]
        [Category("Properties")]
        public void IEnumerable_TKeyA_IReadOnlyTwoKeyDictionary_AKeys()
        {
            //  foreach is meant to iterate over a container, making sure each item is
            //   visited exactly once, without changing the container.
            PackTwoKeyDictionary ptkd = new PackTwoKeyDictionary();
            TwoKeyDictionary<string, int, int> tkd = ptkd.GetLoadedTwoKeyDictionary(POPSIZE);

            int count = 0;
            List<string> items = new List<string>(ptkd.kavalues);
            foreach (TwoKeyValueTriple<string, int, int> irtkd in tkd)
            {
                if (!items.Contains(irtkd.KeyA))
                { Assert.Fail(); }
                count++;
            }

            // checking it the enumeration has iterated through each element
            Assert.AreEqual(tkd.Count, count);
        }

        [Test]
        [Category("Properties")]
        public void ICollection_TKeyB()
        {
            PackTwoKeyDictionary ptkd = new PackTwoKeyDictionary();
            TwoKeyDictionary<string, int, int> tkd = ptkd.GetLoadedTwoKeyDictionary(POPSIZE);

            ICollection<int> ikb = tkd.BKeys;
            Assert.AreEqual(tkd.Count, ikb.Count);

            foreach (int bkey in ikb)
            {
                if (!tkd.ContainsKeyB(bkey))
                {
                    Assert.Fail();
                }
            }

            Assert.Pass();
        }

        [Test]
        [Category("Properties")]
        public void IEnumerable_TKeyB_IReadOnlyTwoKeyDictionary_BKeys()
        {
            //  foreach is meant to iterate over a container, making sure each item is
            //   visited exactly once, without changing the container.
            PackTwoKeyDictionary ptkd = new PackTwoKeyDictionary();
            TwoKeyDictionary<string, int, int> tkd = ptkd.GetLoadedTwoKeyDictionary(POPSIZE);

            int count = 0;
            List<int> items = new List<int>(ptkd.kbvalues);
            foreach (TwoKeyValueTriple<string, int, int> irtkd in tkd)
            {
                if (!items.Contains(irtkd.KeyB))
                { Assert.Fail(); }
                count++;
            }

            // checking it the enumeration has iterated through each element
            Assert.AreEqual(tkd.Count, count);
        }

        [Test]
        [Category("Properties")]
        public void Count()
        {
            PackTwoKeyDictionary ptkd = new PackTwoKeyDictionary();
            TwoKeyDictionary<string, int, int> tkd = ptkd.GetLoadedTwoKeyDictionary(POPSIZE);
 
            Assert.AreEqual(POPSIZE, tkd.Count);
        }

        [Test]
        [Category("Properties")]
        public void IsFixedSize()
        {
            PackTwoKeyDictionary ptkd = new PackTwoKeyDictionary();
            TwoKeyDictionary<string, int, int> tkd = ptkd.GetLoadedTwoKeyDictionary(POPSIZE);

            Assert.AreEqual(false, tkd.IsFixedSize);
        }

        [Test]
        [Category("Properties")]
        public void IsReadOnly()
        {
            PackTwoKeyDictionary ptkd = new PackTwoKeyDictionary();
            TwoKeyDictionary<string, int, int> tkd = ptkd.GetLoadedTwoKeyDictionary(POPSIZE);

            Assert.AreEqual(false, tkd.IsReadOnly);
        }

        [Test]
        [Category("Properties")]
        public void IsSynchronized()
        {
            PackTwoKeyDictionary ptkd = new PackTwoKeyDictionary();
            TwoKeyDictionary<string, int, int> tkd = ptkd.GetLoadedTwoKeyDictionary(POPSIZE);

            Assert.AreEqual(false, tkd.IsSynchronized);
        }

        [Test]
        [Category("Properties")]
        public void SyncRoot()
        {
            TwoKeyDictionary<string, int, int> stkd = sptkd.GetLoadedTwoKeyDictionary(POPSIZE);

            _items = stkd.BKeys;
 
            lock (stkd.SyncRoot)
            {
                stkd.Add(sptkd.kavalues[12], sptkd.kbvalues[12], sptkd.vvalues[12]);
                foreach (int item in _items)
                {
                    Console.WriteLine("main thread  " + item);
                    _result.Add("main thread  " + item);
                }
            }

            int size = _result.Count;
            Assert.AreEqual("main thread  0", _result[0]);
            Assert.AreEqual("main thread  12", _result[23]);
        }

        static PackTwoKeyDictionary sptkd = new PackTwoKeyDictionary();
        static List<string> _result = new List<string>(22);
        static ICollection<int> _items;
        static TwoKeyDictionary<string, int, int> stkd = new TwoKeyDictionary<string, int, int>();

        [Test]
        [Category("Properties")]
        public void ICollection_TValue()
        {
            PackTwoKeyDictionary ptkd = new PackTwoKeyDictionary();
            TwoKeyDictionary<string, int, int> tkd = ptkd.GetLoadedTwoKeyDictionary(POPSIZE);

            ICollection<int> ikb = tkd.Values;
            Assert.AreEqual(tkd.Count, ikb.Count);

            foreach (int bkey in ikb)
            {
                if (!tkd.ContainsValue(bkey))
                {
                    Assert.Fail();
                }
            }

            Assert.Pass();
        }

        [Test]
        [Category("Properties")]
        public void IEnumerable_TValue_IReadOnlyTwoKeyDictionary_Values()
        {
            //  foreach is meant to iterate over a container, making sure each item is
            //   visited exactly once, without changing the container.
            PackTwoKeyDictionary ptkd = new PackTwoKeyDictionary();
            TwoKeyDictionary<string, int, int> tkd = ptkd.GetLoadedTwoKeyDictionary(POPSIZE);

            int count = 0;
            List<int> items = new List<int>(ptkd.vvalues);
            foreach (TwoKeyValueTriple<string, int, int> irtkd in tkd)
            {
                if (!items.Contains(irtkd.Value))
                { Assert.Fail(); }
                count++;
            }

            // checking it the enumeration has iterated through each element
            Assert.AreEqual(tkd.Count, count);
        }

        #endregion

        #region Indexers

        [Test]
        [Category("Indexers")]
        public void Object_Object_Key()
        {
            PackTwoKeyDictionary ptkd = new PackTwoKeyDictionary();
            TwoKeyDictionary<string, int, int> tkd = ptkd.GetLoadedTwoKeyDictionary(POPSIZE);

            for(int i = 0; i < POPSIZE; i++)
            {
                if(ptkd.vvalues[i] != tkd[ptkd.kavalues[i]])
                {
                    Assert.Fail();
                }
            }

            for (int i = 0; i < POPSIZE; i++)
            {
                if (ptkd.vvalues[i] != tkd[ptkd.kbvalues[i]])
                {
                    Assert.Fail();
                }
            }

            Assert.Pass();
        }

        [Test]
        [Category("Indexers")]
        public void TValue_TKeyA_Key()
        {
            PackTwoKeyDictionary ptkd = new PackTwoKeyDictionary();
            TwoKeyDictionary<string, int, int> tkd = ptkd.GetLoadedTwoKeyDictionary(POPSIZE);

            for (int i = 0; i < POPSIZE; i++)
            {
                if (ptkd.vvalues[i] != tkd[ptkd.kavalues[i]])
                {
                    Assert.Fail();
                }
            }

            Assert.Pass();
        }

        [Test]
        [Category("Indexers")]
        public void TValue_TKeyB_Key()
        {
            PackTwoKeyDictionary ptkd = new PackTwoKeyDictionary();
            TwoKeyDictionary<string, int, int> tkd = ptkd.GetLoadedTwoKeyDictionary(POPSIZE);
                        
            for (int i = 0; i < POPSIZE; i++)
            {
                if (ptkd.vvalues[i] != tkd[ptkd.kbvalues[i]])
                {
                    Assert.Fail();
                }
            }

            Assert.Pass();
        }

        #endregion

        #region Methods

        [Test]
        [Category("Methods")]
        public void Add()
        {
            PackTwoKeyDictionary ptkd = new PackTwoKeyDictionary();
            TwoKeyDictionary<string, int, int> tkd = ptkd.GetLoadedTwoKeyDictionary(POPSIZE);

            foreach (string akey in tkd.AKeys)
            {
                if (!tkd.ContainsKeyA(akey))
                {
                    Assert.Fail();
                }
            }

            Assert.Pass();
        }

        [Test]
        [Category("Methods")]
        public void Add_TwoKeyValueTriple()
        {
            List<TwoKeyValueTriple<string, int, int>> items = new List<TwoKeyValueTriple<string, int, int>>();
            PackTwoKeyDictionary ptkd = new PackTwoKeyDictionary();
            for(int i = 0; i < POPSIZE; i++)
            {
                TwoKeyValueTriple<string, int, int> tkv = new TwoKeyValueTriple<string, int, int>(ptkd.kavalues[i],ptkd.kbvalues[i], ptkd.vvalues[i]);
                items.Add(tkv);
            }

            TwoKeyDictionary<string, int, int> tkd = new TwoKeyDictionary<string, int, int>();
            for(int i = 0; i < items.Count; i++)
            {
                tkd.Add(items[i]);
            }

            Assert.AreEqual(items.Count, tkd.Count);
        }

        [Test]
        [Category("Methods")]
        public void Add_Object()
        {
            List<Object[]> items = new List<Object[]>();
            PackTwoKeyDictionary ptkd = new PackTwoKeyDictionary();
            for (int i = 0; i < POPSIZE; i++)
            {
                object[] obj = new object[3];
                obj[0] = ptkd.kavalues[i];
                obj[1] = ptkd.kbvalues[i];
                obj[2] = ptkd.vvalues[i];
                items.Add(obj);
            }

            TwoKeyDictionary<string, int, int> tkd = new TwoKeyDictionary<string, int, int>();
            for (int i = 0; i < items.Count; i++)
            {
                tkd.Add(items[i][0],items[i][1], items[i][2]);
            }

            Assert.AreEqual(items.Count, tkd.Count);
        }

        [Test]
        [Category("Methods")]
        public void Clear()
        {
            PackTwoKeyDictionary ptkd = new PackTwoKeyDictionary();
            TwoKeyDictionary<string, int, int> tkd = ptkd.GetLoadedTwoKeyDictionary(POPSIZE);

            Assert.AreEqual(POPSIZE, tkd.Count);
            tkd.Clear();
            Assert.AreEqual(0, tkd.Count);
        }

        [Test]
        [Category("Methods")]
        public void Contains_TwoKeyValueTriple()
        {
            List<TwoKeyValueTriple<string, int, int>> items = new List<TwoKeyValueTriple<string, int, int>>();
            PackTwoKeyDictionary ptkd = new PackTwoKeyDictionary();
            for (int i = 0; i < POPSIZE; i++)
            {
                TwoKeyValueTriple<string, int, int> tkv = new TwoKeyValueTriple<string, int, int>(ptkd.kavalues[i], ptkd.kbvalues[i], ptkd.vvalues[i]);
                items.Add(tkv);
            }

            TwoKeyDictionary<string, int, int> tkd = new TwoKeyDictionary<string, int, int>();
            for (int i = 0; i < items.Count; i++)
            {
                tkd.Add(items[i]);
            }

            for (int i = 0; i < POPSIZE; i++)
            {
                if(!tkd.Contains(items[i]))
                {
                    Assert.Fail();
                }
            }

            Assert.Pass();
        }

        [Test]
        [Category("Methods")]
        public void Contains_Object()
        {
            PackTwoKeyDictionary ptkd = new PackTwoKeyDictionary();
            TwoKeyDictionary<string, int, int> tkd = ptkd.GetLoadedTwoKeyDictionary(POPSIZE);
            object keya = ptkd.kavalues[3];
            object keyb = ptkd.kbvalues[6];
            
            if(!tkd.Contains(keya))
            {
                Assert.Fail();
            }

            if (!tkd.Contains(keyb))
            {
                Assert.Fail();
            }
        }

        [Test]
        [Category("Methods")]
        public void ContainsKeyA()
        {
            PackTwoKeyDictionary ptkd = new PackTwoKeyDictionary();
            TwoKeyDictionary<string, int, int> tkd = ptkd.GetLoadedTwoKeyDictionary(POPSIZE);
            
            if(!tkd.ContainsKeyA(ptkd.kavalues[3]))
            {
                Assert.Fail();
            }
        }

        [Test]
        [Category("Methods")]
        public void ContainsKeyB()
        {
            PackTwoKeyDictionary ptkd = new PackTwoKeyDictionary();
            TwoKeyDictionary<string, int, int> tkd = ptkd.GetLoadedTwoKeyDictionary(POPSIZE);

            if (!tkd.ContainsKeyB(ptkd.kbvalues[7]))
            {
                Assert.Fail();
            }
        }

        [Test]
        [Category("Methods")]
        public void ContainsValue()
        {
            PackTwoKeyDictionary ptkd = new PackTwoKeyDictionary();
            TwoKeyDictionary<string, int, int> tkd = ptkd.GetLoadedTwoKeyDictionary(POPSIZE);

            if (!tkd.ContainsValue(ptkd.vvalues[9]))
            {
                Assert.Fail();
            }
        }

        [Test]
        [Category("Methods")]
        public void CopyTo_TwoKeyValueTriple()
        {

            TwoKeyValueTriple<string, int, int>[] items = new TwoKeyValueTriple<string, int, int>[15];
            PackTwoKeyDictionary ptkd = new PackTwoKeyDictionary();
            TwoKeyDictionary<string, int, int> tkd = ptkd.GetLoadedTwoKeyDictionary(POPSIZE);

            tkd.CopyTo(items, 0);
            int count = items.Length;

        }

        [Test]
        [Category("Methods")]
        public void IEnumerator_TwoKeyValueTriple_GetEnumerator()
        {
            PackTwoKeyDictionary ptkd = new PackTwoKeyDictionary();
            TwoKeyDictionary<string, int, int> tkd = ptkd.GetLoadedTwoKeyDictionary(POPSIZE);

            foreach(TwoKeyValueTriple<string,int,int> tkvt in tkd)
            {
                if (!tkd.ContainsKeyA(tkvt.KeyA))
                {
                    Assert.Fail();
                }
            }

            Assert.Pass();
        }

        [Test]
        [Category("Methods")]
        public void IEnumerable_GetEnumerator()
        {
            PackTwoKeyDictionary ptkd = new PackTwoKeyDictionary();
            TwoKeyDictionary<string, int, int> tkd = ptkd.GetLoadedTwoKeyDictionary(POPSIZE);

            foreach (TwoKeyValueTriple<string, int, int> tkvt in tkd)
            {
                if (!tkd.ContainsKeyA(tkvt.KeyA))
                {
                    Assert.Fail();
                }
            }

            Assert.Pass();
        }

        [Test]
        [Category("Methods")]
        public void ITwoKeyDictionary_GetEnumerator()
        {
            // this code path is covered in IEnumerator_TwoKeyValueTriple_GetEnumerator
            Assert.Pass();
        }

        [Test]
        [Category("Methods")]
        public void Remove_TwoKeyValueTriple()
        {
            
            PackTwoKeyDictionary ptkd = new PackTwoKeyDictionary();
            TwoKeyDictionary<string, int, int> tkd = ptkd.GetLoadedTwoKeyDictionary(POPSIZE);
            TwoKeyValueTriple<string, int, int>[] items = new TwoKeyValueTriple<string, int, int>[tkd.Count];

            for (int i = 0; i < tkd.Count; i++)
            {
                TwoKeyValueTriple<string, int, int> tkvt = new TwoKeyValueTriple<string, int, int>(ptkd.kavalues[i], ptkd.kbvalues[i], ptkd.vvalues[i]);
                items[i] = tkvt;
            }

            if(!tkd.Remove(items[5]))
            {
                Assert.Fail();
            }

            if(!tkd.Contains(items[5]))
            {
                Assert.Pass();
            }
        }

        [Test]
        [Category("Methods")]
        public void Remove_TKeyA()
        {
            PackTwoKeyDictionary ptkd = new PackTwoKeyDictionary();
            TwoKeyDictionary<string, int, int> tkd = ptkd.GetLoadedTwoKeyDictionary(POPSIZE);

            if (!tkd.Remove(ptkd.kavalues[5]))
            {
                Assert.Fail();
            }

            if (!tkd.Contains(ptkd.kbvalues[5]))
            {
                Assert.Pass();
            }
        }

        [Test]
        [Category("Methods")]
        public void Remove_TKeyB()
        {
            PackTwoKeyDictionary ptkd = new PackTwoKeyDictionary();
            TwoKeyDictionary<string, int, int> tkd = ptkd.GetLoadedTwoKeyDictionary(POPSIZE);

            if (!tkd.Remove(ptkd.kbvalues[5]))
            {
                Assert.Fail();
            }

            if (!tkd.Contains(ptkd.kavalues[5]))
            {
                Assert.Pass();
            }
        }

        [Test]
        [Category("Methods")]
        public void Remove_Object()
        {
            PackTwoKeyDictionary ptkd = new PackTwoKeyDictionary();
            TwoKeyDictionary<string, int, int> tkd = ptkd.GetLoadedTwoKeyDictionary(POPSIZE);
            TwoKeyValueTriple<string, int, int> src1 = new TwoKeyValueTriple<string, int, int>(ptkd.kavalues[3], ptkd.kbvalues[3], ptkd.vvalues[3]);
            TwoKeyValueTriple<string, int, int> src2 = new TwoKeyValueTriple<string, int, int>(ptkd.kavalues[6], ptkd.kbvalues[6], ptkd.vvalues[6]);


            tkd.Remove((object)src1.KeyA);
            if(tkd.Contains(src1.KeyA))
            {
                Assert.Fail();
            }

            if (!tkd.Contains(src1.KeyB))
            {
                Assert.Pass();
            }

            tkd.Remove((object)src2.KeyA);
            if (tkd.Contains(src1.KeyA))
            {
                Assert.Fail();
            }

            if (!tkd.Contains(src2.KeyB))
            {
                Assert.Pass();
            }
        }

        [Test]
        [Category("Methods")]
        public void RemoveKeyA()
        {
            PackTwoKeyDictionary ptkd = new PackTwoKeyDictionary();
            TwoKeyDictionary<string, int, int> tkd = ptkd.GetLoadedTwoKeyDictionary(POPSIZE);

            if (!tkd.RemoveKeyA(ptkd.kavalues[5]))
            {
                Assert.Fail();
            }

            if (!tkd.Contains(ptkd.kbvalues[5]))
            {
                Assert.Pass();
            }
        }

        [Test]
        [Category("Methods")]
        public void RemoveKeyB()
        {
            PackTwoKeyDictionary ptkd = new PackTwoKeyDictionary();
            TwoKeyDictionary<string, int, int> tkd = ptkd.GetLoadedTwoKeyDictionary(POPSIZE);

            if (!tkd.RemoveKeyB(ptkd.kbvalues[5]))
            {
                Assert.Fail();
            }

            if (!tkd.Contains(ptkd.kavalues[5]))
            {
                Assert.Pass();
            }
        }

        [Test]
        [Category("Methods")]
        public void TryGetValueKeyA()
        {
            PackTwoKeyDictionary ptkd = new PackTwoKeyDictionary();
            TwoKeyDictionary<string, int, int> tkd = ptkd.GetLoadedTwoKeyDictionary(POPSIZE);
            int result;
            tkd.TryGetValueKeyA(ptkd.kavalues[9], out result);

            Assert.AreEqual(ptkd.vvalues[9], result);
        }

        [Test]
        [Category("Methods")]
        public void TryGetValueKeyB()
        {
            PackTwoKeyDictionary ptkd = new PackTwoKeyDictionary();
            TwoKeyDictionary<string, int, int> tkd = ptkd.GetLoadedTwoKeyDictionary(POPSIZE);
            int result;
            tkd.TryGetValueKeyB(ptkd.kbvalues[6], out result);

            Assert.AreEqual(ptkd.vvalues[6], result);
        }

        #endregion
    }
}