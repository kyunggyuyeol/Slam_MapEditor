using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using System.Windows.Controls;

namespace Slam_MapEditor.Model
{
    public class NaviPoint : MapPoint
    {

        public int _index;
        public float NaviPosX
        {
            get;set;
          
        }
        public float NaviPosY
        {
            get;set;
           
        }
  
        public int INDEX { get; set; }
        public string NaviMessage
        {

            get
            {
                return "[ X : " + NaviPosX.ToString() + " Y : " + NaviPosY.ToString() + " ]" ;
            }
        }

        public string navipoint { get; set; }
    

        public NaviPoint(float x, float y )
        {
            
            NaviPosX = x;
            NaviPosY = y;
    
        }
      


      
    }
}
