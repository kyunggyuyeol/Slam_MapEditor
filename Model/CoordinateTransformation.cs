using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Media;
using OpenCvSharp;
using System.Windows.Media.Media3D;
using Slam_MapEditor.View;

namespace Slam_MapEditor.Model
{
    public class CoordinateTransformation
    {

        public static CoordinateTransformation instance { get; set; }
        public Point3D getvalue(int ImageNumber, int InputPixelX, int InPutPixelY)
        {
            instance = this;

            Point3D pt = new Point3D();
            SlamImageAnalyzer slam = new SlamImageAnalyzer();

            Mat Matrix4Cam3D1 = new Mat(4, 1, MatType.CV_32F);
            //Matrix4Cam3D1.Set<float>(0, 0, x); //world x
            //Matrix4Cam3D1.Set<float>(1, 0, z); //world y
            //Matrix4Cam3D1.Set<float>(2, 0, y); //world z
            //Matrix4Cam3D1.Set<float>(3, 0, 1); // homogenius coordinate 1

            float rgbX1 = 0;
            float rgbY1 = 0;
            Mat MatrixUV1 = new Mat(4, 1, MatType.CV_32F);
            MatrixUV1.Set<float>(0, 0, 0);
            MatrixUV1.Set<float>(1, 0, 0);
            int imCols; //image width
            int imRows; //image height
            float depthMaxLimit = 6; // 6 meter
            float eps = (float)0.001;

            int inputX1 = InputPixelX;
            int inputY1 = InPutPixelY;


            float D1 = 0;

            float dd1 = 0;

            Mat rgb;
            Mat depth;

            string rgbpath = System.IO.Path.Combine(Global.ImageDataPath, "rgb_"+ImageNumber.ToString("D5")+".jpg");
            rgb = Cv2.ImRead(rgbpath);
            string depthpath = System.IO.Path.Combine(Global.ImageDataPath, "depth_" + ImageNumber.ToString("D5") + ".png");
            //rgb = Cv2.ImRead($"../../image_map/rgb_{ImageNumber:D5}.jpg");
            depth = Cv2.ImRead(depthpath);

            float depthvalue = slam.GetDepth(depth, new Point(InputPixelX, InPutPixelY));

            float depthgood = checkDepthValue(depthvalue, (int)depthMaxLimit * 1000);
            Size imSize = rgb.Size();


            Mat tcw = slam.GetTcw(rgb);
            Vec3f pos = slam.GetPos(rgb);

            //   size of tcw
            Size sizeOfTcw = tcw.Size();

            if (depthgood == 0 || depthgood == -1)
            {
                VIewer3D.Instance.RemoveSomething();
            }
            else
            {
                Mat Matrix4W1 = new Mat(4, 1, MatType.CV_32F);
                for (int i = 0; i < 4; i++)
                    Matrix4W1.Set<float>(i, 0, 0);
                // RGB->get x, y, z world coordinate
                Matrix4W1 = estimateWorldCoordinateFromCameraCoordinate(tcw, imSize, inputX1, inputY1, depthgood, eps);//TODO: Input depth value
                if (!Matrix4W1.Empty())
                {
                    float mTx = Matrix4W1.At<float>(0, 0);
                    float mTy = Matrix4W1.At<float>(1, 0);
                    float mTz = Matrix4W1.At<float>(2, 0);
                    pt = new Point3D(mTx, mTy, mTz);



                }

            //    Console.WriteLine(depthgood);
                // Console.WriteLine($"{pos.Item0}, {pos.Item1}, {pos.Item2}" + "\n" + depthgood);

            }
            return pt;
        }






