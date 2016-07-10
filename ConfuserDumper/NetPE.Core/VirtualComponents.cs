using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetPE.Core.Pe;

namespace NetPE.Core
{
    public abstract class VirtualLocation
    {
        Rva adr;
        public Rva Address { get { return adr; } set { adr = value; } }
        uint s;
        public uint Size { get { return s; } set { s = value; } }

        public abstract VirtualComponent GetComponent();
    }

    public abstract class VirtualComponent
    {
        public VirtualComponent(VirtualLocation loc) { this.loc = loc; }
        VirtualLocation loc;
        public VirtualLocation Location { get { return loc; } internal set { loc = value; } }
        public abstract void Save(VirtualWriter wtr);
        public abstract void Load(VirtualReader rdr);
    }

    public interface IVirtualLocationContainer
    {
        void ClearCache();
        void CacheAll();
    }
}
