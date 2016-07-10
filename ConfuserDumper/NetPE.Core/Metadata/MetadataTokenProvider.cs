using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetPE.Core.Metadata
{
    public abstract class MetadataTokenProvider
    {
        public MetadataTokenProvider(object can) { this.can = can; }

        MetadataToken tkn;
        public MetadataToken Token { get { return tkn; } protected internal set { tkn = value; } }

        object can;
        public object Container { get { return can; } }

        public override string ToString()
        {
            return tkn.ToString();
        }
    }
}
