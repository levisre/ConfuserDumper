using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetPE.Core.Metadata.Signature.Types
{
    public class VALUETYPE : TypeElement
    {
        TableToken tkn;
        public TableToken Type { get { return tkn; } set { tkn = value; } }

        public override void Read(SignatureReader rdr)
        {
            Element = rdr.ReadElementType();
            tkn = rdr.ReadTypeDefOrRefEncoded();
        }

        public override uint GetSize()
        {
            return 1 + SignatureHelper.GetCompressedSize(tkn.Token.Value);
        }

        public override void Write(SignatureWriter wtr)
        {
            wtr.Write(Element);
            wtr.WriteTypeDefOrRefEncoded(tkn);
        }
    }
}
