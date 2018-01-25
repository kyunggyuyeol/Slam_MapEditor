using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace Slam_MapEditor.Model
{
    public class NaviLine : ModelVisual3D
    {


        public int INDEX
        {
            get; set;
        }

        public string naviline { get; set; }
        public float NaviDistance { get; set; }

       

    }
}
