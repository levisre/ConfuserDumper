using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using NetPE.Core.Metadata.Tables;
using NetPE.Core.DataDirectories;

namespace NetPE.Core.Metadata.Methods
{
    public abstract class MethodBody : IMetadataData
    {
        MetadataRow par;
        public MethodBody(MetadataRow par) { this.par = par; }
        public MetadataRow Parent { get { return par; } }

        public abstract uint Size { get; }
        public abstract void Load(BinaryReader rdr);
        public abstract void Save(BinaryWriter wtr);

        public void Load(byte[] data)
        {
            Load(new BinaryReader(new MemoryStream(data)));
        }

        public void Save(out byte[] data)
        {
            MemoryStream ret = new MemoryStream();
            Save(new BinaryWriter(ret));
            data = ret.ToArray();
        }
    }
}
