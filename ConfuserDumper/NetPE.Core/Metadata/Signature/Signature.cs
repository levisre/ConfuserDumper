using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace NetPE.Core.Metadata.Signature
{
    public interface ISignature
    {
        void Read(SignatureReader rdr);
        uint GetSize();
        void Write(SignatureWriter wtr);
    }
}
