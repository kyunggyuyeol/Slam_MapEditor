using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using OpenCvSharp;

namespace Slam_MapEditor.Model
{
    
    class ExistingPath
    {
        public static ExistingPath Instance { get; set; }

        public ExistingPath()
        {
            Instance = this;
        }

        public Vec3f GetPath()
        {
            int imageNumber = 0;

            SlamImageAnalyzer slam = new SlamImageAnalyzer();

            while (true)
            {
                //Mat rgb;
                //Mat depth;

               
                //    rgb = Cv2.ImRead($"../../image_map/rgb_{imageNumber:D5}.jpg");
                //    depth = Cv2.ImRead($"../../image_map/depth_{imageNumber:D5}.png");
                

                //Mat tcw = slam.GetTcw(rgb);
                //Vec3f pos = slam.GetPos(rgb);
                //Console.WriteLine($"{pos.Item0}, {pos.Item1}, {pos.Item2}");
                //bool ok;

              
                //imageNumber++;

                //return pos;
            }
        }
    }
}


