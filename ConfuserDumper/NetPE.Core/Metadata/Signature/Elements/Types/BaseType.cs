using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetPE.Core.Metadata.Signature.Types
{
    public class BaseType : TypeElement
    {
        public override void Read(SignatureReader rdr)
        {
            Element = rdr.ReadElementType();
        }

        public override uint GetSize()
        {
            return 1;
        }

        public override void Write(SignatureWriter wtr)
        {
            wtr.Write(Element);
        }
    }
}
