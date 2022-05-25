using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Hygenus
{
    public class Figures
    {
        /*
        public static Vector2[] Quad(float width, float height) {
            Vector2[] result = new Vector2[4];
            result[0] = new Vector2(width / 2, height / 2);
            result[1] = new Vector2(width / 2, -height / 2);
            result[2] = new Vector2(-width / 2, -height / 2);
            result[3] = new Vector2(-width / 2, height / 2);
            return result;
        }
        */
        public static Vector2[] Quad(float width, float height)
        {
            Vector2[] result = new Vector2[4];
            result[3] = new Vector2(width / 2, height / 2);
            result[2] = new Vector2(width / 2, -height / 2);
            result[1] = new Vector2(-width / 2, -height / 2);
            result[0] = new Vector2(-width / 2, height / 2);
            return result;
        }
    }
}
