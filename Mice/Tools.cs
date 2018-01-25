using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenCvSharp;

using Slam_MapEditor.Model;

namespace Slam_MapEditor.Mice
{
    public static class Tools
    {
        public static Mat GetTexture(List<Tile> mapData, out Rect2f area)
        {
            float minX = float.MaxValue;
            float maxX = float.MinValue;
            float minY = float.MaxValue;
            float maxY = float.MinValue;
            float step = Tile.Width;

            foreach (Tile item in mapData)
            {
                float x = item.X;
                float y = item.Y;
                if (minX > x)
                {
                    minX = x;
                }

                if (maxX < x)
                {
                    maxX = x;
                }

                if (minY > y)
                {
                    minY = y;
                }

                if (maxY < y)
                {
                    maxY = y;
                }
            }

            int imageWidth = (int)Math.Round((maxX - minX) / step) + 1;
            int imageHeight = (int)Math.Round((maxY - minY) / step) + 1;

          //  Console.WriteLine("높이" + imageHeight + "넓이" + imageWidth);
            Mat mat = new Mat(new Size(imageWidth, imageHeight), MatType.CV_8UC3);


            for (int y = 0; y < imageHeight; y++)
            {
                for (int x = 0; x < imageWidth; x++)
                {
                    mat.Set(y, x, new Vec3b(255, 255, 255));
                }
            }

            int mx = 0;
            int my = 0;

            foreach (Tile item in mapData)
            {
                int x = (int)Math.Round((item.X - minX) / step);
                int y = (int)Math.Round((item.Y - minY) / step);




                mat.Set(y, x, new Vec3b(item.B, item.G, item.R));

                mx = Math.Max(mx, x);
                my = Math.Max(my, y);
            }
           // Cv2.ImShow("asdf", mat);
            area = new Rect2f(minX, minY, maxX - minX, maxY - minY);
            return mat;

        }
    }
}
