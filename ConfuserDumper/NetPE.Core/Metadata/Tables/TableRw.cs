using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetPE.Core.Metadata.Heaps;
using NetPE.Core.Pe;
using NetPE.Core.Metadata.Signature;
using NetPE.Core.Metadata.Signature.Types;
using NetPE.Core.Metadata.Methods;
using NetPE.Core.DataDirectories;

namespace NetPE.Core.Metadata.Tables
{
    internal class TableRw
    {
        static readonly Dictionary<Type, Reader> DicReader = new Dictionary<Type, Reader>();
        static readonly Dictionary<Type, Reader> DicEnumReader = new Dictionary<Type, Reader>();
        static TableRw()
        {
            DicEnumReader[typeof(byte)] = ReadByteFlag;
            DicEnumReader[typeof(System.UInt16)] = ReadUInt16Flag;
            DicEnumReader[typeof(System.UInt32)] = ReadUInt32Flag;

            DicReader[typeof(byte)] = ReadByte;
            DicReader[typeof(System.UInt16)] = ReadUInt16;
            DicReader[typeof(System.UInt32)] = ReadUInt32;
            DicReader[typeof(NetPE.Core.Rva)] = ReadRva;
            DicReader[typeof(NetPE.Core.Metadata.BlobToken)] = ReadBlobToken;
            DicReader[typeof(NetPE.Core.Metadata.GUIDToken)] = ReadGUIDToken;
            DicReader[typeof(NetPE.Core.Metadata.StringToken)] = ReadStringToken;
        }

        delegate object Reader(int col, MetadataTable tbl, MetadataReader rdr, uint[] lens);

        static object ReadByteFlag(int col, MetadataTable tbl, MetadataReader rdr, uint[] lens)
        {
            return Enum.ToObject(tbl.TableDef.Columns[col].Value as Type, rdr.ReadByte());
        }
        static object ReadUInt16Flag(int col, MetadataTable tbl, MetadataReader rdr, uint[] lens)
        {
            return Enum.ToObject(tbl.TableDef.Columns[col].Value as Type, rdr.ReadUInt16());
        }
        static object ReadUInt32Flag(int col, MetadataTable tbl, MetadataReader rdr, uint[] lens)
        {
            return Enum.ToObject(tbl.TableDef.Columns[col].Value as Type, rdr.ReadUInt32());
        }

        static object ReadByte(int col, MetadataTable tbl, MetadataReader rdr, uint[] lens)
        {
            return rdr.ReadByte();
        }
        static object ReadUInt16(int col, MetadataTable tbl, MetadataReader rdr, uint[] lens)
        {
            return rdr.ReadUInt16();
        }
        static object ReadUInt32(int col, MetadataTable tbl, MetadataReader rdr, uint[] lens)
        {
            return rdr.ReadUInt32();
        }
        static object ReadRva(int col, MetadataTable tbl, MetadataReader rdr, uint[] lens)
        {
            return new Rva(rdr.ReadUInt32());
        }

