using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections.ObjectModel;
using NetPE.Core.Metadata.Tables;

namespace NetPE.Core.Metadata.Methods
{
    public class DataSectionCollection : Collection<DataSection>
    {
        MetadataRow par;
        public DataSectionCollection(MetadataRow par) { this.par = par; }
        public MetadataRow Parent { get { return par; } }

        public uint GetSize()
        {
            uint ret = 0;
            foreach (DataSection sect in Items)
            {
                ret += sect.GetSize();
            }
            return ret;
        }

        public void Load(BinaryReader rdr)
        {
            while (true)
            {
                DataSection sect = new DataSection(par);
                sect.Load(rdr);
                Items.Add(sect);
                if ((sect.Flags & SectionFlags.MoreSects) != SectionFlags.MoreSects)
                    break;
            }
        }

        public void Save(BinaryWriter wtr)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                if (i == Items.Count - 1)
                    Items[i].Flags &= ~SectionFlags.MoreSects;
                else
                    Items[i].Flags |= SectionFlags.MoreSects;
                Items[i].Save(wtr);
            }
        }
    }
    public class DataSection : Collection<ExceptionClause>
    {
        MetadataRow par;
        public DataSection(MetadataRow par) { this.par = par; }
        public MetadataRow Parent { get { return par; } }

        SectionFlags f;
        public SectionFlags Flags { get { return f; } set { f = value; } }

        public bool IsTinyHeader()
        {
            bool tiny = true;
            foreach (ExceptionClause c in Items)
            {
                tiny &= c.IsTinyHeader();
            }
            if (tiny && this.Count < 21) //tiny, n*12+4<=255,n=21
            {
                return true;
            }
            else //fat
            {
                return false;
            }
        }

        public uint GetSize()
        {
            if (IsTinyHeader())
            {
                return (uint)(Items.Count * 12 + 4);
            }
            else
            {
                return (uint)(Items.Count * 24 + 4);
            }
        }

        public void Load(BinaryReader rdr)
        {
            f = (SectionFlags)rdr.ReadByte();
            uint s;
            if ((f & SectionFlags.FatFormat) == SectionFlags.FatFormat)
            {
                rdr.BaseStream.Seek(-1, SeekOrigin.Current);
                s = ((rdr.ReadUInt32() >> 8) - 4) / 24;
                for (int i = 0; i < s; i++)
                {
                    ExceptionClause c = new ExceptionClause(par);
                    c.Load(false, rdr);
                    Items.Add(c);
                }
            }
            else
            {
                s = (rdr.ReadByte() - 4U) / 12;
                rdr.ReadByte(); rdr.ReadByte();
                for (int i = 0; i < s; i++)
                {
                    ExceptionClause c = new ExceptionClause(par);
                    c.Load(true, rdr);
                    Items.Add(c);
                }
            }
        }

        public void Save(BinaryWriter wtr)
        {
            if (IsTinyHeader())
            {
                wtr.Write((byte)f);
                wtr.Write((byte)this.GetSize());
                wtr.Write((byte)0);
                wtr.Write((byte)0);
                foreach (ExceptionClause c in Items)
                    c.Save(true, wtr);
            }
            else
            {
                wtr.Write((uint)((uint)f << 24 | this.GetSize()));
                foreach (ExceptionClause c in Items)
                    c.Save(false, wtr);
            }
        }
    }
}
