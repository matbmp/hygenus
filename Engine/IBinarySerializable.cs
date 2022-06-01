using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Engine
{
    public interface IBinarySerializable
    {
        public void Serialize(BinaryWriter writer);
        public void Deserialize(BinaryReader reader);
    }
}
