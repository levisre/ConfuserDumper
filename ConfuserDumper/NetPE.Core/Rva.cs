using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetPE.Core
{
    public struct Rva:IComparable<Rva>
    {
        public Rva(uint val) { rva = val; }
        private uint rva;
        public uint Value { get { return rva; } set { rva = value; } }

        public static implicit operator uint(Rva rva)
        {
            return rva.rva;
        }

        public static implicit operator Rva(uint rva)
        {
            return new Rva(rva);
        }

        public override string ToString()
        {
            return rva.ToString("x8");
        }

        public int CompareTo(Rva other)
        {
            return Comparer<uint>.Default.Compare(rva, other.rva);
        }
    }
}
