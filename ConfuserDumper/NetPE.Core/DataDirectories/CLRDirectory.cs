using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetPE.Core.Pe;
using NetPE.Core.Metadata;
using System.Collections.ObjectModel;

namespace NetPE.Core.DataDirectories
{
    public interface IMetadataData
    {
        void Load(byte[] data);
        void Save(out byte[] data);
    }

    public class CLRData
    {
        Rva rva;
        public Rva Address { get { return rva; } set { rva = value; } }
        byte[] dat = new byte[0];
        public byte[] Data { get { return dat; } set { dat = value; } }
    }
    public class CLRDatas : Collection<CLRData>
    {
        CLRDirectory dir;
        internal CLRDatas(CLRDirectory dir) { this.dir = dir; }

        public uint GetSize()
        {
            uint ret = 0;
            foreach (CLRData i in Items)
                ret += (uint)((i.Data.Length + 3) & ~3);
            return ret;
        }

        public Rva GetNewRva(Rva key)
        {
            if (key == 0) return key;
            Rva ret = DatasLocation;
            IEnumerator<CLRData> etr = base.GetEnumerator();
            while (etr.MoveNext() && etr.Current.Address != key)
            {
                ret += (uint)((etr.Current.Data.Length + 3) & ~3);
            }
            return (uint)(ret);
        }

        internal void Save(VirtualWriter wtr)
        {
            foreach (CLRData i in Items)
            {
                i.Address = wtr.GetPosition();
                wtr.Write(i.Data);
                wtr.Write(new byte[((i.Data.Length + 3) & ~3) - i.Data.Length]);
            }
        }

        public void UpdateRva(Rva old, Rva @new)
        {
            IEnumerator<CLRData> etr = base.GetEnumerator();
            while (etr.MoveNext() && etr.Current.Address != old);
            etr.Current.Address = @new;
        }

        public bool ContainsAddress(Rva rva)
        {
            foreach (CLRData i in Items)
                if (i.Address == rva)
                    return true;
            return false;
        }

        public CLRData this[Rva rva]
        {
            get
            {
                foreach (CLRData i in Items)
                    if (i.Address == rva)
                        return i;
                return null;
            }
        }

        Rva datloc;
        internal Rva DatasLocation { get { return datloc; } set { datloc = value; } }

        bool lding;
        public bool IsLoading { get { return lding; } set { lding = value; } }
        protected override void InsertItem(int index, CLRData item)
        {
            if (lding && (datloc == 0 || datloc > item.Address)) datloc = item.Address;
            base.InsertItem(index, item);
        }
    }

    public class CLRDirectory : DataDirectory
    {
        public CLRDirectory(DataDirectoryEntry entry) : base(entry) { }

        [Flags]
        public enum RuntimeFlags
        {
           ILOnly = 0x00000001,
           Required32Bit = 0x00000002,
           ILLibrary = 0x00000004,
           StrongNameSigned = 0x00000008,
           NativeEntryPoint = 0x00000010,
           TrackDebugData = 0x00010000
        }

        public override DataDirectoryType Type
        {
            get { return DataDirectoryType.CLR; }
        }

        uint s;
        public uint Size { get { return s; } set { s = value; } }
        ushort maVer;
        public ushort MajorRuntimeVersion { get { return maVer; } set { maVer = value; } }
        ushort miVer;
        public ushort MinorRuntimeVersion { get { return miVer; } set { miVer = value; } }
        MetadataRoot md;
        public MetadataRoot Metadata { get { return md; } set { md = value; } }
        RuntimeFlags f;
        public RuntimeFlags Flags { get { return f; } set { f = value; } }

        MetadataToken t;
        public MetadataToken EntryPointToken { get { if ((f & RuntimeFlags.NativeEntryPoint) == RuntimeFlags.NativeEntryPoint)throw new InvalidOperationException(); return t; } set { if ((f & RuntimeFlags.NativeEntryPoint) == RuntimeFlags.NativeEntryPoint)throw new InvalidOperationException(); t = value; } }
        Rva e;
        public Rva EntryPointCodes { get { if ((f & RuntimeFlags.NativeEntryPoint) != RuntimeFlags.NativeEntryPoint)throw new InvalidOperationException(); return e; } set { if ((f & RuntimeFlags.NativeEntryPoint) != RuntimeFlags.NativeEntryPoint)throw new InvalidOperationException(); e = value; } }

