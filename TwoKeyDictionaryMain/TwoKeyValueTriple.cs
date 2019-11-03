using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ekstrand.Collections.Generic
{
    /// <summary>
    /// Defines a A-key,B-key/value triple that can be set or retrieved.
    /// </summary>
    /// <typeparam name="TKeyA">The type of the A-key.</typeparam>
    /// <typeparam name="TKeyB">The type of the B-key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    [Serializable]
    public struct TwoKeyValueTriple<TKeyA, TKeyB, TValue>
    {
        private TKeyA keyA;
        private TKeyB keyB;
        private TValue value;

        /// <summary>
        /// Defines a A-key,B-key/value triple that can be set or retrieved.
        /// </summary>
        /// <param name="keyA">The type of the A-key.</param>
        /// <param name="keyB">The type of the B-key.</param>
        /// <param name="value">The type of the value.</param>
        public TwoKeyValueTriple(TKeyA keyA, TKeyB keyB, TValue value)
        {
            this.keyA = keyA;
            this.keyB = keyB;
            this.value = value;
        }

        /// <summary>
        /// Gets the A-key in the A-key,B-key/value triple.
        /// </summary>
        public TKeyA KeyA
        {
            get { return keyA; }
        }

        /// <summary>
        /// Gets the B-key in the A-key,B-key/value triple.
        /// </summary>
        public TKeyB KeyB
        {
            get { return keyB; }
        }

        /// <summary>
        /// Gets the value in the A-key,B-key/value triple.
        /// </summary>
        public TValue Value
        {
            get { return value; }
        }

        /// <summary>
        /// Returns a string representation of the TwoKeyValueTriple&lt;TKeyA, TKeyB, TValue&gt;, using the string representations of the key and value.
        /// </summary>
        /// <returns>A string representation of the TwoKeyValueTriple&lt;TKeyA, TKeyB, TValue&gt;, which includes the string representations of the key and value.</returns>
        public override string ToString()
        {
            StringBuilder s = new StringBuilder();
            s.Append('[');
            if (KeyA != null)
            {
                s.Append(KeyA.ToString());
            }
            s.Append(", ");
            if (KeyB != null)
            {
                s.Append(KeyB.ToString());
            }
            s.Append(", ");
            if (Value != null)
            {
                s.Append(Value.ToString());
            }
            s.Append(']');
            return s.ToString();
        }
    }
}