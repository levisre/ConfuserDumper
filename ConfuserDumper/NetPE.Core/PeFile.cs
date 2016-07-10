using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using NetPE.Core.Pe;
using NetPE.Core.DataDirectories;

namespace NetPE.Core
{
    public enum PeFileType
    {
        Object,
        Image
    }
    public class PeFile
    {
        internal PeFile(PeFileType t) { this.t = t; }

        PeFileType t;
        public PeFileType Type { get { return t; } }

        private DOSHeader dos;
        public DOSHeader DOSHeader
        {
            get
            {
                if (t != PeFileType.Image) throw new InvalidOperationException();
                return dos;
            }
            set
            {
                if (t != PeFileType.Image) throw new InvalidOperationException();
                dos = value;
            }
        }
        private byte[] sign;
        public byte[] PeSignature
        {
            get
            {
                if (t != PeFileType.Image) throw new InvalidOperationException();
                return sign;
            }
            set
            {
                if (t != PeFileType.Image) throw new InvalidOperationException();
                sign = value;
            }
        }
        private PEHeader pe;
        public PEHeader PEHeader { get { return pe; } set { pe = value; } }
        private OptionalHeader op;
        public OptionalHeader OptionalHeader
        {
            get
            {
                if (t != PeFileType.Image) throw new InvalidOperationException();
                return op;
            }
            set
            {
                if (t != PeFileType.Image) throw new InvalidOperationException();
                op = value;
            }
        }
        private SectionHeaders sects;
        public SectionHeaders SectionHeaders { get { return sects; } set { sects = value; } }

        private CertificateDirectory certs;
        public CertificateDirectory Certificates
        {
            get
            {
                if (t != PeFileType.Image) throw new InvalidOperationException();
                return certs;
            }
            set
            {
                if (t != PeFileType.Image) throw new InvalidOperationException();
                certs = value;
            }
        }

        internal void Read(PeReader rdr)
        {
            rdr.SetPosition(0);
            if (t == PeFileType.Image)
            {
                dos = new DOSHeader(this);
                dos.Read(rdr);

                rdr.SetPosition(dos.PEHeaderOffset);
                sign = rdr.ReadBytes(4);

                pe = new PEHeader(this);
                pe.Read(rdr);

                op = new OptionalHeader(this);
                op.Read(rdr);

                sects = new SectionHeaders(this);
                sects.Read(rdr);

                certs = new CertificateDirectory(op.DataDirectories[DataDirectoryType.Certificate]);
                certs.Load(rdr, op.DataDirectories[DataDirectoryType.Certificate].Address.Value);
            }
            else if (t == PeFileType.Object)
            {
                pe = new PEHeader(this);
                pe.Read(rdr);

                sects = new SectionHeaders(this);
                sects.Read(rdr);
            }
        }

        internal void Write(PeWriter wtr)
        {
            wtr.SetPosition(0);
            if (t == PeFileType.Image)
            {
                dos.Write(wtr);

                wtr.SetPosition(dos.PEHeaderOffset);

                wtr.Write(sign);

                pe.Write(wtr);

                op.Write(wtr);

                sects.Write(wtr);

                certs.Save(wtr, op.DataDirectories[DataDirectoryType.Certificate].Address.Value);
            }
            else if (t == PeFileType.Object)
            {
                pe.Write(wtr);

                sects.Write(wtr);
            }
        }
    }
}
