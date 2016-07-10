using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetPE.Core.Metadata.Signature.Types
{
    public class GENERICINST : TypeElement
    {
        bool vt;
        public bool IsValueType { get { return vt; } set { vt = value; } }
        TableToken t;
        public TableToken Type { get { return t; } set { t = value; } }
        TypeElementCollection ts;
        public TypeElementCollection GenericArgs { get { return ts; } }

        public override void Read(SignatureReader rdr)
        {
            Element = rdr.ReadElementType();
            vt = rdr.ReadElementType() == ElementType.ValueType;
            t = rdr.ReadTypeDefOrRefEncoded();
            ts = new TypeElementCollection();
            ts.Read(rdr);
        }

        public override uint GetSize()
        {
            return 2 + SignatureHelper.GetCompressedSize(t.Token.Value) + ts.GetSize();
        }

        public override void Write(SignatureWriter wtr)
        {
            wtr.Write(Element);
            wtr.Write(vt ? ElementType.ValueType : ElementType.Class);
            wtr.WriteTypeDefOrRefEncoded(t);
            ts.Write(wtr);
        }
    }
}
