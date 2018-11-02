using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaSon.FrameWork.Common.ChinaRailway
{
    public class Serializer
    {
        protected readonly DataSerializer serializer;

        public Serializer(DataSerializer serializer)
        {
            this.serializer = serializer;
        }

        public string Serialize<TObject>(TObject obj)
        {
            return this.serializer.SerializeString<TObject>(obj);
        }

        public TObject Deserialize<TObject>(string data)
        {
            return this.serializer.DeserializeString<TObject>(data);
        }
    }
}
