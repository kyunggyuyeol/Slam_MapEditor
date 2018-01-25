using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slam_MapEditor.Model
{
    public class Tile
    {

        public int Index { get; set; }
        //좌표
        public float X { get; set; }
        public float Y { get; set; }
        
        //컬러값
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }

        //width, size
        public static float Width { get; set; }
        public static int Size { get; set; }

     

        public Tile(int index, float x, float y, byte r, byte g, byte b)
        {
            this.Index = index;

            this.X = x;
            this.Y = y;

            this.R = r;
            this.G = g;
            this.B = b;


        }
   
    }
}
