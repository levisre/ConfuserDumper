using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetPE.Core.Metadata.Signature.Types
{
    public class ARRAY : TypeElement
    {
        TypeElement t;
        public TypeElement Type { get { return t; } set { t = value; } }
        ArrayShapeElement s;
        public ArrayShapeElement Shape { get { return s; } set { s = value; } }

        public override void Read(SignatureReader rdr)
        {
            Element = rdr.ReadElementType();
            t = TypeElement.ReadType(rdr);
            s = new ArrayShapeElement();
            s.Read(rdr);
        }

        public override uint GetSize()
        {
            return 1 + t.GetSize() + s.GetSize();
        }

        public override void Write(SignatureWriter wtr)
        {
            wtr.Write(Element);
            t.Write(wtr);
            s.Write(wtr);
        }
    }
}