        public static Mat estimatePixelPointFromWorldPoint(Mat tcw, Mat Matrix4Cam3D, int imCols, int imRows)
        {
            tcw.ConvertTo(tcw, MatType.CV_32F);


            Mat Matrix4rgb = new Mat(4, 1, MatType.CV_32F);
            Matrix4rgb.Set<float>(0, 0, 0);
            Matrix4rgb.Set<float>(1, 0, 0);
            Matrix4rgb.Set<float>(2, 0, 0);
            Matrix4rgb.Set<float>(3, 0, 0);

            // Mat Matrix4rgb = (Mat_<float>(4, 1) << 0, 0, 0, 0); // corresponding image position = x,y,z

            Matrix4rgb = tcw * Matrix4Cam3D;

            int scaleValue = 1000;

            //Matrix4rgb1 = Matrix4rgb1 * scaleValue;
            // Matrix4rgb2 = Matrix4rgb2*scaleValue;

            //pixel location

            float x1 = Matrix4rgb.At<float>(0, 0);
            float y1 = Matrix4rgb.At<float>(1, 0);
            float z1 = Matrix4rgb.At<float>(2, 0);
            float rgbX1 = 0, rgbY1 = 0;

            if (z1 < 0)
                z1 = 0;

            float fx = (float)699.713; //focal length
            float fy = (float)699.713; //focal length
            rgbX1 = (float)x1 * ((float)fx / (float)z1 + (float)0.00000001);
            rgbY1 = (float)y1 * ((float)fy / (float)z1 + (float)0.00000001);

            // rgbX1 = x1/z1*scaleValue; 
            // rgbY1 = y1/z1*scaleValue; 

            if (float.IsNaN(rgbX1)) rgbX1 = 0;

            if (float.IsNaN(rgbY1)) rgbY1 = 0;

            int k_u = 1, k_v = 1;
            // Mat MatrixUV = (Mat_<float>(2, 1) << 0, 0);
            // Mat MatrixKUV = (Mat_<float>(2, 2) << k_u, 0, 0, k_v);
            // Mat MatrixXY = (Mat_<float>(2, 1) << rgbX1, rgbY1);
            //  Mat MatrixUV0 = (Mat_<float>(2, 1) << imCols / 2, imRows / 2);
            Mat MatrixUV = new Mat(2, 1, MatType.CV_32F);
            MatrixUV.Set<float>(0, 0, 0);
            MatrixUV.Set<float>(1, 0, 0);

            Mat MatrixKUV = new Mat(2, 2, MatType.CV_32F);
            MatrixKUV.Set<float>(0, 0, k_u);
            MatrixKUV.Set<float>(0, 1, 0);
            MatrixKUV.Set<float>(1, 0, 0);
            MatrixKUV.Set<float>(1, 1, k_v);
            Mat MatrixXY = new Mat(2, 1, MatType.CV_32F);
            MatrixXY.Set<float>(0, 0, rgbX1);
            MatrixXY.Set<float>(1, 0, rgbY1);

            Mat MatrixUV0 = new Mat(2, 1, MatType.CV_32F);
            MatrixUV0.Set<float>(0, 0, imCols / 2);
            MatrixUV0.Set<float>(1, 0, imRows / 2);
            //  Console.WriteLine("UV사이즈 " + MatrixUV.Size() + "KUV사이즈" + MatrixKUV.Size() + " XY사이즈" + MatrixXY.Size() + " uv0 사이즈" + MatrixUV0.Size());
            MatrixUV = MatrixKUV * MatrixXY + MatrixUV0;

            return MatrixUV;
        }


        /// <summary>
        /// 함수
        /// </summary>
        /// <param name="depthVal"></param>
        /// <param name="depthMaxLimit"></param>
        /// <returns></returns>
        public static float checkDepthValue(float depthVal, int depthMaxLimit)
        {
            float depthValReal = 0;
            // int depthMaxLimit = 5;// 5 meters
            if (depthVal > 0 && depthVal < depthMaxLimit)
            {
                depthValReal = depthVal;
                // std::cout<<"[depth value:] "<<depthValReal<<std::endl;
            }
            else
            {
                depthValReal = 0;
            }
            // else if (depthVal <= 0)
            // {
            // 	// depthValReal = 0;
            // 	false;
            // //  std::cout<<"depth <=: "<<depthVal<<endl;
            // }
            // else if (depthVal > depthMaxLimit)
            // {
            // 	// depthValReal = 0;
            // 	false;
            //  	// std::cout<<"depth >depthMaxLimit: "<<depthVal<<endl;
            // }
            // else if (isnan(depthVal))
            // {
            // 	// depthValReal = 0;
            // 	false;
            // //  std::cout<<"depth is nan"<<depthVal<<endl;
            // }
            // else
            // 	false;
            // 	// std::cout<<"depth else:"<<depthVal<<endl;

            return depthValReal;

        }

