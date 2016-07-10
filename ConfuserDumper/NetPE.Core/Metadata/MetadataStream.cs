using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using NetPE.Core.Metadata.Heaps;
using NetPE.Core.Metadata.Tables;

namespace NetPE.Core.Metadata
{
    public enum MetadataStreamType
    {
        Strings,
        US,
        Blob,
        GUID,
        Tables,
        Unknown
    }
    public class MetadataStream : Stream
    {
        public MetadataStream(MetadataRoot root) { this.root = root; }
        MetadataRoot root;
        public MetadataRoot Root { get { return root; } }

        public MetadataStreamType Type
        {
            get
            {
                switch (n)
                {
                    case "#Strings": return MetadataStreamType.Strings;
                    case "#US": return MetadataStreamType.US;
                    case "#Blob": return MetadataStreamType.Blob;
                    case "#GUID": return MetadataStreamType.GUID;
                    case "#~": return MetadataStreamType.Tables;
                    case "#-": return MetadataStreamType.Tables;
                    default: return MetadataStreamType.Unknown;
                }
            }
        }
        string n;
        public string Name { get { return n; } set { n = value; } }
        PeFile f;
        public PeFile File { get { return f; } set { f = value; } }

        byte[] dat;
        public byte[] Data { get { return dat; } internal set { dat = value; } }
        long now;

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override void Flush()
        {
            //
        }

        public override long Length
        {
            get { return dat.Length; }
        }

        public override long Position
        {
            get
            {
                return now;
            }
            set
            {
                now = value;
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int ret = 0;
            for (int i = 0; i < count; i++)
            {
                if (now == dat.Length) break;
                buffer[offset + i] = dat[now];
                now++; ret++;
            }
            return ret;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    now = offset; break;
                case SeekOrigin.Current:
                    now += offset; break;
                case SeekOrigin.End:
                    now = dat.Length - offset; break;
            }
            return now;
        }

        public override void SetLength(long value)
        {
            byte[] newDat = new byte[value];
            Buffer.BlockCopy(dat, 0, newDat, 0, (int)Math.Min(dat.Length, value));
            dat = newDat;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (now + count > dat.Length) SetLength(dat.Length + 0x100);
            for (int i = 0; i < count; i++)
            {
                dat[now] = buffer[offset + i];
                now++;
            }
        }

        public override string ToString()
        {
            return n;
        }

        object h;
        public object Heap { get { if (h == null)Load(); return h; } }

        internal void Load()
        {
            switch (Type)
            {
                case MetadataStreamType.Blob:
                    h = new BlobHeap(this); 
                    break;
                case MetadataStreamType.GUID: 
                    h = new GUIDHeap(this);
                    break;
                case MetadataStreamType.Strings: 
                    h = new StringsHeap(this); 
                    break;
                case MetadataStreamType.US:
                    h = new USHeap(this);
                    break;
                case MetadataStreamType.Tables:
                    h = new TablesHeap(this);
                    (h as TablesHeap).Load();
                    break;
                default:
                    h = null;
                    break;
            }
        }
    }
}