        byte[] res;
        public byte[] Resources { get { return res; } set { res = value; } }
        byte[] sn;
        public byte[] StrongNameSignature { get { return sn; } set { sn = value; } }
        byte[] cm;
        public byte[] CodeManagerTable { get { return cm; } set { cm = value; } }
        VTableFixups vt;
        public VTableFixups VTableFixup { get { return vt; } set { vt = value; } }
        byte[] eat;
        public byte[] ExportAddressTableJumps { get { return eat; } set { eat = value; } }
        byte[] mn;
        public byte[] ManagedNativeHeader { get { return mn; } set { mn = value; } }

        CLRDatas dats;
        public CLRDatas Datas { get { return dats; } set { dats = value; } }

        public override uint GetTotalSize()
        {
            return (uint)(72 + dats.GetSize() + md.GetSize() + res.Length + sn.Length + cm.Length + vt.GetSize() + eat.Length + mn.Length);
        }

        public override uint GetDirectorySize()
        {
            return 72;
        }

        protected override void SaveInternal(VirtualWriter wtr)
        {
            wtr.Write(s);
            wtr.Write(maVer);
            wtr.Write(miVer);
            Rva mdAdr = 0;
            uint mdSize = 0;
            Rva resAdr = 0;
            Rva snAdr = 0;
            Rva cmAdr = 0;
            Rva vtAdr = 0;
            uint vtSize = 0;
            Rva eatAdr = 0;
            Rva mnAdr = 0;

            Rva mdHdrAdr = wtr.GetPosition();
            wtr.Write((uint)0);
            wtr.Write((uint)0);
            wtr.Write((uint)f);
            if ((f & RuntimeFlags.NativeEntryPoint) == RuntimeFlags.NativeEntryPoint)
            {
                wtr.Write(e);
            }
            else
            {
                wtr.Write(t);
            }

            Rva otherHdrAdr = wtr.GetPosition();

            wtr.Write((uint)0);
            wtr.Write((uint)0);

            wtr.Write((uint)0);
            wtr.Write((uint)0);

            wtr.Write((uint)0);
            wtr.Write((uint)0);

            wtr.Write((uint)0);
            wtr.Write((uint)0);

            wtr.Write((uint)0);
            wtr.Write((uint)0);

            wtr.Write((uint)0);
            wtr.Write((uint)0);

            //////////////////////////////////////////////
            if (dats.DatasLocation == 0)
            {
                dats.DatasLocation = this.Location.Address + 72;
                dats.Save(wtr);
                wtr.Write(new byte[((wtr.BaseStream.Position + 3) & ~3) - wtr.BaseStream.Position]);
            }
            else
            {
                //wtr.SaveLocation();
                wtr.SetPosition(dats.DatasLocation);
                dats.Save(wtr);
                wtr.Write(new byte[((wtr.BaseStream.Position + 3) & ~3) - wtr.BaseStream.Position]);
                //wtr.LoadLocation();
            }

            mdAdr = wtr.GetPosition();
            mdSize = md.GetSize();
            md.Save(wtr, wtr.GetPosition());
            wtr.Write(new byte[(mdSize + 3) & ~3 - mdSize]);

            if (res.Length != 0)
            {
                resAdr = wtr.GetPosition();
                wtr.Write(res);
                wtr.Write(new byte[(res.Length + 3) & ~3 - res.Length]);
            }

            if (sn.Length != 0)
            {
                snAdr = wtr.GetPosition();
                wtr.Write(sn);
                wtr.Write(new byte[(sn.Length + 3) & ~3 - sn.Length]);
            }

            if (cm.Length != 0)
            {
                cmAdr = wtr.GetPosition();
                wtr.Write(cm);
                wtr.Write(new byte[(cm.Length + 3) & ~3 - cm.Length]);
            }

            if (vtSize != 0)
            {
                vtAdr = wtr.GetPosition();
                vtSize = vt.GetSize();
                vt.Save(wtr, vtAdr);
                wtr.Write(new byte[(vtSize + 3) & ~3 - vtSize]);
            }

            if (eat.Length != 0)
            {
                eatAdr = wtr.GetPosition();
                wtr.Write(eat);
                wtr.Write(new byte[(eat.Length + 3) & ~3 - eat.Length]);
            }

            if (mn.Length != 0)
            {
                mnAdr = wtr.GetPosition();
                wtr.Write(mn);
                wtr.Write(new byte[(mn.Length + 3) & ~3 - mn.Length]);
            }
            //////////////////////////////////////////////////////
            wtr.SetPosition(mdHdrAdr);
            wtr.Write(mdAdr);
            wtr.Write(mdSize);

            wtr.SetPosition(otherHdrAdr);
            wtr.Write(resAdr);
            wtr.Write((uint)res.Length);
            wtr.Write(snAdr);
            wtr.Write((uint)sn.Length);
            wtr.Write(cmAdr);
            wtr.Write((uint)cm.Length);
            wtr.Write(vtAdr);
            wtr.Write(vtSize);
            wtr.Write(eatAdr);
            wtr.Write((uint)eat.Length);
            wtr.Write(mnAdr);
            wtr.Write((uint)mn.Length);
        }

