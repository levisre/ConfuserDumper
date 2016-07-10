using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetPE.Core.Pe;
using System.Collections.ObjectModel;

namespace NetPE.Core.DataDirectories
{
    public class IATDirectory : DataDirectory
    {
        ImportDirectory dir;
        public IATDirectory(DataDirectoryEntry entry, ImportDirectory dir) : base(entry) { this.dir = dir; }
        public ImportDirectory Parent { get { return dir; } set { dir = value; } }

        public override DataDirectoryType Type
        {
            get { return DataDirectoryType.IAT; }
        }

        public override uint GetTotalSize()
        {
            return dir.GetIATSize();
        }

        public override uint GetDirectorySize()
        {
            return dir.GetIATSize();
        }

        protected override void SaveInternal(VirtualWriter wtr)
        {
            throw new InvalidOperationException();
        }

        protected override void LoadInternal(VirtualReader rdr)
        {
            throw new InvalidOperationException();
        }
    }
}
