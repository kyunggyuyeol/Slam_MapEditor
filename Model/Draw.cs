using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

using System.Windows.Media;
using System.Windows.Media.Media3D;
using Slam_MapEditor.View;

namespace Slam_MapEditor.Model
{
    public static class DrawDots
    {


        private static void drawTriangle(MeshGeometry3D mesh, Point3D p0, Point3D p1, Point3D p2)
        {

            mesh.Positions.Add(p0);
            mesh.Positions.Add(p1);
            mesh.Positions.Add(p2);

            int n0 = 0;
            int n1 = 1;
            int n2 = 2;

            mesh.TriangleIndices.Add(n0);
            mesh.TriangleIndices.Add(n1);
            mesh.TriangleIndices.Add(n2);

            


        }

        public static void drawRectangle(this MeshGeometry3D mesh, Point3D p0, Point3D p1, Point3D p2, Point3D p3)
        {


            drawTriangle(mesh, p0, p1, p2);
            drawTriangle(mesh, p2, p3, p0);


        }

        //public void drawRectangle(Point3D p0, Point3D p1, Point3D p2, Point3D p3, Brush brush = null)
        //{
        //    drawTriangle(p0, p1, p2, brush);
        //    drawTriangle(p2, p3, p0, brush);


        //}



    }
}