        public CLRDataDirectoryEntry MetadataDirEntry { get; set; }
        public CLRDataDirectoryEntry ResourceDirEntry { get; set; }
        public CLRDataDirectoryEntry StrongNameDirEntry { get; set; }
        public CLRDataDirectoryEntry CodeManagerTableDirEntry { get; set; }
        public CLRDataDirectoryEntry VTableFixupsDirEntry { get; set; }
        public CLRDataDirectoryEntry ExportAddressTableJumpsDirEntry { get; set; }
        public CLRDataDirectoryEntry ManagedNativeHeaderDirEntry { get; set; }

        protected override void LoadInternal(VirtualReader rdr)
        {
            s = rdr.ReadUInt32();
            maVer = rdr.ReadUInt16();
            miVer = rdr.ReadUInt16();
            dats = new CLRDatas(this);
            dats.IsLoading = true;

            Rva mdAdr = rdr.ReadRva();
            uint mdSze = rdr.ReadUInt32();
            f = (RuntimeFlags)rdr.ReadUInt32();
            if ((f & RuntimeFlags.NativeEntryPoint) == RuntimeFlags.NativeEntryPoint)
            {
                Rva ep = rdr.ReadRva();
                rdr.SaveLocation();
                rdr.SetPosition(ep);
                byte[] ec = NativeHelper.GetNativeCodes(rdr);
                rdr.LoadLocation();
                e = ep;
                dats.Add(new CLRData() { Address = ep, Data = ec });
            }
            else
            {
                t = rdr.ReadUInt32();
            }
            Rva resAdr = rdr.ReadRva();
            uint resSze = rdr.ReadUInt32();
            Rva snAdr = rdr.ReadRva();
            uint snSze = rdr.ReadUInt32();
            Rva cmAdr = rdr.ReadRva();
            uint cmSze = rdr.ReadUInt32();
            Rva vtAdr = rdr.ReadRva();
            uint vtSze = rdr.ReadUInt32();
            Rva eatAdr = rdr.ReadRva();
            uint eatSze = rdr.ReadUInt32();
            Rva mnAdr = rdr.ReadRva();
            uint mnSze = rdr.ReadUInt32();

            rdr.SetPosition(resAdr);
            res = rdr.ReadBytes((int)resSze);

            rdr.SetPosition(snAdr);
            sn = rdr.ReadBytes((int)snSze);

            rdr.SetPosition(cmAdr);
            cm = rdr.ReadBytes((int)cmSze);

            rdr.SetPosition(eatAdr);
            eat = rdr.ReadBytes((int)eatSze);

            rdr.SetPosition(mnAdr);
            mn = rdr.ReadBytes((int)mnSze);

            vt = new VTableFixups(this);
            vt.Load(rdr, vtAdr, vtSze);

            md = new MetadataRoot(this);
            md.Load(rdr, mdAdr);

            dats.IsLoading = false;

            // Save the read info
            MetadataDirEntry = new CLRDataDirectoryEntry("MetaData", mdAdr, mdSze);
            ResourceDirEntry = new CLRDataDirectoryEntry("Resources", resAdr, resSze);
            StrongNameDirEntry = new CLRDataDirectoryEntry("Strong Name Signature", snAdr, snSze);
            CodeManagerTableDirEntry = new CLRDataDirectoryEntry("Code Manager Table", cmAdr, cmSze);
            VTableFixupsDirEntry = new CLRDataDirectoryEntry("VTable Fixups", vtAdr, vtSze);
            ExportAddressTableJumpsDirEntry = new CLRDataDirectoryEntry("Export Address Table Jumps", eatAdr, eatSze);
            ManagedNativeHeaderDirEntry = new CLRDataDirectoryEntry("Managed Native Header", mnAdr, mnSze);
        }
    }

    public class CLRDataDirectoryEntry : VirtualLocation
    {
        string mName;
        internal CLRDataDirectoryEntry(string name, uint adr, uint s)
        {
            mName = name;
            Address = adr;
            Size = s;
        }

        public string DirectoryEntryName
        {
            get
            {
                return mName;
            }
        }

        public override VirtualComponent GetComponent()
        {
            return null;
        }

        public override string ToString()
        {
            return string.Format("{0}: RVA={1} Size={2}", DirectoryEntryName, Address, Size);
        }
    }
}
