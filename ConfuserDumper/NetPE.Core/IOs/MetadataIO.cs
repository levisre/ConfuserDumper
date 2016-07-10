using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using NetPE.Core.Metadata.Heaps;
using NetPE.Core.Metadata.Tables;

namespace NetPE.Core.Metadata
{
    public class MetadataReader : BinaryReader
    {
        TablesHeap tbls;
        public MetadataReader(MetadataStream str) : base(str) { tbls = str.Root[MetadataStreamType.Tables].Heap as TablesHeap; }
        public new MetadataStream BaseStream { get { return base.BaseStream as MetadataStream; } }

        public uint ReadCompressedUInt() { return (uint)base.Read7BitEncodedInt(); }
        public int ReadCompressedInt() { return base.Read7BitEncodedInt(); }
    }

    public class MetadataWriter : BinaryWriter
    {
        TablesHeap tbls;
        public MetadataWriter(MetadataStream str) : base(str) { tbls = str.Root[MetadataStreamType.Tables].Heap as TablesHeap; }
        public new MetadataStream BaseStream { get { return base.BaseStream as MetadataStream; } }

        public void WriteCompressedUInt(uint val) { base.Write7BitEncodedInt((int)val); }
        public void WriteCompressedInt(int val) { base.Write7BitEncodedInt(val); }
    }
}
