using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetPE.Core.Metadata.Signature
{
    public class MethodSpecSig : ISignature
    {
        byte pl;
        public byte Prolog { get { return pl; } set { pl = value; } }
        TypeElementCollection ts;
        public TypeElementCollection GenericArgs { get { return ts; } }
        
        public void Read(SignatureReader rdr)
        {
            pl = rdr.ReadByte();
            ts = new TypeElementCollection();
            ts.Read(rdr);
        }

        public uint GetSize()
        {
            return 1 + ts.GetSize();
        }

        public void Write(SignatureWriter wtr)
        {
            wtr.Write(pl);
            ts.Write(wtr);
        }
    }
}