        static object ReadBlobToken(int col, MetadataTable tbl, MetadataReader rdr, uint[] lens)
        {
            if (rdr.BaseStream.Root[MetadataStreamType.Blob] == null)
            {
                rdr.ReadUInt16();
                return BlobToken.Null;
            }
            BlobHeap bh = rdr.BaseStream.Root[MetadataStreamType.Blob].Heap as BlobHeap;
            uint tkn;
            if (bh.Stream.Length > ushort.MaxValue)
                tkn = rdr.ReadUInt32();
            else
                tkn = rdr.ReadUInt16();
            return bh.Resolve(new MetadataToken(tkn));
        }
        static object ReadGUIDToken(int col, MetadataTable tbl, MetadataReader rdr, uint[] lens)
        {
            if (rdr.BaseStream.Root[MetadataStreamType.GUID] == null)
            {
                rdr.ReadUInt16();
                return GUIDToken.Null;
            }
            GUIDHeap gh = rdr.BaseStream.Root[MetadataStreamType.GUID].Heap as GUIDHeap;
            uint tkn;
            if (gh.Stream.Length > ushort.MaxValue)
                tkn = rdr.ReadUInt32();
            else
                tkn = rdr.ReadUInt16();
            return gh.Resolve(new MetadataToken(tkn));
        }
        static object ReadStringToken(int col, MetadataTable tbl, MetadataReader rdr, uint[] lens)
        {
            if (rdr.BaseStream.Root[MetadataStreamType.Strings] == null)
            {
                rdr.ReadUInt16();
                return StringToken.Null;
            }
            StringsHeap sh = rdr.BaseStream.Root[MetadataStreamType.Strings].Heap as StringsHeap;
            uint tkn;
            if (sh.Stream.Length > ushort.MaxValue)
                tkn = rdr.ReadUInt32();
            else
                tkn = rdr.ReadUInt16();
            return sh.Resolve(new MetadataToken(tkn));
        }
        static object ReadTableToken(int col, MetadataTable tbl, MetadataReader rdr, uint[] lens)
        {
            TableToken tkn = new TableToken(tbl.Heap);
            tkn.Token = new MetadataToken((MetadataTokenType)tbl.TableDef.Columns[col].Value, 0);
            if (lens[(int)tbl.Type] > ushort.MaxValue)
                tkn.Token.Index = rdr.ReadUInt32();
            else
                tkn.Token.Index = rdr.ReadUInt16();
            return tkn;
        }
        static object ReadCodedToken(int col, MetadataTable tbl, MetadataReader rdr, uint[] lens)
        {
            TableToken tkn = new TableToken(tbl.Heap);
            tkn.Token = CodedIndexDefs.MdCodedIndexDefs[(CodedIndex)tbl.TableDef.Columns[col].Value].ReadToken(rdr, lens);
            return tkn;
        }

        void ReadRvaData(MetadataTable tbl, MetadataReader mdRdr, List<Rva> rvas, MetadataRowCollection rows)
        {
            List<Rva> sorted = new List<Rva>(rvas);
            sorted.Sort();
            for (int i = 0; i < rvas.Count; i++)
            {
                Rva rva = rvas[i];
                MetadataRow r = rows[i + 1];
                byte[] dat = null;
                VirtualReader rdr = new VirtualReader(mdRdr.BaseStream.File.SectionHeaders.GetVirtualStream());
                if (tbl.Type == TableType.FieldRVA)
                {
                    MetadataRoot root = tbl.Heap.Stream.Root;
                    rdr.SetPosition(rva);
                    MetadataRow fd = (r["Field"] as TableToken).ResolveRow();
                    FieldSig sig = new FieldSig();
                    SignatureReader sigRdr = new SignatureReader(new MetadataReader(root[MetadataStreamType.Blob]));
                    sigRdr.BaseStream.Position = (fd["Signature"] as BlobToken).Token.Index;
                    sig.Read(sigRdr);
                    int c = 0;
                    switch (sig.Type.Element)
                    {
                        case ElementType.Boolean:
                        case ElementType.UInt8:
                        case ElementType.Int8:
                            c = 1; break;
                        case ElementType.UInt16:
                        case ElementType.Int16:
                        case ElementType.Char:
                            c = 2; break;
                        case ElementType.UInt32:
                        case ElementType.Int32:
                        case ElementType.Single:
                            c = 4; break;
                        case ElementType.UInt64:
                        case ElementType.Int64:
                        case ElementType.Double:
                            c = 8; break;
                        case ElementType.ValueType:
                            TableToken vt = (sig.Type as VALUETYPE).Type;
                            foreach (MetadataRow cl in tbl.Heap[TableType.ClassLayout])
                            {
                                if (cl["Parent"] == vt)
                                {
                                    c = (int)cl["ClassSize"];
                                    break;
                                }
                            }
                            break;
                        default:
                            throw new InvalidOperationException("ReadRvaData");
                    }
                    dat = rdr.ReadBytes(c);
                }
                else if (tbl.Type == TableType.MethodDef && rva != 0)
                {
                    //MethodBody bdy;
                    //if (((MethodImplAttributes)r["ImplFlags"] & MethodImplAttributes.Native) == MethodImplAttributes.Native)
                    //{
                    //    bdy = new NativeMethodBody(r);
                    //}
                    //else
                    //{
                    //    bdy = new ManagedMethodBody(r);
                    //}
                    //rdr.SetPosition(rva);
                    //bdy.Load(rdr);
                    //rdr.SetPosition(rva);
                    //dat = rdr.ReadBytes((int)bdy.Size);
                    int idx;
                    Rva next;
                    if ((idx = sorted.IndexOf(rva)) == sorted.Count - 1)
                    {
                        CLRDirectory root = tbl.Heap.Stream.Root.Directory;
                        Rva now;
                        next = uint.MaxValue;
                        rdr.SetPosition(root.Location.Address);
                        rdr.BaseStream.Seek(0x8, System.IO.SeekOrigin.Current);
                        if ((now = rdr.ReadRva()) < next && now != 0 && now > rva) next = now;
                        rdr.BaseStream.Seek(0xC, System.IO.SeekOrigin.Current);
                        if ((now = rdr.ReadRva()) < next && now != 0 && now > rva) next = now;
                        rdr.BaseStream.Seek(0x4, System.IO.SeekOrigin.Current);
                        if ((now = rdr.ReadRva()) < next && now != 0 && now > rva) next = now;
                        rdr.BaseStream.Seek(0x4, System.IO.SeekOrigin.Current);
                        if ((now = rdr.ReadRva()) < next && now != 0 && now > rva) next = now;
                        rdr.BaseStream.Seek(0x4, System.IO.SeekOrigin.Current);
                        if ((now = rdr.ReadRva()) < next && now != 0 && now > rva) next = now;
                        rdr.BaseStream.Seek(0x4, System.IO.SeekOrigin.Current);
                        if ((now = rdr.ReadRva()) < next && now != 0 && now > rva) next = now;
                        rdr.BaseStream.Seek(0x4, System.IO.SeekOrigin.Current);
                        if ((now = rdr.ReadRva()) < next && now != 0 && now > rva) next = now;
                    }
                    else
                        next = sorted[idx + 1];
                    rdr.SetPosition(rva);
                    dat = rdr.ReadBytes((int)(next - rva));
                }
                if (!tbl.Heap.Stream.Root.Directory.Datas.ContainsAddress(rva) && dat != null)
                    tbl.Heap.Stream.Root.Directory.Datas.Add(new CLRData() { Address = rva, Data = dat });
            }
        }

