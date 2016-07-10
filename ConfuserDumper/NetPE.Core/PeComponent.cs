using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace NetPE.Core
{
    public abstract class PeComponent
    {
        PeFile file;
        public PeFile File { get { return file; } }

        public PeComponent(PeFile file)
        {
            this.file = file;
        }
        public abstract void Read(PeReader rdr);
        public abstract void Write(PeWriter wtr);
    }
}
