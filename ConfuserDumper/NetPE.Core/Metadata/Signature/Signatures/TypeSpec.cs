using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetPE.Core.Metadata.Signature
{
    public class TypeSpecSig : ISignature
    {
        TypeElement t;
        TypeElement InnerType { get { return t; } set { t = value; } }

        public void Read(SignatureReader rdr)
        {
            t.Read(rdr);
        }

        public uint GetSize()
        {
            return t.GetSize();
        }

        public void Write(SignatureWriter wtr)
        {
            t.Write(wtr);
        }
    }
}