        public MetadataTable Load(TablesHeap h, MetadataReader rdr, uint[] lens, TableType type)
        {
            MetadataTable ret = new MetadataTable(h, type);
            Reader[] read = new Reader[ret.Columns.Length];
            for (int i = 0; i < ret.Columns.Length; i++)
            {
                int k = i;
                var retColTypeObj = ret.Columns[i].Type;
                if (retColTypeObj is Type)
                {
                    var retColType = retColTypeObj as Type;
                    //if (retColType.IsSubclassOf(typeof(Enum)))
                    if (retColType.IsEnum)
                    {
                        var underEnumType = Enum.GetUnderlyingType(retColType);
                        //read[i] = DicEnumReader[underEnumType];
                        if (underEnumType == typeof(byte))
                            read[i] = new Reader(ReadByteFlag);
                        else if (underEnumType == typeof(System.UInt16))
                            read[i] = new Reader(ReadUInt16Flag);
                        else if (underEnumType == typeof(System.UInt32))
                            read[i] = new Reader(ReadUInt32Flag);

                        //switch (Enum.GetUnderlyingType(retColType).FullName)
                        //{
                        //    case "System.Byte":
                        //        read[i] = new Reader(ReadByteFlag);
                        //        break;
                        //    case "System.UInt16":
                        //        read[i] = new Reader(ReadUInt16Flag);
                        //        break;
                        //    case "System.UInt32":
                        //        read[i] = new Reader(ReadUInt32Flag);
                        //        break;
                        //}
                    }
                    else
                    {
                        //read[i] = DicReader[retColType];
                        if (retColType == typeof(byte))
                            read[i] = new Reader(ReadByte);
                        else if (retColType == typeof(System.UInt16))
                            read[i] = new Reader(ReadUInt16);
                        else if (retColType == typeof(System.UInt32))
                            read[i] = new Reader(ReadUInt32);
                        else if (retColType == typeof(NetPE.Core.Rva))
                            read[i] = new Reader(ReadRva);
                        else if (retColType == typeof(NetPE.Core.Metadata.BlobToken))
                            read[i] = new Reader(ReadBlobToken);
                        else if (retColType == typeof(NetPE.Core.Metadata.GUIDToken))
                            read[i] = new Reader(ReadGUIDToken);
                        else if (retColType == typeof(NetPE.Core.Metadata.StringToken))
                            read[i] = new Reader(ReadStringToken);


                        //switch ((ret.Columns[i].Type as Type).FullName)
                        //{
                        //    case "System.Byte":
                        //        read[i] = new Reader(ReadByte);
                        //        break;
                        //    case "System.UInt16":
                        //        read[i] = new Reader(ReadUInt16);
                        //        break;
                        //    case "System.UInt32":
                        //        read[i] = new Reader(ReadUInt32);
                        //        break;
                        //    case "NetPE.Core.Rva":
                        //        read[i] = new Reader(ReadRva);
                        //        break;
                        //    case "NetPE.Core.Metadata.BlobToken":
                        //        read[i] = new Reader(ReadBlobToken);
                        //        break;
                        //    case "NetPE.Core.Metadata.GUIDToken":
                        //        read[i] = new Reader(ReadGUIDToken);
                        //        break;
                        //    case "NetPE.Core.Metadata.StringToken":
                        //        read[i] = new Reader(ReadStringToken);
                        //        break;
                        //}
                    }
                }
                else if (retColTypeObj is TableType)
                    read[i] = new Reader(ReadTableToken);
                else if (retColTypeObj is CodedIndex)
                    read[i] = new Reader(ReadCodedToken);
            }

            List<Rva> rvas = new List<Rva>();
            for (int i = 1; i <= lens[(int)ret.Type]; i++)
            {
                MetadataRow r = new MetadataRow(ret, false);

                bool hasRva = ret.Type == TableType.MethodDef || ret.Type == TableType.FieldRVA;
                int rvaIdx = 0;
                Rva rva = 0;

                for (int ii = 0; ii < read.Length; ii++)
                {
                    if (hasRva && (ret.Columns[ii].Type is Type) && ((ret.Columns[ii].Type as Type) == typeof(NetPE.Core.Rva)))
                    {
                        rvaIdx = ii;
                        rva = (Rva)rdr.ReadUInt32();
                    }
                    else
                    {
                        r[ii] = read[ii](ii, ret, rdr, lens);
                    }
                }
                if (hasRva)
                {
                    r[rvaIdx] = rva;
                    rvas.Add(rva);
                }

                r.Token = new MetadataToken((MetadataTokenType)ret.Type, (uint)i);
                ret.Rows.Add(r);
            }
            ReadRvaData(ret, rdr, rvas, ret.Rows);

            return ret;
        }


