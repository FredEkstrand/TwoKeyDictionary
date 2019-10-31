using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ekstrand.Collections.Generic
{
    interface IReadOnlyTwoKeyDictionary<TKeyA, TKeyB, TValue> : IReadOnlyCollection<TwoKeyValueTriple<TKeyA, TKeyB, TValue>>
    {
        bool ContainsKeyA(TKeyA keyA);

        bool ContainsKeyB(TKeyB keyB);

        bool TryGetValueKeyA(TKeyA keyA, out TValue value);

        bool TryGetValueKeyB(TKeyB keyB, out TValue value);

        TValue this[TKeyA keyA] { get; }

        TValue this[TKeyB keyB] { get; }

        IEnumerable<TKeyA> AKeys { get; }

        IEnumerable<TKeyB> BKeys { get; }

        IEnumerable<TValue> Values { get; }
    }

    public interface IReadOnlyCollection<out T> : IEnumerable<T>
    {
        int Count { get; }
    }
}
