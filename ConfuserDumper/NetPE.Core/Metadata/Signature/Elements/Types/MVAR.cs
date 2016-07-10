using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetPE.Core.Metadata.Signature.Types
{
    public class MVAR : TypeElement
    {
        int num;
        public int Number { get { return num; } set { num = value; } }

        public override void Read(SignatureReader rdr)
        {
            Element = rdr.ReadElementType();
            num = rdr.ReadCompressedInt();
        }

        public override uint GetSize()
        {
            return 1 + SignatureHelper.GetCompressedSize(num);
        }

        public override void Write(SignatureWriter wtr)
        {
            wtr.Write(Element);
            wtr.WriteCompressedInt(num);
        }
    }
}
