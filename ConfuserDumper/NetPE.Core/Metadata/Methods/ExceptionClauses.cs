using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetPE.Core.Metadata.Tables;
using System.IO;

namespace NetPE.Core.Metadata.Methods
{
    public class ExceptionClause
    {
        MetadataRow par;
        public ExceptionClause(MetadataRow par) { this.par = par; }
        public MetadataRow Parent { get { return par; } }

        ExceptionClauseFlags f;
        public ExceptionClauseFlags Flags { get { return f; } set { f = value; } }
        uint to;
        uint tl;
        public uint TryOffset { get { return to; } set { to = value; } }
        public uint TryLength { get { return tl; } set { tl = value; } }
        uint ho;
        uint hl;
        public uint HandlerOffset { get { return ho; } set { ho = value; } }
        public uint HandlerLength { get { return hl; } set { hl = value; } }
        TableToken tkn;
        public TableToken ClassToken { get { return tkn; } set { tkn = value; } }
        uint fo;
        public uint FilterOffset { get { return fo; } set { fo = value; } }

        public bool IsTinyHeader()
        {
            if (to < ushort.MaxValue && ho < ushort.MaxValue &&
                tl < byte.MaxValue && hl < byte.MaxValue)
                return true;
            else
                return false;
        }

        public void Load(bool isTiny, BinaryReader rdr)
        {
            if (isTiny)
            {
                f = (ExceptionClauseFlags)rdr.ReadUInt16();
                to = rdr.ReadUInt16();
                tl = rdr.ReadByte();
                ho = rdr.ReadUInt16();
                hl = rdr.ReadByte();
            }
            else
            {
                f = (ExceptionClauseFlags)rdr.ReadUInt32();
                to = rdr.ReadUInt32();
                tl = rdr.ReadUInt32();
                ho = rdr.ReadUInt32();
                hl = rdr.ReadUInt32();
            }
            if ((f & ExceptionClauseFlags.Filter) == ExceptionClauseFlags.Filter)
                fo = rdr.ReadUInt32();
            else
            {
                tkn = new TableToken(par.Container.Heap);
                tkn.Token = rdr.ReadUInt32();
            }
        }

        public void Save(bool isTiny, BinaryWriter wtr)
        {
            if (isTiny)
            {
                wtr.Write((ushort)f);
                wtr.Write((ushort)to);
                wtr.Write((byte)tl);
                wtr.Write((ushort)ho);
                wtr.Write((byte)hl);
            }
            else
            {
                wtr.Write((uint)f);
                wtr.Write((uint)to);
                wtr.Write((uint)tl);
                wtr.Write((uint)ho);
                wtr.Write((uint)hl);
            }
            if ((f & ExceptionClauseFlags.Filter) == ExceptionClauseFlags.Filter)
                wtr.Write((uint)fo);
            else
                wtr.Write((uint)tkn.Token);
        }
    }
}