        public static Mat estimateWorldCoordinateFromCameraCoordinate(Mat Tcw, Size imSize, int inputX, int inputY, float depthValReal, float eps)
        {
            // cv::Point targetPoint(inputX, inputY);	//1280/2 720/2
            // float depthVal = depthmap.at<float>(inputX, inputY); 



            Tcw.ConvertTo(Tcw, MatType.CV_32F);

            Size s = Tcw.Size();
            int rows = s.Height;
            int cols = s.Width;
            int detTcw = 0;
            // cv::Mat TcwInv = Mat::zeros(rows, cols, CV_32F);		
            // Mat Matrix4W = Mat::zeros(rows, 1, MatType.CV_32F);
            Mat Matrix4W = new Mat(rows, 1, MatType.CV_32F);

            for (int i = 0; i < rows; i++)
                Matrix4W.Set<float>(i, 0, 0);

            //Mat Matrix4C = Mat::zeros(rows, 1, MatType.CV_32F);



            // std::cout<<"imSizeSystem: "<<imSize.width<<", "<<imSize.height<<std::endl;
            //values of x,y,z,1 (z is depth value?) 4x1 matrix 	
            // float fx = 699.713; //focal length
            // float fy = 699.713; //focal length

            // float camX = fx*(inputX-imSize.width/2)/depthValReal*eps;
            // float camY = fy*(inputY-imSize.height/2)/depthValReal*eps;

            float camX = (inputX - imSize.Width / 2) * eps;
            float camY = (inputY - imSize.Height / 2) * eps;
            float camZ = depthValReal * eps;

            //Matrix4C = (Mat_<float>(rows, 1) << camX, camY, camZ, 1.0);
            Mat Matrix4C = new Mat(rows, 1, MatType.CV_32F);
            Matrix4C.Set<float>(0, 0, camX);

            Matrix4C.Set<float>(1, 0, camY);
            Matrix4C.Set<float>(2, 0, camZ);
            Matrix4C.Set<float>(3, 0, (float)1.0);
            //Mat Matrix4C = new Mat(2, 2, MatType.CV_32F);
            //Matrix4C.Set<float>(0, 0, k_u);
            //Matrix4C.Set<float>(0, 1, 0);
            //Matrix4C.Set<float>(1, 0, 0);
            //Matrix4C.Set<float>(1, 1, k_v);

            ///////// calculate world coordinate at clickLocX and clickLocY /////////
            //find determinant of TcwInv

            //detTcw = determinant(Tcw);

            //if (detTcw != 0)
            //{
            Mat TcwInv = Tcw.Inv();                         //inverse of Tcw fors estimating u,v,w   
            Matrix4W = TcwInv * Matrix4C;         // u,v,w,1 = world coordinate

            // float D = Matrix4W.at<float>(2,0)/eps;
            // // if (D > 0 && D<6) 
            // 	Matrix4W.at<float>(2,0) = D;
            // else
            // 	Matrix4W.at<float>(2,0) = 0;
            // cout << "Tcw = " << endl << " " << Tcw << endl << endl;
            // cout << "Tcw^-1 = " << endl << " " << TcwInv << endl << endl;
            // cout << "Matrix4C = " << endl << " " << Matrix4C << endl << endl;
            // cout << "Matrix4W = " << endl << " " << Matrix4W << endl << endl;

            // cout << "Depth1" << d+0.0001<<", Depth2"<< d2+0.0001<<endl;

            // cout <<"Image location at (" << targetPoint1.x <<", "<< targetPoint1.y << "), World Coordinate at " <<endl;
            // cout<<"u:"<<Matrix4W.at<float>(0,0)<<",v:"<<Matrix4W.at<float>(1,0)<<",w:"<<Matrix4W.at<float>(2,0)<<"gggg"<<Matrix4W.at<float>(3,0)<<endl;

            return Matrix4W;
            //}
            //else
            //{
            //    return cv::Mat();
            //}

        }

    }
}
