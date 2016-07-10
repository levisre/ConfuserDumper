using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using NetPE.Core.Metadata.Heaps;
using NetPE.Core.Metadata.Tables;

namespace NetPE.Core.Metadata
{
    public class GUIDToken : MetadataTokenProvider
    {
        internal GUIDToken(GUIDHeap h) : base(h) { }

        public GUIDHeap Heap
        {
            get
            {
                return Container as GUIDHeap;
            }
        }

        public static readonly GUIDToken Null = new GUIDToken(null) { Token = new MetadataToken(MetadataTokenType.Unknown, 0) };
    }
    public class StringToken : MetadataTokenProvider
    {
        internal StringToken(StringsHeap h) : base(h) { }

        public StringsHeap Heap
        {
            get
            {
                return Container as StringsHeap;
            }
        }

        public static readonly StringToken Null = new StringToken(null) { Token = new MetadataToken(MetadataTokenType.Unknown, 0) };
    }
    public class BlobToken : MetadataTokenProvider
    {
        internal BlobToken(BlobHeap h) : base(h) { }

        public BlobHeap Heap
        {
            get
            {
                return Container as BlobHeap;
            }
        }

        public static readonly BlobToken Null = new BlobToken(null) { Token = new MetadataToken(MetadataTokenType.Unknown, 0) };
    }
    public class USToken : MetadataTokenProvider
    {
        internal USToken(USHeap h) : base(h) { }

        public USHeap Heap
        {
            get
            {
                return Container as USHeap;
            }
        }

        public static readonly USToken Null = new USToken(null) { Token = new MetadataToken(MetadataTokenType.String, 0) };
    }
    public class TableToken : MetadataTokenProvider
    {
        public TableToken(TablesHeap h, MetadataToken tkn) : base(h) { this.Token = tkn; }
        public TableToken(TablesHeap h) : base(h) { }

        public TablesHeap Heap
        {
            get
            {
                return Container as TablesHeap;
            }
        }

        public MetadataRow ResolveRow()
        {
            MetadataTable tbl = (Container as TablesHeap)[(TableType)Token.Type];
            if (tbl == null) // table type not found
                return null;
            return tbl.Rows[(int)Token.Index];
        }

        public MetadataTable ResolveTable()
        {
            return Heap[(TableType)Token.Type];
        }
    }
}
