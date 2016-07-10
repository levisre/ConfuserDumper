using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetPE.Core.Metadata.Signature.Types
{
    public class FNPTR : TypeElement
    {
        MethodSig mtd;
        public MethodSig Method { get { return mtd; } set { mtd = value; } }

        public override void Read(SignatureReader rdr)
        {
            Element = rdr.ReadElementType();
            mtd = new MethodSig();
            mtd.Read(rdr);
        }

        public override uint GetSize()
        {
            return 1 + mtd.GetSize();
        }

        public override void Write(SignatureWriter wtr)
        {
            wtr.Write(Element);
            mtd.Write(wtr);
        }
    }
}
