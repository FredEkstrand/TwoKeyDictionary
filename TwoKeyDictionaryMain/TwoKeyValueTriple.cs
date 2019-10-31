using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ekstrand.Collections.Generic
{
    [Serializable]
    public struct TwoKeyValueTriple<TKeyA, TKeyB, TValue>
    {
        private TKeyA keyA;
        private TKeyB keyB;
        private TValue value;

        public TwoKeyValueTriple(TKeyA keyA, TKeyB keyB, TValue value)
        {
            this.keyA = keyA;
            this.keyB = keyB;
            this.value = value;
        }

        public TKeyA KeyA
        {
            get { return keyA; }
        }

        public TKeyB KeyB
        {
            get { return keyB; }
        }

        public TValue Value
        {
            get { return value; }
        }

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