        delegate void Writer(int col, MetadataTable tbl, MetadataWriter wtr, uint[] lens, object val);

        void WriteByteFlag(int col, MetadataTable tbl, MetadataWriter wtr, uint[] lens, object val)
        {
            wtr.Write((byte)val);
        }
        void WriteUInt16Flag(int col, MetadataTable tbl, MetadataWriter wtr, uint[] lens, object val)
        {
            wtr.Write((ushort)val);
        }
        void WriteUInt32Flag(int col, MetadataTable tbl, MetadataWriter wtr, uint[] lens, object val)
        {
            wtr.Write((uint)val);
        }

        void WriteByte(int col, MetadataTable tbl, MetadataWriter wtr, uint[] lens, object val)
        {
            wtr.Write((byte)val);
        }
        void WriteUInt16(int col, MetadataTable tbl, MetadataWriter wtr, uint[] lens, object val)
        {
            wtr.Write((ushort)val);
        }
        void WriteUInt32(int col, MetadataTable tbl, MetadataWriter wtr, uint[] lens, object val)
        {
            wtr.Write((uint)val);
        }
        void WriteRva(int col, MetadataTable tbl, MetadataWriter wtr, uint[] lens, object val)
        {
            wtr.Write(((Rva)val).Value);
        }

        void WriteBlobToken(int col, MetadataTable tbl, MetadataWriter wtr, uint[] lens, object val)
        {
            BlobHeap bh = wtr.BaseStream.Root[MetadataStreamType.Blob].Heap as BlobHeap;
            if (bh.Stream.Length > ushort.MaxValue)
                wtr.Write((uint)(val as BlobToken).Token.Value);
            else
                wtr.Write((ushort)(val as BlobToken).Token.Value);
        }
        void WriteGUIDToken(int col, MetadataTable tbl, MetadataWriter wtr, uint[] lens, object val)
        {
            GUIDHeap gh = wtr.BaseStream.Root[MetadataStreamType.GUID].Heap as GUIDHeap;
            if (gh.Stream.Length > ushort.MaxValue)
                wtr.Write((uint)(val as GUIDToken).Token.Value);
            else
                wtr.Write((ushort)(val as GUIDToken).Token.Value);
        }
        void WriteStringToken(int col, MetadataTable tbl, MetadataWriter wtr, uint[] lens, object val)
        {
            StringsHeap sh = wtr.BaseStream.Root[MetadataStreamType.Strings].Heap as StringsHeap;
            if (sh.Stream.Length > ushort.MaxValue)
                wtr.Write((uint)(val as StringToken).Token.Value);
            else
                wtr.Write((ushort)(val as StringToken).Token.Value);
        }
        void WriteTableToken(int col, MetadataTable tbl, MetadataWriter wtr, uint[] lens, object val)
        {
            if (lens[(int)(val as TableToken).Token.Type] > ushort.MaxValue)
                wtr.Write((uint)(val as TableToken).Token.Value);
            else
                wtr.Write((ushort)(val as TableToken).Token.Value);
        }
        void WriteCodedToken(int col, MetadataTable tbl, MetadataWriter wtr, uint[] lens, object val)
        {
            CodedIndexDefs.MdCodedIndexDefs[(CodedIndex)tbl.TableDef.Columns[col].Value].WriteToken(wtr, (val as TableToken).Token, lens);
        }

