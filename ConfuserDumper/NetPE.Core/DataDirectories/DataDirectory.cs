using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetPE.Core.Pe;
using NetPE.Core.DataDirectories;

namespace NetPE.Core.Pe
{
    public enum DataDirectoryType
    {
        Export = 0,
        Import = 1,
        Resource = 2,
        Exception = 3,
        Certificate = 4,
        Relocation = 5,
        Debug = 6,
        Architecture = 7,
        Global_Ptr = 8,
        TLS = 9,
        Load_Config = 10,
        Bound_Import = 11,
        IAT = 12,
        Delay_Import = 13,
        CLR = 14,
        Reserved = 15
    }
    public class DataDirectoryEntry : VirtualLocation
    {
        internal DataDirectoryEntry(DataDirectoryType type, uint adr, uint s)
        {
            Type = type;
            Address = adr;
            Size = s;
        }

        private DataDirectoryType t;
        public DataDirectoryType Type { get { return t; } set { t = value; } }

        DataDirectory cache;
        uint tSize;
        public uint TotalSize { get { return tSize; } internal set { tSize = value; } }

        public override VirtualComponent GetComponent()
        {
            if (cache != null) return cache;
            Reload();
            return cache;
        }
        public void Reload()
        {
            switch (this.Type)
            {
                case DataDirectoryType.Export:
                    cache = new ExportDirectory(this); break;
                case DataDirectoryType.Import:
                    cache = new ImportDirectory(this); break;
                case DataDirectoryType.Resource:
                    cache = new ResourceDirectory(this); break;
                case DataDirectoryType.Exception:
                    throw new NotSupportedException();      //!!!
                case DataDirectoryType.Certificate:
                    throw new InvalidOperationException();
                case DataDirectoryType.Relocation:
                    cache = new RelocationDirectory(this); break;
                case DataDirectoryType.Debug:
                    cache = new DebugDirectory(this); break;
                case DataDirectoryType.Architecture:
                    throw new InvalidOperationException();
                case DataDirectoryType.Global_Ptr:
                    throw new InvalidOperationException();
                case DataDirectoryType.TLS:
                    cache = new TLSDirectory(this); break;
                case DataDirectoryType.Load_Config:
                    throw new NotSupportedException();     //!!!
                case DataDirectoryType.Bound_Import:
                    throw new NotSupportedException();     //!!!
                case DataDirectoryType.IAT:
                    throw new InvalidOperationException();
                case DataDirectoryType.Delay_Import:
                    throw new NotSupportedException();     //!!!
                case DataDirectoryType.CLR:
                    cache = new CLRDirectory(this); break;
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}
namespace NetPE.Core.DataDirectories
{
    public abstract class DataDirectory : VirtualComponent
    {
        public DataDirectory(VirtualLocation loc) : base(loc) { }

        public abstract DataDirectoryType Type { get; }

        public abstract uint GetTotalSize();

        public abstract uint GetDirectorySize();

        public override void Save(VirtualWriter wtr)
        {
            if (Location.Address == 0) throw new InvalidOperationException();
            wtr.SetPosition(Location.Address);
            SaveInternal(wtr);
            (Location as DataDirectoryEntry).TotalSize = GetTotalSize();
        }

        public override void Load(VirtualReader rdr)
        {
            if (Location.Address == 0) throw new InvalidOperationException();
            rdr.SetPosition(Location.Address);
            LoadInternal(rdr);
            (Location as DataDirectoryEntry).TotalSize = GetTotalSize();
        }

        protected abstract void SaveInternal(VirtualWriter wtr);

        protected abstract void LoadInternal(VirtualReader rdr);
    }
}