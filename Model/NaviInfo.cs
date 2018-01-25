using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slam_MapEditor.Model
{
    public class NaviInfo : MapPoint
    {
        //네비 포인트 정보 3개
        public int Navi_Index { get; set; }

        public float NaviPosY { get; set; }

        public float NaviPosX { get; set; }

     
    }
}
