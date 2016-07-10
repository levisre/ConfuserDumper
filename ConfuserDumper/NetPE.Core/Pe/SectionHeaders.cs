using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections.ObjectModel;

namespace NetPE.Core.Pe
{
    public class SectionHeaders : Collection<Section>
    {
        PeFile file;
        internal SectionHeaders(PeFile file) { this.file = file; }

        public uint SectionHeadersOffset
        {
            get
            {
                if (file.Type == PeFileType.Image)
                {
                    return file.OptionalHeader.StandardFieldsOffset + file.PEHeader.SizeOfOptionalHeader;
                }
                else
                {
                    return file.PEHeader.PEHeaderOffset + 20;
                }
            }
        }

        public VirtualStream GetVirtualStream()
        {
            return new VirtualStream(file);
        }

        public void Read(PeReader rdr)
        {
            rdr.SetPosition(SectionHeadersOffset);
            for (int i = 0; i < file.PEHeader.NumberOfSections; i++)
            {
                Section sect = new Section(this);
                sect.Read(rdr);
                this.Add(sect);
            }
        }

        public void Write(PeWriter wtr)
        {
            wtr.SetPosition(SectionHeadersOffset);
            uint datPtr = SectionHeadersOffset + 40 * (uint)this.Count;
            if (file.Type == PeFileType.Image)
            {
                uint align = file.OptionalHeader.WindowsSpecificFields.FileAlignment;
                if (datPtr < align)
                    datPtr = align;
                else
                {
                    uint newPtr = align;
                    while (datPtr > align) { newPtr += align; datPtr -= align; }
                    datPtr = newPtr;
                }
            }
            for (int i = 0; i < file.PEHeader.NumberOfSections; i++)
            {
                Items[i].Write(wtr, ref datPtr);
            }
        }

        public bool Resolve(Rva rva, out Section sect, out int idx)
        {
            foreach (Section s in this)
            {
                if (rva >= s.VirtualAddress && rva < (s.VirtualAddress + s.VirtualSize))
                {
                    sect = s;
                    idx = (int)(rva - s.VirtualAddress);
                    return true;
                }
            }
            sect = null;
            idx = 0;
            return false;
        }

        public DataDirectoryType Occupied(Rva rva)
        {
            foreach (DataDirectoryEntry e in file.OptionalHeader.DataDirectories)
            {
                if (rva > e.Address && rva < (e.Address + e.Size))
                {
                    return e.Type;
                }
            }
            return DataDirectoryType.Reserved;
        }
    }
}