        public void Save(MetadataTable tbl, MetadataWriter wtr, uint[] lens)
        {
            Writer[] write = new Writer[tbl.Columns.Length];
            for (int i = 0; i < tbl.Columns.Length; i++)
            {
                int k = i;
                if (tbl.Columns[i].Type is Type)
                {
                    if ((tbl.Columns[i].Type as Type).IsSubclassOf(typeof(Enum)))
                    {
                        switch (Enum.GetUnderlyingType(tbl.Columns[i].Type as Type).FullName)
                        {
                            case "System.Byte":
                                write[i] = new Writer(WriteByteFlag);
                                break;
                            case "System.UInt16":
                                write[i] = new Writer(WriteUInt16Flag);
                                break;
                            case "System.UInt32":
                                write[i] = new Writer(WriteUInt32Flag);
                                break;
                        }
                    }
                    else
                        switch ((tbl.Columns[i].Type as Type).FullName)
                        {
                            case "System.Byte":
                                write[i] = new Writer(WriteByte);
                                break;
                            case "System.UInt16":
                                write[i] = new Writer(WriteUInt16);
                                break;
                            case "System.UInt32":
                                write[i] = new Writer(WriteUInt32);
                                break;
                            case "NetPE.Core.Rva":
                                write[i] = new Writer(WriteRva);
                                break;
                            case "NetPE.Core.Metadata.BlobToken":
                                write[i] = new Writer(WriteBlobToken);
                                break;
                            case "NetPE.Core.Metadata.GUIDToken":
                                write[i] = new Writer(WriteGUIDToken);
                                break;
                            case "NetPE.Core.Metadata.StringToken":
                                write[i] = new Writer(WriteStringToken);
                                break;
                        }
                }
                else if (tbl.Columns[i].Type is TableType)
                {
                    write[i] = new Writer(WriteTableToken);
                }
                else if (tbl.Columns[i].Type is CodedIndex)
                {
                    write[i] = new Writer(WriteCodedToken);
                }
            }

            for (int i = 1; i <= lens[(int)tbl.Type]; i++)
            {
                for (int ii = 0; ii < write.Length; ii++)
                {
                    if (tbl.Columns[ii].Type is Type && (tbl.Columns[ii].Type as Type).FullName == "NetPE.Core.Rva")
                        tbl.Rows[i][ii] = tbl.Heap.Stream.Root.Directory.Datas.GetNewRva((Rva)tbl.Rows[i][ii]);
                    write[ii](ii, tbl, wtr, lens, tbl.Rows[i][ii]);
                }
            }
        }
    }
}
