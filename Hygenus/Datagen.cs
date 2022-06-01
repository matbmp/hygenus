using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using Engine;

namespace Hygenus
{
    public static class Datagen
    {
        public static void generate()
        {
            Polygon p = new Polygon(Figures.Quad(0.5F, 0.5F));
            Stream stream = new FileStream("quad", FileMode.Create);
            BinaryWriter bw = new BinaryWriter(stream);
            p.Serialize(bw);
            bw.Close();
            stream.Close();
        }
    }
}
