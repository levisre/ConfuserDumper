using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace NetPE.Core.Metadata.Heaps
{
    public abstract class Heap<Tkn, Val> where Tkn : MetadataTokenProvider
    {
        class TokenRef
        {
            public Tkn Token;
            public int RefCount = 1;
        }

        MetadataStream str;
        public Heap(MetadataStream str) { this.str = str; }
        public MetadataStream Stream { get { return str; } }

        Dictionary<MetadataToken, TokenRef> tkns = new Dictionary<MetadataToken, TokenRef>();

        public Tkn Resolve(MetadataToken tkn)
        {
            if (tkns.ContainsKey(tkn))
            {
                tkns[tkn].RefCount++;
                return tkns[tkn].Token;
            }
            Tkn ret = (Tkn)typeof(Tkn).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { this.GetType() }, null).Invoke(new object[] { this });
            ret.Token = tkn;
            tkns[tkn] = new Heap<Tkn, Val>.TokenRef();
            tkns[tkn].Token = ret;
            return ret;
        }

        public bool ContainsChild(Val val)
        {
            foreach (TokenRef tkn in tkns.Values)
            {
                if (Comparer<Val>.Default.Compare(GetValue(tkn.Token), val) == 0) return true;
            }
            return false;
        }

        public Tkn NewChild(uint len)
        {
            TokenRef tr = new Heap<Tkn, Val>.TokenRef();
            tr.Token = NewChildCore(len);
            tkns[tr.Token.Token] = tr;
            return tr.Token;
        }

        public void RemoveChild(Tkn tkn)
        {
            if (!tkns.ContainsKey(tkn.Token)) throw new InvalidOperationException();
            if (tkns[tkn.Token].RefCount == 1)
            {
                tkns.Remove(tkn.Token);
                uint len = GetValueLen(tkn);

                for (uint i = tkn.Token.Index + len; i < str.Data.Length; i++)
                    str.Data[i - len] = str.Data[i];
                str.SetLength(str.Data.Length - len);
                foreach (TokenRef tr in tkns.Values)
                {
                    if (tr.Token.Token.Index > tkn.Token.Index)
                    {
                        tr.Token.Token.Index -= (uint)len;
                    }
                }
            }
            else
                tkns[tkn.Token].RefCount--;
            
        }

        protected void ResizeChild(Tkn tkn, uint nLen)
        {
            uint oLen = GetValueLen(tkn);

            if (nLen > oLen)
            {
                str.SetLength(str.Data.Length + nLen - oLen);
                for (uint i = (uint)str.Data.Length + oLen - nLen - 1; i != tkn.Token.Index + oLen; i--)
                    str.Data[i] = str.Data[i - nLen + oLen];
                foreach (TokenRef tr in tkns.Values)
                {
                    if (tr.Token.Token.Index > tkn.Token.Index)
                    {
                        tr.Token.Token.Index += (uint)nLen - oLen;
                    }
                }
            }
            else
            {
                str.SetLength(str.Data.Length - oLen + nLen);
                for (uint i = tkn.Token.Index + nLen; i < str.Data.Length + nLen - oLen; i++)
                    str.Data[i] = str.Data[i + oLen - nLen];
                foreach (TokenRef tr in tkns.Values)
                {
                    if (tr.Token.Token.Index > tkn.Token.Index)
                    {
                        tr.Token.Token.Index -= (uint)oLen - nLen;
                    }
                }
            }
        }


        public abstract Tkn NewChildCore(uint len);

        public abstract uint GetValueLen(Tkn tkn);

        public abstract Val GetValue(Tkn tkn);

        public abstract void SetValue(Tkn tkn, Val val);
    }
}
