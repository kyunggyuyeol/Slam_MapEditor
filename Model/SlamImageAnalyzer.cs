using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenCvSharp;

namespace Slam_MapEditor.Model
{
    public class SlamImageAnalyzer
    {
        public float GetDepth(Mat depth, Point point)
        {
            Vec3b pixel = depth.At<Vec3b>(point.Y, point.X);
            int b = pixel.Item0;
            int g = pixel.Item1;
            int r = pixel.Item2;

            int value = r + (g << 8) + (b << 16);
            if (value == 0xffffff)
            {
                value = -1;
            }
            return value;
        }

        private float ReadWatermark(Mat mat, int index)
        {
            unsafe
            {
                byte[] valueArray = new byte[4];

                byte* dat = mat.DataPointer + index;

                for (int k = 0; k < 4; k++)
                {
                    //a src byte
                    byte letter = 0;

                    for (int i = 0; i < 8; i++)
                    {
                        if (dat[index++] + dat[index++] + dat[index++] > 500)
                        {
                            letter |= (byte)(1 << i);
                        }
                    }

                    valueArray[k] = letter;
                }

                return BitConverter.ToSingle(valueArray, 0);
            }

        }

        public static readonly int WatermarkStep = 96;

        public Mat GetTcw(Mat rgb)
        {
            Mat tcw = new Mat(4, 4, MatType.CV_32F);

            for (int y = 0; y < 4; y++)
            {
                for (int x = 0; x < 4; x++)
                {
                    int index = (y * 4 + x + 2) * WatermarkStep;
                    tcw.Set(y, x, ReadWatermark(rgb, index));
                }
            }

            return tcw;
        }

        public Vec3f GetPos(Mat rgb)
        {
            float x = ReadWatermark(rgb, 18 * WatermarkStep);
            float y = ReadWatermark(rgb, 19 * WatermarkStep);
            float z = ReadWatermark(rgb, 20 * WatermarkStep);

            return new Vec3f(x, y, z);
        }
    }
}
